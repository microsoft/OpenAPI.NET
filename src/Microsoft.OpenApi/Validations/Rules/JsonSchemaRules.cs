// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="JsonSchema"/>.
    /// </summary>
    [OpenApiRule]
    public static class JsonSchemaRules
    {
        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<JsonSchema> SchemaMismatchedDataType =>
            new ValidationRule<JsonSchema>(
                (context, jsonSchema) =>
                {
                    // default
                    context.Enter("default");

                    if (jsonSchema.GetDefault() != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), jsonSchema.GetDefault(), jsonSchema);
                    }

                    context.Exit();

                    // examples
                    context.Enter("examples");

                    if (jsonSchema.GetExamples() is { } examples)
                    {
                        for (int i = 0; i < jsonSchema.GetExamples().Count(); i++)
                        {
                            context.Enter(i.ToString());
                            RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), jsonSchema.GetExamples().ElementAt(i), jsonSchema);
                            context.Exit();
                        }
                        
                    }

                    context.Exit();
                    
                    // example
                    context.Enter("example");

                    if (jsonSchema.GetExample() != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), jsonSchema.GetExample(), jsonSchema);
                    }

                    context.Exit();

                    // enum
                    context.Enter("enum");

                    if (jsonSchema.GetEnum() != null)
                    {
                        for (int i = 0; i < jsonSchema.GetEnum().Count; i++)
                        {
                            context.Enter(i.ToString());
                            RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), jsonSchema.GetEnum().ElementAt(i), jsonSchema);
                            context.Exit();
                        }
                    }

                    context.Exit();
                });

        /// <summary>
        /// Validates Schema Discriminator
        /// </summary>
        public static ValidationRule<JsonSchema> ValidateSchemaDiscriminator =>
            new ValidationRule<JsonSchema>(
                (context, jsonSchema) =>
                {
                    // discriminator
                    context.Enter("discriminator");

                    if (jsonSchema.GetRef() != null && jsonSchema.GetOpenApiDiscriminator() != null)
                    {
                        var discriminatorName = jsonSchema.GetOpenApiDiscriminator()?.PropertyName;

                        if (!ValidateChildSchemaAgainstDiscriminator(jsonSchema, discriminatorName))
                        {
                            context.CreateError(nameof(ValidateSchemaDiscriminator),
                            string.Format(SRResource.Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator,
                                jsonSchema.GetRef(), discriminatorName));
                        }
                    }

                    context.Exit();
                });

        /// <summary>
        /// Validates the property name in the discriminator against the ones present in the children schema
        /// </summary>
        /// <param name="schema">The parent schema.</param>
        /// <param name="discriminatorName">Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.</param>
        public static bool ValidateChildSchemaAgainstDiscriminator(JsonSchema schema, string discriminatorName)
        {
            if (!schema.GetRequired()?.Contains(discriminatorName) ?? true)
            {
                // recursively check nested schema.OneOf, schema.AnyOf or schema.AllOf and their required fields for the discriminator
                if (schema.GetOneOf()?.Count != 0 && TraverseSchemaElements(discriminatorName, schema.GetOneOf()))
                {
                    return true;
                }
                if (schema.GetAnyOf()?.Count != 0 && TraverseSchemaElements(discriminatorName, schema.GetAnyOf()))
                {
                    return true;
                }
                if (schema.GetAllOf()?.Count != 0 && TraverseSchemaElements(discriminatorName, schema.GetAllOf()))
                {
                    return true; 
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Traverses the schema elements and checks whether the schema contains the discriminator.
        /// </summary>
        /// <param name="discriminatorName">Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.</param>
        /// <param name="childSchema">The child schema.</param>
        /// <returns></returns>
        public static bool TraverseSchemaElements(string discriminatorName, IReadOnlyCollection<JsonSchema> childSchema)
        {
            if (!childSchema?.Any() ?? true)
                return false;

            foreach (var childItem in childSchema)
            {
                if ((!childItem.GetProperties()?.ContainsKey(discriminatorName) ?? true) &&
                                    (!childItem.GetRequired()?.Contains(discriminatorName) ?? true))
                {
                    return ValidateChildSchemaAgainstDiscriminator(childItem, discriminatorName);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}
