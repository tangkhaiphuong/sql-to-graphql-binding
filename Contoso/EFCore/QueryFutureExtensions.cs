using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Z.EntityFramework.Plus;

namespace Contoso.EFCore
{
    public static class QueryFutureExtensions
    {
        /// <summary>
        ///     Defer the execution of the <paramref name="query" /> and batch the query command with other
        ///     future queries. The batch is executed when a future query requires a database round trip.
        /// </summary>
        /// <param name="query">
        ///     The query to defer the execution of and to add in the batch of future
        ///     queries.
        /// </param>
        /// <returns>
        ///     The QueryFutureEnumerable&lt;TEntity&gt; added to the batch of futures queries.
        /// </returns>
        public static QueryFutureEnumerable FutureObject(this IQueryable query)
        {
            if (!QueryFutureManager.AllowQueryBatch)
            {
                var queryFuture = new QueryFutureEnumerable(null, null);
                if (query is { }) queryFuture.GetResultDirectly(query);
                return queryFuture;
            }

            var context = query.GetDbContext();
            var futureBatch = QueryFutureManager.AddOrGetBatch(context);
            var futureQuery = new QueryFutureEnumerable(futureBatch, query);

            futureBatch.Queries.Add(futureQuery);

            return futureQuery;
        }

        public static DbContext GetDbContext(this IQueryable query)
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var queryCompiler = typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags)?.GetValue(query?.Provider);
            var queryContextFactory = queryCompiler!.GetType().GetField("_queryContextFactory", bindingFlags)!.GetValue(queryCompiler);

            var dependencies = typeof(RelationalQueryContextFactory)
                .GetField("_dependencies", bindingFlags)
                ?.GetValue(queryContextFactory);
            var queryContextDependencies = typeof(DbContext).Assembly.GetType(typeof(QueryContextDependencies).FullName ?? string.Empty);
            var stateManagerProperty = queryContextDependencies.GetProperty("StateManager", bindingFlags | BindingFlags.Public)?.GetValue(dependencies);
            var stateManager = (IStateManager)stateManagerProperty;

            return stateManager!.Context;
        }
    }
}