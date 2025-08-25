// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
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
        public static ValidationRule<OpenApiInfo> InfoRequiredFields =>
            new(nameof(InfoRequiredFields),
                (context, item) =>
                {
                    // title
                    if (item.Title == null)
                    {
                        context.Enter("title");
                        context.CreateError(nameof(InfoRequiredFields),
                            string.Format(SRResource.Validation_FieldIsRequired, "title", "info"));
                        context.Exit();
                    }

                    // version
                    if (item.Version == null)
                    {
                        context.Enter("version");
                        context.CreateError(nameof(InfoRequiredFields),
                            string.Format(SRResource.Validation_FieldIsRequired, "version", "info"));
                        context.Exit();
                    }
                });
    }
}
