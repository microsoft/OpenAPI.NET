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
    /// Security Scheme Object Reference.
    /// </summary>
    internal class OpenApiSecuritySchemeReference : OpenApiSecurityScheme
    {
        private OpenApiSecurityScheme _target;
        private readonly OpenApiReference _reference;
        private string _description;

        private OpenApiSecurityScheme Target
        {
            get
            {
                _target ??= _reference.HostDocument.ResolveReferenceTo<OpenApiSecurityScheme>(_reference);
                return _target;
            }
        }

        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        public OpenApiSecuritySchemeReference(string referenceId, OpenApiDocument hostDocument)
        {
            if (string.IsNullOrEmpty(referenceId))
            {
                Utils.CheckArgumentNullOrEmpty(referenceId);
            }
            if (hostDocument == null)
            {
                Utils.CheckArgumentNull(hostDocument);
            }

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.SecurityScheme
            };
        }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target.Description : _description;
            set => _description = value;
        }

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
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, SerializeAsV3WithoutReference);
        }
        
        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, SerializeAsV31WithoutReference);
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
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter> action)
        {
            Utils.CheckArgumentNull(writer);;
            action(writer);
        }
    }
}
