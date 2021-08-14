using GraphQL;

namespace Contoso.Unicorn.GraphQL.Queries
{
    /// <inheritdoc />
    [GraphQLAuthorize("default")]
    public sealed partial class UnicornQuery
    { }
}
