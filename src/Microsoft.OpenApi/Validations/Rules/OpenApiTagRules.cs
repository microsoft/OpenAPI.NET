// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiTag"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiTagRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiTag> TagRequiredFields =>
            new(nameof(TagRequiredFields),
                (context, tag) =>
                {
                    if (tag.Name == null)
                    {
                        context.Enter("name");
                        context.CreateError(nameof(TagRequiredFields),
                            string.Format(SRResource.Validation_FieldIsRequired, "name", "tag"));
                        context.Exit();
                    }
                });
    }
}
