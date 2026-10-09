// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiComponents"/>.
    /// </summary>
    [OpenApiRule]
    public static partial class OpenApiComponentsRules
    {
        /// <summary>
        /// The key regex pattern.
        /// </summary>
        internal const string KeyPattern = @"^[a-zA-Z0-9\.\-_]+$";

#if NET8_0_OR_GREATER
        [GeneratedRegex(KeyPattern, RegexOptions.None, matchTimeoutMilliseconds: 100)]
        private static partial Regex KeyRegex();
#endif

        /// <summary>
        /// All the fixed fields declared above are objects
        /// that MUST use keys that match the regular expression: ^[a-zA-Z0-9\.\-_]+$.
        /// </summary>
        public static ValidationRule<OpenApiComponents> KeyMustBeRegularExpression =>
            new(nameof(KeyMustBeRegularExpression),
                (context, components) =>
                {
                    ValidateKeys(context, components.Schemas?.Keys, "schemas");

                    ValidateKeys(context, components.Responses?.Keys, "responses");

                    ValidateKeys(context, components.Parameters?.Keys, "parameters");

                    ValidateKeys(context, components.Examples?.Keys, "examples");

                    ValidateKeys(context, components.RequestBodies?.Keys, "requestBodies");

                    ValidateKeys(context, components.Headers?.Keys, "headers");

                    ValidateKeys(context, components.SecuritySchemes?.Keys, "securitySchemes");

                    ValidateKeys(context, components.Links?.Keys, "links");

                    ValidateKeys(context, components.Callbacks?.Keys, "callbacks");
                });

        private static void ValidateKeys(IValidationContext context, IEnumerable<string>? keys, string component)
        {
            if (keys == null)
            {
                return;
            }

            foreach (var key in keys)
            {
#if NET8_0_OR_GREATER
                var isValidKey = KeyRegex().IsMatch(key);
#else
                var isValidKey = IsValidKey(key);
#endif
                if (!isValidKey)
                {
                    context.CreateError(nameof(KeyMustBeRegularExpression),
                        string.Format(SRResource.Validation_ComponentsKeyMustMatchRegularExpr, key, component, KeyPattern));
                }
            }
        }

        internal static bool IsValidKey(string key)
        {
            // Match the fixed character class without wall-clock timeouts on downlevel targets.
            var length = key.Length;
            // The regex's $ anchor also accepts a single final newline.
            if (length > 0 && key[length - 1] == '\n')
            {
                length--;
            }

            if (length == 0)
            {
                return false;
            }

            for (var i = 0; i < length; i++)
            {
                var character = key[i];
                if (!(character >= 'a' && character <= 'z') &&
                    !(character >= 'A' && character <= 'Z') &&
                    !(character >= '0' && character <= '9') &&
                    character != '.' && character != '-' && character != '_')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
