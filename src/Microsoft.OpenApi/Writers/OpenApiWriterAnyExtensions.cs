// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Extensions methods for writing the <see cref="OpenApiAny"/>
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

                    if (item.Value == null)
                    {
                        writer.WriteNull();
                    }
                    else
                    {
                        item.Value.Write(writer, specVersion);
                    }
                }
            }
        }

        /// <summary>
        /// Write the <see cref="JsonNode"/> value.
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="any">The Any value</param>
        public static void WriteAny(this IOpenApiWriter writer, OpenApiAny any)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));
            
            if (any.Node == null)
            {
                writer.WriteNull();
                return;
            }

            var node = any.Node;
            var element = JsonDocument.Parse(node.ToJsonString()).RootElement;
            switch (element.ValueKind)
            {
                case JsonValueKind.Array: // Array
                    writer.WriteArray(node as JsonArray);
                    break;                    
                case JsonValueKind.Object: // Object
                    writer.WriteObject(node as JsonObject);
                    break;                    
                case JsonValueKind.String: // Primitive
                    writer.WritePrimitive(element);
                    break;
                case JsonValueKind.Number: // Primitive
                    writer.WritePrimitive(element);
                    break;
                case JsonValueKind.True or JsonValueKind.False: // Primitive
                    writer.WritePrimitive(element);
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
                writer.WriteAny(new OpenApiAny(item));
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
                writer.WriteAny(new OpenApiAny(item.Value));
            }

            writer.WriteEndObject();
        }

        private static void WritePrimitive(this IOpenApiWriter writer, JsonElement primitive)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (primitive.ValueKind == JsonValueKind.String)
            {
                // check whether string is actual string or date time object
                if (primitive.TryGetDateTime(out var dateTime))
                {
                    writer.WriteValue(dateTime);
                }
                else if (primitive.TryGetDateTimeOffset(out var dateTimeOffset))
                {
                        writer.WriteValue(dateTimeOffset);
                }
                else
                {
                    writer.WriteValue(primitive.GetString());
                }
            }

            if (primitive.ValueKind == JsonValueKind.Number)
            {
                if (primitive.TryGetDecimal(out var decimalValue))
                {
                    writer.WriteValue(decimalValue);
                }
                else if (primitive.TryGetDouble(out var doubleValue))
                {
                    writer.WriteValue(doubleValue);
                }
                else if (primitive.TryGetInt64(out var longValue))
                {
                    writer.WriteValue(longValue);
                }
                else if (primitive.TryGetInt32(out var intValue))
                {
                    writer.WriteValue(intValue);
                }
            }
            if (primitive.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                writer.WriteValue(primitive.GetBoolean());
            }
        }
    }
}
