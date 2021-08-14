using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

// ReSharper disable once IdentifierTypo
namespace Contoso
{
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public static partial class Utilities
    {
        /// <summary>
        /// Gets messages.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetMessages(this Exception ex)
        {
            string previousMessage = null;
            while (ex != null)
            {
                if (ex.Message != previousMessage || previousMessage == null)
                    yield return ex.Message;
                previousMessage = ex.Message;
                ex = ex.InnerException;
            }
        }

        /// <summary>
        /// To lower first word in string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref>culture
        ///     <name>culture</name>
        /// </paramref> is null.</exception>
        public static string ToLowerFirst(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            var array = input.ToCharArray();
            array[0] = char.ToLower(array[0], CultureInfo.InvariantCulture);

            return new string(array);
        }

        /// <summary>
        /// To upper first word in string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref>culture
        ///     <name>culture</name>
        /// </paramref> is null.</exception>
        public static string ToUpperFirst(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            var array = input.ToCharArray();
            array[0] = char.ToUpper(array[0], CultureInfo.InvariantCulture);

            return new string(array);
        }

        private static readonly HashSet<char> RemoveChars = new HashSet<char> { ' ', '\r', '\n', '!', '@', '#', '$', '%', '&', '*', '(', ')', '-', '_', '+', '=', '{', '[', '}', ']', '|', '\\', ':', ';', '"', '\'', '<', ',', '>', '.', '?', '/' };

        /// <summary>
        /// To normal text with lower case and remove sign character.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns></returns>
        /// <exception cref="OverflowException">The array is multidimensional and contains more than <see>
        ///     <cref>System.Int32.MaxValue</cref>
        /// </see> elements.</exception>
        public static string ToNormalText(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            var array = input.ToCharArray();
            var leftIndex = 0;
            var rightIndex = array.Length - 1;

            for (; leftIndex < array.Length; ++leftIndex)
            {
                if (RemoveChars.Contains(array[leftIndex])) continue;
                array[leftIndex] = char.ToLower(array[leftIndex], CultureInfo.InvariantCulture);
                break;
            }
            for (; rightIndex > leftIndex; --rightIndex)
            {
                if (RemoveChars.Contains(array[rightIndex])) continue;
                break;
            }

            return new string(array, leftIndex, rightIndex - leftIndex + 1);
        }

        /// <summary>
        /// Checking string is base 64 format.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns></returns>
        /// <exception cref="RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="input">input</paramref> is null.</exception>
        public static bool IsBase64(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var regex = new Regex(
                "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$");
            var match = regex.Match(input);
            return match.Success;
        }

        /// <summary>
        /// Convert value to iso-date
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">The date and time is outside the range of dates supported by the calendar used by <paramref name="provider">provider</paramref>.</exception>
        /// <exception cref="FormatException">The length of <paramref name="format">format</paramref> is 1, and it is not one of the format specifier characters defined for <see cref="T:System.Globalization.DateTimeFormatInfo"></see>.   -or-  <paramref name="format">format</paramref> does not contain a valid custom format pattern.</exception>
        public static string ToIsoDateTimeString(this DateTime value)
        {
            return value.ToUniversalTime().ToString(Constants.IsoDateTimeFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert value to iso-date
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">The date and time is outside the range of dates supported by the calendar used by <paramref name="formatProvider">formatProvider</paramref>.</exception>
        /// <exception cref="FormatException">The length of <paramref name="format">format</paramref> is one, and it is not one of the standard format specifier characters defined for <see cref="T:System.Globalization.DateTimeFormatInfo"></see>.   -or-  <paramref name="format">format</paramref> does not contain a valid custom format pattern.</exception>
        public static string ToIsoDateTimeString(this DateTimeOffset value)
        {
            return value.ToUniversalTime().ToString(Constants.IsoDateTimeFormat, CultureInfo.InvariantCulture);
        }
    }
}
