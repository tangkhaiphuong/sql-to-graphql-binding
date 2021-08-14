using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Humanizer;

namespace Contoso.Unicorn.Responses
{
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    public class IndexResponse
    {
        private static readonly string HostName = Environment.GetEnvironmentVariable("HOST_HOSTNAME");
        private static readonly Process Process = Process.GetCurrentProcess();

        [JsonPropertyName("statusCode")] public long StatusCode { get; } = 200;

        [JsonPropertyName("message")] public string Message { get; } = "Hello!";

        [JsonPropertyName("name")] public string Name { get; } = "contoso-unicorn";

        [JsonPropertyName("server")]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public string Server => string.IsNullOrEmpty(HostName) ? Environment.MachineName : HostName;

        [JsonPropertyName("description")] public string Description { get; } = "Contoso Unicorn";

        [JsonPropertyName("version")] public string Version { get; } = "1.0.0";

        [JsonPropertyName("releasedDate")]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public DateTime ReleaseDate
        {
            get
            {
                var vi = FileVersionInfo.GetVersionInfo(typeof(IndexResponse).Assembly.Location);
                var fileInfo = new FileInfo(vi.FileName);
                var createTime = fileInfo.LastWriteTime;
                return createTime;
            }
        }

        [JsonPropertyName("startedAt")] public DateTime StartedAt { get; } = Process.StartTime;

        [JsonPropertyName("upTime")] public string UpTime => (DateTime.Now - StartedAt).Humanize(3, true);

        [JsonPropertyName("now")] public DateTime Now { get; } = DateTime.UtcNow;

        [JsonPropertyName("system")]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public object System => new
        {
            architect = RuntimeInformation.ProcessArchitecture.ToString(),
            platform = RuntimeInformation.OSDescription,
            framework = RuntimeInformation.FrameworkDescription,
            cpus = Environment.ProcessorCount,
        };
    }
}