// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Tag Object.
    /// </summary>
    public class OpenApiTag : IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A short description for the tag.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Additional external documentation for this tag.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference.
        /// </summary>
        public OpenApiReference Reference { get; set; }
    }
}
