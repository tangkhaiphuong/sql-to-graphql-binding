using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using Contoso.Unicorn.Entities;
using Contoso.Unicorn.GraphQL.Proxies;
using Fluid;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Relay.Types;
using GraphQL.Types.Relay.DataObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Z.EntityFramework.Plus;

namespace Contoso.Unicorn.GraphQL.Mutations
{
    /// <inheritdoc />
    public partial class UnicornMutation
    {
        /// <summary>
        /// Resolve mutation by DbSet factory.
        /// </summary>
        /// <typeparam name="T">Entity data type.</typeparam>
        /// <param name="factory">DbSet factory</param>
        /// <param name="proxyFactory"></param>
        /// <returns></returns>
        public static Func<IResolveConnectionContext<object>, Task<object>> ResolveMutation<T>(
            Func<UnicornContext, DbSet<T>> factory,
            Func<T, BaseProxy> proxyFactory)
            where T : BaseEntity, new()
        {
            return async (resolveConnectionContext) =>
            {
                if (resolveConnectionContext == null) throw new ArgumentNullException(nameof(resolveConnectionContext));

                await ((Func<Task>)resolveConnectionContext.UserContext["transaction"])().ConfigureAwait(false);

                var serviceProvider = resolveConnectionContext.UserContext[nameof(ServiceProvider)] as IServiceProvider;

                var unicornContext = serviceProvider!.GetService<UnicornContext>();

                var dbSet = factory(unicornContext);

                var nodes = resolveConnectionContext.GetArgument<object[]>("nodes")?.OfType<IDictionary<string, object>>().ToArray();
                var node = (IDictionary<string, object>)resolveConnectionContext.GetArgument<object>("node");
                var field = resolveConnectionContext.GetArgument<object[]>("fields")?.OfType<string>()
                    .ToHashSet(StringComparer.InvariantCultureIgnoreCase);
                var predicate = resolveConnectionContext.GetArgument<string>("predicate")?.Trim();
                var ordering = resolveConnectionContext.GetArgument<string>("ordering")?.Trim();
                var args = resolveConnectionContext.GetArgument<object[]>("args");
                var ignore = resolveConnectionContext.GetArgument<bool?>("ignore");
                var skip = resolveConnectionContext.GetArgument<bool?>("skip");
                var multiple = resolveConnectionContext.GetArgument<bool>("multiple");
                var template = resolveConnectionContext.GetArgument<string>("template");
                var force = (bool)resolveConnectionContext.UserContext["force"];
                var take = (int)resolveConnectionContext.UserContext["paging"];
                var action = resolveConnectionContext.GetArgument<MutationAction>("action");
                var liquid = (bool)resolveConnectionContext.UserContext["template"];
                var global = resolveConnectionContext.UserContext["global"] as ConcurrentDictionary<string, object>;

                Connection<BaseProxy> result;
                switch (action)
                {
                    case MutationAction.Create:
                        {
                            if (multiple)
                                result = await CreateMultipleNodeAsync(serviceProvider, resolveConnectionContext, global, dbSet, field, node, nodes, template, predicate, args, ignore.GetValueOrDefault(false), skip.GetValueOrDefault(false), liquid, proxyFactory).ConfigureAwait(false);
                            else
                                result = await CreateSingleNodeAsync(serviceProvider, resolveConnectionContext, global, dbSet, field, node, nodes, template, predicate, args, ignore.GetValueOrDefault(false), liquid, proxyFactory).ConfigureAwait(false);
                            break;
                        }
                    case MutationAction.Modify:
                        {
                            if (ignore != null) throw new NotSupportedException("Not support ignore for modify.");
                            if (skip != null) throw new NotSupportedException("Not support skip for modify.");
                            if (multiple)
                                result = await ModifyMultipleNodeAsync(serviceProvider, resolveConnectionContext, global, dbSet, field, node, nodes, template, predicate, args, force, ordering, take, liquid, proxyFactory).ConfigureAwait(false);
                            else result = await ModifySingleNodeAsync(serviceProvider, resolveConnectionContext, dbSet, field, node, nodes, predicate, args, force, ordering, take, liquid, proxyFactory).ConfigureAwait(false);
                            break;
                        }
                    case MutationAction.Remove:
                        {
                            if (ignore != null) throw new NotSupportedException("Not support ignore for remove.");
                            if (skip != null) throw new NotSupportedException("Not support skip for remove.");
                            if (multiple)
                                result = await RemoveMultipleNodeAsync(serviceProvider, resolveConnectionContext, global, dbSet, node, nodes, template, predicate, args, force, ordering, take, liquid, proxyFactory).ConfigureAwait(false);
                            else
                                result = await RemoveSingleNodeAsync(serviceProvider, resolveConnectionContext, dbSet, node, nodes, predicate, args, force, ordering, take, liquid, proxyFactory).ConfigureAwait(false);
                            break;
                        }
                    default:
                        throw new NotSupportedException();
                }

                result.TotalCount = await unicornContext.SaveChangesAsync(resolveConnectionContext.CancellationToken)
                    .ConfigureAwait(false);

                if (liquid)
                {
                    global[resolveConnectionContext.FieldAst.Alias ?? resolveConnectionContext.FieldAst.Name] = result;
                }

                return result;
            };
        }

        #region SingleNode

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static async Task<Connection<BaseProxy>> CreateSingleNodeAsync<T>(
            IServiceProvider serviceProvider,
            IResolveConnectionContext<object> resolveConnectionContext,
            ConcurrentDictionary<string, object> global,
            DbSet<T> dbSet,
            ISet<string> field,
            IDictionary<string, object> node,
            IDictionary<string, object>[] nodes,
            string template,
            string predicate,
            object[] args,
            bool ignore,
            bool isEnabledLiquid,
            Func<T, BaseProxy> proxyFactory)
                where T : BaseEntity, new()
        {
            var templateOptions = serviceProvider.GetService<TemplateOptions>();
            var fluidParser = new FluidParser();

            var utcNow = DateTimeOffset.Now;
            var createdDateName = nameof(BaseEntity.CreatedDate).ToCamelCase();
            var modifiedDateName = nameof(BaseEntity.ModifiedDate).ToCamelCase();
            var deletedDateName = nameof(BaseEntity.DeletedDate).ToCamelCase();
            var idName = nameof(BaseEntity.Id).ToCamelCase();

            object lastGuid = "";

            var manyNodes = Enumerable.Empty<IDictionary<string, object>>();

            if (node != null) manyNodes = manyNodes.Append(node);
            if (nodes != null) manyNodes = manyNodes.Concat(nodes);

            if (!string.IsNullOrEmpty(template) && isEnabledLiquid)
            {
                var templateContext = new TemplateContext(templateOptions);
                templateContext.SetValue("global", global);
                templateContext.SetValue("args", args);

                var nodeTemplates = await fluidParser.Parse(template).RenderAsync(templateContext).ConfigureAwait(false);

                manyNodes = manyNodes.Concat(JsonSerializer.Deserialize<IList<IDictionary<string, object>>>(nodeTemplates)!);
            }

            var updatedEntities = new LinkedList<T>();
            var createdEntities = new LinkedList<T>();

            if (!string.IsNullOrEmpty(predicate))
            {
                var templateContext = new TemplateContext(templateOptions);
                templateContext.SetValue("node", node);
                templateContext.SetValue("nodes", nodes);
                templateContext.SetValue("items", manyNodes);

                var queryable = dbSet.AsNoTracking();

                if (isEnabledLiquid)
                {
                    predicate = await fluidParser.Parse(predicate).RenderAsync(templateContext).ConfigureAwait(false);
                    predicate = predicate?.Trim();
                }

                if (!string.IsNullOrEmpty(predicate))
                {
                    queryable = args == null ? queryable.Where(predicate) : queryable.Where(predicate, args);
                }

                var entities = await queryable
                    .WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T))
                    .ToListAsync(resolveConnectionContext.CancellationToken).ConfigureAwait(false);

                if (entities.Any())
                {
                    if (ignore) // Update
                    {
                        var updatedNodes = new LinkedList<IDictionary<string, object>>();

                        foreach (var item in manyNodes)
                        {
                            if (!item.ContainsKey(modifiedDateName))
                                item[modifiedDateName] = utcNow;

                            updatedNodes.AddLast(item);
                        }

                        var updatedNodeNode = updatedNodes.First;

                        for (var index = 0;
                            index < Math.Min(entities.Count, updatedNodes.Count) && updatedNodeNode != null;
                            ++index, updatedNodeNode = updatedNodeNode.Next)
                        {
                            var entity = entities[index];
                            var updatedNode = updatedNodeNode.Value;

                            entity.Update(field == null
                                ? updatedNode
                                : updatedNode.Where(d => field.Contains(d.Key)));

                            if (updatedNode.ContainsKey(modifiedDateName) == false ||
                                entity.ModifiedDate == null)
                                entity.ModifiedDate = utcNow;

                            updatedEntities.AddLast(entity);
                        }
                    }
                    else
                    {
                        var createdNodes = new LinkedList<IDictionary<string, object>>();

                        foreach (var item in manyNodes)
                        {
                            if (!item.ContainsKey(createdDateName))
                                item[createdDateName] = utcNow;

                            if (!item.ContainsKey(idName))
                            {
                                do
                                {
                                    item[idName] = Guid.NewGuid().ToString();
                                } while (Equals(item[idName], lastGuid));

                                lastGuid = item[idName];
                            }

                            createdNodes.AddLast(item);
                        }


                        foreach (var createdEntity in createdNodes.Select(createdNode =>
                            new T().Update(field == null
                                ? createdNode
                                : createdNode.Where(keyValue => field.Contains(keyValue.Key)))))
                            createdEntities.AddLast(createdEntity);
                    }
                }
                else
                {
                    if (ignore)
                    {
                        var createdNodes = new LinkedList<IDictionary<string, object>>();

                        foreach (var item in manyNodes)
                        {
                            if (!item.ContainsKey(createdDateName))
                                item[createdDateName] = utcNow;

                            if (!item.ContainsKey(idName))
                            {
                                do
                                {
                                    item[idName] = Guid.NewGuid().ToString();
                                } while (Equals(item[idName], lastGuid));

                                lastGuid = item[idName];
                            }

                            createdNodes.AddLast(item);
                        }

                        foreach (var createdEntity in createdNodes.Select(createdNode =>
                            new T().Update(field == null
                                ? createdNode
                                : createdNode.Where(keyValue => field.Contains(keyValue.Key)))))
                            createdEntities.AddLast(createdEntity);
                    }
                }
            }
            else
            {
                var createdNodes = new LinkedList<IDictionary<string, object>>();

                if (ignore == false)
                {
                    foreach (var item in manyNodes)
                    {
                        if (!item.ContainsKey(createdDateName))
                            item[createdDateName] = utcNow;

                        if (!item.ContainsKey(idName))
                        {
                            do
                            {
                                item[idName] = Guid.NewGuid().ToString();
                            } while (Equals(item[idName], lastGuid));

                            lastGuid = item[idName];
                        }

                        createdNodes.AddLast(item);
                    }

                    foreach (var createdEntity in createdNodes.Select(createdNode =>
                        new T().Update(field == null
                            ? createdNode
                            : createdNode.Where(d => field.Contains(d.Key)))))
                        createdEntities.AddLast(createdEntity);


                }
                else
                {
                    var remainNodes = new Dictionary<object, IDictionary<string, object>>();

                    foreach (var item in manyNodes)
                    {
                        item.TryGetValue(idName, out var id);

                        if (!item.ContainsKey(createdDateName))
                            item[createdDateName] = utcNow;

                        if (id == null)
                        {
                            do
                            {
                                item[idName] = Guid.NewGuid().ToString();
                            } while (Equals(item[idName], lastGuid));

                            lastGuid = item[idName];

                            createdNodes.AddLast(item);
                        }
                        else
                        {
                            remainNodes[id] = item;
                        }
                    }

                    if (remainNodes.Count > 0)
                    {
                        var ids = string.Join(", ",
                            remainNodes.Keys.Select(id => JsonSerializer.Serialize(id)).ToArray());

                        var idPredicate = $"{idName} in ({ids})";

                        var queryable = dbSet.Where(idPredicate);

                        var entities = await queryable
                            .WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T))
                            .ToListAsync().ConfigureAwait(false);

                        foreach (var entity in entities)
                        {
                            remainNodes.Remove(entity.Id, out var item);

                            entity.Update(field == null ? item : item.Where(keyValue => field.Contains(keyValue.Key)));

                            if (item.ContainsKey(modifiedDateName) == false ||
                                entity.ModifiedDate == null)
                                entity.ModifiedDate = utcNow;

                            if (item.ContainsKey(deletedDateName) == false)
                                entity.DeletedDate = null;

                            updatedEntities.AddLast(entity);
                        }

                        foreach (var item in remainNodes.Values)
                        {
                            if (!item.ContainsKey(createdDateName))
                                item[createdDateName] = utcNow;

                            if (!item.ContainsKey(idName))
                            {
                                do
                                {
                                    item[idName] = Guid.NewGuid().ToString();
                                } while (item[idName] == lastGuid);

                                lastGuid = item[idName];
                            }

                            createdNodes.AddLast(item);
                        }
                    }

                    foreach (var createdEntity in createdNodes.Select(createdNode =>
                        new T().Update(field == null
                            ? createdNode
                            : createdNode.Where(d => field.Contains(d.Key)))))
                        createdEntities.AddLast(createdEntity);
                }
            }

            if (createdEntities.Any())
                await dbSet.AddRangeAsync(createdEntities,
                    resolveConnectionContext.CancellationToken)
                .ConfigureAwait(false);

            if (updatedEntities.Any())
                dbSet.UpdateRange(updatedEntities);

            var result = ConnectionUtils.ToConnection(createdEntities.Concat(updatedEntities).Select(proxyFactory), resolveConnectionContext);

            return result;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static async Task<Connection<BaseProxy>> ModifySingleNodeAsync<T>(
            IServiceProvider serviceProvider,
            IResolveConnectionContext<object> resolveConnectionContext,
            DbSet<T> dbSet,
            ISet<string> field,
            IDictionary<string, object> node,
            IDictionary<string, object>[] nodes,
            string predicate,
            object[] args,
            bool force,
            string ordering,
            int take,
            bool isEnabledLiquid,
            Func<T, BaseProxy> proxyFactory)
                where T : BaseEntity, new()
        {
            var templateOptions = serviceProvider.GetService<TemplateOptions>();
            var fluidParser = new FluidParser();

            var modifiedDateName = nameof(BaseEntity.ModifiedDate).ToCamelCase();

            var utcNow = DateTimeOffset.Now;

            var queryable = dbSet.AsTracking();

            if (!force) queryable = queryable.Where(_ => _.DeletedDate == null);

            Connection<BaseProxy> result;

            if (!string.IsNullOrEmpty(predicate))
            {
                var templateContext = new TemplateContext(templateOptions);
                templateContext.SetValue("node", node);
                templateContext.SetValue("nodes", nodes);

                if (isEnabledLiquid)
                {
                    predicate = await fluidParser.Parse(predicate).RenderAsync(templateContext).ConfigureAwait(false);
                    predicate = predicate?.Trim();
                }

                if (!string.IsNullOrEmpty(predicate))
                {
                    queryable = args == null ? queryable.Where(predicate) : queryable.Where(predicate, args);
                }
            }

            if (!string.IsNullOrEmpty(ordering))
                queryable = queryable.OrderBy(ordering);

            if (node != null && nodes != null)
            {
                result = await queryable.WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T)).ToConnectionAsync(resolveConnectionContext, take: take,
                    converter: (_, __) => _.ToAsyncEnumerable().Select(proxyFactory)).ConfigureAwait(false);

                var hasModifiedDate = node.ContainsKey(modifiedDateName);

                dbSet.UpdateRange(result.Items.Select(_ => _.Entity).OfType<T>().Select((entity, index) =>
                {
                    if (index < nodes.Length)
                    {
                        var updatedNode = nodes[index];

                        entity = entity.Update(field == null ? updatedNode : updatedNode.Where(fieldValue => field.Contains(fieldValue.Key)));

                        if (updatedNode.ContainsKey(modifiedDateName) == false ||
                            entity.ModifiedDate == null)
                            entity.ModifiedDate = utcNow;
                    }
                    else
                    {
                        entity = entity.Update(field == null ? node : node.Where(fieldValue => field.Contains(fieldValue.Key)));

                        if (hasModifiedDate == false || entity.ModifiedDate == null)
                            entity.ModifiedDate = utcNow;
                    }

                    return entity;
                }));
            }
            else if (node != null)
            {
                result = await queryable.WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T)).ToConnectionAsync(resolveConnectionContext, take: take,
                    converter: (_, __) => _.ToAsyncEnumerable().Select(proxyFactory)).ConfigureAwait(false);

                var hasModifiedDate = node.ContainsKey(modifiedDateName);
                
                dbSet.UpdateRange(result.Items.Select(_ => _.Entity).OfType<T>().Select(entity =>
                {
                    entity = entity.Update(field == null ? node : node.Where(keyValue => field.Contains(keyValue.Key)));

                    if (hasModifiedDate == false || entity.ModifiedDate == null)
                        entity.ModifiedDate = utcNow;

                    return entity;
                }));
            }
            else if (nodes != null)
            {
                result = await queryable.WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T)).ToConnectionAsync(resolveConnectionContext, take: nodes.Length,
                    converter: (_, __) => _.ToAsyncEnumerable().Select(proxyFactory)).ConfigureAwait(false);

                dbSet.UpdateRange(result.Items.Select(_ => _.Entity).OfType<T>().Select((entity, index) =>
                {
                    var updatedNode = nodes[index];

                    entity = entity.Update(field == null ? updatedNode : updatedNode.Where(keyValue => field.Contains(keyValue.Key)));

                    if (updatedNode.ContainsKey(modifiedDateName) == false ||
                        entity.ModifiedDate == null)
                        entity.ModifiedDate = utcNow;

                    return entity;
                }));
            }
            else
            {
                result = await queryable.WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T)).ToConnectionAsync(resolveConnectionContext, take: take,
                    converter: (_, __) => _.ToAsyncEnumerable().Select(proxyFactory)).ConfigureAwait(false);
            }

            return result;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static async Task<Connection<BaseProxy>> RemoveSingleNodeAsync<T>(
            IServiceProvider serviceProvider,
            IResolveConnectionContext<object> resolveConnectionContext,
            DbSet<T> dbSet,
            IDictionary<string, object> node,
            IDictionary<string, object>[] nodes,
            string predicate,
            object[] args,
            bool force,
            string ordering,
            int take,
            bool isEnabledLiquid,
            Func<T, BaseProxy> proxyFactory)
            where T : BaseEntity, new()
        {
            var templateOptions = serviceProvider.GetService<TemplateOptions>();
            var fluidParser = new FluidParser();

            var utcNow = DateTimeOffset.Now;

            var queryable = dbSet.AsTracking();

            if (!force) queryable = queryable.Where(_ => _.DeletedDate == null);

            if (!string.IsNullOrEmpty(predicate))
            {
                var templateContext = new TemplateContext(templateOptions);
                templateContext.SetValue("node", node);
                templateContext.SetValue("nodes", nodes);

                if (isEnabledLiquid)
                {
                    predicate = await fluidParser.Parse(predicate).RenderAsync(templateContext).ConfigureAwait(false);
                    predicate = predicate?.Trim();
                }

                if (!string.IsNullOrEmpty(predicate))
                {
                    queryable = args == null ? queryable.Where(predicate) : queryable.Where(predicate, args);
                }
            }

            if (!string.IsNullOrEmpty(ordering))
                queryable = queryable.OrderBy(ordering);

            var result = await queryable.WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T)).ToConnectionAsync(resolveConnectionContext, take: take,
                converter: (_, __) => _.ToAsyncEnumerable().Select(proxyFactory)).ConfigureAwait(false);

            if (force)
                dbSet.RemoveRange(result.Items.Select(_ => _.Entity).OfType<T>());
            else
            {
                var deletedDate = utcNow;
                dbSet.UpdateRange(result.Items.Select(_ => _.Entity).OfType<T>().Select(entity =>
                {
                    entity.DeletedDate = deletedDate;
                    return entity;
                }));
            }

            return result;
        }

        #endregion

        #region MultipleNode

        private static async Task<Connection<BaseProxy>> CreateMultipleNodeAsync<T>(
            IServiceProvider serviceProvider,
            IResolveConnectionContext<object> resolveConnectionContext,
            ConcurrentDictionary<string, object> global,
            DbSet<T> dbSet,
            ISet<string> field,
            IDictionary<string, object> node,
            IDictionary<string, object>[] nodes,
            string template,
            string predicate,
            object[] args,
            bool ignore,
            bool skip,
            bool isEnabledLiquid,
            Func<T, BaseProxy> proxyFactory)
                where T : BaseEntity, new()
        {
            var templateOptions = serviceProvider.GetService<TemplateOptions>();
            var fluidParser = new FluidParser();

            var utcNow = DateTimeOffset.Now;
            var createdDateName = nameof(BaseEntity.CreatedDate).ToCamelCase();
            var modifiedDateName = nameof(BaseEntity.ModifiedDate).ToCamelCase();
            var deletedDateName = nameof(BaseEntity.DeletedDate).ToCamelCase();
            var idName = nameof(BaseEntity.Id).ToCamelCase();

            object lastGuid = "";

            var manyNodes = Enumerable.Empty<IDictionary<string, object>>();

            if (node != null) manyNodes = manyNodes.Append(node);
            if (nodes != null) manyNodes = manyNodes.Concat(nodes);

            if (!string.IsNullOrEmpty(template) && isEnabledLiquid)
            {
                var templateContext = new TemplateContext(templateOptions);
                templateContext.SetValue("global", global);
                templateContext.SetValue("args", args);

                var nodeTemplates = await fluidParser.Parse(template).RenderAsync(templateContext).ConfigureAwait(false);

                manyNodes = manyNodes.Concat(JsonSerializer.Deserialize<IList<IDictionary<string, object>>>(nodeTemplates)!);
            }

            var updatedEntities = new LinkedList<T>();
            var createdEntities = new LinkedList<T>();

            if (!string.IsNullOrEmpty(predicate))
            {
                var fluidTemplate = fluidParser.Parse(predicate);

                var queries = new LinkedList<(QueryFutureEnumerable<T> Query, IDictionary<string, object> Node)>();

                foreach (var singleNode in manyNodes)
                {
                    var templateContext = new TemplateContext(templateOptions);
                    templateContext.SetValue("node", node);
                    templateContext.SetValue("nodes", nodes);
                    templateContext.SetValue("items", manyNodes);
                    templateContext.SetValue("this", singleNode);

                    var queryable = dbSet.AsTracking();

                    var newPredicate = predicate;
                    if (isEnabledLiquid)
                    {
                        newPredicate = await fluidTemplate.RenderAsync(templateContext).ConfigureAwait(false);
                        newPredicate = newPredicate?.Trim();
                    }

                    if (!string.IsNullOrEmpty(newPredicate))
                    {
                        queryable = args == null ? queryable.Where(newPredicate) : queryable.Where(newPredicate, args);
                    }

                    queries.AddLast((queryable.WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T)).Future(), singleNode));
                }

                foreach(var (queryable, singleNode) in queries)
                {
                    var entities = await queryable
                        .ToListAsync().ConfigureAwait(false);

                    if (entities.Any())
                    {
                        if (ignore) // Update
                        {
                            if (skip == false)
                            {
                                foreach (var entity in entities.Select(entity =>
                                {
                                    entity.Update(field == null
                                        ? singleNode
                                        : singleNode.Where(d => field.Contains(d.Key)));

                                    if (singleNode.ContainsKey(modifiedDateName) == false ||
                                        entity.ModifiedDate == null)
                                        entity.ModifiedDate = utcNow;

                                    if (singleNode.ContainsKey(deletedDateName) == false)
                                        entity.DeletedDate = null;

                                    return entity;
                                }))
                                    updatedEntities.AddLast(entity);
                            }
                        }
                        else
                        {
                            if (!singleNode.ContainsKey(createdDateName))
                                singleNode[createdDateName] = utcNow;

                            if (!singleNode.ContainsKey(idName))
                            {
                                do
                                {
                                    singleNode[idName] = Guid.NewGuid().ToString();
                                } while (Equals(singleNode[idName], lastGuid));

                                lastGuid = singleNode[idName];
                            }

                            var createdEntity = new T().Update(field == null
                                ? singleNode
                                : singleNode.Where(d => field.Contains(d.Key)));

                            createdEntities.AddLast(createdEntity);
                        }

                        // ignore == true skip
                    }
                    else
                    {
                        if (ignore)
                        {
                            if (!singleNode.ContainsKey(createdDateName))
                                singleNode[createdDateName] = utcNow;

                            if (!singleNode.ContainsKey(idName))
                            {
                                do
                                {
                                    singleNode[idName] = Guid.NewGuid().ToString();
                                } while (Equals(singleNode[idName], lastGuid));

                                lastGuid = singleNode[idName];
                            }

                            var createdEntity = new T().Update(field == null
                                ? singleNode
                                : singleNode.Where(d => field.Contains(d.Key)));

                            createdEntities.AddLast(createdEntity);
                        }
                    }
                }
            }
            else
            {
                var createdNodes = new LinkedList<IDictionary<string, object>>();

                if (ignore == false)
                {
                    foreach (var item in manyNodes)
                    {
                        if (!item.ContainsKey(createdDateName))
                            item[createdDateName] = utcNow;

                        if (!item.ContainsKey(idName))
                        {
                            do
                            {
                                item[idName] = Guid.NewGuid().ToString();
                            } while (Equals(item[idName], lastGuid));

                            lastGuid = item[idName];
                        }

                        createdNodes.AddLast(item);
                    }

                    foreach (var createdEntity in createdNodes.Select(createdNode =>
                        new T().Update(field == null
                            ? createdNode
                            : createdNode.Where(d => field.Contains(d.Key)))))
                        createdEntities.AddLast(createdEntity);

                }
                else
                {
                    var remainNodes = new Dictionary<object, IDictionary<string, object>>();

                    foreach (var item in manyNodes)
                    {
                        item.TryGetValue(idName, out var id);

                        if (!item.ContainsKey(createdDateName))
                            item[createdDateName] = utcNow;

                        if (id == null)
                        {
                            do
                            {
                                item[idName] = Guid.NewGuid().ToString();
                            } while (Equals(item[idName], lastGuid));

                            lastGuid = item[idName];

                            createdNodes.AddLast(item);
                        }
                        else
                        {
                            remainNodes[id] = item;
                        }
                    }

                    if (remainNodes.Count > 0)
                    {
                        var ids = string.Join(", ",
                            remainNodes.Keys.Select(id => JsonSerializer.Serialize(id)).ToArray());

                        var idPredicate = $"{idName} in ({ids})";

                        var queryable = dbSet.Where(idPredicate);

                        var entities = await queryable
                            .WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T))
                            .ToListAsync().ConfigureAwait(false);

                        foreach (var entity in entities)
                        {
                            remainNodes.Remove(entity.Id, out var item);

                            entity.Update(field == null ? item : item.Where(keyValue => field.Contains(keyValue.Key)));

                            if (item.ContainsKey(modifiedDateName) == false ||
                                entity.ModifiedDate == null)
                                entity.ModifiedDate = utcNow;

                            if (item.ContainsKey(deletedDateName) == false)
                                entity.DeletedDate = null;

                            updatedEntities.AddLast(entity);
                        }

                        foreach (var item in remainNodes.Values)
                        {
                            if (!item.ContainsKey(createdDateName))
                                item[createdDateName] = utcNow;

                            if (!item.ContainsKey(idName))
                            {
                                do
                                {
                                    item[idName] = Guid.NewGuid().ToString();
                                } while (item[idName] == lastGuid);

                                lastGuid = item[idName];
                            }

                            createdNodes.AddLast(item);
                        }
                    }

                    foreach (var createdEntity in createdNodes.Select(createdNode =>
                        new T().Update(field == null
                            ? createdNode
                            : createdNode.Where(d => field.Contains(d.Key)))))
                        createdEntities.AddLast(createdEntity);
                }
            }

            if (createdEntities.Any())
                await dbSet.AddRangeAsync(createdEntities,
                        resolveConnectionContext.CancellationToken)
                    .ConfigureAwait(false);

            if (updatedEntities.Any())
                dbSet.UpdateRange(updatedEntities);

            var result = ConnectionUtils.ToConnection(createdEntities.Concat(updatedEntities).Select(proxyFactory), resolveConnectionContext);

            return result;
        }

        private static async Task<Connection<BaseProxy>> ModifyMultipleNodeAsync<T>(
            IServiceProvider serviceProvider,
            IResolveConnectionContext<object> resolveConnectionContext,
            ConcurrentDictionary<string, object> global,
            DbSet<T> dbSet,
            ISet<string> field,
            IDictionary<string, object> node,
            IDictionary<string, object>[] nodes,
            string template,
            string predicate,
            object[] args,
            bool force,
            string ordering,
            int take,
            bool isEnabledLiquid,
            Func<T, BaseProxy> proxyFactory)
            where T : BaseEntity, new()
        {
            var templateOptions = serviceProvider.GetService<TemplateOptions>();
            var fluidParser = new FluidParser();

            var modifiedDateName = nameof(BaseEntity.ModifiedDate).ToCamelCase();

            var utcNow = DateTimeOffset.Now;

            var fluidTemplate = fluidParser.Parse(predicate);

            var indexKey = $"__{DateTime.Now.Ticks}__";

            var manyNodes = new List<IDictionary<string, object>>();

            if (node != null) manyNodes.Add(node);
            if (nodes != null) manyNodes.AddRange(nodes);

            if (!string.IsNullOrEmpty(template) && isEnabledLiquid)
            {
                var templateContext = new TemplateContext(templateOptions);
                templateContext.SetValue("global", global);
                templateContext.SetValue("args", args);

                var nodeTemplates = await fluidParser.Parse(template).RenderAsync(templateContext).ConfigureAwait(false);

                manyNodes.AddRange(JsonSerializer.Deserialize<IList<IDictionary<string, object>>>(nodeTemplates)!);
            }

            async IAsyncEnumerable<BaseProxy> QueryAsyncUpdatedRows()
            {
                var index = 0;

                foreach (var item in manyNodes)
                {
                    var queryable = dbSet.AsTracking();

                    if (!force) queryable = queryable.Where(_ => _.DeletedDate == null);

                    if (!string.IsNullOrEmpty(predicate))
                    {
                        var templateContext = new TemplateContext(templateOptions);
                        templateContext.SetValue("node", node);
                        templateContext.SetValue("nodes", nodes);
                        templateContext.SetValue("items", manyNodes);
                        templateContext.SetValue("this", item);

                        var newPredicate = predicate;
                        if (isEnabledLiquid)
                        {
                            newPredicate = await fluidTemplate.RenderAsync(templateContext).ConfigureAwait(false);
                            newPredicate = newPredicate?.Trim();
                        }

                        if (!string.IsNullOrEmpty(predicate))
                        {
                            queryable = args == null ? queryable.Where(newPredicate) : queryable.Where(newPredicate, args);
                        }
                    }

                    if (!string.IsNullOrEmpty(ordering))
                        queryable = queryable.OrderBy(ordering);

                    await foreach (var record in queryable.WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T)).ToAsyncEnumerable().Select(proxyFactory))
                    {
                        record[indexKey] = index;
                        yield return record;
                    }

                    ++index;
                }
            }

            var result = await QueryAsyncUpdatedRows().ToConnectionAsync(resolveConnectionContext, take).ConfigureAwait(false);

            dbSet.UpdateRange(result.Items.Select(proxy =>
            {
                var updatedNode = manyNodes[(int)proxy[indexKey]];
                var entity = proxy.Entity as T;

                entity = entity.Update(field == null
                    ? updatedNode
                    : updatedNode.Where(d => field.Contains(d.Key)));

                if (updatedNode.ContainsKey(modifiedDateName) == false ||
                    entity.ModifiedDate == null)
                    entity.ModifiedDate = utcNow;

                return entity;
            }));

            return result;
        }

        private static async Task<Connection<BaseProxy>> RemoveMultipleNodeAsync<T>(
            IServiceProvider serviceProvider,
            IResolveConnectionContext<object> resolveConnectionContext,
            ConcurrentDictionary<string, object> global,
            DbSet<T> dbSet,
            IDictionary<string, object> node,
            IDictionary<string, object>[] nodes,
            string template,
            string predicate,
            object[] args,
            bool force,
            string ordering,
            int take,
            bool isEnabledLiquid,
            Func<T, BaseProxy> proxyFactory)
            where T : BaseEntity, new()
        {
            var templateOptions = serviceProvider.GetService<TemplateOptions>();
            var fluidParser = new FluidParser();

            var fluidTemplate = fluidParser.Parse(predicate);

            async IAsyncEnumerable<BaseProxy> QueryAsyncDeletedRows()
            {
                var manyNodes = Enumerable.Empty<IDictionary<string, object>>();

                if (node != null) manyNodes = manyNodes.Append(node);
                if (nodes != null) manyNodes = manyNodes.Concat(nodes);

                if (!string.IsNullOrEmpty(template) && isEnabledLiquid)
                {
                    var templateContext = new TemplateContext(templateOptions);
                    templateContext.SetValue("global", global);
                    templateContext.SetValue("args", args);

                    var nodeTemplates = await fluidParser.Parse(template).RenderAsync(templateContext).ConfigureAwait(false);

                    manyNodes = manyNodes.Concat(JsonSerializer.Deserialize<IList<IDictionary<string, object>>>(nodeTemplates)!);
                }

                foreach (var item in manyNodes)
                {
                    var queryable = dbSet.AsTracking();

                    if (!force) queryable = queryable.Where(_ => _.DeletedDate == null);

                    if (!string.IsNullOrEmpty(predicate))
                    {
                        var templateContext = new TemplateContext(templateOptions);
                        templateContext.SetValue("node", node);
                        templateContext.SetValue("nodes", nodes);
                        templateContext.SetValue("this", item);

                        if (isEnabledLiquid)
                        {
                            predicate = await fluidTemplate.RenderAsync(templateContext).ConfigureAwait(false);
                            predicate = predicate?.Trim();
                        }

                        if (!string.IsNullOrEmpty(predicate))
                        {
                            queryable = args == null ? queryable.Where(predicate) : queryable.Where(predicate, args);
                        }
                    }

                    if (!string.IsNullOrEmpty(ordering))
                        queryable = queryable.OrderBy(ordering);

                    await foreach (var record in queryable.WithHint(SqlServerTableHintFlags.NOLOCK, typeof(T)).ToAsyncEnumerable().Select(proxyFactory))
                        yield return record;
                }
            }

            var utcNow = DateTimeOffset.Now;

            var result = await QueryAsyncDeletedRows().ToConnectionAsync(resolveConnectionContext, take).ConfigureAwait(false);

            if (force)
                dbSet.RemoveRange(result.Items.Select(_ => _.Entity).OfType<T>());
            else
            {
                var deletedDate = utcNow;
                dbSet.UpdateRange(result.Items.Select(_ => _.Entity).OfType<T>().Select(entity =>
                {
                    entity.DeletedDate = deletedDate;
                    return entity;
                }));
            }

            return result;
        }

        #endregion
    }
}