// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (securityScheme == null)
            {
                throw Error.ArgumentNull(nameof(securityScheme));
            }

            context.Validate(securityScheme.Flows);

            base.Next(context, securityScheme);
        }
    }
}
