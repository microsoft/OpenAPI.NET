// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal static class OpenApiAnyConverter
    {
        /// <summary>
        /// Converts the <see cref="OpenApiString"/>s in the given <see cref="IOpenApiAny"/>
        /// into the appropriate <see cref="IOpenApiPrimitive"/> type based on the given <see cref="OpenApiSchema"/>.
        /// For those strings that the schema does not specify the type for, convert them into
        /// the most specific type based on the value.
        /// </summary>
        public static IOpenApiAny GetSpecificOpenApiAny(IOpenApiAny openApiAny, OpenApiSchema schema = null)
        {
            if (openApiAny is OpenApiArray openApiArray)
            {
                var newArray = new OpenApiArray();
                foreach (var element in openApiArray)
                {
                    newArray.Add(GetSpecificOpenApiAny(element, schema?.Items));
                }

                return newArray;
            }

            if (openApiAny is OpenApiObject openApiObject)
            {
                var newObject = new OpenApiObject();

                foreach (var key in openApiObject.Keys.ToList())
                {
                    if (schema?.Properties != null && schema.Properties.TryGetValue(key, out var property))
                    {
                        newObject[key] = GetSpecificOpenApiAny(openApiObject[key], property);
                    }
                    else
                    {
                        newObject[key] = GetSpecificOpenApiAny(openApiObject[key], schema?.AdditionalProperties);
                    }
                }

                return newObject;
            }

            if (openApiAny is not OpenApiString apiString)
            {
                return openApiAny;
            }

            var value = apiString.Value;
            var type = schema?.Type;
            var format = schema?.Format;

            if (apiString.IsExplicit())
            {
                // More narrow type detection for explicit strings, only check types that are passed as strings
                if (schema == null)
                {
                    if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
                    {
                        // if the time component is exactly midnight(00:00:00) meaning no time has elapsed, return a date-only value
                        return dateTimeValue.TimeOfDay == TimeSpan.Zero ? new OpenApiDate(dateTimeValue.Date) 
                            : new OpenApiDateTime(dateTimeValue);
                    }
                }
                else if (type == "string")
                {
                    if (format == "byte")
                    {
                        try
                        {
                            return new OpenApiByte(Convert.FromBase64String(value));
                        }
                        catch (FormatException)
                        { }
                    }

                    if (format == "binary")
                    {
                        try
                        {
                            return new OpenApiBinary(Encoding.UTF8.GetBytes(value));
                        }
                        catch (EncoderFallbackException)
                        { }
                    }

                    if (format == "date")
                    {
                        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
                        {
                            return new OpenApiDate(dateValue.Date);
                        }
                    }

                    if (format == "date-time")
                    {
                        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
                        {
                            return new OpenApiDateTime(dateTimeValue);
                        }
                    }

                    if (format == "password")
                    {
                        return new OpenApiPassword(value);
                    }
                }

                return apiString;
            }

            if (value is null or "null")
            {
                return new OpenApiNull();
            }

            if (schema?.Type == null)
            {
                if (value == "true")
                {
                    return new OpenApiBoolean(true);
                }

                if (value == "false")
                {
                    return new OpenApiBoolean(false);
                }

                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
                {
                    return new OpenApiInteger(intValue);
                }

                if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
                {
                    return new OpenApiLong(longValue);
                }

                if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
                {
                    return new OpenApiDouble(doubleValue);
                }

                if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
                {
                    return new OpenApiDateTime(dateTimeValue);
                }
            }
            else
            {
                if (type == "integer" && format == "int32")
                {
                    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
                    {
                        return new OpenApiInteger(intValue);
                    }
                }

                if (type == "integer" && format == "int64")
                {
                    if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
                    {
                        return new OpenApiLong(longValue);
                    }
                }

                if (type == "integer")
                {
                    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
                    {
                        return new OpenApiInteger(intValue);
                    }
                }

                if (type == "number" && format == "float")
                {
                    if (float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var floatValue))
                    {
                        return new OpenApiFloat(floatValue);
                    }
                }

                if (type == "number" && format == "double")
                {
                    if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
                    {
                        return new OpenApiDouble(doubleValue);
                    }
                }

                if (type == "number")
                {
                    if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
                    {
                        return new OpenApiDouble(doubleValue);
                    }
                }

                if (type == "string" && format == "byte")
                {
                    try
                    {
                        return new OpenApiByte(Convert.FromBase64String(value));
                    }
                    catch (FormatException)
                    { }
                }

                // binary
                if (type == "string" && format == "binary")
                {
                    try
                    {
                        return new OpenApiBinary(Encoding.UTF8.GetBytes(value));
                    }
                    catch (EncoderFallbackException)
                    { }
                }

                if (type == "string" && format == "date")
                {
                    if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
                    {
                        return new OpenApiDate(dateValue.Date);
                    }
                }

                if (type == "string" && format == "date-time")
                {
                    if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
                    {
                        return new OpenApiDateTime(dateTimeValue);
                    }
                }

                if (type == "string" && format == "password")
                {
                    return new OpenApiPassword(value);
                }

                if (type == "string")
                {
                    return apiString;
                }

                if (type == "boolean")
                {
                    if (bool.TryParse(value, out var booleanValue))
                    {
                        return new OpenApiBoolean(booleanValue);
                    }
                }
            }

            // If data conflicts with the given type, return a string.
            // This converter is used in the parser, so it does not perform any validations,
            // but the validator can be used to validate whether the data and given type conflicts.
            return apiString;
        }
    }
}
