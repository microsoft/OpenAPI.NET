// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
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
                    writer.WritePrimitive(node);
                    break;
                case JsonValueKind.Number: // Primitive
                    writer.WritePrimitive(node);
                    break;
                case JsonValueKind.True or JsonValueKind.False: // Primitive
                    writer.WritePrimitive(node);
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

        private static void WritePrimitive(this IOpenApiWriter writer, JsonNode primitive)
        {
            Utils.CheckArgumentNull(writer);

            var valueKind = primitive.GetValueKind();

            if (valueKind == JsonValueKind.String && primitive is JsonValue jsonStrValue)
            {
                if (jsonStrValue.TryGetValue<DateTimeOffset>(out var dto))
                {
                    writer.WriteValue(dto);
                }
                else if (jsonStrValue.TryGetValue<DateTime>(out var dt))
                {
                    writer.WriteValue(dt);
                }
                else if (jsonStrValue.TryGetValue<string>(out var strValue))
                {
                    // check whether string is actual string or date time object
                    if (DateTimeOffset.TryParse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTimeOffset))
                    {
                        writer.WriteValue(dateTimeOffset);
                    }
                    else if (DateTime.TryParse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime))
                    { // order matters, DTO needs to be checked first!!!
                        writer.WriteValue(dateTime);
                    }
                    else
                    {
                        writer.WriteValue(strValue);
                    }
                }
            }

            else if (valueKind == JsonValueKind.Number && primitive is JsonValue jsonValue)
            {

                if (jsonValue.TryGetValue<decimal>(out var decimalValue))
                {
                    writer.WriteValue(decimalValue);
                }
                else if (jsonValue.TryGetValue<double>(out var doubleValue))
                {
                    writer.WriteValue(doubleValue);
                }
                else if (jsonValue.TryGetValue<float>(out var floatValue))
                {
                    writer.WriteValue(floatValue);
                }
                else if (jsonValue.TryGetValue<long>(out var longValue))
                {
                    writer.WriteValue(longValue);
                }
                else if (jsonValue.TryGetValue<int>(out var intValue))
                {
                    writer.WriteValue(intValue);
                }
            }
            else if (valueKind is JsonValueKind.False)
            {
                writer.WriteValue(false);
            }
            else if (valueKind is JsonValueKind.True)
            {
                writer.WriteValue(true);
            }
        }
    }
}
