// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiEncoding"/>.
    /// </summary>
    internal class EncodingVisitor : VisitorBase<OpenApiEncoding>
    {
        /// <summary>
        /// Visit the children in <see cref="OpenApiEncoding"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="encoding">The <see cref="OpenApiEncoding"/>.</param>
        protected override void Next(ValidationContext context, OpenApiEncoding encoding)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (encoding == null)
            {
                throw Error.ArgumentNull(nameof(encoding));
            }

            context.ValidateMap(encoding.Headers);

            base.Next(context, encoding);
        }
    }
}
