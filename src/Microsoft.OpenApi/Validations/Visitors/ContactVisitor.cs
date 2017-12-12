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
    internal class ContactVisitor : VisitorBase<OpenApiContact>
    {
        /// <summary>
        /// Visit the children in <see cref="OpenApiContact"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="contact">The <see cref="OpenApiContact"/>.</param>
        protected override void Next(ValidationContext context, OpenApiContact contact)
        {
        }
    }
}
