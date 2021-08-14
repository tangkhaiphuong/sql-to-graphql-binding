namespace Contoso.Unicorn.GraphQL
{
    /// <inheritdoc />
    public class GraphQLRequest : global::GraphQL.Server.GraphQLRequest
    {
        public string Template { get; set; }
    }
}