// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiMediaType"/>.
    /// </summary>
    /// <remarks>
    /// Removed this in v1.3 as a default rule as the OpenAPI specification does not require that example
    /// values validate against the schema.  Validating examples against the schema is particularly difficult
    /// as it requires parsing of the example using the schema as a guide.  This is not possible when the schema
    /// is ref'd.  Even if we fix this issue, this rule should be treated as a warning, not an error
    /// Future versions of the validator should make that distinction.
    /// Future versions of the example parsers should not try an infer types.
    /// Example validation should be done as a separate post reading step so all schemas can be fully available.
    /// </remarks>
    //[OpenApiRule]
    public static class OpenApiMediaTypeRules
    {
        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<OpenApiMediaType> MediaTypeMismatchedDataType =>
            new ValidationRule<OpenApiMediaType>(
                (context, mediaType) =>
                {
                    // example
                    context.Enter("example");

                    if (mediaType.Example != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(MediaTypeMismatchedDataType), mediaType.Example, mediaType.Schema);
                    }

                    context.Exit();


                    // enum
                    context.Enter("examples");

                    if (mediaType.Examples != null)
                    {
                        foreach (var key in mediaType.Examples.Keys)
                        {
                            if (mediaType.Examples[key] != null)
                            {
                                context.Enter(key);
                                context.Enter("value");
                                RuleHelpers.ValidateDataTypeMismatch(context, nameof(MediaTypeMismatchedDataType), mediaType.Examples[key]?.Value, mediaType.Schema);
                                context.Exit();
                                context.Exit();
                            }
                        }
                    }

                    context.Exit();
                });

        // add more rule.
    }
}
