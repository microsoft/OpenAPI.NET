// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Header Object Reference.
    /// </summary>
    public class OpenApiHeaderReference : OpenApiHeader
    {
        internal OpenApiHeader _target;
        private readonly OpenApiReference _reference;
        private string _description;

        private OpenApiHeader Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiHeader>(_reference);
                OpenApiHeader resolved = new OpenApiHeader(_target);
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
        public OpenApiHeaderReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Header,
                ExternalResource = externalResource
            };

            Reference = _reference;
        }

        internal OpenApiHeaderReference(OpenApiHeader target, string referenceId)
        {
            _target = target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Header,
            };
        }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target.Description : _description;
            set => _description = value;
        }

        /// <inheritdoc/>
        public override bool Required { get => Target.Required; set => Target.Required = value; }

        /// <inheritdoc/>
        public override bool Deprecated { get => Target.Deprecated; set => Target.Deprecated = value; }

        /// <inheritdoc/>
        public override bool AllowEmptyValue { get => Target.AllowEmptyValue; set => Target.AllowEmptyValue = value; }

        /// <inheritdoc/>
        public override OpenApiSchema Schema { get => Target.Schema; set => Target.Schema = value; }

        /// <inheritdoc/>
        public override ParameterStyle? Style { get => Target.Style; set => Target.Style = value; }

        /// <inheritdoc/>
        public override bool Explode { get => Target.Explode; set => Target.Explode = value; }

        /// <inheritdoc/>
        public override bool AllowReserved { get => Target.AllowReserved; set => Target.AllowReserved = value; }

        /// <inheritdoc/>
        public override JsonNode Example { get => Target.Example; set => Target.Example = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiExample> Examples { get => Target.Examples; set => Target.Examples = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiMediaType> Content { get => Target.Content; set => Target.Content = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => base.Extensions; set => base.Extensions = value; }
        
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
            Utils.CheckArgumentNull(writer);
            action(writer, Target);
        }
    }
}
