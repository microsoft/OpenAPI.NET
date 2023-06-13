// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Callback Object Reference: A reference to a map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallbackReference : OpenApiCallback
    {
        private OpenApiCallback _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiCallback Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiCallback>(Reference);
                return _target;
            }
        }
        
        /// <inheritdoc/>
        public override Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get => Target.PathItems; set => Target.PathItems = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override bool UnresolvedReference { get => base.UnresolvedReference; set => base.UnresolvedReference = value; }

        /// <inheritdoc/>
        public override OpenApiReference Reference 
        { 
            get => base.Reference;
            set
            {
                base.Reference = value;
                _target = null;
            }
        }

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
