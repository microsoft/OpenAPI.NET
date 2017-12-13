// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Validators
{
    /// <summary>
    /// Visit <see cref="OpenApiPaths"/>.
    /// </summary>
    internal class PathsValidator : ValidatorBase<OpenApiPaths>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiPaths"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="paths">The <see cref="OpenApiPaths"/>.</param>
        protected override void Next(ValidationContext context, OpenApiPaths paths)
        {
            Debug.Assert(context != null);
            Debug.Assert(paths != null);

            context.ValidateMap(paths);
            base.Next(context, paths);
        }
    }
}
