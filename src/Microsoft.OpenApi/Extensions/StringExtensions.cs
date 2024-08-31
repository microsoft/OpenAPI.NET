// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> EnumDisplayCache = new();

        /// <summary>
        /// Gets the enum value based on the given enum type and display name.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        public static T GetEnumFromDisplayName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this string displayName)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                return default;

            var displayMap = EnumDisplayCache.GetOrAdd(type, _ => new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase));

            if (displayMap.TryGetValue(displayName, out var cachedValue))
                return (T)cachedValue;


            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null && displayAttribute.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase))
                {
                    var enumValue = (T)field.GetValue(null);
                    displayMap.TryAdd(displayName, enumValue);
                    return enumValue;
                }
            }

            return default;
        }
        internal static string ToFirstCharacterLowerCase(this string input)
            => string.IsNullOrEmpty(input) ? string.Empty : char.ToLowerInvariant(input[0]) + input.Substring(1);
    }
}
