using GraphQL;

namespace Contoso.Unicorn.GraphQL.Mutations
{
    /// <inheritdoc />
    [GraphQLAuthorize("default")]
    public partial class UnicornMutation
    { }
}
