// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
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
            Debug.Assert(context != null);
            Debug.Assert(response != null);

            context.ValidateMap(response.Headers);
            context.ValidateMap(response.Content);
            context.ValidateMap(response.Links);

            base.Next(context, response);
        }
    }
}
