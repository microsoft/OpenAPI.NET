// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiMediaType"/>.
    /// </summary>
    internal class MediaTypeVisitor : VisitorBase<OpenApiMediaType>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiMediaType"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="mediaType">The <see cref="OpenApiMediaType"/>.</param>
        protected override void Next(ValidationContext context, OpenApiMediaType mediaType)
        {
            Debug.Assert(context != null);
            Debug.Assert(mediaType != null);

            context.Validate(mediaType.Schema);
            context.ValidateMap(mediaType.Examples);
            context.ValidateMap(mediaType.Encoding);

            base.Next(context, mediaType);
        }
    }
}
