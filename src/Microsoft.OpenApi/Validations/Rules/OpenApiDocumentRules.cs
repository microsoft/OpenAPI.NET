// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.OpenApi
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
        public static ValidationRule<OpenApiDocument> OpenApiDocumentFieldIsMissing =>
            new(nameof(OpenApiDocumentFieldIsMissing),
                (context, item) =>
                {
                    // info
                    context.Enter("info");
                    if (item.Info == null)
                    {
                        context.CreateError(nameof(OpenApiDocumentFieldIsMissing),
                            String.Format(SRResource.Validation_FieldIsRequired, "info", "document"));
                    }
                    context.Exit();
                });
    }
}
