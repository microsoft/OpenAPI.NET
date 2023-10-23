// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text.Json;
using Json.Schema;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using JsonSchema = Json.Schema.JsonSchema;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        public static JsonSchema LoadSchema(ParseNode node)
        {
            var mapNode = node.CheckMapNode(OpenApiConstants.Schema);
            var builder = new JsonSchemaBuilder();

            // check for a $ref and if present, add it to the builder as a Ref keyword
            if (mapNode.GetReferencePointer() is {} pointer)
            {
                builder = builder.Ref(pointer);

                // Check for summary and description and append to builder
                var summary = mapNode.GetSummaryValue();
                var description = mapNode.GetDescriptionValue(); 
                if (!string.IsNullOrEmpty(summary))
                {
                    builder.Summary(summary);
                }
                if (!string.IsNullOrEmpty(description))
                {
                    builder.Description(description);
                }

                return builder.Build();
            }
            else
            {
                return node.JsonNode.Deserialize<JsonSchema>();
            }
        }
    }

}
