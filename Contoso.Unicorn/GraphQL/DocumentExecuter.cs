using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contoso.Net.Http;
using Contoso.Unicorn.Entities;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Language.AST;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using IExecutionStrategy = GraphQL.Execution.IExecutionStrategy;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Unicorn.GraphQL
{
    /// <inheritdoc />
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class DocumentExecuter : IDocumentExecuter
    {
        private class InternalDocumentExecuter : global::GraphQL.DocumentExecuter
        {
            protected override IExecutionStrategy SelectExecutionStrategy(ExecutionContext context)
            {
                var userContext = (GraphQLUserContext)context.UserContext;

                if (userContext?.Request.Query.TryGetValue("serial", out var serialStr) == true)
                {
                    if (serialStr == "" || (bool.TryParse(serialStr, out var serial) && serial))
                        return new SerialExecutionStrategy();

                    // If mutation is parallel, disable transaction.
                    if (context.Operation.OperationType == OperationType.Mutation)
                        userContext["transaction"] = new Func<Task>(() => Task.CompletedTask);
                }

                var result = base.SelectExecutionStrategy(context);
                return result;
            }
        }

        private readonly IDocumentExecuter _documentExecuter = new InternalDocumentExecuter();

        /// <inheritdoc />
        public async Task<global::GraphQL.ExecutionResult> ExecuteAsync(ExecutionOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var userContext = (GraphQLUserContext)options.UserContext;

            if (userContext.Request.Query.ContainsKey("debug"))
                options.EnableMetrics = true;

            if (userContext.Request.Query.TryGetValue("concurrent", out var concurrentStr) &&
                int.TryParse(concurrentStr.ToString(), out var concurrent))
                options.MaxParallelExecutionCount = concurrent;

            IDbContextTransaction transaction = null;
            options.UserContext["paging"] = 20;
            options.UserContext["template"] = true;
            options.UserContext["force"] = false;
            options.ThrowOnUnhandledException = true;
            options.UserContext["transaction"] = new Func<Task>(async () =>
            {
                if (transaction != null) return;

                var unicornContext = userContext.ServiceProvider.GetService<UnicornContext>();

                transaction = await unicornContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted, options.CancellationToken).ConfigureAwait(false);
            });

            if (userContext?.Request.Query.TryGetValue("paging", out var pagingStr) == true)
            {
                if (bool.TryParse(pagingStr, out var pagingBool) && pagingBool == false)
                    options.UserContext["paging"] = -1;
                else if (int.TryParse(pagingStr, out var pagingInt))
                    options.UserContext["paging"] = pagingInt;
            }

            if (userContext?.Request.Query.TryGetValue("force", out var forceStr) == true)
            {
                if (bool.TryParse(forceStr, out var force))
                    options.UserContext["force"] = force;
                else if (forceStr == "")
                    options.UserContext["force"] = true;
            }

            if (userContext?.Request.Query.TryGetValue("template", out var templateStr) == true)
            {
                if (bool.TryParse(templateStr, out var template))
                    options.UserContext["template"] = template;
            }

            if (userContext?.Request.Query.TryGetValue("strict", out var strictStr) == true)
            {
                if (bool.TryParse(strictStr, out var strict))
                    options.ThrowOnUnhandledException = strict;
            }

            var stopWatch = Stopwatch.StartNew();

            var result = await _documentExecuter.ExecuteAsync(options).ConfigureAwait(false);

            var statusCode = HttpStatusCode.OK;

            if (result.Errors != null && result.Errors.Count > 0)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync().ConfigureAwait(false);
                    statusCode = HttpStatusCode.InternalServerError;
                }
                else if (result.Data == null || result.Data is IDictionary<string, object> dic && dic.All(_ => _.Value == null))
                    statusCode = result.Errors.Count == 1 ? result.Errors[0].GetStatusCode() : new AggregateException(result.Errors).GetStatusCode();
            }
            else
            {
                if (transaction != null) await transaction.CommitAsync().ConfigureAwait(false);
            }

            userContext.Response.StatusCode = (int)statusCode;

            return new ExecutionResult(result)
            {
                StatusCode = statusCode,
                Message = statusCode.GetStatusReason(),
                Error = string.Join(Environment.NewLine, result.Errors?.Select(_ => _.Message) ?? Enumerable.Empty<string>()),
                Elapsed = stopWatch.Elapsed,
                Executed = result.Executed
            };
        }
    }
}