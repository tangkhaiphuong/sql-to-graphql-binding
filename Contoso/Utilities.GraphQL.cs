using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Builders;
using GraphQL.Relay.Types;
using GraphQL.Types.Relay.DataObjects;

// ReSharper disable once IdentifierTypo
namespace Contoso
{
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public static partial class Utilities
    {
        /// <summary>
        /// Convert queryable to connection.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TKSource"></typeparam>
        /// <param name="query"></param>
        /// <param name="context"></param>
        /// <param name="totalCount"></param>
        /// <param name="take"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static async Task<Connection<TKSource>> ToConnectionAsync<TSource, TParent, TKSource>(
            this IQueryable<TSource> query,
            IResolveConnectionContext<TParent> context,
            Func<IQueryable<TSource>, IResolveConnectionContext<TParent>, IAsyncEnumerable<TKSource>> converter,
            int take = -1,
            int totalCount = -1)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (converter == null) throw new ArgumentNullException(nameof(converter));

            if (context.First < 0) throw new ArgumentOutOfRangeException(nameof(context), "context.First is less than 0.");

            if (context.Last != null) throw new NotSupportedException("Not support last.");

            if (context.Before != null) throw new NotSupportedException("Not support before cursor.");

            var hasPreviousPage = false;
            var offset = 0;

            if (!string.IsNullOrEmpty(context.After))
            {
                offset = ConnectionUtils.CursorToOffset(context.After) + 1;
                hasPreviousPage = offset > 0;
            }

            if (hasPreviousPage) query = query.Skip(offset);

            if (context.First != null)
                query = query.Take(context.First.Value);
            else if (take > -1)
                query = query.Take(take);

            var list = converter(query, context);

            var edges = await list.Select((item, i) => new Edge<TKSource>
            {
                Node = item,
                Cursor = ConnectionUtils.OffsetToCursor(offset + i)
            }).ToListAsync(context.CancellationToken).ConfigureAwait(false);

            if (context.First == null && take < 0 && totalCount < 0)
                totalCount = edges.Count + offset;
            else if (context.First > edges.Count)
                totalCount = edges.Count + offset;

            var hasNextPage = edges.Count == take;

            var firstEdge = edges.FirstOrDefault();
            var lastEdge = edges.LastOrDefault();

            return new Connection<TKSource>
            {
                Edges = edges,
                TotalCount = totalCount,
                PageInfo = new PageInfo
                {
                    StartCursor = firstEdge?.Cursor,
                    EndCursor = lastEdge?.Cursor,
                    HasPreviousPage = hasPreviousPage,
                    HasNextPage = hasNextPage,
                }
            };
        }

        /// <summary>
        /// Convert async source to connection.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TParent"></typeparam>
        /// <param name="query"></param>
        /// <param name="context"></param>
        /// <param name="totalCount"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static async Task<Connection<TSource>> ToConnectionAsync<TSource, TParent>(
            this IAsyncEnumerable<TSource> query,
            IResolveConnectionContext<TParent> context,
            int take = -1,
            int totalCount = -1)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.First < 0) throw new ArgumentOutOfRangeException(nameof(context), "context.First is less than 0.");

            if (context.Last != null) throw new NotSupportedException("Not support last.");

            if (context.Before != null) throw new NotSupportedException("Not support before cursor.");

            var hasPreviousPage = false;
            var offset = 0;

            if (!string.IsNullOrEmpty(context.After))
            {
                offset = ConnectionUtils.CursorToOffset(context.After) + 1;
                hasPreviousPage = offset > 0;
            }

            if (hasPreviousPage) query = query.Skip(offset);

            if (context.First != null)
                query = query.Take(context.First.Value);
            else if (take > 0)
                query = query.Take(take);

            var edges = await query.Select((item, i) => new Edge<TSource>
            {
                Node = item,
                Cursor = ConnectionUtils.OffsetToCursor(offset + i)
            }).ToListAsync(context.CancellationToken).ConfigureAwait(false);

            if (context.First == null && take < 0 && totalCount < 0)
                totalCount = edges.Count + offset;
            else if (context.First > edges.Count)
                totalCount = edges.Count + offset;

            var hasNextPage = edges.Count == take;

            var firstEdge = edges.FirstOrDefault();
            var lastEdge = edges.LastOrDefault();

            return new Connection<TSource>
            {
                Edges = edges,
                TotalCount = totalCount,
                PageInfo = new PageInfo
                {
                    StartCursor = firstEdge?.Cursor,
                    EndCursor = lastEdge?.Cursor,
                    HasPreviousPage = hasPreviousPage,
                    HasNextPage = hasNextPage,
                }
            };
        }
    }
}
