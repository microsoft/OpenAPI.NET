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
    internal static class OpenApiInfoRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiInfo> FieldIsRequired =>
            new ValidationRule<OpenApiInfo>(
                (context, item) =>
                {
                    // title
                    context.Push("title");
                    if (String.IsNullOrEmpty(item.Title))
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "url", "info"));
                        context.AddError(error);
                    }
                    context.Pop();

                    // version
                    context.Push("version");
                    if (String.IsNullOrEmpty(item.Version))
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "version", "info"));
                        context.AddError(error);
                    }
                    context.Pop();
                });

        // add more rule.
    }
}
