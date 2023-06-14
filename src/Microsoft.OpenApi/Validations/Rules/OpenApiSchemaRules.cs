﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Default.Node, schema);
                    }

                    context.Exit();

                    // example
                    context.Enter("example");

                    if (schema.Example != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Example.Node, schema);
                    }

                    context.Exit();

                    // enum
                    context.Enter("enum");

                    if (schema.Enum != null)
                    {
                        for (int i = 0; i < schema.Enum.Count; i++)
                        {
                            context.Enter(i.ToString());
                            RuleHelpers.ValidateDataTypeMismatch(context, nameof(SchemaMismatchedDataType), schema.Enum[i].Node, schema);
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
                        var discriminatorName = schema.Discriminator?.PropertyName;

                        if (!ValidateChildSchemaAgainstDiscriminator(schema, discriminatorName))
                        {
                            context.CreateError(nameof(ValidateSchemaDiscriminator),
                            string.Format(SRResource.Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator,
                                schema.Reference.Id, discriminatorName));
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
        public static bool ValidateChildSchemaAgainstDiscriminator(OpenApiSchema schema, string discriminatorName)
        {
            if (!schema.Required?.Contains(discriminatorName) ?? false)
            {
                // recursively check nested schema.OneOf, schema.AnyOf or schema.AllOf and their required fields for the discriminator
                if (schema.OneOf.Count != 0)
                {
                    return TraverseSchemaElements(discriminatorName, schema.OneOf);
                }
                if (schema.AnyOf.Count != 0)
                {
                    return TraverseSchemaElements(discriminatorName, schema.AnyOf);
                }
                if (schema.AllOf.Count != 0)
                {
                    return TraverseSchemaElements(discriminatorName, schema.AllOf);
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
        public static bool TraverseSchemaElements(string discriminatorName, IList<OpenApiSchema> childSchema)
        {
            foreach (var childItem in childSchema)
            {
                if ((!childItem.Properties?.ContainsKey(discriminatorName) ?? false) &&
                                    (!childItem.Required?.Contains(discriminatorName) ?? false))
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
