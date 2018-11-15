// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Any;
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
        /// Validates OneOf Discriminator
        /// </summary>
        public static ValidationRule<OpenApiSchema> ValidateOneOfDiscriminator =>
            new ValidationRule<OpenApiSchema>(
                (context, schema) =>
                {
                    // oneOf
                    context.Enter("oneOf");

                    if (schema.OneOf != null && schema.Discriminator != null)
                    {
                        ValidateSchemaListDiscriminator(context, nameof(ValidateOneOfDiscriminator),
                            schema.OneOf, schema.Discriminator);
                    }

                    context.Exit();
                });

        // <summary>
        /// Validates AnyOf Discriminator
        /// </summary>
        public static ValidationRule<OpenApiSchema> ValidateAnyOfDiscriminator =>
            new ValidationRule<OpenApiSchema>(
                (context, schema) =>
                {
                    // oneOf
                    context.Enter("anyOf");

                    if (schema.AnyOf != null && schema.Discriminator != null)
                    {
                        ValidateSchemaListDiscriminator(context, nameof(ValidateAnyOfDiscriminator),
                            schema.AnyOf, schema.Discriminator);
                    }

                    context.Exit();
                });

        // add more rule.

        
        /// <summary>
        /// Checks if the schemas in the list contain a property with the property name specified by the discriminator.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ruleName"></param>
        /// <param name="schemas">OneOf/AnyOf schemas</param>
        /// <param name="discriminator">discriminator</param>
        private static void ValidateSchemaListDiscriminator(IValidationContext context, string ruleName,
                        IList<OpenApiSchema> schemas, OpenApiDiscriminator discriminator)
        {
            foreach (var schema in schemas)
            {
                if (!schema.Properties.ContainsKey(discriminator.PropertyName))
                {
                    context.AddError(new OpenApiValidatorError(ruleName, context.PathString,
                            string.Format(SRResource.Validation_SchemaDoesntContainDiscriminatorProperty,
                                        schema.Reference.Id, discriminator.PropertyName)));
                }
                if (!schema.Required.Contains(discriminator.PropertyName))
                {
                    context.AddError(new OpenApiValidatorError(ruleName, context.PathString,
                        string.Format(SRResource.Validation_SchemaDoesntContainDiscriminatorPropertyInRequiredFieldList,
                                        schema.Reference.Id, discriminator.PropertyName)));
                }
            }
        }
    }
}
