// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiExternalDocs"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiExternalDocsRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiExternalDocs> FieldIsRequired =>
            new ValidationRule<OpenApiExternalDocs>(
                (context, item) =>
                {
                    // url
                    context.Enter("url");
                    if (item.Url == null)
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "url", "External Documentation"));
                        context.AddError(error);
                    }
                    context.Exit();
                });

        // add more rule.
    }
}
