// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Rules
{
    internal static class RuleHelpers
    {
        internal const string DataTypeMismatchedErrorMessage = "Data and type mismatch found.";

        /// <summary>
        /// Input string must be in the format of an email address
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>True if it's an email address. Otherwise False.</returns>
        public static bool IsEmailAddress(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var splits = input.Split('@');
            if (splits.Length != 2)
            {
                return false;
            }

            if (string.IsNullOrEmpty(splits[0]) || string.IsNullOrEmpty(splits[1]))
            {
                return false;
            }

            // Add more rules.

            return true;
        }

        public static void ValidateDataTypeMismatch(
            IValidationContext context,
            string ruleName,
            JsonNode value,
            OpenApiSchema schema)
        {
            if (schema == null)
            {
                return;
            }

            // convert value to JsonElement and access the ValueKind property to determine the type.
            var jsonElement = JsonDocument.Parse(JsonSerializer.Serialize(value)).RootElement;

            var type = OpenApiTypeMapper.ToIdentifier(schema.Type);
            var format = schema.Format;
            var nullable = schema.Nullable;

            // Before checking the type, check first if the schema allows null.
            // If so and the data given is also null, this is allowed for any type.
            if (nullable && jsonElement.ValueKind is JsonValueKind.Null)
            {
                return;
            }

            if (type == "object")
            {
                // It is not against the spec to have a string representing an object value.
                // To represent examples of media types that cannot naturally be represented in JSON or YAML,
                // a string value can contain the example with escaping where necessary
                if (jsonElement.ValueKind is JsonValueKind.String)
                {
                    return;
                }

                // If value is not a string and also not an object, there is a data mismatch.
                if (value is not JsonObject anyObject)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                    return;
                }

                foreach (var kvp in anyObject)
                {
                    var key = kvp.Key;
                    context.Enter(key);

                    if (schema.Properties != null &&
                        schema.Properties.TryGetValue(key, out var property))
                    {
                        ValidateDataTypeMismatch(context, ruleName, anyObject[key], property);
                    }
                    else
                    {
                        ValidateDataTypeMismatch(context, ruleName, anyObject[key], schema.AdditionalProperties);
                    }

                    context.Exit();
                }

                return;
            }

            if (type == "array")
            {
                // It is not against the spec to have a string representing an array value.
                // To represent examples of media types that cannot naturally be represented in JSON or YAML,
                // a string value can contain the example with escaping where necessary
                if (jsonElement.ValueKind is JsonValueKind.String)
                {
                    return;
                }

                // If value is not a string and also not an array, there is a data mismatch.
                if (value is not JsonArray anyArray)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                    return;
                }

                for (var i = 0; i < anyArray.Count; i++)
                {
                    context.Enter(i.ToString());

                    ValidateDataTypeMismatch(context, ruleName, anyArray[i], schema.Items);

                    context.Exit();
                }

                return;
            }

            if (type == "integer" && format == "int32")
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "integer" && format == "int64")
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                       ruleName,
                       DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "integer" && jsonElement.ValueKind is not JsonValueKind.Number)
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "number" && format == "float")
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "number" && format == "double")
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "number")
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "byte")
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "date")
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "date-time")
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "password")
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string")
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "boolean")
            {
                if (jsonElement.ValueKind is not JsonValueKind.True && jsonElement.ValueKind is not JsonValueKind.False)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }
        }
    }
}
