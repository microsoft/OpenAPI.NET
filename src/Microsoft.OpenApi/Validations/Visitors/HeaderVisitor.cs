// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiHeader"/>.
    /// </summary>
    internal class HeaderVisitor : VisitorBase<OpenApiHeader>
    {
        /// <summary>
        /// Visit the children in <see cref="OpenApiHeader"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="header">The <see cref="OpenApiHeader"/>.</param>
        protected override void Next(ValidationContext context, OpenApiHeader header)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (header == null)
            {
                throw Error.ArgumentNull(nameof(header));
            }

            context.Validate(header.Schema);

            context.ValidateCollection(header.Examples);

            context.ValidateMap(header.Content);

            base.Next(context, header);
        }
    }
}
