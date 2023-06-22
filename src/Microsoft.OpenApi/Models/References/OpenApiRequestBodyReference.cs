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
    /// Request Body Object Reference.
    /// </summary>
    internal class OpenApiRequestBodyReference : OpenApiRequestBody
    {
        private OpenApiRequestBody _target;
        private readonly OpenApiReference _reference;

        private OpenApiRequestBody Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiRequestBody>(Reference);
                return _target;
            }
        }

        public OpenApiRequestBodyReference(OpenApiReference reference)
        {
            if (reference == null)
            {
                throw Error.ArgumentNull(nameof(reference));
            }
            if (reference.HostDocument == null)
            {
                throw Error.Argument(nameof(reference), SRResource.ReferencedElementHostDocumentIsNull);
            }
            if (string.IsNullOrEmpty(reference.Id))
            {
                throw Error.Argument(nameof(reference), SRResource.ReferencedElementIdentifierIsNullOrEmpty);
            }

            reference.Type = ReferenceType.RequestBody;
            _reference = reference;
        }

        public OpenApiRequestBodyReference(string referenceId, OpenApiDocument hostDocument)
        {
            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.RequestBody
            };
        }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiMediaType> Content { get => Target.Content; set => Target.Content = value; }

        /// <inheritdoc/>
        public override bool Required { get => Target.Required; set => Target.Required = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override OpenApiReference Reference => _reference;

        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer),
                (writer, element) => element.SerializeAsV3WithoutReference(writer));
        }
        
        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer),
                (writer, element) => element.SerializeAsV31WithoutReference(writer));
        }

        /// <inheritdoc/>
        public override void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_0,
                (writer, element) => element.SerializeAsV3(writer));
        }
        
        /// <inheritdoc/>
        public override void SerializeAsV31WithoutReference(IOpenApiWriter writer)
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_1,
                    (writer, element) => element.SerializeAsV31(writer));
        }   

        /// <inheritdoc/>
        internal override void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiSerializable> callback,
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));

            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                callback(writer, Reference);
                return;
            }

            action(writer, Target);
        }
    }
}
