using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Contoso.Unicorn.Entities;
using Contoso.Unicorn.GraphQL;
using Contoso.Unicorn.Miscellaneous;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders;

namespace Contoso.Unicorn
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UnicornContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("Unicorn");

                options.UseSqlServer(connectionString, providerOptions => providerOptions.CommandTimeout(60))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddMemoryCache();
            services.AddResponseCompression();

            services.AddSingleton(_ => new Fluid.TemplateOptions().ConfigureTemplateOptions());

            services.AddMvc(_ => { _.EnableEndpointRouting = false; });

            services.AddMvcCore();

            services.AddHttpContextAccessor();

            services.AddGraphQLServer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            app.UseResponseCompression();
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Playground")),
                RequestPath = "/dev/playground",
                EnableDirectoryBrowsing = true
            });
            app.UseWebSockets();
            app.UseMvc();
            app.UseGraphQLServer();
        }
    }
}