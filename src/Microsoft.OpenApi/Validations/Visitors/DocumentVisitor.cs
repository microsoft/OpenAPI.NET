﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Vistor for <see cref="OpenApiDocument"/>
    /// </summary>
    internal class DocumentVisitor : VisitorBase<OpenApiDocument>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="document">The <see cref="OpenApiDocument"/>.</param>
        protected override void Next(ValidationContext context, OpenApiDocument document)
        {
            if (document == null)
            {
                return;
            }

            context.Validate(document.Info);

            context.ValidateCollection(document.Tags);

            // add more.
        }
    }
}
