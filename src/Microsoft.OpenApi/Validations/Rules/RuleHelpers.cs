// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using Microsoft.OpenApi.Extensions;

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
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            var splits = input.Split('@');
            if (splits.Length != 2)
            {
                return false;
            }

            if (String.IsNullOrEmpty(splits[0]) || String.IsNullOrEmpty(splits[1]))
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
            JsonSchema schema)
        {
            if (schema == null)
            {
                return;
            }

            // Resolve the Json schema in memory before validating the data types.
            var reference = schema.GetRef();
            if (reference != null)
            {
                var referencePath = string.Concat("https://registry", reference.OriginalString.Split('#').Last());
                var resolvedSchema = (JsonSchema)SchemaRegistry.Global.Get(new Uri(referencePath));
                schema = resolvedSchema ?? schema;
            }

            var type = schema.GetJsonType()?.GetDisplayName();
            var format = schema.GetFormat()?.Key;
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(value);

            // Before checking the type, check first if the schema allows null.
            // If so and the data given is also null, this is allowed for any type.
            if (jsonElement.ValueKind is JsonValueKind.Null)
            {
                return;
            }

            if ("object".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                // It is not against the spec to have a string representing an object value.
                // To represent examples of media types that cannot naturally be represented in JSON or YAML,
                // a string value can contain the example with escaping where necessary
                if (jsonElement.ValueKind is JsonValueKind.String)
                {
                    return;
                }

                // If value is not a string and also not an object, there is a data mismatch.
                if (jsonElement.ValueKind is not JsonValueKind.Object)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                    return;
                }

                if (value is JsonObject anyObject)
                {
                    foreach (var property in anyObject)
                    {
                        context.Enter(property.Key);
                        if ((schema.GetProperties()?.TryGetValue(property.Key, out var propertyValue)) ?? false)
                        {
                            ValidateDataTypeMismatch(context, ruleName, anyObject[property.Key], propertyValue);
                        }
                        else
                        {
                            ValidateDataTypeMismatch(context, ruleName, anyObject[property.Key], schema.GetAdditionalProperties());
                        }

                        context.Exit();
                    }
                }

                return;
            }

            if ("array".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                // It is not against the spec to have a string representing an array value.
                // To represent examples of media types that cannot naturally be represented in JSON or YAML,
                // a string value can contain the example with escaping where necessary
                if (jsonElement.ValueKind is JsonValueKind.String)
                {
                    return;
                }

                // If value is not a string and also not an array, there is a data mismatch.
                if (value is not JsonArray)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                    return;
                }

                var anyArray = value as JsonArray;

                for (int i = 0; i < anyArray.Count; i++)
                {
                    context.Enter(i.ToString());

                    ValidateDataTypeMismatch(context, ruleName, anyArray[i], schema.GetItems());

                    context.Exit();
                }

                return;
            }

            if ("integer".Equals(type, StringComparison.OrdinalIgnoreCase) &&
                "int32".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("integer".Equals(type, StringComparison.OrdinalIgnoreCase) &&
                "int64".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                       ruleName,
                       DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("integer".Equals(type, StringComparison.OrdinalIgnoreCase) &&
                jsonElement.ValueKind is not JsonValueKind.Number)
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("number".Equals(type, StringComparison.OrdinalIgnoreCase) &&
                "float".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("number".Equals(type, StringComparison.OrdinalIgnoreCase) &&
                "double".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("number".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.Number)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("string".Equals(type, StringComparison.OrdinalIgnoreCase) &&
                "byte".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("string".Equals(type, StringComparison.OrdinalIgnoreCase) &&
                "date".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("string".Equals(type, StringComparison.OrdinalIgnoreCase) &&
                "date-time".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("string".Equals(type, StringComparison.OrdinalIgnoreCase) &&
                "password".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("string".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.String)
                {
                    context.CreateWarning(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if ("boolean".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                if (jsonElement.ValueKind is not JsonValueKind.True and not JsonValueKind.False)
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
