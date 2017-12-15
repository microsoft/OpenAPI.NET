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
    internal static class OpenApiDocumentRules
    {
        /// <summary>
        /// The Info field is required.
        /// </summary>
        public static readonly ValidationRule<OpenApiDocument> FieldIsRequired =
            new ValidationRule<OpenApiDocument>(
                (context, item) =>
                {
                    // info
                    context.Push("info");
                    if (item.Info == null)
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "info", "document"));
                        context.AddError(error);
                    }
                    context.Pop();

                    // paths
                    context.Push("paths");
                    if (item.Paths == null)
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "paths", "document"));
                        context.AddError(error);
                    }
                    context.Pop();
                });
    }
}
