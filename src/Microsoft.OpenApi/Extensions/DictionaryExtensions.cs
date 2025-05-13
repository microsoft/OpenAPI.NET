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
        public static SortedDictionary<TKey, TValue> Sort<TKey, TValue>(
            this Dictionary<TKey, TValue> source)
            where TKey : notnull
        {
            Utils.CheckArgumentNull(source);

            return new SortedDictionary<TKey, TValue>(source);
        }

        /// <summary>
        /// Returns a new dictionary with entries sorted by key using a custom comparer.
        /// </summary>
        public static SortedDictionary<TKey, TValue> Sort<TKey, TValue>(
            this Dictionary<TKey, TValue> source,
            IComparer<TKey> comparer)
            where TKey : notnull
        {
            Utils.CheckArgumentNull(source);
            Utils.CheckArgumentNull(comparer);

            return new SortedDictionary<TKey, TValue>(source, comparer);
        }
    }
}
