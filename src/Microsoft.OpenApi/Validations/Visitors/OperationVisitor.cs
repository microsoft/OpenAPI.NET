// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Visit <see cref="OpenApiOperation"/>.
    /// </summary>
    internal class OperationVisitor : VisitorBase<OpenApiOperation>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiOperation"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="operation">The <see cref="OpenApiOperation"/>.</param>
        protected override void Next(ValidationContext context, OpenApiOperation operation)
        {
            Debug.Assert(context != null);
            Debug.Assert(operation != null);

            context.ValidateCollection(operation.Tags);
            context.Validate(operation.ExternalDocs);
            context.ValidateCollection(operation.Parameters);
            context.Validate(operation.RequestBody);
            context.Validate(operation.Responses);
            context.ValidateMap(operation.Callbacks);
            context.ValidateCollection(operation.Security);
            context.ValidateCollection(operation.Servers);

            base.Next(context, operation);
        }
    }
}
