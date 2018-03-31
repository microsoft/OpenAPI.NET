// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiInfo"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiInfoRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiInfo> FieldIsRequired =>
            new ValidationRule<OpenApiInfo>(
                (context, item) =>
                {

                    // title
                    context.Enter("title");
                    if (String.IsNullOrEmpty(item.Title))
                    {
                        OpenApiError error = new OpenApiError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "title", "info"));
                        context.AddError(error);
                    }
                    context.Exit();

                    // version
                    context.Enter("version");
                    if (String.IsNullOrEmpty(item.Version))
                    {
                        OpenApiError error = new OpenApiError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "version", "info"));
                        context.AddError(error);
                    }
                    context.Exit();

                });

        // add more rule.
    }
}
