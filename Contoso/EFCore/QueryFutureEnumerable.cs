using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Contoso.EFCore
{
    /// <summary>Class for query future value.</summary>
    public class QueryFutureEnumerable : BaseQueryFuture, IEnumerable<object>
    {
        /// <summary>The result of the query future.</summary>
        private IEnumerable<object> _result;

        /// <summary>Constructor.</summary>
        /// <param name="ownerBatch">The batch that owns this item.</param>
        /// <param name="query">
        ///     The query to defer the execution and to add in the batch of future
        ///     queries.
        /// </param>

        public QueryFutureEnumerable(QueryFutureBatch ownerBatch, IQueryable query)
        {
            OwnerBatch = ownerBatch;
            Query = query;
        }

        /// <summary>Gets the enumerator of the query future.</summary>
        /// <returns>The enumerator of the query future.</returns>
        public IEnumerator<object> GetEnumerator()
        {
            if (!HasValue)
            {
                OwnerBatch.ExecuteQueries();
            }

            return _result == null ? new List<object>().GetEnumerator() : _result.GetEnumerator();
        }

        /// <summary>Gets the enumerator of the query future.</summary>
        /// <returns>The enumerator of the query future.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public async Task<List<object>> ToListAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!HasValue)
            {
                await OwnerBatch.ExecuteQueriesAsync(cancellationToken).ConfigureAwait(false);
            }

            if (_result == null)
            {
                return new List<object>();
            }

            using var enumerator = _result.GetEnumerator();
            var list = new List<object>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return list;
        }


            public async Task<object[]> ToArrayAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!HasValue)
            {
                await OwnerBatch.ExecuteQueriesAsync(cancellationToken).ConfigureAwait(false);
            }

            if (_result == null)
            {
                return Array.Empty<object>();
            }

            using var enumerator = _result.GetEnumerator();
            var list = new List<object>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return list.ToArray();
        }

        /// <summary>Sets the result of the query deferred.</summary>
        /// <param name="reader">The reader returned from the query execution.</param>
        public override void SetResult(DbDataReader reader)
        {
            var fullName = reader?.GetType().FullName;
            if (fullName is { } && fullName.Contains("Oracle", StringComparison.InvariantCulture))
            {
                using var reader2 = new QueryFutureOracleDbReader(reader);

                using var enumerator = GetQueryEnumerator<object>(reader);

                SetResult(enumerator);
            }
            else
            {

                using var enumerator = GetQueryEnumerator<object>(reader);

                SetResult(enumerator);
            }
        }

        public void SetResult(IEnumerator enumerator)
        {
            // Enumerate on all items
            var list = new List<object>();
            while (enumerator is { } && enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            _result = list;

            HasValue = true;
        }

        /// <inheritdoc />
        public override void GetResultDirectly() => GetResultDirectly(Query);

        /// <inheritdoc />
        public override Task GetResultDirectlyAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GetResultDirectly(Query);
            return Task.CompletedTask;
        }

        internal void GetResultDirectly(IQueryable query) => SetResult(query.GetEnumerator());
    }
}