// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Example Object Reference.
    /// </summary>
    internal class OpenApiExampleReference : OpenApiExample
    {
        private OpenApiExample _target;
        private readonly OpenApiReference _reference;
        private string _summary;
        private string _description;

        private OpenApiExample Target
        {
            get
            {
                _target ??= _reference.HostDocument.ResolveReferenceTo<OpenApiExample>(_reference);
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
        public OpenApiExampleReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
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
                Type = ReferenceType.Example,
                ExternalResource = externalResource
            };
        }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target.Description : _description;
            set => _description = value; 
        }

        /// <inheritdoc/>
        public override string Summary
        { 
            get => string.IsNullOrEmpty(_summary) ? Target.Summary : _summary;
            set => _summary = value;
        }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override string ExternalValue { get => Target.ExternalValue; set => Target.ExternalValue = value; }

        /// <inheritdoc/>
        public override OpenApiAny Value { get => Target.Value; set => Target.Value = value; }

        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, referenceElement) => referenceElement.SerializeAsV3WithoutReference(writer));
        }

        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, referenceElement) => referenceElement.SerializeAsV31WithoutReference(writer));
        }

        /// <inheritdoc/>
        public override void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <inheritdoc/>
        public override void SerializeAsV31WithoutReference(IOpenApiWriter writer)
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_1);
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
