// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Utilities methods
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Check whether the input argument value is null or not.
        /// </summary>
        /// <typeparam name="T">The input value type.</typeparam>
        /// <param name="value">The input value.</param>
        /// <param name="parameterName">The input parameter name.</param>
        /// <returns>The input value.</returns>
        internal static T CheckArgumentNull<T>(
            T value,
            [CallerArgumentExpression(nameof(value))] string parameterName = "")
        {
            return value ?? throw new ArgumentNullException(parameterName, $"Value cannot be null: {parameterName}");
        }

        /// <summary>
        /// Check whether the input string value is null or empty.
        /// </summary>
        /// <param name="value">The input string value.</param>
        /// <param name="parameterName">The input parameter name.</param>
        /// <returns>The input value.</returns>
        internal static string CheckArgumentNullOrEmpty(
            string value,
            [CallerArgumentExpression(nameof(value))] string parameterName = "")
        {
            return string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(parameterName, $"Value cannot be null or empty: {parameterName}") : value;
        }
    }
}
