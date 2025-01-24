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
    public class OpenApiRequestBodyReference : OpenApiRequestBody, IOpenApiReferenceableWithTarget<OpenApiRequestBody>
    {
        internal OpenApiRequestBody _target;
        private readonly OpenApiReference _reference;
        private string _description;

        /// <summary>
        /// Gets the target request body.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiRequestBody Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiRequestBody>(_reference);
                if (!string.IsNullOrEmpty(_description)) _target.Description = _description;
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
        public OpenApiRequestBodyReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.RequestBody,
                ExternalResource = externalResource
            };

            Reference = _reference;
        }

        internal OpenApiRequestBodyReference(OpenApiRequestBody target, string referenceId)
        {
            _target = target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.RequestBody,
            };
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

        private bool? _required;
        /// <inheritdoc/>
        public override bool Required { get => _required is not null ? _required.Value : Target?.Required ?? false; set => _required = value; }

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
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            Utils.CheckArgumentNull(writer);
            action(writer, Target);
        }

        /// <inheritdoc/>
        internal override OpenApiParameter ConvertToBodyParameter(IOpenApiWriter writer)
        {
            if (writer.GetSettings().ShouldInlineReference(_reference))
            {
                return Target.ConvertToBodyParameter(writer);
            }
            else
            {
                return new OpenApiParameterReference(_reference.Id, _reference.HostDocument);
            }
        }
    }
}
