// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (parameter == null)
            {
                throw Error.ArgumentNull(nameof(parameter));
            }

            context.Validate(parameter.Schema);
            context.ValidateCollection(parameter.Examples);
            context.ValidateMap(parameter.Content);

            base.Next(context, parameter);
        }
    }
}
