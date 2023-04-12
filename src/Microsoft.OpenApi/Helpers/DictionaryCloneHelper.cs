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
            if (dictionary is null) return null;
            
            var clonedDictionary = new Dictionary<T, U>(dictionary.Keys.Count);
            var clonedObjects = new Dictionary<object, object>();
            
            foreach (var keyValuePair in dictionary)
            {
                // If the object has already been cloned, use the cloned object instead of cloning it again
                if (clonedObjects.TryGetValue(keyValuePair.Value, out var clonedValue))
                {
                    clonedDictionary[keyValuePair.Key] = (U)clonedValue;
                }
                else
                {
                    // Create instance of the specified type using the constructor matching the specified parameter types.
                    clonedDictionary[keyValuePair.Key] = (U)Activator.CreateInstance(keyValuePair.Value.GetType(), keyValuePair.Value);
                    
                    // Add the cloned object to the dictionary of cloned objects
                    clonedObjects.Add(keyValuePair.Value, clonedDictionary[keyValuePair.Key]);
                }                
            }            

            return clonedDictionary;
        }
    }
}
