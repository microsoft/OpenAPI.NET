// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Link Object Reference.
    /// </summary>
    public class OpenApiLinkReference : BaseOpenApiReferenceHolder<OpenApiLink, IOpenApiLink>, IOpenApiLink
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
        public OpenApiLinkReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null):base(referenceId, hostDocument, ReferenceType.Link, externalResource)
        {
        }
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="reference">The reference to copy</param>
        private OpenApiLinkReference(OpenApiLinkReference reference):base(reference)
        {
        }

        /// <inheritdoc/>
        public string? Description
        {
            get => string.IsNullOrEmpty(Reference.Description) ? Target?.Description : Reference.Description;
            set => Reference.Description = value;
        }

        /// <inheritdoc/>
        public string? OperationRef { get => Target?.OperationRef; }

        /// <inheritdoc/>
        public string? OperationId { get => Target?.OperationId; }

        /// <inheritdoc/>
        public OpenApiServer? Server { get => Target?.Server; }

        /// <inheritdoc/>
        public IDictionary<string, RuntimeExpressionAnyWrapper>? Parameters { get => Target?.Parameters; }

        /// <inheritdoc/>
        public RuntimeExpressionAnyWrapper? RequestBody { get => Target?.RequestBody; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            // Link object does not exist in V2.
        }

        /// <inheritdoc/>
        public override IOpenApiLink CopyReferenceAsTargetElementWithOverrides(IOpenApiLink source)
        {
            return source is OpenApiLink ? new OpenApiLink(this) : source;
        }

        /// <inheritdoc/>
        public IOpenApiLink CreateShallowCopy()
        {
            return new OpenApiLinkReference(this);
        }
    }
}
