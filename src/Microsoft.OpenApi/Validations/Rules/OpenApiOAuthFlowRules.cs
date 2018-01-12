// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiOAuthFlow"/>.
    /// </summary>
    [OpenApiRule]
    internal static class OpenApiOAuthFlowRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiOAuthFlow> FieldIsRequired =>
            new ValidationRule<OpenApiOAuthFlow>(
                (context, flow) =>
                {
                    // authorizationUrl
                    context.Enter("authorizationUrl");
                    if (flow.AuthorizationUrl == null)
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "authorizationUrl", "OAuth Flow"));
                        context.AddError(error);
                    }
                    context.Exit();

                    // tokenUrl
                    context.Enter("tokenUrl");
                    if (flow.TokenUrl == null)
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "tokenUrl", "OAuth Flow"));
                        context.AddError(error);
                    }
                    context.Exit();

                    // scopes
                    context.Enter("scopes");
                    if (flow.Scopes == null)
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "scopes", "OAuth Flow"));
                        context.AddError(error);
                    }
                    context.Exit();
                });

        // add more rule.
    }
}
