// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Tag Object Reference
    /// </summary>
    public class OpenApiTagReference : OpenApiTag, IOpenApiReferenceHolder<OpenApiTag>
    {
        internal OpenApiTag _target;

        /// <summary>
        /// Reference.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Resolved target of the reference.
        /// </summary>
        public OpenApiTag Target
        {
            get
            {
                _target ??= Reference.HostDocument?.Tags.FirstOrDefault(t => StringComparer.Ordinal.Equals(t.Name, Reference.Id));
                return _target;
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

            Reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Tag
            };
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="source">The source to copy information from.</param>
        public OpenApiTagReference(OpenApiTagReference source):base()
        {
            Reference = source?.Reference != null ? new(source.Reference) : null;
            _target = source?._target;
        }

        private const string ReferenceErrorMessage = "Setting the value from the reference is not supported, use the target property instead.";
        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => throw new InvalidOperationException(ReferenceErrorMessage); }

        /// <inheritdoc/>
        public override OpenApiExternalDocs ExternalDocs { get => Target.ExternalDocs; set => throw new InvalidOperationException(ReferenceErrorMessage); }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => throw new InvalidOperationException(ReferenceErrorMessage); }

        /// <inheritdoc/>
        public override string Name { get => Target.Name; set => throw new InvalidOperationException(ReferenceErrorMessage); }
        
        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV3(writer);
            }
            else
            {
                SerializeInternal(writer);
            }
        }

        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV31(writer);
            }
            else
            {
                SerializeInternal(writer);
            }
        }

        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV2(writer);
            }
            else
            {
                SerializeInternal(writer);
            }
        }

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);
            writer.WriteValue(Name);
        }
    }
}
