// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// Defines a non-default set of rules for validating examples in header, media type and parameter objects against the schema
    /// </summary>
    public static class OpenApiNonDefaultRules
    {
        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<OpenApiHeader> HeaderMismatchedDataType =>
            new(nameof(HeaderMismatchedDataType),
                (context, header) =>
                {
                    ValidateMismatchedDataType(context, nameof(HeaderMismatchedDataType), header.Example, header.Examples, header.Schema);
                });

        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<OpenApiMediaType> MediaTypeMismatchedDataType =>
            new(nameof(MediaTypeMismatchedDataType),
                (context, mediaType) =>
                {
                    ValidateMismatchedDataType(context, nameof(MediaTypeMismatchedDataType), mediaType.Example, mediaType.Examples, mediaType.Schema);
                });

        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<OpenApiParameter> ParameterMismatchedDataType =>
        new(nameof(ParameterMismatchedDataType),
                (context, parameter) =>
                {
                    ValidateMismatchedDataType(context, nameof(ParameterMismatchedDataType), parameter.Example, parameter.Examples, parameter.Schema);
                });

        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<OpenApiSchema> SchemaMismatchedDataType =>
            new(nameof(SchemaMismatchedDataType),
                (context, schema) =>
                {
                    // default
                    context.Enter("default");

                    if (schema.Default != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Default, schema);
                    }

                    context.Exit();

                    // example
                    context.Enter("example");

                    if (schema.Example != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Example, schema);
                    }

                    context.Exit();

                    // enum
                    context.Enter("enum");

                    if (schema.Enum != null)
                    {
                        for (var i = 0; i < schema.Enum.Count; i++)
                        {
                            context.Enter(i.ToString());
                            RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Enum[i], schema);
                            context.Exit();
                        }
                    }

                    context.Exit();
                });

        private static void ValidateMismatchedDataType(IValidationContext context,
                                                      string ruleName,
                                                      JsonNode example,
                                                      IDictionary<string, OpenApiExample> examples,
                                                      OpenApiSchema schema)
        {
            // example
            context.Enter("example");

            if (example != null)
            {
                RuleHelpers.ValidateDataTypeMismatch(context, ruleName, example, schema);
            }

            context.Exit();

            // enum
            context.Enter("examples");

            if (examples != null)
            {
                foreach (var key in examples.Keys.Where(k => examples[k] != null))
                {
                    context.Enter(key);
                    context.Enter("value");
                    RuleHelpers.ValidateDataTypeMismatch(context, ruleName, examples[key]?.Value, schema);
                    context.Exit();
                    context.Exit();
                }
            }

            context.Exit();
        }
    }
}
