// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Enumeration type extension methods.
    /// </summary>
    public static class EnumExtensions
    {
        // Cache to store display names of enum values
        private static readonly ConcurrentDictionary<Enum, string> DisplayNameCache = new();

        /// <summary>
        /// Gets an attribute on an enum field value.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to retrieve.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>
        /// The attribute of the specified type or null.
        /// </returns>
        [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Fields are never trimmed for enum types.")]
        public static T GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
        {
            var type = enumValue.GetType();
            // Use GetField to get the field info for the enum value
            var memInfo = type.GetField(enumValue.ToString(), BindingFlags.Public | BindingFlags.Static);

            if (memInfo == null)
                return null;

            // Retrieve the custom attributes of type T
            var attributes = memInfo.GetCustomAttributes<T>(false);
            return attributes.FirstOrDefault();
        }

        /// <summary>
        /// Gets the enum display name.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>
        /// Use <see cref="DisplayAttribute"/> if it exists.
        /// Otherwise, use the standard string representation.
        /// </returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            // Retrieve the display name from the cache if it exists
            return DisplayNameCache.GetOrAdd(enumValue, e =>
            {
                // Get the DisplayAttribute
                var attribute = e.GetAttributeOfType<DisplayAttribute>();

                // Return the DisplayAttribute name if it exists, otherwise return the enum's string representation
                return attribute == null ? e.ToString() : attribute.Name;
            });
        }
    }
}
