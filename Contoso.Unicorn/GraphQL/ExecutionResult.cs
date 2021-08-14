using System;
using System.Net;

namespace Contoso.Unicorn.GraphQL
{
    /// <inheritdoc />
    public class ExecutionResult : global::GraphQL.ExecutionResult
    {
        /// <inheritdoc />
        public ExecutionResult(global::GraphQL.ExecutionResult executionResult) : base(executionResult) { }

        /// <summary>
        /// Gets or sets status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets elapsed.
        /// </summary>
        public TimeSpan? Elapsed { get; set; }

        /// <summary>
        /// Gets or sets error.
        /// </summary>
        public string Error { get; set; }
    }
}