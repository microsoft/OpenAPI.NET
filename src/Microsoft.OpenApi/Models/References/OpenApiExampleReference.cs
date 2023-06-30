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
        public OpenApiExampleReference(string referenceId, OpenApiDocument hostDocument)
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
                Type = ReferenceType.Example
            };
        }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override string Summary { get => Target.Summary; set => Target.Summary = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override string ExternalValue { get => Target.ExternalValue; set => Target.ExternalValue = value; }

        /// <inheritdoc/>
        public override IOpenApiAny Value { get => Target.Value; set => Target.Value = value; }
        

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
