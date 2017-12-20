// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Vistor for <see cref="OpenApiDocument"/>
    /// </summary>
    internal class DocumentVisitor : VisitorBase<OpenApiDocument>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="document">The <see cref="OpenApiDocument"/>.</param>
        protected override void Next(ValidationContext context, OpenApiDocument document)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (document == null)
            {
                throw Error.ArgumentNull(nameof(document));
            }

            context.Validate(document.Info);

            context.ValidateCollection(document.Servers);

            context.Validate(document.Paths);

            context.Validate(document.Components);

            context.ValidateCollection(document.SecurityRequirements);

            context.ValidateCollection(document.Tags);

            context.Validate(document.ExternalDocs);

            base.Next(context, document);
        }
    }
}
