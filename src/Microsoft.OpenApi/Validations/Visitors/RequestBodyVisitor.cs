// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiRequestBody"/>.
    /// </summary>
    internal class RequestBodyVisitor : VisitorBase<OpenApiRequestBody>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiRequestBody"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="requestBody">The <see cref="OpenApiRequestBody"/>.</param>
        protected override void Next(ValidationContext context, OpenApiRequestBody requestBody)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (requestBody == null)
            {
                throw Error.ArgumentNull(nameof(requestBody));
            }

            context.ValidateMap(requestBody.Content);

            base.Next(context, requestBody);
        }
    }
}
