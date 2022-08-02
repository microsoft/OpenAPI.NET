// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiSchema"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiSchemaRules
    {
        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<OpenApiSchema> SchemaMismatchedDataType =>
            new ValidationRule<OpenApiSchema>(
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
                        for (int i = 0; i < schema.Enum.Count; i++)
                        {
                            context.Enter(i.ToString());
                            RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Enum[i], schema);
                            context.Exit();
                        }
                    }

                    context.Exit();
                });

        /// <summary>
        /// Validates Schema Discriminator
        /// </summary>
        public static ValidationRule<OpenApiSchema> ValidateSchemaDiscriminator =>
            new ValidationRule<OpenApiSchema>(
                (context, schema) =>
                {
                    // discriminator
                    context.Enter("discriminator");

                    if (schema.Reference != null && schema.Discriminator != null)
                    {
                        var discriminator = schema.Discriminator?.PropertyName;
                        var schemaReferenceId = schema.Reference.Id;

                        if (!ValidateChildSchemaAgainstDiscriminator(schema, discriminator, schemaReferenceId, context))
                        {
                            context.CreateError(nameof(ValidateSchemaDiscriminator),
                            string.Format(SRResource.Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator,
                                schemaReferenceId, discriminator));
                        }
                    }

                    context.Exit();
                });

        /// <summary>
        /// Validates the property name in the discriminator against the ones present in the children schema
        /// </summary>
        /// <param name="schema">The parent schema.</param>
        /// <param name="discriminator">Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.</param>
        /// <param name="schemaReferenceId"></param>
        /// <param name="context">A validation context.</param>
        public static bool ValidateChildSchemaAgainstDiscriminator(OpenApiSchema schema, string discriminator, string schemaReferenceId, IValidationContext context)
        {
            bool containsDiscriminator = false;

            if (!schema.Required.Contains(discriminator))
            {
                // recursively check nested schema.OneOf, schema.AnyOf or schema.AllOf and their required fields for the discriminator
                if (schema.OneOf.Count != 0)
                {
                    return TraverseSchemaElements(discriminator, schema.OneOf, schemaReferenceId, context, containsDiscriminator);
                }
                if (schema.AnyOf.Count != 0)
                {
                    return TraverseSchemaElements(discriminator, schema.AnyOf, schemaReferenceId, context, containsDiscriminator);
                }
                if (schema.AllOf.Count != 0)
                {
                    return TraverseSchemaElements(discriminator, schema.AllOf, schemaReferenceId, context, containsDiscriminator);
                }
            }

            return containsDiscriminator;
        }

        /// <summary>
        /// Traverses the schema elements and checks whether the schema contains the discriminator.
        /// </summary>
        /// <param name="discriminator">Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.</param>
        /// <param name="childSchema">The child schema.</param>
        /// <param name="schemaReferenceId"> The schema reference Id.</param>
        /// <param name="context"> A validation context.</param>
        /// <param name="containsDiscriminator">Tracks whether the discriminator is present.</param>
        /// <returns></returns>
        public static bool TraverseSchemaElements(string discriminator, IList<OpenApiSchema> childSchema, string schemaReferenceId, IValidationContext context, bool containsDiscriminator)
        {
            foreach (var childItem in childSchema)
            {
                if (!childItem.Properties.ContainsKey(discriminator) && !childItem.Required.Contains(discriminator))
                {
                    return ValidateChildSchemaAgainstDiscriminator(childItem, discriminator, schemaReferenceId, context);
                }
                else
                {
                    return containsDiscriminator = true;
                }
            }

            return containsDiscriminator;
        }
    }
}
