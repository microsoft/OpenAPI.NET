// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Parameter Object Reference.
    /// </summary>
    public class OpenApiParameterReference : BaseOpenApiReferenceHolder<OpenApiParameter, IOpenApiParameter>, IOpenApiParameter
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
        public string? Name { get => Target?.Name; }

        /// <inheritdoc/>
        public string? Description
        {
            get => string.IsNullOrEmpty(Reference.Description) ? Target?.Description : Reference.Description;
            set => Reference.Description = value;
        }

        /// <inheritdoc/>
        public bool Required { get => Target?.Required ?? default; }

        /// <inheritdoc/>
        public bool Deprecated { get => Target?.Deprecated ?? default; }

        /// <inheritdoc/>
        public bool AllowEmptyValue { get => Target?.AllowEmptyValue ?? default; }

        /// <inheritdoc/>
        public bool AllowReserved { get => Target?.AllowReserved ?? default; }

        /// <inheritdoc/>
        public IOpenApiSchema? Schema { get => Target?.Schema; }

        /// <inheritdoc/>
        public Dictionary<string, IOpenApiExample>? Examples { get => Target?.Examples; }

        /// <inheritdoc/>
        public JsonNode? Example { get => Target?.Example; }

        /// <inheritdoc/>
        public ParameterLocation? In { get => Target?.In; }

        /// <inheritdoc/>
        public ParameterStyle? Style { get => Target?.Style; }
        
        /// <inheritdoc/>
        public bool Explode { get => Target?.Explode ?? default; }

        /// <inheritdoc/>
        public Dictionary<string, OpenApiMediaType>? Content { get => Target?.Content; }

        /// <inheritdoc/>
        public OpenApiExtensionDictionary? Extensions { get => Target?.Extensions; }
        
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
    }
}
