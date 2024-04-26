// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Text.Json.Nodes;
using Json.Schema;
using Microsoft.OpenApi.Services;

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
            if (schema is not null)
            {
                if (context.HostDocument != null)
                {
                    var visitor = new JsonSchemaReferenceResolver(context.HostDocument);
                    var walker = new OpenApiWalker(visitor);
                    schema = walker.Walk(schema);                    

                    var options = new EvaluationOptions();
                    options.OutputFormat = OutputFormat.List;

                    var results = schema.Evaluate(value, options);

                    if (!results.IsValid)
                    {
                        foreach (var detail in results.Details)
                        {
                            if (detail.Errors != null && detail.Errors.Any())
                            {
                                foreach (var error in detail.Errors)
                                {
                                    if (!string.IsNullOrEmpty(error.Key) || !string.IsNullOrEmpty(error.Value.Trim()))
                                    {
                                        context.CreateWarning(ruleName, string.Format("{0} : {1} at {2}", error.Key, error.Value.Trim(), detail.InstanceLocation));
                                    }
                                }
                            }
                        }
                    }
                }
            }            
        }
    }
}
