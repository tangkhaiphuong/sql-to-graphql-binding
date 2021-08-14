using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Contoso.Unicorn
{
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    public static partial class Program
    {
        public static Task Main(string[] args) {
            return CreateWebHostBuilder(args).Build().RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel(Options)
                .UseIIS()
                .UseStartup<Startup>()
                .ConfigureKestrel(Options);
        }

        private static void Options(KestrelServerOptions o)
        {
            o.Limits.MaxConcurrentConnections = null;
            o.Limits.MaxConcurrentUpgradedConnections = null;
            o.Limits.MaxRequestBufferSize = null;
            o.Limits.MaxRequestHeaderCount = 4096;
            o.Limits.MaxRequestHeadersTotalSize = 32768 * o.Limits.MaxRequestHeaderCount;
            // <see cref="https://stackoverflow.com/a/47112438"/> 
            o.Limits.MaxRequestBodySize = null;
            // <see cref="https://stackoverflow.com/a/47809150"/>
            // o.Limits.KeepAliveTimeout = TimeSpan.FromHours(1);
            // o.Limits.RequestHeadersTimeout = TimeSpan.FromHours(1);
            o.Limits.MaxResponseBufferSize = null;
            o.AddServerHeader = false;
        }
    }
}
