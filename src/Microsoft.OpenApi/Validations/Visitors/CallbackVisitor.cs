// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
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
            Debug.Assert(context != null);
            Debug.Assert(callback != null);

            foreach (var pathItem in callback.PathItems)
            {
                context.Validate(pathItem.Value);
            }

            base.Next(context, callback);
        }
    }
}
