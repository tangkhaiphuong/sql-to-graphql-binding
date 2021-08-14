using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.SystemTextJson;
using Microsoft.AspNetCore.Http;

namespace Contoso.Unicorn.GraphQL
{
    public class GraphQLRequestDeserializer : IGraphQLRequestDeserializer
    {
#pragma warning disable CA1034
        public sealed class InternalGraphQLRequest
#pragma warning restore CA1034
        {
            [JsonPropertyName("query")]
            public string Query { get; set; }

            [JsonPropertyName("operationName")]
            public string OperationName { get; set; }

            [JsonPropertyName("variables")]
#pragma warning disable CA2227
            public Dictionary<string, object> Variables { get; set; }
#pragma warning restore CA2227

            [JsonPropertyName("template")]
            public string Template { get; set; }
        }

        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions();

        public GraphQLRequestDeserializer(Action<JsonSerializerOptions> configure)
        {
            _serializerOptions.Converters.Add(new ObjectDictionaryConverter());
            configure?.Invoke(_serializerOptions);
        }

        public async Task<GraphQLRequestDeserializationResult> DeserializeFromJsonBodyAsync(
            HttpRequest httpRequest,
            CancellationToken cancellationToken = default)
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

            var bodyReader = httpRequest.BodyReader;
            JsonTokenType jsonTokenType;
            try
            {
                jsonTokenType = await PeekJsonTokenTypeAsync(bodyReader, cancellationToken).ConfigureAwait(false);
            }
            catch (JsonException)
            {
                jsonTokenType = JsonTokenType.None;
            }
            cancellationToken.ThrowIfCancellationRequested();
            var result = new GraphQLRequestDeserializationResult()
            {
                IsSuccessful = true
            };
            GraphQLRequestDeserializationResult deserializationResult;
            switch (jsonTokenType)
            {
                case JsonTokenType.StartObject:
                    deserializationResult = result;
                    deserializationResult.Single = ToGraphQLRequest(await JsonSerializer.DeserializeAsync<InternalGraphQLRequest>(bodyReader.AsStream(), _serializerOptions, cancellationToken).ConfigureAwait(false));
                    return result;
                case JsonTokenType.StartArray:
                    deserializationResult = result;
                    deserializationResult.Batch = (await JsonSerializer.DeserializeAsync<InternalGraphQLRequest[]>(bodyReader.AsStream(), _serializerOptions, cancellationToken).ConfigureAwait(false)).Select(new Func<InternalGraphQLRequest, global::GraphQL.Server.GraphQLRequest>(ToGraphQLRequest)).ToArray();
                    return result;
                default:
                    result.IsSuccessful = false;
                    return result;
            }
        }

        private static async ValueTask<JsonTokenType> PeekJsonTokenTypeAsync(
            PipeReader reader,
            CancellationToken cancellationToken)
        {
            ReadResult readResult;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                var buffer = readResult.Buffer;
                if (DetermineTokenType(in buffer, out var jsonToken))
                {
                    reader.AdvanceTo(buffer.Start, buffer.Start);
                    return jsonToken;
                }
                reader.AdvanceTo(buffer.Start, buffer.End);
            }
            while (!readResult.IsCompleted);
            return JsonTokenType.None;

            bool DetermineTokenType(in ReadOnlySequence<byte> buffer, out JsonTokenType jsonToken)
            {
                var utf8JsonReader = new Utf8JsonReader(buffer);
                if (utf8JsonReader.Read())
                {
                    jsonToken = utf8JsonReader.TokenType;
                    return true;
                }
                jsonToken = JsonTokenType.None;
                return false;
            }
        }

        public Inputs DeserializeInputsFromJson(string json)
        {
            return json?.ToInputs();
        }

        private static GraphQLRequest ToGraphQLRequest(
            InternalGraphQLRequest internalGraphQLRequest)
        {
            var graphQlRequest = new GraphQLRequest
            {
                OperationName = internalGraphQLRequest.OperationName,
                Query = internalGraphQLRequest.Query
            };
            var variables = internalGraphQLRequest.Variables;
            graphQlRequest.Inputs = variables?.ToInputs();
            graphQlRequest.Template = internalGraphQLRequest.Template;
            return graphQlRequest;
        }
    }
}