// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Path Item Object Reference: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItemReference : BaseOpenApiReferenceHolder<OpenApiPathItem, IOpenApiPathItem>, IOpenApiPathItem
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
        public OpenApiPathItemReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null): base(referenceId, hostDocument, ReferenceType.PathItem, externalResource)
        {
        }

        internal OpenApiPathItemReference(OpenApiPathItem target, string referenceId):base(target, referenceId, ReferenceType.PathItem)
        {
        }

        /// <inheritdoc/>
        public string Summary
        {
            get => string.IsNullOrEmpty(Reference?.Summary) ? Target?.Summary : Reference.Summary;
            set
            {
                if (Reference is not null)
                {
                    Reference.Summary = value;
                }
            }
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
        public IDictionary<OperationType, OpenApiOperation> Operations { get => Target.Operations; }

        /// <inheritdoc/>
        public IList<OpenApiServer> Servers { get => Target.Servers; }

        /// <inheritdoc/>
        public IList<IOpenApiParameter> Parameters { get => Target.Parameters; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; }

        /// <inheritdoc/>
        public override IOpenApiPathItem CopyReferenceAsTargetElementWithOverrides(IOpenApiPathItem source)
        {
            return source is OpenApiPathItem ? new OpenApiPathItem(this) : null;
        }

        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            Reference.SerializeAsV2(writer);
        }
    }
}
