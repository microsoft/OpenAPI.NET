// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Callback Object Reference: A reference to a map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallbackReference : BaseOpenApiReferenceHolder<OpenApiCallback, IOpenApiCallback>, IOpenApiCallback
    {
        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        /// <param name="externalResource">Optional: External resource in the reference.
        /// It may be:
        /// 1. an absolute/relative file path, for example:  ../commons/pet.json
        /// 2. a Url, for example: http://localhost/pet.json
        /// </param>
        public OpenApiCallbackReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null):base(referenceId, hostDocument, ReferenceType.Callback, externalResource)
        {
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="callback">The reference to copy</param>
        private OpenApiCallbackReference(OpenApiCallbackReference callback):base(callback)
        {
            
        }

        /// <inheritdoc/>
        public OrderedDictionary<RuntimeExpression, IOpenApiPathItem>? PathItems { get => Target?.PathItems; }

        /// <inheritdoc/>
        public OrderedDictionary<string, IOpenApiExtension>? Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public override IOpenApiCallback CopyReferenceAsTargetElementWithOverrides(IOpenApiCallback source)
        {
            return source is OpenApiCallback ? new OpenApiCallback(this) : source;
        }

        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            // examples components are not supported in OAS 2.0
            Reference.SerializeAsV2(writer);
        }

        /// <inheritdoc/>
        public IOpenApiCallback CreateShallowCopy()
        {
            return new OpenApiCallbackReference(this);
        }
    }
}
