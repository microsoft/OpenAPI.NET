﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// A wrapper class for JsonNode
    /// </summary>
    public class JsonNodeExtension : IOpenApiElement, IOpenApiExtension
    {
        private readonly JsonNode jsonNode;

        /// <summary>
        /// Initializes the <see cref="JsonNodeExtension"/> class.
        /// </summary>
        /// <param name="jsonNode"></param>
        public JsonNodeExtension(JsonNode jsonNode)
        {
            this.jsonNode = jsonNode;
        }

        /// <summary>
        /// Gets the underlying JsonNode.
        /// </summary>
        public JsonNode Node { get { return jsonNode; } }

        /// <summary>
        /// Writes out the JsonNodeExtension type.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="specVersion"></param>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteAny(Node);
        }
    }
}
