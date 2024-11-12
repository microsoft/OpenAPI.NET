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
    public class OpenApiPathItemReference : OpenApiPathItem
    {
        internal OpenApiPathItem _target;
        private readonly OpenApiReference _reference;
        private string _description;
        private string _summary;

        private OpenApiPathItem Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiPathItem>(_reference);
                OpenApiPathItem resolved = new OpenApiPathItem(_target);
                if (!string.IsNullOrEmpty(_description)) resolved.Description = _description;
                if (!string.IsNullOrEmpty(_summary)) resolved.Summary = _summary;
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
            get => string.IsNullOrEmpty(_summary) ? Target.Summary : _summary;
            set => _summary = value;
        }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target.Description : _description;
            set => _description = value;
        }

        /// <inheritdoc/>
        public override IDictionary<OperationType, OpenApiOperation> Operations { get => Target.Operations; set => Target.Operations = value; }

        /// <inheritdoc/>
        public override IList<OpenApiServer> Servers { get => Target.Servers; set => Target.Servers = value; }

        /// <inheritdoc/>
        public override IList<OpenApiParameter> Parameters { get => Target.Parameters; set => Target.Parameters = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }
               
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
