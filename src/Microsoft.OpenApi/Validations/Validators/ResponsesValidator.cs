// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Validators
{
    /// <summary>
    /// Visit <see cref="OpenApiResponses"/>.
    /// </summary>
    internal class ResponsesValidator : ValidatorBase<OpenApiResponses>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiResponses"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="responses">The <see cref="OpenApiResponses"/>.</param>
        protected override void Next(ValidationContext context, OpenApiResponses responses)
        {
            Debug.Assert(context != null);
            Debug.Assert(responses != null);

            context.ValidateMap(responses);

            base.Next(context, responses);
        }
    }
}
