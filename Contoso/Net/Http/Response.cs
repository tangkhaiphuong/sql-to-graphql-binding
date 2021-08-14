using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using Contoso.Text.Json;

// ReSharper disable once IdentifierTypo
namespace Contoso.Net.Http
{
    public class Response
    {
        [JsonPropertyName("statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("elapsed")]
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan? Elapsed { get; set; }

        [JsonPropertyName("traces")]
        public IEnumerable<string> Traces { get; set; }
    }

    public class Response<T> : Response
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}