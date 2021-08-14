using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;

// ReSharper disable once IdentifierTypo
namespace Contoso.Net.Http
{
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
#pragma warning disable CA1724
    public static partial class Utilities
#pragma warning restore CA1724
    {
        public static string GetStatusReason(this HttpStatusCode statusCode)
        {
            if (statusCode == (HttpStatusCode)499) return "Client Closed Request";
            if (statusCode == (HttpStatusCode)444) return "No Response";
            if (statusCode == HttpStatusCode.OK) return "OK";
            if (statusCode == (HttpStatusCode)226) return "IM Used";
            var key = statusCode.ToString();
            var result = string.Concat(
                key.Select((c, i) =>
                    char.IsUpper(c) && i > 0
                        ? " " + c.ToString(CultureInfo.InvariantCulture)
                        : c.ToString(CultureInfo.InvariantCulture)
                )
            );
            return result;
        }

        /// <summary>
        /// Create exception response.
        /// </summary>
        /// <typeparam name="T">Response type.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <param name="trace">Is trace exception.</param>
        /// <param name="elapsed">Elapse time.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> is <see langword="null"/></exception>
        public static T CreateResponse<T>(
            this Exception exception,
            HttpStatusCode statusCode,
            bool trace,
            TimeSpan? elapsed) where T : Response, new()
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            var result = new T
            {
                StatusCode = statusCode,
                Message = string.Join(Environment.NewLine, exception.GetMessages()),
                Error = GetStatusReason(statusCode),
                Elapsed = elapsed,

#if NETSTANDARD2_0
                Traces = trace ? exception.StackTrace.Split(Environment.NewLine.ToArray()) : null,
#else
                Traces = trace ? exception.StackTrace.Split(Environment.NewLine) : null,
#endif
            };

            return result;
        }
    }
}
