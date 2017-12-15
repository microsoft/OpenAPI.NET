// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiParameter"/>.
    /// </summary>
    internal class ParameterVisitor : VisitorBase<OpenApiParameter>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiParameter"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="parameter">The <see cref="OpenApiParameter"/>.</param>
        protected override void Next(ValidationContext context, OpenApiParameter parameter)
        {
            Debug.Assert(context != null);
            Debug.Assert(parameter != null);

            context.Validate(parameter.Schema);
            context.ValidateCollection(parameter.Examples);
            context.ValidateMap(parameter.Content);

            base.Next(context, parameter);
        }
    }
}
