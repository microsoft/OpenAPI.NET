// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiLicense"/>.
    /// </summary>
    public static class OpenApiLicenseRules
    {
        /// <summary>
        /// REQUIRED. REQUIRED. The license name used for the API.
        /// </summary>
        public static readonly ValidationRule<OpenApiLicense> FieldIsRequired =
            new ValidationRule<OpenApiLicense>(
                (context, license) =>
                {
                    context.Push("name");
                    if (String.IsNullOrEmpty(license.Name))
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "name", "license"));
                        context.AddError(error);
                    }
                    context.Pop();
                });
    }
}