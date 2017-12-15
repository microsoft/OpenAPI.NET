// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiTag"/>.
    /// </summary>
    internal class TagVisitor : VisitorBase<OpenApiTag>
    {
        /// <summary>
        /// Visit the children in <see cref="OpenApiTag"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="tag">The <see cref="OpenApiTag"/>.</param>
        protected override void Next(ValidationContext context, OpenApiTag tag)
        {
            Debug.Assert(context != null);
            Debug.Assert(tag != null);

            context.Validate(tag.ExternalDocs);

            base.Next(context, tag);
        }
    }
}
