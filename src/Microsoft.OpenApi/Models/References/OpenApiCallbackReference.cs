// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Callback Object Reference: A reference to a map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallbackReference : OpenApiCallback
    {
        private OpenApiCallback _target;
        private readonly OpenApiReference _reference;

        private OpenApiCallback Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiCallback>(Reference);
                return _target;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        public OpenApiCallbackReference(OpenApiReference reference)
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

            reference.Type = ReferenceType.Callback;
            _reference = reference;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="hostDocument"></param>
        public OpenApiCallbackReference(string referenceId, OpenApiDocument hostDocument)
        {
            _reference = new OpenApiReference()
            {                
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Callback         
            };            
        }

        /// <inheritdoc/>
        public override Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get => Target.PathItems; set => Target.PathItems = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override OpenApiReference Reference => _reference;

        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer),
                (writer, referenceElement) => referenceElement.SerializeAsV3WithoutReference(writer));
        }

        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer),
                (writer, referenceElement) => referenceElement.SerializeAsV31WithoutReference(writer));
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
