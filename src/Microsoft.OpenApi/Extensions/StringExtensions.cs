// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Attributes;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    internal static class StringExtensions
    {
        private static readonly ConcurrentDictionary<Type, ReadOnlyDictionary<string, object>> EnumDisplayCache = new();

        internal static bool TryGetEnumFromDisplayName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this string displayName, ParsingContext parsingContext, out T result) where T : Enum
        {
            if (TryGetEnumFromDisplayName(displayName, out result))
            {
                return true;
            }

            parsingContext.Diagnostic.Errors.Add(new OpenApiError(parsingContext.GetLocation(), $"Enum value {displayName} is not recognized."));
            return false;

        }
        internal static bool TryGetEnumFromDisplayName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this string displayName, out T result) where T : Enum
        {
            var type = typeof(T);

            var displayMap = EnumDisplayCache.GetOrAdd(type, GetEnumValues<T>);

            if (displayMap.TryGetValue(displayName, out var cachedValue))
            {
                result = (T)cachedValue;
                return true;
            }

            result = default;
            return false;
        }
        private static ReadOnlyDictionary<string, object> GetEnumValues<T>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type enumType) where T : Enum
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.GetCustomAttribute<DisplayAttribute>() is {} displayAttribute)
                {
                    var enumValue = (T)field.GetValue(null);
                    result.Add(displayAttribute.Name, enumValue);
                }
            }
            return new ReadOnlyDictionary<string, object>(result);
        }
        internal static string ToFirstCharacterLowerCase(this string input)
            => string.IsNullOrEmpty(input) ? string.Empty : char.ToLowerInvariant(input[0]) + input.Substring(1);
    }
}
