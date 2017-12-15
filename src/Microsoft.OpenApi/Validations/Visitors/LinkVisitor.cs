// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiLink"/>.
    /// </summary>
    internal class LinkVisitor : VisitorBase<OpenApiLink>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiLink"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="link">The <see cref="OpenApiLink"/>.</param>
        protected override void Next(ValidationContext context, OpenApiLink link)
        {
            Debug.Assert(context != null);
            Debug.Assert(link != null);

            context.Validate(link.Server);

            base.Next(context, link);
        }
    }
}
