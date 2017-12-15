// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiComponents"/>.
    /// </summary>
    internal class ComponentsVisitor : VisitorBase<OpenApiComponents>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiComponents"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="components">The <see cref="OpenApiComponents"/>.</param>
        protected override void Next(ValidationContext context, OpenApiComponents components)
        {
            Debug.Assert(context != null);
            Debug.Assert(components != null);

            context.ValidateMap(components.Schemas);
            context.ValidateMap(components.Responses);
            context.ValidateMap(components.Parameters);
            context.ValidateMap(components.Examples);
            context.ValidateMap(components.RequestBodies);
            context.ValidateMap(components.Headers);
            context.ValidateMap(components.SecuritySchemes);
            context.ValidateMap(components.Links);
            context.ValidateMap(components.Callbacks);
            base.Next(context, components);
        }
    }
}
