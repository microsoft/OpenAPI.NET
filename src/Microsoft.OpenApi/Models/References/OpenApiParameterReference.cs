// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
#pragma warning disable CS0618
    /// <summary>
    /// Parameter Object Reference.
    /// </summary>
    public class OpenApiParameterReference : BaseOpenApiReferenceHolder<OpenApiParameter, IOpenApiParameter, OpenApiReferenceWithDescription>, IOpenApiParameter
    {
        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        /// <param name="externalResource">Optional: External resource in the reference.
        /// It may be:
        /// 1. a absolute/relative file path, for example:  ../commons/pet.json
        /// 2. a Url, for example: http://localhost/pet.json
        /// </param>
        public OpenApiParameterReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null):base(referenceId, hostDocument, ReferenceType.Parameter, externalResource)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="parameter">The parameter reference to copy</param>
        private OpenApiParameterReference(OpenApiParameterReference parameter):base(parameter)
        {
        }

        /// <inheritdoc/>
        public string? Name { get => GetFromTarget(static target => target.Name); }

        /// <inheritdoc/>
        public string? Description
        {
            get => string.IsNullOrEmpty(Reference.Description) ? GetFromTarget(static target => target.Description) : Reference.Description;
            set => Reference.Description = value;
        }

        /// <inheritdoc/>
        public bool Required { get => GetFromTarget(static target => target.Required); }

        /// <inheritdoc/>
        public bool Deprecated { get => GetFromTarget(static target => target.Deprecated); }

        /// <inheritdoc/>
        [Obsolete("Use of AllowEmptyValue is not recommended and it is likely to be removed in a later revision.")]
        public bool AllowEmptyValue { get => GetFromTarget(static target => target.AllowEmptyValue); }

        /// <inheritdoc/>
        public bool AllowReserved { get => GetFromTarget(static target => target.AllowReserved); }

        /// <inheritdoc/>
        public IOpenApiSchema? Schema { get => GetFromTarget(static target => target.Schema); }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExample>? Examples { get => GetFromTarget(static target => target.Examples); }

        /// <inheritdoc/>
        public JsonNode? Example { get => GetFromTarget(static target => target.Example); }

        /// <inheritdoc/>
        public ParameterLocation? In { get => GetFromTarget(static target => target.In); }

        /// <inheritdoc/>
        public ParameterStyle? Style { get => GetFromTarget(static target => target.Style); }
        
        /// <inheritdoc/>
        public bool Explode { get => GetFromTarget(static target => target.Explode); }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiMediaType>? Content { get => GetFromTarget(static target => target.Content); }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get => GetFromTarget(static target => target.Extensions); }
        
        /// <inheritdoc/>
        public override IOpenApiParameter CopyReferenceAsTargetElementWithOverrides(IOpenApiParameter  source)
        {
            return source is OpenApiParameter ? new OpenApiParameter(this) : source;
        }

        /// <inheritdoc/>
        public IOpenApiParameter CreateShallowCopy()
        {
            return new OpenApiParameterReference(this);
        }

        /// <inheritdoc/>
        protected override OpenApiReferenceWithDescription CopyReference(OpenApiReferenceWithDescription sourceReference)
        {
            return new OpenApiReferenceWithDescription(sourceReference);
        }
    }
}
