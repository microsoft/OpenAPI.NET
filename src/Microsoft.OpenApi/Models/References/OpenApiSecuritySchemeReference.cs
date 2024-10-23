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
    public class OpenApiSecuritySchemeReference : OpenApiSecurityScheme
    {
        internal OpenApiSecurityScheme _target;
        private readonly OpenApiReference _reference;
        private string _description;

        private OpenApiSecurityScheme Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiSecurityScheme>(_reference);
                OpenApiSecurityScheme resolved = new OpenApiSecurityScheme(_target);
                if (!string.IsNullOrEmpty(_description)) resolved.Description = _description;
                return resolved;
            }
        }

        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        /// <param name="externalResource">The externally referenced file.</param>
        public OpenApiSecuritySchemeReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.SecurityScheme,
                ExternalResource = externalResource
            };

            Reference = _reference;
        }

        internal OpenApiSecuritySchemeReference(string referenceId, OpenApiSecurityScheme target)
        {
            _target = target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.SecurityScheme,
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
            if (!writer.GetSettings().ShouldInlineReference(_reference))
            {
                _reference.SerializeAsV3(writer);
                return;
            }
            else
            {
                SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer));
            }
        }
        
        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(_reference))
            {
                _reference.SerializeAsV31(writer);
                return;
            }
            else
            {
                SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer));
            }
        }

        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(_reference))
            {
                _reference.SerializeAsV2(writer);
                return;
            }
            else
            {
                SerializeInternal(writer, (writer, element) => element.SerializeAsV2(writer));
            }
        }

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            Utils.CheckArgumentNull(writer);;
            action(writer, Target);
        }
    }
}
