// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Response Object Reference.
    /// </summary>
    public class OpenApiResponseReference : OpenApiResponse
    {
        private OpenApiResponse _target;
        private readonly OpenApiReference _reference;
        private string _description;

        private OpenApiResponse Target
        {
            get
            {
                _target ??= _reference.HostDocument.ResolveReferenceTo<OpenApiResponse>(_reference);
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
                Type = ReferenceType.Response,
                ExternalResource = externalResource
            };

            Reference = _reference;
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
        public override IDictionary<string, OpenApiHeader> Headers { get => Target.Headers; set => Target.Headers = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiLink> Links { get => Target.Links; set => Target.Links = value; }

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
                SerializeInternal(writer, (writer, element) => element.SerializeAsV3WithoutReference(writer));
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
                SerializeInternal(writer, (writer, element) => element.SerializeAsV31WithoutReference(writer));
            }
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
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            Utils.CheckArgumentNull(writer);;
            action(writer, this);
        }
    }
}
