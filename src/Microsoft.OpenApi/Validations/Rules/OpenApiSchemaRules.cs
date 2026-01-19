// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// The validation rules for <see cref="OpenApiSchema"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiSchemaRules
    {
        /// <summary>
        /// Validates Schema Property has value
        /// </summary>
        public static ValidationRule<IOpenApiSchema> ValidateSchemaPropertyHasValue =>
            new(nameof(ValidateSchemaPropertyHasValue),
                (context, schema) =>
                {
                    if (schema.Properties is not null)
                    {
                        foreach (var property in schema.Properties
                                     .Where(entry => entry.Value is null))
                        {
                            context.Enter(property.Key);
                            context.CreateError(nameof(ValidateSchemaPropertyHasValue),
                                string.Format(SRResource.Validation_SchemaPropertyObjectRequired,
                                    schema is OpenApiSchemaReference { Reference: not null } schemaReference
                                        ? schemaReference.Reference.Id
                                        : string.Empty, property.Key));
                            context.Exit();
                        }
                    }
                });
        
        /// <summary>
        /// Validates Schema Discriminator
        /// </summary>
        public static ValidationRule<IOpenApiSchema> ValidateSchemaDiscriminator =>
            new(nameof(ValidateSchemaDiscriminator),
                (context, schema) =>
                {
                    // discriminator
                    if (schema is not null && schema.Discriminator != null)
                    {
                        var discriminatorName = schema.Discriminator?.PropertyName;

#pragma warning disable CS0618 // Type or member is obsolete
                        if (!ValidateChildSchemaAgainstDiscriminator(schema, discriminatorName))
                        {
                            context.Enter("discriminator");
                            context.CreateError(nameof(ValidateSchemaDiscriminator),
                            string.Format(SRResource.Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator,
                                schema is OpenApiSchemaReference { Reference: not null} schemaReference ? schemaReference.Reference.Id : string.Empty, discriminatorName));
                            context.Exit();
                        }
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                });

        /// <summary>
        /// Validates the property name in the discriminator against the ones present in the children schema
        /// </summary>
        /// <param name="schema">The parent schema.</param>
        /// <param name="discriminatorName">Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.</param>
        [Obsolete("This method will be made private in future versions.")]
        [Browsable(false)]
        public static bool ValidateChildSchemaAgainstDiscriminator(IOpenApiSchema schema, string? discriminatorName)
        {
            if (discriminatorName is not null)
            {
                if (schema.Required is null || !schema.Required.Contains(discriminatorName))
                {
                    // recursively check nested schema.OneOf, schema.AnyOf or schema.AllOf and their required fields for the discriminator
                    if (schema.OneOf is { Count: > 0})
                    {
                        return TraverseSchemaElements(discriminatorName, schema.OneOf);
                    }
                    if (schema.AnyOf is { Count: > 0})
                    {
                        return TraverseSchemaElements(discriminatorName, schema.AnyOf);
                    }
                    if (schema.AllOf is { Count: > 0})
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

            return false;
        }

        /// <summary>
        /// Traverses the schema elements and checks whether the schema contains the discriminator.
        /// </summary>
        /// <param name="discriminatorName">Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.</param>
        /// <param name="childSchema">The child schema.</param>
        /// <returns></returns>
        [Obsolete("This method will be made private in future versions.")]
        [Browsable(false)]
        public static bool TraverseSchemaElements(string discriminatorName, IList<IOpenApiSchema>? childSchema)
        {
            if (childSchema is null)
            {
                return false;
            }
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
