using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Contoso.Net.Http;

// ReSharper disable once CheckNamespace
namespace Contoso.Unicorn
{
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public static partial class Utilities
    {
        /// <summary>
        /// Get status code.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <returns></returns>
        public static HttpStatusCode GetStatusCode(this Exception exception)
        {
            return GetStatusCode(exception, out _);
        }

        /// <summary>
        /// Get status code.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <param name="trace">Is show trace or not?</param>
        /// <returns></returns>
        public static HttpStatusCode GetStatusCode(this Exception exception, out bool trace)
        {
            HttpStatusCode code; // 500 if unexpected
            trace = false;
            switch (exception)
            {
                case TargetInvocationException targetInvocationException:
                    if (targetInvocationException.InnerException == null)
                    {
                        code = HttpStatusCode.NotAcceptable;
                        trace = true;
                        break;
                    }
                    else
                    {
                        code = GetStatusCode(targetInvocationException.InnerException, out trace);
                        break;
                    }
                case FormatException _:
                    code = HttpStatusCode.UnprocessableEntity;
                    break;
                case AccessViolationException _:
                    code = HttpStatusCode.Forbidden;
                    break;
                case UnauthorizedAccessException _:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case FileNotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
                case InvalidDataException _:
                    code = HttpStatusCode.NotAcceptable;
                    trace = true;
                    break;
                case TaskCanceledException _:
                    code = (HttpStatusCode)(444);
                    break;
                case OperationCanceledException _:
                    code = (HttpStatusCode)(499);
                    break;
                case TimeoutException _:
                    code = HttpStatusCode.RequestTimeout;
                    break;
                case OverflowException _:
                    code = HttpStatusCode.BadGateway;
                    trace = true;
                    break;
                case StackOverflowException _:
                case InvalidOperationException _:
                    code = HttpStatusCode.InternalServerError;
                    trace = true;
                    break;
                case ArgumentOutOfRangeException _:
                case IndexOutOfRangeException _:
                    code = HttpStatusCode.RequestedRangeNotSatisfiable;
                    trace = true;
                    break;
                case NullReferenceException _:
                    code = HttpStatusCode.ExpectationFailed;
                    trace = true;
                    break;
                case ArgumentException _:
                    code = HttpStatusCode.PreconditionRequired;
                    trace = true;
                    break;
                case global::GraphQL.ExecutionError _:
                    code = HttpStatusCode.BadRequest;
                    trace = false;
                    break;
                case NotSupportedException _:
                    code = HttpStatusCode.Conflict;
                    trace = true;
                    break;
                default:
                    code = HttpStatusCode.BadRequest;
                    trace = true;
                    break;
            }

            return code;
        }

        /// <summary>
        /// CreateAsync exception response.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <param name="elapsed">Elapse time.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">exception is <see langword="null"/></exception>
        public static T CreateResponse<T>(this Exception exception, TimeSpan? elapsed) where T : Response, new()
        {
            var result = GetStatusCode(exception, out var trace);
            return exception.CreateResponse<T>(result, trace, elapsed);
        }
    }
}
