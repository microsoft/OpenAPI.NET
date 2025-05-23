using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Dictionary extension methods
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns a new dictionary with entries sorted by key using a custom comparer.
        /// </summary>
        public static IDictionary<TKey, TValue> Sort<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            IComparer<TKey> comparer)
            where TKey : notnull
        {
#if NET7_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(nameof(source));
            ArgumentNullException.ThrowIfNull(nameof(comparer));
#else
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
#endif
            return source.OrderBy(kvp => kvp.Key, comparer)
                               .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Sorts any IEnumerable<T> using the specified comparer and returns a List</T>.
        /// </summary>
        public static List<T> Sort<T>(this IEnumerable<T> source, IComparer<T> comparer)
        {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(comparer);
#else
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
#endif
            return source.OrderBy(item => item, comparer).ToList();
        }
    }
}
