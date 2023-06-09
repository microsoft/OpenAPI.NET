// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Security Scheme Object Reference.
    /// </summary>
    internal class OpenApiSecuritySchemeReference : OpenApiSecurityScheme
    {
        private OpenApiSecurityScheme _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiSecurityScheme Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiSecurityScheme>(Reference);
                return _target;
            }
        }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override string Name { get => Target.Name; set => Target.Name = value; }

        /// <inheritdoc/>
        public override ParameterLocation In { get => Target.In; set => Target.In = value; }

        /// <inheritdoc/>
        public override string Scheme { get => Target.Scheme; set => Target.Scheme = value; }

        /// <inheritdoc/>
        public override string BearerFormat { get => Target.BearerFormat; set => Target.BearerFormat = value; }

        /// <inheritdoc/>
        public override OpenApiOAuthFlows Flows { get => Target.Flows; set => Target.Flows = value; }

        /// <inheritdoc/>
        public override Uri OpenIdConnectUrl { get => Target.OpenIdConnectUrl; set => Target.OpenIdConnectUrl = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override SecuritySchemeType Type { get => Target.Type; set => Target.Type = value; }

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
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer), SerializeAsV3WithoutReference);
        }
        
        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer), SerializeAsV31WithoutReference);
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
        internal override void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback,
            Action<IOpenApiWriter> action)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));

            if (Reference != null)
            {
                callback(writer, Reference);
                return;
            }

            action(writer);
        }
    }
}
