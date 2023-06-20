// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="JsonSchema"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiSchemaRules
    {
        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<JsonSchemaWrapper> SchemaMismatchedDataType =>
            new ValidationRule<JsonSchemaWrapper>(
                (context, schemaWrapper) =>
                {
                    // default
                    context.Enter("default");

                    if (schemaWrapper.JsonSchema.GetDefault() != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schemaWrapper.JsonSchema.GetDefault(), schemaWrapper.JsonSchema);
                    }

                    context.Exit();

                    // example
                    context.Enter("example");

                    if (schemaWrapper.JsonSchema.GetExample() != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schemaWrapper.JsonSchema.GetExample(), schemaWrapper.JsonSchema);
                    }

                    context.Exit();

                    // enum
                    context.Enter("enum");

                    if (schemaWrapper.JsonSchema.GetEnum() != null)
                    {
                        for (int i = 0; i < schemaWrapper.JsonSchema.GetEnum().Count; i++)
                        {
                            context.Enter(i.ToString());
                            RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schemaWrapper.JsonSchema.GetEnum().ElementAt(i), schemaWrapper.JsonSchema);
                            context.Exit();
                        }
                    }

                    context.Exit();
                });

        /// <summary>
        /// Validates Schema Discriminator
        /// </summary>
        public static ValidationRule<JsonSchemaWrapper> ValidateSchemaDiscriminator =>
            new ValidationRule<JsonSchemaWrapper>(
                (context, schemaWrapper) =>
                {
                    // discriminator
                    context.Enter("discriminator");

                    if (schemaWrapper.JsonSchema.GetRef() != null && schemaWrapper.JsonSchema.GetDiscriminator() != null)
                    {
                        var discriminatorName = schemaWrapper.JsonSchema.GetDiscriminator()?.PropertyName;

                        if (!ValidateChildSchemaAgainstDiscriminator(schemaWrapper.JsonSchema, discriminatorName))
                        {
                            context.CreateError(nameof(ValidateSchemaDiscriminator),
                            string.Format(SRResource.Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator,
                                schemaWrapper.JsonSchema.GetRef(), discriminatorName));
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
            if (!schema.GetRequired()?.Contains(discriminatorName) ?? false)
            {
                // recursively check nested schema.OneOf, schema.AnyOf or schema.AllOf and their required fields for the discriminator
                if (schema.GetOneOf().Count != 0)
                {
                    return TraverseSchemaElements(discriminatorName, schema.GetOneOf());
                }
                if (schema.GetOneOf().Count != 0)
                {
                    return TraverseSchemaElements(discriminatorName, schema.GetAnyOf());
                }
                if (schema.GetAllOf().Count != 0)
                {
                    return TraverseSchemaElements(discriminatorName, schema.GetAllOf());
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
            foreach (var childItem in childSchema)
            {
                if ((!childItem.GetProperties()?.ContainsKey(discriminatorName) ?? false) &&
                                    (!childItem.GetRequired()?.Contains(discriminatorName) ?? false))
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
