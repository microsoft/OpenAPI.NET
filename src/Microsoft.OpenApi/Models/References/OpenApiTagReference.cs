// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Tag Object Reference
    /// </summary>
    public class OpenApiTagReference : OpenApiTag
    {
        internal OpenApiTag _target;
        private readonly OpenApiReference _reference;
        private string _description;

        private OpenApiTag Target
        {
            get
            {
                _target ??= Reference.HostDocument?.ResolveReferenceTo<OpenApiTag>(_reference);
                _target ??= new OpenApiTag() { Name = _reference.Id };
                OpenApiTag resolved = new OpenApiTag(_target);
                if (!string.IsNullOrEmpty(_description)) resolved.Description = _description;
                return resolved;
            }
        }

        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        public OpenApiTagReference(string referenceId, OpenApiDocument hostDocument)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Tag
            };

            Reference = _reference;
        }

        internal OpenApiTagReference(OpenApiTag target, string referenceId)
        {
            _target = target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Tag,
            };
        }

        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target?.Description : _description;
            set => _description = value;
        }

        /// <inheritdoc/>
        public override OpenApiExternalDocs ExternalDocs { get => Target?.ExternalDocs; set => Target.ExternalDocs = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target?.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override string Name { get => Target?.Name; set => Target.Name = value; }
        
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
                SerializeInternal(writer);
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
                SerializeInternal(writer);
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
                SerializeInternal(writer);
            }
        }

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);;
            writer.WriteValue(Name);
        }
    }
}
