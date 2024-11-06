// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Link Object Reference.
    /// </summary>
    public class OpenApiLinkReference : OpenApiLink
    {
        internal OpenApiLink _target;
        private readonly OpenApiReference _reference;
        private string _description;

        private OpenApiLink Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiLink>(_reference);
                OpenApiLink resolved = new OpenApiLink(_target);
                if (!string.IsNullOrEmpty(_description)) resolved.Description = _description;
                return resolved;
            }
        }

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
        public OpenApiLinkReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Link,
                ExternalResource = externalResource
            };

            Reference = _reference;
        }

        internal OpenApiLinkReference(OpenApiLink target, string referenceId)
        {
            _target = target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Link,
            };
        }

        /// <inheritdoc/>
        public override string OperationRef { get => Target.OperationRef; set => Target.OperationRef = value; }

        /// <inheritdoc/>
        public override string OperationId { get => Target.OperationId; set => Target.OperationId = value; }

        /// <inheritdoc/>
        public override OpenApiServer Server { get => Target.Server; set => Target.Server = value; }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target.Description : _description;
            set => _description = value;
        }

        /// <inheritdoc/>
        public override Dictionary<string, RuntimeExpressionAnyWrapper> Parameters { get => Target.Parameters; set => Target.Parameters = value; }

        /// <inheritdoc/>
        public override RuntimeExpressionAnyWrapper RequestBody { get => Target.RequestBody; set => Target.RequestBody = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => base.Extensions; set => base.Extensions = value; }

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
            Utils.CheckArgumentNull(writer);;
            action(writer, Target);
        }
    }
}
