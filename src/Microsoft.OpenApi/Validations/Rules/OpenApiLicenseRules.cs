﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

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
                    context.Enter("name");
                    if (license.Name == null)
                    {
                        context.CreateError(nameof(LicenseRequiredFields),
                            String.Format(SRResource.Validation_FieldIsRequired, "name", "license"));
                    }
                    context.Exit();
                });

        // add more rules
    }
}
