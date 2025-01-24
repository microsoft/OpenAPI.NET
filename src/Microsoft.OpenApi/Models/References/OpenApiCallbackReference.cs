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
    public class OpenApiCallbackReference : IOpenApiCallback, IOpenApiReferenceHolder<OpenApiCallback, IOpenApiCallback>
    {
#nullable enable
        internal OpenApiCallback _target;
        /// <inheritdoc/>
        public OpenApiReference Reference { get; set; }

        /// <inheritdoc/>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Gets the target callback.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiCallback Target
#nullable restore
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiCallback>(Reference);
                return _target;
            }
        }

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
        public OpenApiCallbackReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            Reference = new OpenApiReference()
            {                
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Callback,
                ExternalResource = externalResource
            };
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="callback">The callback reference to copy</param>
        public OpenApiCallbackReference(OpenApiCallbackReference callback)
        {
            Utils.CheckArgumentNull(callback);
            Reference = callback?.Reference != null ? new(callback.Reference) : null;
            UnresolvedReference = callback?.UnresolvedReference ?? false;
        }

        internal OpenApiCallbackReference(OpenApiCallback target, string referenceId)
        {
            _target = target;

            Reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Callback,
            };
        }

        /// <inheritdoc/>
        public Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get => Target.PathItems; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; }

        /// <inheritdoc/>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV3(writer);
            }
            else
            {
                SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer));
            }
        }

        /// <inheritdoc/>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV31(writer);
            }
            else
            {
                SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer));
            }
        }

        /// <inheritdoc/>
        public IOpenApiCallback CopyReferenceAsTargetElementWithOverrides(IOpenApiCallback openApiExample)
        {
            // the copy here is never called since callbacks do not have any overridable fields.
            // if the spec evolves to include overridable fields for callbacks, the serialize methods will need to call this copy method.
            return openApiExample is OpenApiCallback ? new OpenApiCallback(this) : openApiExample;
        }

        /// <inheritdoc/>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // examples components are not supported in OAS 2.0
            Reference.SerializeAsV2(writer);
        }

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            Utils.CheckArgumentNull(writer);
            action(writer, Target);
        }
    }
}
