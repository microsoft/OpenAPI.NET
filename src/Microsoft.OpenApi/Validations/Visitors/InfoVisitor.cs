// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiInfo"/>.
    /// </summary>
    internal class InfoVisitor : VisitorBase<OpenApiInfo>
    {
        /// <summary>
        /// Visit the children in <see cref="OpenApiInfo"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="info">The <see cref="OpenApiInfo"/>.</param>
        protected override void Next(ValidationContext context, OpenApiInfo info)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (info == null)
            {
                throw Error.ArgumentNull(nameof(info));
            }

            context.Validate(info.Contact);

            context.Validate(info.License);

            base.Next(context, info);
        }
    }
}
