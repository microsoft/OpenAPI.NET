// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Validators
{
    /// <summary>
    /// Vistor for <see cref="OpenApiDocument"/>
    /// </summary>
    internal class DocumentValidator : ValidatorBase<OpenApiDocument>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="document">The <see cref="OpenApiDocument"/>.</param>
        protected override void Next(ValidationContext context, OpenApiDocument document)
        {
            Debug.Assert(context != null);
            Debug.Assert(document != null);

            context.Validate(document.Info);
            context.ValidateCollection(document.Servers);
            context.Validate(document.Paths);
            context.Validate(document.Components);
            context.ValidateCollection(document.SecurityRequirements);
            context.ValidateCollection(document.Tags);
            base.Next(context, document);
        }
    }
}
