// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// A dictionary of OpenApi extensions.
    /// </summary>
    public class OpenApiExtensionDictionary : Dictionary<string, IOpenApiExtension>
    {
        /// <summary>
        /// Initializes a copy of <see cref="OpenApiExtensionDictionary"/> object
        /// </summary>
        /// <param name="extensions"></param>
        public OpenApiExtensionDictionary(OpenApiExtensionDictionary extensions) : base(dictionary: extensions) { }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiExtensionDictionary() { }

        /// <summary>
        /// Override the base class indexer to return OpenApiAny.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new OpenApiAny this[string key]
        {           
            get => (OpenApiAny)base[key];
            set => base[key] = ConvertIfJsonNode(value)!;
        }

        /// <summary>
        /// Adds an extension to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            base.Add(key, ConvertIfJsonNode(value)!);
        }

        private static IOpenApiExtension? ConvertIfJsonNode(object? value)
        {
            return value switch
            {
                IOpenApiExtension extension => extension,
                JsonNode node => (OpenApiAny)node,
                _ => null
            };
        }
    }
}
