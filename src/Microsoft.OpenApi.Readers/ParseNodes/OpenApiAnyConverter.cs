// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal static class OpenApiAnyConverter
    {
        /// <summary>
        /// Converts the <see cref="JsonValue"/>s in the given <see cref="JsonValue"/>
        /// into the appropriate <see cref="JsonNode"/> type based on the given <see cref="OpenApiSchema"/>.
        /// For those strings that the schema does not specify the type for, convert them into
        /// the most specific type based on the value.
        /// </summary>
        public static JsonNode GetSpecificOpenApiAny(JsonNode jsonNode, OpenApiSchema schema = null)
        {
            if (jsonNode is JsonArray jsonArray)
            {
                var newArray = new JsonArray();
                foreach (var element in jsonArray)
                {
                    if(element.Parent != null)
                    {
                        var newNode = element.Deserialize<JsonNode>();
                        newArray.Add(GetSpecificOpenApiAny(newNode, schema?.Items));

                    }
                    else
                    {
                        newArray.Add(GetSpecificOpenApiAny(element, schema?.Items));
                    }
                }

                return newArray;
            }

            if (jsonNode is JsonObject jsonObject)
            {
                var newObject = new JsonObject();
                foreach (var property in jsonObject)
                {
                    if (schema?.Properties != null && schema.Properties.TryGetValue(property.Key, out var propertySchema))
                    {
                        if (jsonObject[property.Key].Parent != null)
                        {
                            var node = jsonObject[property.Key].Deserialize<JsonNode>();
                            newObject.Add(property.Key, GetSpecificOpenApiAny(node, propertySchema));
                        }
                        else
                        {
                            newObject.Add(property.Key, GetSpecificOpenApiAny(property.Value, propertySchema));

                        }
                    }
                    else
                    {
                        if (jsonObject[property.Key].Parent != null)
                        {
                            var node = jsonObject[property.Key].Deserialize<JsonNode>();
                            newObject[property.Key] = GetSpecificOpenApiAny(node, schema?.AdditionalProperties);
                        }
                        else
                        {
                            newObject[property.Key] = GetSpecificOpenApiAny(jsonObject[property.Key], schema?.AdditionalProperties);
                        }
                    }
                }
                
                return newObject;
            }

            if (jsonNode is not JsonValue jsonValue)
            {
                return jsonNode;
            }

            var value = jsonValue.GetScalarValue();
            var type = schema?.Type;
            var format = schema?.Format;

            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                // More narrow type detection for explicit strings, only check types that are passed as strings
                if (schema == null)
                {
                    if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
                    {
                        return dateTimeValue;
                    }
                }
                else if (type == "string")
                {
                    if (format == "byte")
                    {
                        try
                        {
                            
                            var base64String = Convert.FromBase64String(value);
                            return JsonNode.Parse(base64String);
                        }
                        catch (FormatException)
                        { }
                    }

                    if (format == "binary")
                    {
                        try
                        {
                            return JsonNode.Parse(Encoding.UTF8.GetBytes(value));
                        }
                        catch (EncoderFallbackException)
                        { }
                    }

                    if (format == "date")
                    {
                        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
                        {
                            return dateValue.Date;
                        }
                    }

                    if (format == "date-time")
                    {
                        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
                        {
                            return dateTimeValue;
                        }
                    }

                    if (format == "password")
                    {
                        return value;
                    }
                }

                return jsonNode;
            }

            if (value == null || value == "null")
            {
                return null;
            }

            if (schema?.Type == null)
            {
                if (value == "true")
                {
                    return true;
                }

                if (value == "false")
                {
                    return false;
                }

                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
                {
                    return intValue;
                }

                if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
                {
                    return longValue;
                }

                if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
                {
                    return doubleValue;
                }

                if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
                {
                    return dateTimeValue;
                }
            }
            else
            {
                if (type == "integer" && format == "int32")
                {
                    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
                    {
                        return intValue;
                    }
                }

                if (type == "integer" && format == "int64")
                {
                    if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
                    {
                        return longValue;
                    }
                }

                if (type == "integer")
                {
                    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
                    {
                        return intValue;
                    }
                }

                if (type == "number" && format == "float")
                {
                    if (float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var floatValue))
                    {
                        return floatValue;
                    }
                }

                if (type == "number" && format == "double")
                {
                    if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
                    {
                        return doubleValue;
                    }
                }

                if (type == "number")
                {
                    if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
                    {
                        return doubleValue;
                    }
                }

                if (type == "string" && format == "byte")
                {
                    try
                    {
                        return JsonNode.Parse(Convert.FromBase64String(value));
                    }
                    catch (FormatException)
                    { }
                }

                // binary
                if (type == "string" && format == "binary")
                {
                    try
                    {
                        return JsonNode.Parse(Encoding.UTF8.GetBytes(value));
                    }
                    catch (EncoderFallbackException)
                    { }
                }

                if (type == "string" && format == "date")
                {
                    if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
                    {
                        return dateValue.Date;
                    }
                }

                if (type == "string" && format == "date-time")
                {
                    if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
                    {
                        return dateTimeValue;
                    }
                }

                if (type == "string" && format == "password")
                {
                    return value;
                }

                if (type == "string")
                {
                    return jsonNode;
                }

                if (type == "boolean")
                {
                    if (bool.TryParse(value, out var booleanValue))
                    {
                        return booleanValue;
                    }
                }
            }

            // If data conflicts with the given type, return a string.
            // This converter is used in the parser, so it does not perform any validations, 
            // but the validator can be used to validate whether the data and given type conflicts.
            return jsonNode;
        }
    }
}
