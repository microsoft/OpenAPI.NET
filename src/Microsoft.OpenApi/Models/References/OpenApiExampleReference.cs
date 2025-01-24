﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Example Object Reference.
    /// </summary>
    public class OpenApiExampleReference : OpenApiExample, IOpenApiReferenceableWithTarget<OpenApiExample>
    {
        internal OpenApiExample _target;
        private readonly OpenApiReference _reference;
        private string _summary;
        private string _description;

        /// <summary>
        /// Gets the target example.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiExample Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiExample>(_reference);
                if (!string.IsNullOrEmpty(_description)) _target.Description = _description;
                if (!string.IsNullOrEmpty(_summary)) _target.Summary = _summary;
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
        public OpenApiExampleReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Example,
                ExternalResource = externalResource
            };

            Reference = _reference;
        }

        internal OpenApiExampleReference(OpenApiExample target, string referenceId)
        {
            _target = target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Example,
            };
        }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target?.Description : _description;
            set => _description = value;
        }

        /// <inheritdoc/>
        public override string Summary
        {
            get => string.IsNullOrEmpty(_summary) ? Target?.Summary : _summary;
            set => _summary = value;
        }

        private IDictionary<string, IOpenApiExtension> _extensions;
        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => _extensions is not null ? _extensions : Target?.Extensions; set => _extensions = value; }

        private string _externalValue;
        /// <inheritdoc/>
        public override string ExternalValue { get => !string.IsNullOrEmpty(_externalValue) ? _externalValue : Target?.ExternalValue; set => _externalValue = value; }

        private JsonNode _value;
        /// <inheritdoc/>
        public override JsonNode Value { get => _value is not null ? _value : Target?.Value; set => _value = value; }

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
                SerializeInternal(writer, (writer, referenceElement) => referenceElement.SerializeAsV3(writer));
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
                SerializeInternal(writer, (writer, referenceElement) => referenceElement.SerializeAsV31(writer));
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
