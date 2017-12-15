// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiComponents"/>.
    /// </summary>
    public static class OpenApiComponentsRules
    {
        /// <summary>
        /// The key regex.
        /// </summary>
        public static Regex KeyRegex = new Regex(@"^[a-zA-Z0-9\.\-_]+$");

        /// <summary>
        /// All the fixed fields declared above are objects
        /// that MUST use keys that match the regular expression: ^[a-zA-Z0-9\.\-_]+$.
        /// </summary>
        public static readonly ValidationRule<OpenApiComponents> KeyMustBeRegularExpression =
            new ValidationRule<OpenApiComponents>(
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

        private static void ValidateKeys(ValidationContext context, IEnumerable<string> keys, string component)
        {
            if (keys == null)
            {
                return;
            }

            foreach(var key in keys)
            {
                if (!KeyRegex.IsMatch(key))
                {
                    ValidationError error = new ValidationError(ErrorReason.Format, context.PathString,
                        string.Format(SRResource.Validataion_ComponentsKeyMustMatchRegularExpr, key, component, KeyRegex.ToString()));
                    context.AddError(error);
                }
            }
        }
    }
}
