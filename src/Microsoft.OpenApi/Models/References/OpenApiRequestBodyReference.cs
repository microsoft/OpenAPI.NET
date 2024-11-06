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
    public class OpenApiRequestBodyReference : OpenApiRequestBody
    {
        internal OpenApiRequestBody _target;
        private readonly OpenApiReference _reference;
        private string _description;

        private OpenApiRequestBody Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiRequestBody>(_reference);
                OpenApiRequestBody resolved = new OpenApiRequestBody(_target);
                if (!string.IsNullOrEmpty(_description)) resolved.Description = _description;
                return resolved;
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
            get => string.IsNullOrEmpty(_description) ? Target.Description : _description;
            set => _description = value;
        }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiMediaType> Content { get => Target.Content; set => Target.Content = value; }

        /// <inheritdoc/>
        public override bool Required { get => Target.Required; set => Target.Required = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

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
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            Utils.CheckArgumentNull(writer);
            action(writer, Target);
        }
    }
}
