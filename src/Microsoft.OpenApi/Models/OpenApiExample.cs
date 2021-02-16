// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Example Object.
    /// </summary>
    public class OpenApiExample : IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// Short description for the example.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Long description for the example.
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Embedded literal example. The value field and externalValue field are mutually
        /// exclusive. To represent examples of media types that cannot naturally represented
        /// in JSON or YAML, use a string value to contain the example, escaping where necessary.
        /// </summary>
        public IOpenApiAny Value { get; set; }

        /// <summary>
        /// A URL that points to the literal example.
        /// This provides the capability to reference examples that cannot easily be
        /// included in JSON or YAML documents.
        /// The value field and externalValue field are mutually exclusive.
        /// </summary>
        public string ExternalValue { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Reference object.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Indicates object is a placeholder reference to an actual object and does not contain valid data.
        /// </summary>
        public bool UnresolvedReference { get; set; } = false;
    }
}
