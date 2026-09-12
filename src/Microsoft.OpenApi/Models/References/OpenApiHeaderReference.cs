// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Header Object Reference.
    /// </summary>
    public class OpenApiHeaderReference : BaseOpenApiReferenceHolder<OpenApiHeader, IOpenApiHeader, OpenApiReferenceWithDescription>, IOpenApiHeader
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
        public OpenApiHeaderReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null) : base(referenceId, hostDocument, ReferenceType.Header, externalResource)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="header">The <see cref="OpenApiHeaderReference"/> object to copy</param>
        private OpenApiHeaderReference(OpenApiHeaderReference header) : base(header)
        {
        }

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
        public bool AllowEmptyValue { get => GetFromTarget(static target => target.AllowEmptyValue); }

        /// <inheritdoc/>
        public IOpenApiSchema? Schema { get => GetFromTarget(static target => target.Schema); }

        /// <inheritdoc/>
        public ParameterStyle? Style { get => GetFromTarget(static target => target.Style); }

        /// <inheritdoc/>
        public bool Explode { get => GetFromTarget(static target => target.Explode); }

        /// <inheritdoc/>
        public bool AllowReserved { get => GetFromTarget(static target => target.AllowReserved); }

        /// <inheritdoc/>
        public JsonNode? Example { get => GetFromTarget(static target => target.Example); }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExample>? Examples { get => GetFromTarget(static target => target.Examples); }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiMediaType>? Content { get => GetFromTarget(static target => target.Content); }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get => GetFromTarget(static target => target.Extensions); }

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
        /// <inheritdoc/>
        protected override OpenApiReferenceWithDescription CopyReference(OpenApiReferenceWithDescription sourceReference)
        {
            return new OpenApiReferenceWithDescription(sourceReference);
        }
    }
}
