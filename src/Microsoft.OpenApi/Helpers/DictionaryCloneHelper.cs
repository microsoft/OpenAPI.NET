// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Helpers
{
    /// <summary>
    /// Helper class for deep cloning dictionaries.
    /// </summary>
    internal class DictionaryCloneHelper
    {
        /// <summary>
        /// Deep clone key value pairs in a dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the key of the dictionary.</typeparam>
        /// <typeparam name="U">The type of the value of the dictionary.</typeparam>
        /// <param name="dictionary">The target dictionary to clone.</param>
        /// <returns>The cloned dictionary.</returns>
        internal static Dictionary<T, U> Clone<T, U>(IDictionary<T, U> dictionary)
        {            
            var clonedDictionary = dictionary is null ? null : new Dictionary<T, U>();

            if (dictionary != null)
            {
                foreach (var kvp in dictionary)
                {
                    // Create instance of the specified type using the constructor matching the specified parameter types.
                    clonedDictionary[kvp.Key] = (U)Activator.CreateInstance(kvp.Value.GetType(), kvp.Value);
                }
            }

            return clonedDictionary;
        }
    }
}
