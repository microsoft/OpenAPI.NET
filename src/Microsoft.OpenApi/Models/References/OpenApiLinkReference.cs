// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Link Object Reference.
    /// </summary>
    internal class OpenApiLinkReference : OpenApiLink
    {
        private OpenApiLink _target;
        private readonly OpenApiReference _reference;

        private OpenApiLink Target
        {
            get
            {
                _target ??= _reference.HostDocument.ResolveReferenceTo<OpenApiLink>(_reference);
                return _target;
            }
        }

        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        public OpenApiLinkReference(string referenceId, OpenApiDocument hostDocument)
        {
            if (string.IsNullOrEmpty(referenceId))
            {
                throw Error.Argument(nameof(referenceId), SRResource.ReferenceIdIsNullOrEmpty);
            }
            if (hostDocument == null)
            {
                throw Error.Argument(nameof(hostDocument), SRResource.HostDocumentIsNull);
            }

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Link
            };
        }

        /// <inheritdoc/>
        public override string OperationRef { get => Target.OperationRef; set => Target.OperationRef = value; }

        /// <inheritdoc/>
        public override string OperationId { get => Target.OperationId; set => Target.OperationId = value; }

        /// <inheritdoc/>
        public override OpenApiServer Server { get => Target.Server; set => Target.Server = value; }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override Dictionary<string, RuntimeExpressionAnyWrapper> Parameters { get => Target.Parameters; set => Target.Parameters = value; }

        /// <inheritdoc/>
        public override RuntimeExpressionAnyWrapper RequestBody { get => Target.RequestBody; set => Target.RequestBody = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => base.Extensions; set => base.Extensions = value; }

        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3WithoutReference(writer));
        }

        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31WithoutReference(writer));
        }

        /// <inheritdoc/>
        public override void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            SerializeInternalWithoutReference(writer, (writer, element) => element.SerializeAsV3(writer));
        }
        
        /// <inheritdoc/>
        public override void SerializeAsV31WithoutReference(IOpenApiWriter writer)
        {
            SerializeInternalWithoutReference(writer, (writer, element) => element.SerializeAsV31(writer));
        }               

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));
            action(writer, Target);
        }
    }
}
