// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Example Object Reference.
    /// </summary>
    public class OpenApiExampleReference : IOpenApiReferenceHolder<OpenApiExample, IOpenApiExample>, IOpenApiExample
    {
        /// <inheritdoc/>
        public OpenApiReference Reference { get; set; }

        /// <inheritdoc/>
        public bool UnresolvedReference { get; set; }
        internal OpenApiExample _target;

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
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiExample>(Reference);
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

            Reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Example,
                ExternalResource = externalResource
            };
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="example">The reference to copy.</param>
        public OpenApiExampleReference(OpenApiExampleReference example)
        {
            Utils.CheckArgumentNull(example);
            Reference = example.Reference != null ? new(example.Reference) : null;
            UnresolvedReference = example.UnresolvedReference;
            //no need to copy summary and description as if they are not overridden, they will be fetched from the target
            //if they are, the reference copy will handle it
        }

        internal OpenApiExampleReference(OpenApiExample target, string referenceId)
        {
            _target = target;

            Reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Example,
            };
        }

        /// <inheritdoc/>
        public string Description
        {
            get => string.IsNullOrEmpty(Reference?.Description) ? Target?.Description : Reference.Description;
            set 
            {
                if (Reference is not null)
                {
                    Reference.Description = value;
                }
            }
        }

        /// <inheritdoc/>
        public string Summary
        {
            get => string.IsNullOrEmpty(Reference?.Summary) ? Target?.Summary : Reference.Summary;
            set
            {
                if (Reference is not null)
                {
                    Reference.Summary = value;
                }
            }
        }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension> Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public string ExternalValue { get => Target?.ExternalValue; }

        /// <inheritdoc/>
        public JsonNode Value { get => Target?.Value; }

        /// <inheritdoc/>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV3(writer);
            }
            else
            {
                SerializeInternal(writer, (writer, referenceElement) => CopyReferenceAsTargetElementWithOverrides(referenceElement).SerializeAsV3(writer));
            }
        }

        /// <inheritdoc/>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV31(writer);
            }
            else
            {
                SerializeInternal(writer, (writer, referenceElement) => CopyReferenceAsTargetElementWithOverrides(referenceElement).SerializeAsV31(writer));
            }
        }
        
        /// <inheritdoc/>
        public IOpenApiExample CopyReferenceAsTargetElementWithOverrides(IOpenApiExample source)
        {
            return source is OpenApiExample ? new OpenApiExample(this) : source;
        }

        /// <inheritdoc/>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // examples components are not supported in OAS 2.0
            Reference.SerializeAsV2(writer);
        }

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiExample> action)
        {
            Utils.CheckArgumentNull(writer);
            action(writer, Target);
        }
    }
}
