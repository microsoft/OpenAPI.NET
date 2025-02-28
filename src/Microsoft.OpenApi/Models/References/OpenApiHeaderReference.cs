// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Header Object Reference.
    /// </summary>
    public class OpenApiHeaderReference : BaseOpenApiReferenceHolder<OpenApiHeader, IOpenApiHeader>, IOpenApiHeader
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
        public OpenApiHeaderReference(string referenceId, OpenApiDocument hostDocument = null, string externalResource = null):base(referenceId, hostDocument, ReferenceType.Header, externalResource)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="header">The <see cref="OpenApiHeaderReference"/> object to copy</param>
        private OpenApiHeaderReference(OpenApiHeaderReference header):base(header)
        {
        }

        /// <inheritdoc/>
        public string Description
        {
            get => string.IsNullOrEmpty(Reference?.Description) ? Target?.Description : Reference.Description;
            set 
            {
                if (Reference is not null)
                {
                    Reference.Description = value;
                }
            }
        }

        /// <inheritdoc/>
        public bool Required { get => Target?.Required ?? default; }

        /// <inheritdoc/>
        public bool Deprecated { get => Target?.Deprecated ?? default; }

        /// <inheritdoc/>
        public bool AllowEmptyValue { get => Target?.AllowEmptyValue ?? default; }

        /// <inheritdoc/>
        public IOpenApiSchema Schema { get => Target?.Schema; }

        /// <inheritdoc/>
        public ParameterStyle? Style { get => Target?.Style; }

        /// <inheritdoc/>
        public bool Explode { get => Target?.Explode ?? default; }

        /// <inheritdoc/>
        public bool AllowReserved { get => Target?.AllowReserved ?? default; }

        /// <inheritdoc/>
        public JsonNode Example { get => Target?.Example; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExample> Examples { get => Target?.Examples; }

        /// <inheritdoc/>
        public IDictionary<string, OpenApiMediaType> Content { get => Target?.Content; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension> Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public override IOpenApiHeader CopyReferenceAsTargetElementWithOverrides(IOpenApiHeader source)
        {
            return source is OpenApiHeader ? new OpenApiHeader(this) : source;
        }

        /// <inheritdoc/>
        public IOpenApiHeader CreateShallowCopy()
        {
            return new OpenApiHeaderReference(this);
        }
    }
}
