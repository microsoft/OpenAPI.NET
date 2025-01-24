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
    /// Link Object Reference.
    /// </summary>
    public class OpenApiLinkReference : IOpenApiLink, IOpenApiReferenceHolder<OpenApiLink, IOpenApiLink>
    {
        /// <inheritdoc/>
        public OpenApiReference Reference { get; set; }

        /// <inheritdoc/>
        public bool UnresolvedReference { get; set; }
        internal OpenApiLink _target;
        /// <summary>
        /// Gets the target link.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiLink Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiLink>(Reference);
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
        public OpenApiLinkReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            Reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Link,
                ExternalResource = externalResource
            };
        }
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="reference">The reference to copy</param>
        public OpenApiLinkReference(OpenApiLinkReference reference)
        {
            Utils.CheckArgumentNull(reference);

            Reference = reference?.Reference != null ? new(reference.Reference) : null;
            UnresolvedReference = reference?.UnresolvedReference ?? false;
            //no need to copy summary and description as if they are not overridden, they will be fetched from the target
            //if they are, the reference copy will handle it
        }

        internal OpenApiLinkReference(OpenApiLink target, string referenceId)
        {
            _target = target;

            Reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Link,
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
        public string OperationRef { get => Target?.OperationRef; }

        /// <inheritdoc/>
        public string OperationId { get => Target?.OperationId; }

        /// <inheritdoc/>
        public OpenApiServer Server { get => Target?.Server; }

        /// <inheritdoc/>
        public IDictionary<string, RuntimeExpressionAnyWrapper> Parameters { get => Target?.Parameters; }

        /// <inheritdoc/>
        public RuntimeExpressionAnyWrapper RequestBody { get => Target?.RequestBody; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension> Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV3(writer);
            }
            else
            {
                SerializeInternal(writer, (writer, element) => CopyReferenceAsTargetElementWithOverrides(element).SerializeAsV3(writer));
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
                SerializeInternal(writer, (writer, element) => CopyReferenceAsTargetElementWithOverrides(element).SerializeAsV31(writer));
            }
        }

        /// <inheritdoc/>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Link object does not exist in V2.
        }

        /// <inheritdoc/>
        public IOpenApiLink CopyReferenceAsTargetElementWithOverrides(IOpenApiLink source)
        {
            return source is OpenApiLink ? new OpenApiLink(this) : source;
        }

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiLink> action)
        {
            Utils.CheckArgumentNull(writer);
            action(writer, Target);
        }
    }
}
