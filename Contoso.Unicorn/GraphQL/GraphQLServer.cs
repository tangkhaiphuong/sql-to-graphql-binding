using System.Collections.Generic;
using Contoso.Unicorn.GraphQL.Mutations;
using Contoso.Unicorn.GraphQL.Queries;
using Contoso.Unicorn.GraphQL.Types;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Relay.Types;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Ui.Altair;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server.Ui.Voyager;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Contoso.Unicorn.GraphQL
{
    public static class GraphQLServer
    {
        public static void AddGraphQLServer(this IServiceCollection services)
        {
            // GraphTypeTypeRegistry.Register(typeof(DateTime), typeof(DateTimeGraphType));
            // Add GraphQL services and configure options
            services.AddTransient<NodeInterface>();
            services.AddSingleton<UnicornSchema>();
            services.AddSingleton<UnicornQuery>();
            services.AddSingleton<UnicornMutation>();
            services.AddTransient<EnumerationGraphType<MutationAction>>();
            services.AddTransient<EnumerationGraphType<QueryLink>>();
            services.AddTransient<LinkArgumentInputType>();
            services.TryAddSingleton<IDocumentExecuter, DocumentExecuter>();


            var graphQLBuilder = services.AddGraphQL((options, provider) =>
            {
                options.EnableMetrics = false;
                var logger = provider.GetService<ILogger<Startup>>();
                options.UnhandledExceptionDelegate = ctx =>
                    logger.LogError("{Error} occured", ctx.OriginalException.Message);

            });
            graphQLBuilder.Services.AddSingleton<IGraphQLRequestDeserializer>(p => new GraphQLRequestDeserializer(null));
            // Add required services for de/serialization
            graphQLBuilder.Services.AddSingleton<IDocumentWriter>(p => new DocumentWriter(serializerSettings =>
            {
                serializerSettings.Converters.Add(new Converters.ExecutionResultJsonConverter(p.GetService<IErrorInfoProvider>() ?? new ErrorInfoProvider()));
            }, new ErrorInfoProvider(options => options.ExposeExceptionStackTrace = true)));
            // For everything else
            graphQLBuilder.AddWebSockets()
            // Add required services for web socket support
            .AddDataLoader()
            .AddUserContextBuilder(context => new GraphQLUserContext
            (context.User, context.Request, context.Response, context.RequestServices))
            .AddRelayGraphTypes()
            // Add required services for DataLoader support
            .AddGraphTypes(typeof(UnicornSchema)); // Add all IGraphType implementors in assembly which UnicornSchema exists 
        }

        public static void UseGraphQLServer(this IApplicationBuilder app)
        {
            // this is required for websockets support
            app.UseWebSockets();
            // use websocket middleware for UnicornSchema at path /graphql
            app.UseGraphQLWebSockets<UnicornSchema>();
            // use HTTP middleware for UnicornSchema at path /graphql
            app.UseWhen(
                context => context.Request.Path.StartsWithSegments("/graphql", out var remaining) && string.IsNullOrEmpty(remaining),
                b => b.UseMiddleware<GraphQLHttpMiddleware<UnicornSchema>>());
            // use graphiQL middleware at default url /ui/graphiql
            app.UseGraphQLGraphiQL(new GraphiQLOptions());
            // use graphql-playground middleware at default url /ui/playground
            app.UseGraphQLPlayground(new PlaygroundOptions
            {
                PlaygroundSettings = new Dictionary<string, object>
                {
                    { "prettier.tabWidth", 4 },
                    { "editor.theme" , "light" }
                }
            });
            // use altair middleware at default url /ui/altair
            app.UseGraphQLAltair(new  AltairOptions());
            // use voyager middleware at default url /ui/voyager
            app.UseGraphQLVoyager(new VoyagerOptions());
        }
    }
}
