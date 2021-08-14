// ReSharper disable once IdentifierTypo

using System.Diagnostics.CodeAnalysis;

// ReSharper disable once IdentifierTypo
namespace Contoso
{
    /// <summary>
    /// Defined some constant.
    /// </summary>
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public static partial class Constants
    {
        /// <summary>
        /// Define iso date time format.
        /// </summary>
        public const string IsoDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
    }
}