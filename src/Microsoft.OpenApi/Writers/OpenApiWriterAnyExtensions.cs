// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Extensions methods for writing the <see cref="JsonNodeExtension"/>
    /// </summary>
    public static class OpenApiWriterAnyExtensions
    {
        /// <summary>
        /// Write the specification extensions
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="extensions">The specification extensions.</param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public static void WriteExtensions(this IOpenApiWriter writer, IDictionary<string, IOpenApiExtension>? extensions, OpenApiSpecVersion specVersion)
        {
            Utils.CheckArgumentNull(writer);

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
        /// <param name="node">The JsonNode value</param>
        public static void WriteAny(this IOpenApiWriter writer, JsonNode? node)
        {
            Utils.CheckArgumentNull(writer);

            if (node == null)
            {
                writer.WriteNull();
                return;
            }

            switch (node.GetValueKind())
            {
                case JsonValueKind.Array: // Array
                    writer.WriteArray(node as JsonArray);
                    break;
                case JsonValueKind.Object: // Object
                    writer.WriteObject(node as JsonObject);
                    break;
                case JsonValueKind.String: // Primitive
                    writer.WritePrimitive(node.AsValue());
                    break;
                case JsonValueKind.Number: // Primitive
                    writer.WritePrimitive(node.AsValue());
                    break;
                case JsonValueKind.True or JsonValueKind.False: // Primitive
                    writer.WritePrimitive(node.AsValue());
                    break;
                case JsonValueKind.Null: // null
                    writer.WriteNull();
                    break;
                default:
                    break;
            }
        }

        private static void WriteArray(this IOpenApiWriter writer, JsonArray? array)
        {
            writer.WriteStartArray();
            if (array is not null)
            {
                foreach (var item in array)
                {
                    writer.WriteAny(item);
                }
            }            

            writer.WriteEndArray();
        }

        private static void WriteObject(this IOpenApiWriter writer, JsonObject? entity)
        {
            writer.WriteStartObject();
            if (entity is not null)
            {
                foreach (var item in entity)
                {
                    writer.WritePropertyName(item.Key);
                    writer.WriteAny(item.Value);
                }
            }
            writer.WriteEndObject();
        }

        private static void WritePrimitive(this IOpenApiWriter writer, JsonValue jsonValue)
        {
            if (jsonValue.TryGetValue(out string? stringValue) && stringValue is not null)
                writer.WriteValue(stringValue);
            else if (jsonValue.TryGetValue(out DateTime dateTimeValue))
                writer.WriteValue(dateTimeValue.ToString("o", CultureInfo.InvariantCulture)); // ISO 8601 format
            else if (jsonValue.TryGetValue(out DateTimeOffset dateTimeOffsetValue))
                writer.WriteValue(dateTimeOffsetValue.ToString("o", CultureInfo.InvariantCulture));
#if NET6_0_OR_GREATER
            else if (jsonValue.TryGetValue(out DateOnly dateOnlyValue))
                writer.WriteValue(dateOnlyValue.ToString("o", CultureInfo.InvariantCulture));
            else if (jsonValue.TryGetValue(out TimeOnly timeOnlyValue))
                writer.WriteValue(timeOnlyValue.ToString("o", CultureInfo.InvariantCulture));
#endif
            else if (jsonValue.TryGetValue(out bool boolValue)) 
                writer.WriteValue(boolValue);
            // write number values
            else if (jsonValue.TryGetValue(out decimal decimalValue))
                writer.WriteValue(decimalValue);
            else if (jsonValue.TryGetValue(out double doubleValue))
                writer.WriteValue(doubleValue);
            else if (jsonValue.TryGetValue(out float floatValue))
                writer.WriteValue(floatValue);
            else if (jsonValue.TryGetValue(out long longValue))
                writer.WriteValue(longValue);
            else if (jsonValue.TryGetValue(out int intValue))
                writer.WriteValue(intValue);
        }
    }
}
