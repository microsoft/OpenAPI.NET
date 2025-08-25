// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiLicense"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiLicenseRules
    {
        /// <summary>
        /// REQUIRED.
        /// </summary>
        public static ValidationRule<OpenApiLicense> LicenseRequiredFields =>
            new(nameof(LicenseRequiredFields),
                (context, license) =>
                {
                    if (license.Name == null)
                    {
                        context.Enter("name");
                        context.CreateError(nameof(LicenseRequiredFields),
                            string.Format(SRResource.Validation_FieldIsRequired, "name", "license"));
                        context.Exit();
                    }
                });
    }
}
