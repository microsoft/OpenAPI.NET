// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Path Item Object Reference: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItemReference : OpenApiPathItem, IOpenApiReferenceableWithTarget<OpenApiPathItem>
    {
        internal OpenApiPathItem _target;
        private readonly OpenApiReference _reference;
        private string _description;
        private string _summary;

        /// <summary>
        /// Gets the target path item.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiPathItem Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiPathItem>(_reference);
                if (!string.IsNullOrEmpty(_description)) _target.Description = _description;
                if (!string.IsNullOrEmpty(_summary)) _target.Summary = _summary;
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
        public OpenApiPathItemReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.PathItem,
                ExternalResource = externalResource
            };

            Reference = _reference;
        }

        internal OpenApiPathItemReference(OpenApiPathItem target, string referenceId)
        {
            _target = target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.PathItem,
            };
        }

        /// <inheritdoc/>
        public override string Summary
        {
            get => string.IsNullOrEmpty(_summary) ? Target?.Summary : _summary;
            set => _summary = value;
        }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target?.Description : _description;
            set => _description = value;
        }

        private IDictionary<OperationType, OpenApiOperation> _operations;
        /// <inheritdoc/>
        public override IDictionary<OperationType, OpenApiOperation> Operations { get => _operations is not null ? _operations : Target?.Operations; set => _operations = value; }

        private IList<OpenApiServer> _servers;
        /// <inheritdoc/>
        public override IList<OpenApiServer> Servers { get => _servers is not null ? _servers : Target?.Servers; set => _servers = value; }

        private IList<OpenApiParameter> _parameters;
        /// <inheritdoc/>
        public override IList<OpenApiParameter> Parameters { get => _parameters is not null ? _parameters : Target?.Parameters; set => _parameters = value; }

        private IDictionary<string, IOpenApiExtension> _extensions;
        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => _extensions is not null ? _extensions : Target?.Extensions; set => _extensions = value; }

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
