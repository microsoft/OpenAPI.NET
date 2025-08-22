// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiOAuthFlow"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiOAuthFlowRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiOAuthFlow> OAuthFlowRequiredFields =>
            new(nameof(OAuthFlowRequiredFields),
                (context, flow) =>
                {
                    // authorizationUrl
                    if (flow.AuthorizationUrl == null)
                    {
                        context.Enter("authorizationUrl");
                        context.CreateError(nameof(OAuthFlowRequiredFields),
                            string.Format(SRResource.Validation_FieldIsRequired, "authorizationUrl", "OAuth Flow"));
                        context.Exit();
                    }

                    // tokenUrl
                    if (flow.TokenUrl == null)
                    {
                        context.Enter("tokenUrl");
                        context.CreateError(nameof(OAuthFlowRequiredFields),
                            string.Format(SRResource.Validation_FieldIsRequired, "tokenUrl", "OAuth Flow"));
                        context.Exit();
                    }

                    // scopes
                    if (flow.Scopes == null)
                    {
                        context.Enter("scopes");
                        context.CreateError(nameof(OAuthFlowRequiredFields),
                            string.Format(SRResource.Validation_FieldIsRequired, "scopes", "OAuth Flow"));
                        context.Exit();
                    }
                });
    }
}
