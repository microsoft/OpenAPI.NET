// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Example Object Reference.
    /// </summary>
    public class OpenApiExampleReference : BaseOpenApiReferenceHolder<OpenApiExample, IOpenApiExample>, IOpenApiExample
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
        public OpenApiExampleReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null):base(referenceId, hostDocument, ReferenceType.Example, externalResource)
        {
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="example">The example reference to copy</param>
        private OpenApiExampleReference(OpenApiExampleReference example):base(example)
        {
        }

        /// <inheritdoc/>
        public string? Description
        {
            get => string.IsNullOrEmpty(Reference.Description) ? Target?.Description : Reference.Description;
            set => Reference.Description = value;
        }

        /// <inheritdoc/>
        public string? Summary
        {
            get => string.IsNullOrEmpty(Reference.Summary) ? Target?.Summary : Reference.Summary;
            set => Reference.Summary = value;
        }

        /// <inheritdoc/>
        public Dictionary<string, IOpenApiExtension>? Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public string? ExternalValue { get => Target?.ExternalValue; }

        /// <inheritdoc/>
        public JsonNode? Value { get => Target?.Value; }

        /// <inheritdoc/>
        public override IOpenApiExample CopyReferenceAsTargetElementWithOverrides(IOpenApiExample source)
        {
            return source is OpenApiExample ? new OpenApiExample(this) : source;
        }

        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            // examples components are not supported in OAS 2.0
            Reference.SerializeAsV2(writer);
        }

        /// <inheritdoc/>
        public IOpenApiExample CreateShallowCopy()
        {
            return new OpenApiExampleReference(this);
        }
    }
}
