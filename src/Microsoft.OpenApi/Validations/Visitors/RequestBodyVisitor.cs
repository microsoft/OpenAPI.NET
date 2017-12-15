// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
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
            Debug.Assert(context != null);
            Debug.Assert(requestBody != null);

            context.ValidateMap(requestBody.Content);

            base.Next(context, requestBody);
        }
    }
}
