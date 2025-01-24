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
    public class OpenApiLinkReference : OpenApiLink, IOpenApiReferenceableWithTarget<OpenApiLink>
    {
        internal OpenApiLink _target;
        private readonly OpenApiReference _reference;
        private string _description;

        /// <summary>
        /// Gets the target link.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiLink Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiLink>(_reference);
                if (!string.IsNullOrEmpty(_description)) _target.Description = _description;
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

        private string _operationRef;
        /// <inheritdoc/>
        public override string OperationRef { get => !string.IsNullOrEmpty(_operationRef) ? _operationRef : Target?.OperationRef; set => _operationRef = value; }

        private string _operationId;
        /// <inheritdoc/>
        public override string OperationId { get => !string.IsNullOrEmpty(_operationId) ? _operationId : Target?.OperationId; set => _operationId = value; }

        private OpenApiServer _server;
        /// <inheritdoc/>
        public override OpenApiServer Server { get => _server is not null ? _server : Target?.Server; set => _server = value; }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target.Description : _description;
            set => _description = value;
        }

        private Dictionary<string, RuntimeExpressionAnyWrapper> _parameters;
        /// <inheritdoc/>
        public override Dictionary<string, RuntimeExpressionAnyWrapper> Parameters { get => _parameters is not null ? _parameters : Target?.Parameters; set => _parameters = value; }

        private RuntimeExpressionAnyWrapper _requestBody;
        /// <inheritdoc/>
        public override RuntimeExpressionAnyWrapper RequestBody { get => _requestBody is not null ? _requestBody : Target?.RequestBody; set => _requestBody = value; }

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
            Utils.CheckArgumentNull(writer);;
            action(writer, Target);
        }
    }
}
