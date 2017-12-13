// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Validators
{
    /// <summary>
    /// Visit <see cref="OpenApiInfo"/>.
    /// </summary>
    internal class InfoValidator : ValidatorBase<OpenApiInfo>
    {
        /// <summary>
        /// Visit the children in <see cref="OpenApiInfo"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="info">The <see cref="OpenApiInfo"/>.</param>
        protected override void Next(ValidationContext context, OpenApiInfo info)
        {
            Debug.Assert(context != null);
            Debug.Assert(info != null);

            context.Validate(info.Contact);

            context.Validate(info.License);

            base.Next(context, info);
        }
    }
}
