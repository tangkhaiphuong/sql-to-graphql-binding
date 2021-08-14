namespace Contoso.Unicorn.GraphQL.Mutations
{
    /// <summary>
    /// Define mutation action.
    /// </summary>
    public enum MutationAction
    {
        /// <summary>
        /// Create action.
        /// </summary>
        Create = 1,

        /// <summary>
        /// Modify action.
        /// </summary>
        Modify = 2,

        /// <summary>
        /// Remove action.
        /// </summary>
        Remove = 4
    }
}