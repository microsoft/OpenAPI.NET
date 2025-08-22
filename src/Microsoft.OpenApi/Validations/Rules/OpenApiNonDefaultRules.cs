// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Defines a non-default set of rules for validating examples in header, media type and parameter objects against the schema
    /// </summary>
    public static class OpenApiNonDefaultRules
    {
        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<IOpenApiHeader> HeaderMismatchedDataType =>
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
        public static ValidationRule<IOpenApiSchema> SchemaMismatchedDataType =>
            new(nameof(SchemaMismatchedDataType),
                (context, schema) =>
                {
                    // default
                    if (schema.Default != null)
                    {
                        context.Enter("default");
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Default, schema);
                        context.Exit();
                    }

                    // example
                    if (schema.Example != null)
                    {
                        context.Enter("example");
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Example, schema);
                        context.Exit();
                    }

                    // enum
                    if (schema.Enum != null)
                    {
                        context.Enter("enum");
                        for (var i = 0; i < schema.Enum.Count; i++)
                        {
                            context.Enter(i.ToString());
                            RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Enum[i], schema);
                            context.Exit();
                        }
                        context.Exit();
                    }
                });

        private static void ValidateMismatchedDataType(IValidationContext context,
                                                      string ruleName,
                                                      JsonNode? example,
                                                      IDictionary<string, IOpenApiExample>? examples,
                                                      IOpenApiSchema? schema)
        {
            // example
            if (example != null)
            {
                context.Enter("example");
                RuleHelpers.ValidateDataTypeMismatch(context, ruleName, example, schema);
                context.Exit();
            }

            // enum
            if (examples != null)
            {
                context.Enter("examples");
                foreach (var key in examples.Keys.Where(k => examples[k] != null))
                {
                    context.Enter(key);
                    context.Enter("value");
                    RuleHelpers.ValidateDataTypeMismatch(context, ruleName, examples[key]?.Value, schema);
                    context.Exit();
                    context.Exit();
                }
                context.Exit();
            }
        }
    }
}
