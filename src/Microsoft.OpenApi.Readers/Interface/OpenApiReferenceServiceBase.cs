// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.Properties;

namespace Microsoft.OpenApi.Readers.Interface
{
    /// <summary>
    /// The base class for Open API Reference service.
    /// </summary>
    internal abstract class OpenApiReferenceServiceBase : IOpenApiReferenceService
    {
        /// <summary>
        /// The document root node.
        /// </summary>
        public RootNode RootNode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiReferenceServiceBase"/> class.
        /// </summary>
        /// <param name="rootNode">The document root node.</param>
        public OpenApiReferenceServiceBase(RootNode rootNode)
        {
            RootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        }

        /// <inheritdoc />
        public abstract string ToString(OpenApiReference reference);

        /// <inheritdoc />
        public virtual IOpenApiReference LoadReference(OpenApiReference reference)
        {
            if (reference == null)
            {
                return null;
            }

            if (reference.IsExternal)
            {
                // TODO: need to read the external document and load the referenced object.
                throw new NotImplementedException(SRResource.LoadReferencedObjectFromExternalNotImplmented);
            }

            var node = RootNode.Find(GetLocalPointer(reference));
            if (node == null && reference.ReferenceType != ReferenceType.Tag)
            {
                return null;
            }

            return LoadReference(reference, node);
        }

        /// <inheritdoc />
        public virtual OpenApiReference FromString(string pointer)
        {
            if (!String.IsNullOrWhiteSpace(pointer))
            {
                var segments = pointer.Split('#');
                if (segments.Length == 1)
                {
                    // "$ref": "Pet.json"
                    return new OpenApiReference(segments[0]);
                }
                else if (segments.Length == 2)
                {
                    if (pointer.StartsWith("#"))
                    {
                        // "$ref": "#/components/schemas/Pet"
                        return ParseLocalPointer(segments[1]);
                    }
                    else
                    {
                        // $ref: definitions.yaml#/Pet
                        return new OpenApiReference(segments[0], segments[1].Substring(1)); // remove '/'
                    }
                }
            }

            throw new OpenApiException(String.Format(SRResource.ReferenceHasInvalidFormat, pointer));
        }

        /// <summary>
        /// Get the local pointer from a <see cref="OpenApiReference"/>.
        /// </summary>
        /// <param name="reference">The reference object.</param>
        /// <returns>The <see cref="JsonPointer"/> object or null.</returns>
        protected abstract JsonPointer GetLocalPointer(OpenApiReference reference);

        /// <summary>
        /// Load the local element from the Node.
        /// </summary>
        /// <param name="reference">The reference object.</param>
        /// <param name="node">The element node.</param>
        /// <returns>The referenced object or null.</returns>
        protected abstract IOpenApiReference LoadReference(OpenApiReference reference, ParseNode node);

        /// <summary>
        /// Parse the local pointer to return a <see cref="OpenApiReference"/>.
        /// </summary>
        /// <param name="localPointer">The local pointer.</param>
        /// <returns>The <see cref="OpenApiReference"/> or null.</returns>
        protected abstract OpenApiReference ParseLocalPointer(string localPointer);
    }
}
