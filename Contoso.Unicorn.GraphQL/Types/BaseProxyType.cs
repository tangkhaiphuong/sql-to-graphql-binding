using System.Diagnostics.CodeAnalysis;
using Contoso.Unicorn.GraphQL.Proxies;
using GraphQL.Relay.Types;

namespace Contoso.Unicorn.GraphQL.Types
{
    /// <summary>
    /// Represent base entity type.
    /// </summary>
    /// <typeparam name="T">The entity base type.</typeparam>
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public abstract class BaseProxyType<T> : AsyncNodeGraphType<T> where T : BaseProxy
    {
        /// <inheritdoc />
        protected BaseProxyType(string name)
        {
            Name = name;

            Id(_ => _.Id);

            Field(_ => _.CreatedDate).Description("Gets created date.");

            Field(_ => _.ModifiedDate, true).Description("Gets modified date.");

            Field(_ => _.DeletedDate, true).Description("Gets deleted date.");

            Field(_ => _.State, true).Description("Gets state.");
        }
    }
}