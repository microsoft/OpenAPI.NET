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
    internal class OpenApiLinkReference : OpenApiLink
    {
        private OpenApiLink _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiLink Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiLink>(Reference);
                return _target;
            }
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
            SerializeInternalWithoutReference(writer, (writer, element) => element.SerializeAsV3(writer));
        }
        
        /// <inheritdoc/>
        public override void SerializeAsV31WithoutReference(IOpenApiWriter writer)
        {
            SerializeInternalWithoutReference(writer, (writer, element) => element.SerializeAsV31(writer));
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
