using GraphQL.Types;

namespace Contoso.Unicorn.GraphQL.Types
{
    /// <inheritdoc />
    public sealed class LinkArgumentInputType : InputObjectGraphType<LinkArgumentInput>
    {
        /// <inheritdoc />
        public LinkArgumentInputType()
        {
            Name = "LinkArgumentInputType";

            Description = "link argument input type.";

            Field(_ => _.Predicate, true).Description("An expression string to test each element for a condition.");

            Field(_ => _.Args, true).Description("An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings that contains elements from the input sequence that satisfy the condition specified by predicate.");

            const string deprecate = "Obsoleted! Please use all = false for INNER, all = true for OUTER";

            Field(_ => _.Type, true).Description(deprecate).DeprecationReason(deprecate);

            Field(_ => _.All, true).Description("Link all nodes with default false for inner join, true for left outer join.").DefaultValue();
        }
    }
}