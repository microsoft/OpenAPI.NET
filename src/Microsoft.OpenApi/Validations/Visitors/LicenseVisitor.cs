// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiContact"/>.
    /// </summary>
    internal class LicenseVisitor : VisitorBase<OpenApiLicense>
    {
        /// <summary>
        /// Visit the children in <see cref="OpenApiLicense"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="license">The <see cref="OpenApiLicense"/>.</param>
        protected override void Next(ValidationContext context, OpenApiLicense license)
        {
            if (license == null)
            {
                return;
            }
        }
    }
}
