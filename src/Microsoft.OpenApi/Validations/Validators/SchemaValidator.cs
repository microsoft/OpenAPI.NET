// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Validators
{
    /// <summary>
    /// Visit <see cref="OpenApiSchema"/>.
    /// </summary>
    internal class SchemaValidator : ValidatorBase<OpenApiSchema>
    {
        /// <summary>
        /// Visit the children of the <see cref="OpenApiSchema"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="schema">The <see cref="OpenApiSchema"/>.</param>
        protected override void Next(ValidationContext context, OpenApiSchema schema)
        {
            Debug.Assert(context != null);
            Debug.Assert(schema != null);

            context.ValidateCollection(schema.AllOf);

            context.ValidateCollection(schema.OneOf);

            context.ValidateCollection(schema.AnyOf);

            context.Validate(schema.Not);

            context.Validate(schema.Items);

            context.ValidateMap(schema.Properties);

            context.Validate(schema.AdditionalProperties);

            context.Validate(schema.Discriminator);

            context.Validate(schema.ExternalDocs);

            context.Validate(schema.Xml);

            base.Next(context, schema);
        }
    }
}
