// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Extensions methods for writing the <see cref="JsonNode"/>
    /// </summary>
    public static class OpenApiWriterAnyExtensions
    {
        /// <summary>
        /// Write the specification extensions
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="extensions">The specification extensions.</param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public static void WriteExtensions(this IOpenApiWriter writer, IDictionary<string, IOpenApiExtension> extensions, OpenApiSpecVersion specVersion)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (extensions != null)
            {
                foreach (var item in extensions)
                {
                    writer.WritePropertyName(item.Key);
                    item.Value.Write(writer, specVersion);
                }
            }
        }

        /// <summary>
        /// Write the <see cref="JsonNode"/> value.
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="node">The Any value</param>
        public static void WriteAny(this IOpenApiWriter writer, JsonNode node)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));
            
            if (node == null)
            {
                writer.WriteNull();
                return;
            }

            JsonElement element = JsonSerializer.Deserialize<JsonElement>(node);
            switch (element.ValueKind)
            {
                case JsonValueKind.Array: // Array
                    writer.WriteArray(node as JsonArray);
                    break;                    
                case JsonValueKind.Object: // Object
                    writer.WriteObject(node as JsonObject);
                    break;                    
                case JsonValueKind.String: // Primitive
                    writer.WritePrimitive(node as JsonValue);
                    break;
                case JsonValueKind.Number: // Primitive
                    writer.WritePrimitive(node as JsonValue);
                    break;
                case JsonValueKind.True: // Primitive
                    writer.WritePrimitive(node as JsonValue);
                    break;
                case JsonValueKind.False: // Primitive
                    writer.WritePrimitive(node as JsonValue);
                    break;
                case JsonValueKind.Null: // null
                    writer.WriteNull();
                    break;
                default:
                    break;
            }
        }

        private static void WriteArray(this IOpenApiWriter writer, JsonArray array)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (array == null)
            {
                throw Error.ArgumentNull(nameof(array));
            }

            writer.WriteStartArray();

            foreach (var item in array)
            {
                writer.WriteAny(item);
            }

            writer.WriteEndArray();
        }

        private static void WriteObject(this IOpenApiWriter writer, JsonObject entity)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (entity == null)
            {
                throw Error.ArgumentNull(nameof(entity));
            }

            writer.WriteStartObject();

            foreach (var item in entity)
            {
                writer.WritePropertyName(item.Key);
                writer.WriteAny(item.Value);
            }

            writer.WriteEndObject();
        }

        private static void WritePrimitive(this IOpenApiWriter writer, JsonValue primitive)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (primitive == null)
            {
                throw Error.ArgumentNull(nameof(primitive));
            }

            writer.WriteAny(primitive);
            
            // The Spec version is meaning for the Any type, so it's ok to use the latest one.
            //primitive.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        }
    }
}
