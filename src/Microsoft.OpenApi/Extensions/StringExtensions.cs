// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
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

        /// <summary>
        /// Capitalizes each letter of a word in a string.
        /// </summary>
        /// <param name="input">String containing the words to be capitalized, delimited by the '-' character.</param>
        /// <returns>String value with each word capitalized and concatenated.</returns>
        public static string ToPascalCase(this string input)
            => string.IsNullOrEmpty(input) ? input : string.Join(
                                                        null, input.Split(
                                                            new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)
                                                                            .Select(ToFirstCharacterUpperCase));

        /// <summary>
        /// Capitalizes the first letter of an input string.
        /// </summary>
        /// <param name="input">String with first letter to be capitalized. </param>
        /// <returns>The string value with the first letter capitalized.</returns>
        public static string ToFirstCharacterUpperCase(this string input)
           => string.IsNullOrEmpty(input) ? input : char.ToUpperInvariant(input.FirstOrDefault()) + input.Substring(1);
    }
}
