// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Callback Object Reference: A reference to a map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallbackReference : OpenApiCallback, IOpenApiReferenceableWithTarget<OpenApiCallback>
    {
#nullable enable
        internal OpenApiCallback _target;
        private readonly OpenApiReference _reference;

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
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiCallback>(_reference);
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

            _reference = new OpenApiReference()
            {                
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Callback,
                ExternalResource = externalResource
            };

            Reference = _reference;
        }

        internal OpenApiCallbackReference(OpenApiCallback target, string referenceId)
        {
            _target = target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Callback,
            };
        }

        private Dictionary<RuntimeExpression, OpenApiPathItem> _pathItems;
        /// <inheritdoc/>
        public override Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get => _pathItems is not null ? _pathItems : Target?.PathItems; set => _pathItems = value; }

        private IDictionary<string, IOpenApiExtension> _extensions;
        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => _extensions is not null ? _extensions : Target?.Extensions; set => _extensions = value; }

        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(_reference))
            {
                _reference.SerializeAsV3(writer);
                return;
            }
            else
            {
                SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer));
            }
        }

        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(_reference))
            {
                _reference.SerializeAsV31(writer);
                return;
            }
            else
            {
                SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer));
            }
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
