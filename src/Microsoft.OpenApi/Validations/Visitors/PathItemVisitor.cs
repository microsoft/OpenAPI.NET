// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiPathItem"/>.
    /// </summary>
    internal class PathItemVisitor : VisitorBase<OpenApiPathItem>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiPathItem"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="pathItem">The <see cref="OpenApiPathItem"/>.</param>
        protected override void Next(ValidationContext context, OpenApiPathItem pathItem)
        {
            Debug.Assert(context != null);
            Debug.Assert(pathItem != null);

            foreach (var operation in pathItem.Operations)
            {
                context.Validate(operation.Value);
            }

            context.ValidateCollection(pathItem.Servers);
            context.ValidateCollection(pathItem.Parameters);

            base.Next(context, pathItem);
        }
    }
}
