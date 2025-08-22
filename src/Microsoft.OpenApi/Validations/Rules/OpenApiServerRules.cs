// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
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
                    if (server.Url == null)
                    {
                        context.Enter("url");
                        context.CreateError(nameof(ServerRequiredFields),
                            string.Format(SRResource.Validation_FieldIsRequired, "url", "server"));
                        context.Exit();
                    }

                    if (server.Variables is not null)
                    {
                        context.Enter("variables");
                        foreach (var variable in server.Variables)
                        {
                            context.Enter(variable.Key);
                            ValidateServerVariableRequiredFields(context, variable.Key, variable.Value);
                            context.Exit();
                        }
                        context.Exit();
                    }
                });

        /// <summary>
        /// Validate required fields in server variable
        /// </summary>
        private static void ValidateServerVariableRequiredFields(IValidationContext context, string key, OpenApiServerVariable item)
        {
            if (string.IsNullOrEmpty(item.Default))
            {
                context.Enter("default");
                context.CreateError("ServerVariableMustHaveDefaultValue",
                    string.Format(SRResource.Validation_FieldIsRequired, "default", key));
                context.Exit();
            }
        }
    }
}
