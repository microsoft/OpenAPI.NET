// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiDocument"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiDocumentRules
    {
        /// <summary>
        /// The Info field is required.
        /// </summary>
        public static ValidationRule<OpenApiDocument> FieldIsRequired =>
            new ValidationRule<OpenApiDocument>(
                (context, item) =>
                {
                    // info
                    context.Enter("info");
                    if (item.Info == null)
                    {
                        OpenApiError error = new OpenApiError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "info", "document"));
                        context.AddError(error);
                    }
                    context.Exit();

                    // paths
                    context.Enter("paths");
                    if (item.Paths == null)
                    {
                        OpenApiError error = new OpenApiError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "paths", "document"));
                        context.AddError(error);
                    }
                    context.Exit();
                });
    }
}
