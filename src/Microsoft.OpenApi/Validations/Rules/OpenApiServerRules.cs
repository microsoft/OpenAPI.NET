// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiServer"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiServerRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiServer> ServerRequiredFields =>
            new(nameof(ServerRequiredFields),
                (context, server) =>
                {
                    context.Enter("url");
                    if (server.Url == null)
                    {
                        context.CreateError(nameof(ServerRequiredFields),
                            String.Format(SRResource.Validation_FieldIsRequired, "url", "server"));
                    }

                    context.Exit();
                    context.Enter("variables");
                    foreach (var variable in server.Variables)
                    {
                        context.Enter(variable.Key);
                        ValidateServerVariableRequiredFields(context, variable.Key, variable.Value);
                        context.Exit();
                    }
                    context.Exit();
                });

        // add more rules

        /// <summary>
        /// Validate required fields in server variable
        /// </summary>
        private static void ValidateServerVariableRequiredFields(IValidationContext context, string key, OpenApiServerVariable item)
        {
            context.Enter("default");
            if (string.IsNullOrEmpty(item.Default))
            {
                context.CreateError("ServerVariableMustHaveDefaultValue",
                    String.Format(SRResource.Validation_FieldIsRequired, "default", key));
            }
            context.Exit();
        }
    }
}
