// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Response Object Reference.
    /// </summary>
    public class OpenApiResponseReference : OpenApiResponse, IOpenApiReferenceHolder<OpenApiResponse>
    {
        internal OpenApiResponse _target;
        private readonly OpenApiReference _reference;
        private string _description;

        /// <summary>
        /// Gets the target response.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiResponse Target
        {
            get
            {
                _target ??= Reference.HostDocument?.ResolveReferenceTo<OpenApiResponse>(_reference);
                return _target;
            }
        }

        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        /// <param name="externalResource">Optional: External resource in the reference.
        /// It may be:
        /// 1. a absolute/relative file path, for example:  ../commons/pet.json
        /// 2. a Url, for example: http://localhost/pet.json
        /// </param>
        public OpenApiResponseReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Response,
                ExternalResource = externalResource
            };

            Reference = _reference;
        }

        internal OpenApiResponseReference(string referenceId, OpenApiResponse target)
        {
            _target ??= target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Response,
            };

            Reference = _reference;
        }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target?.Description : _description;
            set => _description = value;
        }

        private IDictionary<string, OpenApiMediaType> _content;
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiMediaType> Content { get => _content is not null ? _content : Target?.Content; set => _content = value; }

        private IDictionary<string, IOpenApiHeader> _headers;
        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiHeader> Headers { get => _headers is not null ? _headers : Target?.Headers; set => _headers = value; }

        private IDictionary<string, OpenApiLink> _links;
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiLink> Links { get => _links is not null ? _links : Target?.Links; set => _links = value; }

        private IDictionary<string, IOpenApiExtension> _extensions;
        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => _extensions is not null ? _extensions : Target?.Extensions; set => _extensions = value; }
        
        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(_reference))
            {
                _reference.SerializeAsV3(writer);
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
            Utils.CheckArgumentNull(writer);
            action(writer, this);
        }
    }
}
