// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
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
        public static ValidationRule<OpenApiExternalDocs> UrlIsRequired =>
            new(nameof(UrlIsRequired),
                (context, item) =>
                {
                    // url
                    if (item.Url == null)
                    {
                        context.Enter("url");
                        context.CreateError(nameof(UrlIsRequired),
                            string.Format(SRResource.Validation_FieldIsRequired, "url", "External Documentation"));
                        context.Exit();
                    }
                });
    }
}
