﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Reflection;
using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the enum value based on the given enum type and display name.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        public static T GetEnumFromDisplayName<T>(this string displayName)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                return default;
            }

            foreach (var value in Enum.GetValues(type))
            {
                var field = type.GetField(value.ToString());

                var displayAttribute = (DisplayAttribute)field.GetCustomAttribute(typeof(DisplayAttribute));
                if (displayAttribute != null && displayAttribute.Name == displayName)
                {
                    return (T)value;
                }
            }

            return default;
        }
        internal static string ToFirstCharacterLowerCase(this string input)
        => string.IsNullOrEmpty(input) ? string.Empty : char.ToLowerInvariant(input[0]) + input.Substring(1);
    }
}
