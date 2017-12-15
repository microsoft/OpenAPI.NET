// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Validators
{
    /// <summary>
    /// Visit <see cref="OpenApiOAuthFlows"/>.
    /// </summary>
    internal class OAuthFlowsValidator : ValidatorBase<OpenApiOAuthFlows>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiOAuthFlows"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="oAuthFlows">The <see cref="OpenApiOAuthFlows"/>.</param>
        protected override void Next(ValidationContext context, OpenApiOAuthFlows oAuthFlows)
        {
            Debug.Assert(context != null);
            Debug.Assert(oAuthFlows != null);

            context.Validate(oAuthFlows.Implicit);
            context.Validate(oAuthFlows.Password);
            context.Validate(oAuthFlows.ClientCredentials);
            context.Validate(oAuthFlows.AuthorizationCode);

            base.Next(context, oAuthFlows);
        }
    }
}
