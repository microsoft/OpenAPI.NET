using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Dictionary extension methods.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Returns a new dictionary with entries sorted by key using the default comparer.
        /// </summary>
        public static IDictionary<TKey, TValue> Sort<TKey, TValue>(
            this IDictionary<TKey, TValue> source)
            where TKey : notnull
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source
                .OrderBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Returns a new dictionary with entries sorted by key using a custom comparer.
        /// </summary>
        public static IDictionary<TKey, TValue> Sort<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            IComparer<TKey> comparer)
            where TKey : notnull
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            return source
                .OrderBy(kvp => kvp.Key, comparer)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
