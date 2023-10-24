using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Hidi.Extensions
{
    /// <summary>
    /// Extension class for <see cref="string"/>.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Checks if the specified searchValue is equal to the target string based on the specified <see cref="StringComparison"/>.
        /// </summary>
        /// <param name="target">The target string to compare to.</param>
        /// <param name="searchValue">The search string to seek.</param>
        /// <param name="comparison">The <see cref="StringComparison"/> to use. This defaults to <see cref="StringComparison.OrdinalIgnoreCase"/>.</param>
        /// <returns>true if the searchValue parameter occurs within this string; otherwise, false.</returns>
        public static bool IsEquals(this string? target, string? searchValue, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(target) && string.IsNullOrWhiteSpace(searchValue))
            {
                return true;
            } else if (string.IsNullOrWhiteSpace(target))
            {
                return false;
            }
            return target.Equals(searchValue, comparison);
        }

        /// <summary>
        /// Splits the target string in substrings based on the specified char separator.
        /// </summary>
        /// <param name="target">The target string to split by char. </param>
        /// <param name="separator">The char separator.</param>
        /// <returns>An <see cref="IList{String}"/> containing substrings.</returns>
        public static IList<string> SplitByChar(this string target, char separator)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                return new List<string>();
            }
            return target.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
