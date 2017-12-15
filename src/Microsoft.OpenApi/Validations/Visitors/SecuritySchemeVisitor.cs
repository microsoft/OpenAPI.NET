// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiSecurityScheme"/>.
    /// </summary>
    internal class SecuritySchemeVisitor : VisitorBase<OpenApiSecurityScheme>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiSecurityScheme"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="securityScheme">The <see cref="OpenApiSecurityScheme"/>.</param>
        protected override void Next(ValidationContext context, OpenApiSecurityScheme securityScheme)
        {
            Debug.Assert(context != null);
            Debug.Assert(securityScheme != null);

            context.Validate(securityScheme.Flows);

            // add more.
            base.Next(context, securityScheme);
        }
    }
}
