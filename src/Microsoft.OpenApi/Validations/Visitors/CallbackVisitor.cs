// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiCallback"/>.
    /// </summary>
    internal class CallbackVisitor : VisitorBase<OpenApiCallback>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiCallback"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="callback">The <see cref="OpenApiCallback"/>.</param>
        protected override void Next(ValidationContext context, OpenApiCallback callback)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (callback == null)
            {
                throw Error.ArgumentNull(nameof(callback));
            }

            foreach (var pathItem in callback.PathItems)
            {
                context.Validate(pathItem.Value);
            }

            base.Next(context, callback);
        }
    }
}
