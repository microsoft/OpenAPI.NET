// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiInfo"/>.
    /// </summary>
    internal class InfoVisitor : VisitorBase<OpenApiInfo>
    {
        /// <summary>
        /// Visit the children in <see cref="OpenApiInfo"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="info">The <see cref="OpenApiInfo"/>.</param>
        protected override void Next(ValidationContext context, OpenApiInfo info)
        {
            if (info == null)
            {
                return;
            }

            context.Validate(info.Contact);

            context.Validate(info.License);
        }
    }
}
