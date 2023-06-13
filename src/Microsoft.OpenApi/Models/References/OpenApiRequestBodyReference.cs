// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Request Body Object Reference.
    /// </summary>
    internal class OpenApiRequestBodyReference : OpenApiRequestBody
    {
        private OpenApiRequestBody _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiRequestBody Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiRequestBody>(Reference);
                return _target;
            }
        }

        public OpenApiRequestBodyReference(OpenApiDocument hostDocument)
        {
            _hostDocument = hostDocument;
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
        public override bool UnresolvedReference { get => base.UnresolvedReference; set => base.UnresolvedReference = value; }

        /// <inheritdoc/>
        public override OpenApiReference Reference
        {
            get => Target.Reference;
            set
            {
                Target.Reference = value;
                _target = null;
            }
        }

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
