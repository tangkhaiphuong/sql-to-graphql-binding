using System.Diagnostics.CodeAnalysis;
using Contoso.GraphQL.Types;
using Contoso.Unicorn.GraphQL.Proxies;

namespace Contoso.Unicorn.GraphQL.Types
{
    /// <summary>
    /// Represent base entity input type.
    /// </summary>
    /// <typeparam name="T">The entity base type.</typeparam>
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public abstract class BaseProxyInputType<T> : InputDictionaryGraphType<T> where T : BaseProxy
    {
        /// <inheritdoc />
        protected BaseProxyInputType(string name)
        {
            Name = name;

            Field(_ => _.Id, true).Description("Sets identity.");

            Field(_ => _.CreatedDate, true).Description("Sets created date.");

            Field(_ => _.ModifiedDate, true).Description("Sets modified date.");

            Field(_ => _.DeletedDate, true).Description("Sets deleted date.");

            Field(_ => _.State, true).Description("Sets state.");
        }
    }
}