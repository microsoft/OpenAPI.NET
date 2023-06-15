using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using Json.Schema;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Any
{
    public class JsonSchemaWrapper : IOpenApiElement, IOpenApiReferenceable
    {
        private readonly JsonSchema jsonSchema;

        /// <summary>
        /// Initializes the <see cref="OpenApiAny"/> class.
        /// </summary>
        /// <param name="jsonSchema"></param>
        public JsonSchemaWrapper(JsonSchema jsonSchema)
        {
            this.jsonSchema = jsonSchema;
        }

        /// <summary>
        /// Gets the underlying JsonNode.
        /// </summary>
        public JsonSchema JsonSchema { get { return jsonSchema; } }

        /// <inheritdoc/>
        public bool UnresolvedReference { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public OpenApiReference Reference { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc/>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }

        public void SerializeAsV31WithoutReference(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }

        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
