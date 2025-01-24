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
    public class OpenApiSecuritySchemeReference : OpenApiSecurityScheme, IOpenApiReferenceableWithTarget<OpenApiSecurityScheme>
    {
        internal OpenApiSecurityScheme _target;
        private readonly OpenApiReference _reference;
        private string _description;

        /// <summary>
        /// Gets the target security scheme.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiSecurityScheme Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiSecurityScheme>(_reference);
                if (!string.IsNullOrEmpty(_description)) _target.Description = _description;
                return _target;
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
            get => string.IsNullOrEmpty(_description) ? Target?.Description : _description;
            set => _description = value;
        }

        private string _name;
        /// <inheritdoc/>
        public override string Name { get => !string.IsNullOrEmpty(_name) ? _name : Target?.Name; set => _name = value; }

        private ParameterLocation? _in;
        /// <inheritdoc/>
        public override ParameterLocation? In { get => _in is not null ? _in : Target?.In; set => _in = value; }

        private string _scheme;
        /// <inheritdoc/>
        public override string Scheme { get => !string.IsNullOrEmpty(_scheme) ? _scheme : Target?.Scheme; set => _scheme = value; }

        private string _bearerFormat;
        /// <inheritdoc/>
        public override string BearerFormat { get => !string.IsNullOrEmpty(_bearerFormat) ? _bearerFormat : Target?.BearerFormat; set => _bearerFormat = value; }

        private OpenApiOAuthFlows _flows;
        /// <inheritdoc/>
        public override OpenApiOAuthFlows Flows { get => _flows is not null ? _flows : Target?.Flows; set => _flows = value; }

        private Uri _openIdConnectUrl;
        /// <inheritdoc/>
        public override Uri OpenIdConnectUrl { get => _openIdConnectUrl is not null ? _openIdConnectUrl : Target?.OpenIdConnectUrl; set => _openIdConnectUrl = value; }

        private IDictionary<string, IOpenApiExtension> _extensions;
        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => _extensions is not null ? _extensions : Target?.Extensions; set => _extensions = value; }


        /// <inheritdoc/>
        public override SecuritySchemeType? Type { get => Target.Type; set => Target.Type = value; }
        
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
