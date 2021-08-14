using Contoso.Unicorn.GraphQL.Queries;

namespace Contoso.Unicorn.GraphQL.Types
{
    /// <summary>
    /// Presentation of link argument input.
    /// </summary>
    public class LinkArgumentInput
    {
        /// <summary>
        /// Gets or sets predicate.
        /// </summary>
        public string Predicate { get; set; }

        /// <summary>
        /// Gets or sets args.
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        public string[] Args { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        public QueryLink? Type { get; set; }

        /// <summary>
        /// Gets or sets all.
        /// </summary>
        public bool All { get; set; }
    }
}