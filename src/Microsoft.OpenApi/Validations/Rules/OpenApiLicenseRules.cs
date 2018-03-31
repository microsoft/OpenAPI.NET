// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
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
        public static ValidationRule<OpenApiLicense> FieldIsRequired =>
            new ValidationRule<OpenApiLicense>(
                (context, license) =>
                {
                    context.Enter("name");
                    if (String.IsNullOrEmpty(license.Name))
                    {
                        OpenApiError error = new OpenApiError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "name", "license"));
                        context.AddError(error);
                    }
                    context.Exit();
                });

        // add more rules
    }
}
