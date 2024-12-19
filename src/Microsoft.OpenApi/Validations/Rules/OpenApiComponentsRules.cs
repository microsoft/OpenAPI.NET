// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiComponents"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiComponentsRules
    {
        /// <summary>
        /// The key regex.
        /// </summary>
        internal static readonly Regex KeyRegex = new(@"^[a-zA-Z0-9\.\-_]+$");

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

        private static void ValidateKeys(IValidationContext context, IEnumerable<string> keys, string component)
        {
            if (keys == null)
            {
                return;
            }

            foreach (var key in keys)
            {
                if (!KeyRegex.IsMatch(key))
                {
                    context.CreateError(nameof(KeyMustBeRegularExpression),
                        string.Format(SRResource.Validation_ComponentsKeyMustMatchRegularExpr, key, component, KeyRegex.ToString()));
                }
            }
        }
    }
}
