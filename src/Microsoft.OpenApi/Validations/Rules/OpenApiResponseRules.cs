// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiResponse"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiResponseRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<IOpenApiResponse> ResponseRequiredFields =>
            new(nameof(ResponseRequiredFields),
                (context, response) =>
                {
                    // description
                    if (response.Description == null)
                    {
                        context.Enter("description");
                        context.CreateError(nameof(ResponseRequiredFields),
                            string.Format(SRResource.Validation_FieldIsRequired, "description", "response"));
                        context.Exit();
                    }
                });

        // add more rule.
    }
}
