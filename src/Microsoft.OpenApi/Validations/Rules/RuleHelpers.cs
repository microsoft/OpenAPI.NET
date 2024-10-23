// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi.Validations.Rules
{
    internal static class RuleHelpers
    {
        /// <summary>
        /// Input string must be in the format of an email address
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>True if it's an email address. Otherwise False.</returns>
        public static bool IsEmailAddress(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var splits = input.Split('@');
            if (splits.Length != 2)
            {
                return false;
            }

            if (string.IsNullOrEmpty(splits[0]) || string.IsNullOrEmpty(splits[1]))
            {
                return false;
            }

            // Add more rules.

            return true;
        }
    }
}
