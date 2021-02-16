// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// XML Object.
    /// </summary>
    public class OpenApiXml : IOpenApiExtensible
    {
        /// <summary>
        /// Replaces the name of the element/attribute used for the described schema property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URI of the namespace definition. Value MUST be in the form of an absolute URI.
        /// </summary>
        public Uri Namespace { get; set; }

        /// <summary>
        /// The prefix to be used for the name
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Declares whether the property definition translates to an attribute instead of an element.
        /// Default value is false.
        /// </summary>
        public bool Attribute { get; set; }

        /// <summary>
        /// Signifies whether the array is wrapped.
        /// Default value is false.
        /// </summary>
        public bool Wrapped { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();
    }
}
