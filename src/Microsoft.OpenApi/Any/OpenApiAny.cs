// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// A wrapper class for JsonNode
    /// </summary>
    public class OpenApiAny : IOpenApiElement, IOpenApiExtension
    {
        private readonly JsonNode jsonNode;

        /// <summary>
        /// Initializes the <see cref="OpenApiAny"/> class.
        /// </summary>
        /// <param name="jsonNode"></param>
        public OpenApiAny(JsonNode jsonNode)
        {
            this.jsonNode = jsonNode;
        }

        /// <summary>
        /// Gets the underlying JsonNode.
        /// </summary>
        public JsonNode Node { get { return jsonNode; } }

        /// <summary>
        /// Writes out the OpenApiAny type.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="specVersion"></param>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteAny(Node);
        }
    }
}
