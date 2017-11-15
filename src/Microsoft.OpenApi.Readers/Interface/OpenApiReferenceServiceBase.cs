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
        protected OpenApiReferenceServiceBase(RootNode rootNode)
        {
            RootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        }

        /// <summary>
        /// Loads the referenced object matching the given OpenApiReference object.
        /// </summary>
        public abstract IOpenApiReferenceable LoadReference(OpenApiReference reference);

        /// <summary>
        /// Loads <see cref="OpenApiTag"/> from parse node.
        /// </summary>
        protected static OpenApiTag LoadTag(ParseNode parseNode)
        {
            var mapNode = parseNode.CheckMapNode("tag");

            var obj = new OpenApiTag();

            foreach (var node in mapNode)
            {
                var key = node.Name;
                switch (key)
                {
                    case "description":
                        obj.Description = node.Value.GetScalarValue();
                        break;
                    case "name":
                        obj.Name = node.Value.GetScalarValue();
                        break;
                }
            }

            return obj;
        }
        
        /// <summary>
        /// Gets the OpenApiReference object from string and reference type.
        /// </summary>
        public abstract OpenApiReference ConvertToOpenApiReference(
            string referenceString,
            ReferenceType? type);

        /// <summary>
        /// Parse the local pointer to return a <see cref="OpenApiReference"/>.
        /// </summary>
        /// <param name="localPointer">The local pointer.</param>
        /// <returns>The <see cref="OpenApiReference"/> or null.</returns>
        protected abstract OpenApiReference ParseLocalPointer(string localPointer);
    }
}
