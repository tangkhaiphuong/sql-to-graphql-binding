using System;
using GraphQL.Instrumentation;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Contoso.Net.Http;
using Fluid;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;

namespace Contoso.Unicorn.GraphQL
{
    /// <summary>
    /// ASP.NET Core middleware for processing GraphQL requests.
    /// <br/><br/>
    /// GraphQL over HTTP <see href="https://github.com/APIs-guru/graphql-over-http">spec</see> says:
    /// GET requests can be used for executing ONLY queries. If the values of query and operationName indicates that
    /// a non-query operation is to be executed, the server should immediately respond with an error status code, and
    /// halt execution.
    /// <br/><br/>
    /// Attention! The current implementation does not impose such a restriction and allows mutations in GET requests.
    /// </summary>
    /// <typeparam name="TSchema">Type of GraphQL schema that is used to validate and process requests.</typeparam>
    public class GraphQLHttpMiddleware<TSchema>
        where TSchema : ISchema
    {
        private JsonSerializerOptions options = new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        private const string DOCS_URL = "See: http://graphql.org/learn/serving-over-http/.";

        private readonly RequestDelegate _next;
        private readonly IGraphQLRequestDeserializer _deserializer;

        public GraphQLHttpMiddleware(RequestDelegate next, IGraphQLRequestDeserializer deserializer)
        {
            _next = next;
            _deserializer = deserializer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            // Handle requests as per recommendation at http://graphql.org/learn/serving-over-http/
            // Inspiration: https://github.com/graphql/express-graphql/blob/master/src/index.js
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            var writer = context.RequestServices.GetRequiredService<IDocumentWriter>();
            var cancellationToken = GetCancellationToken(context);

            // GraphQL HTTP only supports GET and POST methods
            bool isGet = HttpMethods.IsGet(httpRequest.Method);
            bool isPost = HttpMethods.IsPost(httpRequest.Method);
            if (!isGet && !isPost)
            {
                httpResponse.Headers["Allow"] = "GET, POST";
                await WriteErrorResponseAsync(httpResponse, writer, cancellationToken,
                    $"Invalid HTTP method. Only GET and POST are supported. {DOCS_URL}",
                    httpStatusCode: 405 // Method Not Allowed
                ).ConfigureAwait(false);
                return;
            }

            // Parse POST body
            GraphQLRequest bodyGQLRequest = null;
            global::GraphQL.Server.GraphQLRequest[] bodyGQLBatchRequest = null;
            if (isPost)
            {
                if (!MediaTypeHeaderValue.TryParse(httpRequest.ContentType, out var mediaTypeHeader))
                {
                    await WriteErrorResponseAsync(httpResponse, writer, cancellationToken, $"Invalid 'Content-Type' header: value '{httpRequest.ContentType}' could not be parsed.").ConfigureAwait(false);
                    return;
                }

                switch (mediaTypeHeader.MediaType)
                {
                    case MediaType.JSON:
                        var deserializationResult = await _deserializer.DeserializeFromJsonBodyAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                        if (!deserializationResult.IsSuccessful)
                        {
                            await WriteErrorResponseAsync(httpResponse, writer, cancellationToken, "Body text could not be parsed. Body text should start with '{' for normal graphql query or with '[' for batched query.").ConfigureAwait(false);
                            return;
                        }

                        bodyGQLRequest = (GraphQLRequest)deserializationResult.Single;
                        bodyGQLBatchRequest = deserializationResult.Batch;
                        break;

                    case MediaType.GRAPH_QL:
                        bodyGQLRequest = await DeserializeFromGraphBodyAsync(httpRequest.Body).ConfigureAwait(false);
                        break;

                    case MediaType.FORM:
                        var formCollection = await httpRequest.ReadFormAsync(cancellationToken).ConfigureAwait(false);
                        bodyGQLRequest = DeserializeFromFormBody(formCollection);
                        break;

                    default:
                        await WriteErrorResponseAsync(httpResponse, writer, cancellationToken, $"Invalid 'Content-Type' header: non-supported media type. Must be of '{MediaType.JSON}', '{MediaType.GRAPH_QL}' or '{MediaType.FORM}'. {DOCS_URL}").ConfigureAwait(false);
                        return;
                }
            }

            // If we don't have a batch request, parse the query from URL too to determine the actual request to run.
            // Query string params take priority.
            GraphQLRequest gqlRequest = null;
            if (bodyGQLBatchRequest == null)
            {
                var urlGQLRequest = DeserializeFromQueryString(httpRequest.Query);

                gqlRequest = new GraphQLRequest
                {
                    Query = urlGQLRequest.Query ?? bodyGQLRequest?.Query,
                    Inputs = urlGQLRequest.Inputs ?? bodyGQLRequest?.Inputs,
                    Extensions = urlGQLRequest.Extensions ?? bodyGQLRequest?.Extensions,
                    OperationName = urlGQLRequest.OperationName ?? bodyGQLRequest?.OperationName,
                    Template = urlGQLRequest.Template ?? bodyGQLRequest?.Template
                };
            }

            // Prepare context and execute
            var userContextBuilder = context.RequestServices.GetService<IUserContextBuilder>();
            var userContext = userContextBuilder == null
                ? new Dictionary<string, object>() // in order to allow resolvers to exchange their state through this object
                : await userContextBuilder.BuildUserContext(context).ConfigureAwait(false);

            var executer = context.RequestServices.GetRequiredService<IGraphQLExecuter<TSchema>>();

            // Normal execution with single graphql request
            if (bodyGQLBatchRequest == null)
            {
                var stopwatch = ValueStopwatch.StartNew();
                await RequestExecutingAsync(gqlRequest);
                var result = await ExecuteRequestAsync(gqlRequest, userContext, executer, context.RequestServices, cancellationToken).ConfigureAwait(false);

                await RequestExecutedAsync(new GraphQLRequestExecutionResult(gqlRequest, result, stopwatch.Elapsed)).ConfigureAwait(false);

                await WriteResponseAsync(context.RequestServices.GetService<TemplateOptions>(), gqlRequest, httpResponse, writer, cancellationToken, result).ConfigureAwait(false);
            }
            // Execute multiple graphql requests in one batch
            else
            {
                var executionResults = new global::GraphQL.ExecutionResult[bodyGQLBatchRequest.Length];
                for (int i = 0; i < bodyGQLBatchRequest.Length; ++i)
                {
                    var gqlRequestInBatch = bodyGQLBatchRequest[i];

                    var stopwatch = ValueStopwatch.StartNew();
                    await RequestExecutingAsync(gqlRequestInBatch, i).ConfigureAwait(false);
                    var result = await ExecuteRequestAsync(gqlRequestInBatch, userContext, executer, context.RequestServices, cancellationToken).ConfigureAwait(false);

                    await RequestExecutedAsync(new GraphQLRequestExecutionResult(gqlRequestInBatch, result, stopwatch.Elapsed, i)).ConfigureAwait(false);

                    executionResults[i] = result;
                }

                await WriteResponseAsync(context.RequestServices.GetService<TemplateOptions>(), gqlRequest, httpResponse, writer, cancellationToken, executionResults).ConfigureAwait(false);
            }
        }

        private static Task<global::GraphQL.ExecutionResult> ExecuteRequestAsync(global::GraphQL.Server.GraphQLRequest gqlRequest, IDictionary<string, object> userContext, IGraphQLExecuter<TSchema> executer, IServiceProvider requestServices, CancellationToken token)
            => executer.ExecuteAsync(
                gqlRequest.OperationName,
                gqlRequest.Query,
                gqlRequest.Inputs,
                userContext,
                requestServices,
                token);

        protected virtual CancellationToken GetCancellationToken(HttpContext context) => context.RequestAborted;

        protected virtual Task RequestExecutingAsync(global::GraphQL.Server.GraphQLRequest request, int? indexInBatch = null)
        {
            // nothing to do in this middleware
            return Task.CompletedTask;
        }

        protected virtual Task RequestExecutedAsync(in GraphQLRequestExecutionResult requestExecutionResult)
        {
            // nothing to do in this middleware
            return Task.CompletedTask;
        }

        private Task WriteErrorResponseAsync(HttpResponse httpResponse, IDocumentWriter writer, CancellationToken cancellationToken,
            string errorMessage, int httpStatusCode = 400 /* BadRequest */)
        {
            var result = new global::GraphQL.ExecutionResult
            {
                Errors = new ExecutionErrors
                {
                    new ExecutionError(errorMessage)
                }
            };

            httpResponse.ContentType = "application/json";
            httpResponse.StatusCode = httpStatusCode;

            return writer.WriteAsync(httpResponse.Body, result, cancellationToken);
        }

        private async Task WriteResponseAsync<TResult>(TemplateOptions templateOptions, GraphQLRequest graphQLRequest, HttpResponse httpResponse, IDocumentWriter writer, CancellationToken cancellationToken, TResult result)
        {
            httpResponse.ContentType = "application/json";

            var template = graphQLRequest.Template;
            if (graphQLRequest.Extensions != null && graphQLRequest.Extensions.TryGetValue("template", out var templateValue))
                template = templateValue.ToString();

            if (!string.IsNullOrEmpty(template) && httpResponse.StatusCode == 200)
            {
                var stopWatch = Stopwatch.StartNew();

                await using var memoryStream = new MemoryStream();

                await writer.WriteAsync(memoryStream, result, cancellationToken).ConfigureAwait(false);

                memoryStream.Seek(0, SeekOrigin.Begin);

                using var reader = new StreamReader(memoryStream, Encoding.UTF8);

                var payload = await reader.ReadToEndAsync().ConfigureAwait(false);

                var obj = JsonSerializer.Deserialize<IDictionary<string, object>>(payload, options);

#if FLUID20
                var context = new TemplateContext(templateOptions)
                {
#else
                var context = new TemplateContext
                {
                    MemberAccessStrategy = { IgnoreCasing = true },
#endif
                    Model = obj["data"],
                    CultureInfo = new CultureInfo("vi")
                };

                string dataPayload = null;

                var elapsed = ((JsonElement)obj["elapsed"]).GetString();

                try
                {
#if FLUID20
                    var parser = new FluidParser();
                    var fluidTemplate = parser.Parse(template);
#else
                    var fluidTemplate = FluidTemplate.Parse(template);
#endif

                    dataPayload = await fluidTemplate.RenderAsync(context).ConfigureAwait(false);

                    if (string.IsNullOrEmpty(dataPayload))
                        obj.Remove("data");
                    else
                        obj["data"] = JsonSerializer.Deserialize<JsonElement>(dataPayload, options);

                    var stopWatchElapsed = stopWatch.Elapsed;

                    obj["elapsed"] = (TimeSpan.Parse(elapsed, CultureInfo.InvariantCulture) + stopWatchElapsed).ToString();

                    obj["elapsedTemplate"] = stopWatchElapsed.ToString();
                    obj["elapsedProcess"] = elapsed;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {

                    obj = new Dictionary<string, object>();
                    var statusCode = ex.GetStatusCode();
                    obj["statusCode"] = httpResponse.StatusCode = (int)statusCode;
                    obj["message"] = statusCode.GetStatusReason();
                    obj["error"] = ex.Message;
                    obj["elapsed"] = elapsed;
                    obj["data"] = dataPayload ?? template;

                    payload = JsonSerializer.Serialize(obj);

                    await using var streamWriter2 = new StreamWriter(httpResponse.Body, Encoding.UTF8);

                    await streamWriter2.WriteAsync(payload).ConfigureAwait(false);

                    return;
                }

                payload = JsonSerializer.Serialize(obj);

                await using var streamWriter = new StreamWriter(httpResponse.Body, Encoding.UTF8);

                await streamWriter.WriteAsync(payload).ConfigureAwait(false);
            }
            else
            {
                await writer.WriteAsync(httpResponse.Body, result, cancellationToken).ConfigureAwait(false);
            }
        }

        private GraphQLRequest DeserializeFromQueryString(IQueryCollection queryCollection) => new GraphQLRequest
        {
            Query = queryCollection.TryGetValue(global::GraphQL.Server.GraphQLRequest.QUERY_KEY, out var queryValues) ? queryValues[0] : null,
            Inputs = queryCollection.TryGetValue(global::GraphQL.Server.GraphQLRequest.VARIABLES_KEY, out var variablesValues) ? _deserializer.DeserializeInputsFromJson(variablesValues[0]) : null,
            Extensions = queryCollection.TryGetValue(global::GraphQL.Server.GraphQLRequest.EXTENSIONS_KEY, out var extensionsValues) ? _deserializer.DeserializeInputsFromJson(extensionsValues[0]) : null,
            OperationName = queryCollection.TryGetValue(global::GraphQL.Server.GraphQLRequest.OPERATION_NAME_KEY, out var operationNameValues) ? operationNameValues[0] : null
        };

        private GraphQLRequest DeserializeFromFormBody(IFormCollection formCollection) => new GraphQLRequest
        {
            Query = formCollection.TryGetValue(global::GraphQL.Server.GraphQLRequest.QUERY_KEY, out var queryValues) ? queryValues[0] : null,
            Inputs = formCollection.TryGetValue(global::GraphQL.Server.GraphQLRequest.VARIABLES_KEY, out var variablesValues) ? _deserializer.DeserializeInputsFromJson(variablesValues[0]) : null,
            Extensions = formCollection.TryGetValue(global::GraphQL.Server.GraphQLRequest.EXTENSIONS_KEY, out var extensionsValues) ? _deserializer.DeserializeInputsFromJson(extensionsValues[0]) : null,
            OperationName = formCollection.TryGetValue(global::GraphQL.Server.GraphQLRequest.OPERATION_NAME_KEY, out var operationNameValues) ? operationNameValues[0] : null
        };

        private async Task<GraphQLRequest> DeserializeFromGraphBodyAsync(Stream bodyStream)
        {
            // In this case, the query is the raw value in the POST body

            // Do not explicitly or implicitly (via using, etc.) call dispose because StreamReader will dispose inner stream.
            // This leads to the inability to use the stream further by other consumers/middlewares of the request processing
            // pipeline. In fact, it is absolutely not dangerous not to dispose StreamReader as it does not perform any useful
            // work except for the disposing inner stream.
            string query = await new StreamReader(bodyStream).ReadToEndAsync();

            return new GraphQLRequest { Query = query }; // application/graphql MediaType supports only query text
        }
    }
}