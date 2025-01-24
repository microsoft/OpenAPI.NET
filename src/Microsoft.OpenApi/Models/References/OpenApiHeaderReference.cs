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
    public class OpenApiHeaderReference : OpenApiHeader, IOpenApiReferenceableWithTarget<OpenApiHeader>
    {
        internal OpenApiHeader _target;
        private readonly OpenApiReference _reference;
        private string _description;

        /// <summary>
        /// Gets the target header.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiHeader Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiHeader>(_reference);
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
            get => string.IsNullOrEmpty(_description) ? Target?.Description : _description;
            set => _description = value;
        }

        private bool? _required;
        /// <inheritdoc/>
        public override bool Required { get => _required is not null ? _required.Value : Target?.Required ?? false; set => _required = value; }

        private bool? _deprecated;
        /// <inheritdoc/>
        public override bool Deprecated { get => _deprecated is not null ? _deprecated.Value : Target?.Deprecated ?? false; set => _deprecated = value; }

        private bool? _allowEmptyValue;
        /// <inheritdoc/>
        public override bool AllowEmptyValue { get => _allowEmptyValue is not null ? _allowEmptyValue.Value : Target?.AllowEmptyValue ?? false; set => _allowEmptyValue = value; }

        private OpenApiSchema _schema;
        /// <inheritdoc/>
        public override OpenApiSchema Schema { get => _schema is not null ? _schema : Target?.Schema; set => _schema = value; }

        private ParameterStyle? _style;
        /// <inheritdoc/>
        public override ParameterStyle? Style { get => _style is not null ? _style : Target?.Style; set => _style = value; }

        private bool? _explode;
        /// <inheritdoc/>
        public override bool Explode { get => _explode is not null ? _explode.Value : Target?.Explode ?? false; set => _explode = value; }

        private bool? _allowReserved;
        /// <inheritdoc/>
        public override bool AllowReserved { get => _allowReserved is not null ? _allowReserved.Value : Target?.AllowReserved ?? false ; set => _allowReserved = value; }

        private JsonNode _example;
        /// <inheritdoc/>
        public override JsonNode Example { get => _example is not null ? _example : Target?.Example; set => _example = value; }

        private IDictionary<string, OpenApiExample> _examples;
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiExample> Examples { get => _examples is not null ? _examples : Target?.Examples; set => _examples = value; }

        private IDictionary<string, OpenApiMediaType> _content;
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiMediaType> Content { get => _content is not null ? _content : Target?.Content; set => _content = value; }

        private IDictionary<string, IOpenApiExtension> _extensions;
        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => _extensions is not null ? _extensions : Target?.Extensions; set => _extensions = value; }
        
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
