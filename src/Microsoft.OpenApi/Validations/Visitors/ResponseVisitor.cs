// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiResponse"/>.
    /// </summary>
    internal class ResponseVisitor : VisitorBase<OpenApiResponse>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiResponse"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="response">The <see cref="OpenApiResponse"/>.</param>
        protected override void Next(ValidationContext context, OpenApiResponse response)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (response == null)
            {
                throw Error.ArgumentNull(nameof(response));
            }

            context.ValidateMap(response.Headers);
            context.ValidateMap(response.Content);
            context.ValidateMap(response.Links);

            base.Next(context, response);
        }
    }
}
