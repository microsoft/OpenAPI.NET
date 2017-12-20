// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiOAuthFlows"/>.
    /// </summary>
    internal class OAuthFlowsVisitor : VisitorBase<OpenApiOAuthFlows>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiOAuthFlows"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="oAuthFlows">The <see cref="OpenApiOAuthFlows"/>.</param>
        protected override void Next(ValidationContext context, OpenApiOAuthFlows oAuthFlows)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (oAuthFlows == null)
            {
                throw Error.ArgumentNull(nameof(oAuthFlows));
            }

            context.Validate(oAuthFlows.Implicit);
            context.Validate(oAuthFlows.Password);
            context.Validate(oAuthFlows.ClientCredentials);
            context.Validate(oAuthFlows.AuthorizationCode);

            base.Next(context, oAuthFlows);
        }
    }
}
