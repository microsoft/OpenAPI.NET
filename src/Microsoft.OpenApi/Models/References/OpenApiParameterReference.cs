// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Parameter Object Reference.
    /// </summary>
    internal class OpenApiParameterReference : OpenApiParameter
    {
        private OpenApiParameter _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiParameter Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiParameter>(Reference);
                return _target;
            }
        }

        /// <inheritdoc/>
        public override string Name { get => Target.Name; set => Target.Name = value; }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override bool Required { get => Target.Required; set => Target.Required = value; }

        /// <inheritdoc/>
        public override bool Deprecated { get => Target.Deprecated; set => Target.Deprecated = value; }

        /// <inheritdoc/>
        public override bool AllowEmptyValue { get => Target.AllowEmptyValue; set => Target.AllowEmptyValue = value; }

        /// <inheritdoc/>
        public override bool AllowReserved { get => Target.AllowReserved; set => Target.AllowReserved = value; }

        /// <inheritdoc/>
        public override OpenApiSchema Schema { get => Target.Schema; set => Target.Schema = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiExample> Examples { get => Target.Examples; set => Target.Examples = value; }

        /// <inheritdoc/>
        public override IOpenApiAny Example { get => Target.Example; set => Target.Example = value; }

        /// <inheritdoc/>
        public override ParameterLocation? In { get => Target.In; set => Target.In = value; }

        /// <inheritdoc/>
        public override ParameterStyle? Style { get => Target.Style; set => Target.Style = value; }
        
        /// <inheritdoc/>
        public override bool Explode { get => base.Explode; set => base.Explode = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiMediaType> Content { get => Target.Content; set => Target.Content = value; }

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

            action(writer, this);
        }
    }
}
