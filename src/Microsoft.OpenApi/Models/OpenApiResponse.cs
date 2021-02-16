// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Response object.
    /// </summary>
    public class OpenApiResponse : IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. A short description of the response.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Maps a header name to its definition.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();

        /// <summary>
        /// A map containing descriptions of potential response payloads.
        /// The key is a media type or media type range and the value describes it.
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>
        /// A map of operations links that can be followed from the response.
        /// The key of the map is a short name for the link,
        /// following the naming constraints of the names for Component Objects.
        /// </summary>
        public IDictionary<string, OpenApiLink> Links { get; set; } = new Dictionary<string, OpenApiLink>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Reference { get; set; }
    }
}
