// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Validators
{
    /// <summary>
    /// Visit <see cref="OpenApiHeader"/>.
    /// </summary>
    internal class HeaderValidator : ValidatorBase<OpenApiHeader>
    {
        /// <summary>
        /// Visit the children in <see cref="OpenApiHeader"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="header">The <see cref="OpenApiHeader"/>.</param>
        protected override void Next(ValidationContext context, OpenApiHeader header)
        {
            Debug.Assert(context != null);
            Debug.Assert(header != null);

            context.Validate(header.Schema);

            context.ValidateCollection(header.Examples);

            context.ValidateMap(header.Content);

            base.Next(context, header);
        }
    }
}
