// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

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
            if (discriminatorName is null)
            {
                return false;
            }

            if (schema.Required?.Contains(discriminatorName) == true)
            {
                return true;
            }

            return TraverseSchemaElementsIterative(discriminatorName, GetSchemaCombinators(schema));
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
            => TraverseSchemaElementsIterative(discriminatorName, childSchema);

        private static bool TraverseSchemaElementsIterative(string discriminatorName, IEnumerable<IOpenApiSchema>? childSchemas)
        {
            if (childSchemas is null)
            {
                return false;
            }

            var schemasToVisit = new Queue<IOpenApiSchema>();
            var visitedSchemas = new HashSet<IOpenApiSchema>(SchemaReferenceEqualityComparer.Instance);

            EnqueueSchemas(schemasToVisit, childSchemas);

            while (schemasToVisit.Count > 0)
            {
                var childItem = schemasToVisit.Dequeue();
                if (!visitedSchemas.Add(childItem))
                {
                    continue;
                }

                if (childItem.Properties?.ContainsKey(discriminatorName) == true ||
                    childItem.Required?.Contains(discriminatorName) == true)
                {
                    return true;
                }

                EnqueueSchemas(schemasToVisit, GetSchemaCombinators(childItem));
            }

            return false;
        }

        private static IEnumerable<IOpenApiSchema> GetSchemaCombinators(IOpenApiSchema schema)
        {
            if (schema.OneOf is { Count: > 0 } oneOf)
            {
                foreach (var childSchema in oneOf)
                {
                    yield return childSchema;
                }
            }

            if (schema.AnyOf is { Count: > 0 } anyOf)
            {
                foreach (var childSchema in anyOf)
                {
                    yield return childSchema;
                }
            }

            if (schema.AllOf is { Count: > 0 } allOf)
            {
                foreach (var childSchema in allOf)
                {
                    yield return childSchema;
                }
            }
        }

        private static void EnqueueSchemas(Queue<IOpenApiSchema> schemasToVisit, IEnumerable<IOpenApiSchema> childSchemas)
        {
            foreach (var childSchema in childSchemas.Where(childSchema => childSchema is not null))
            {
                schemasToVisit.Enqueue(childSchema);
            }
        }

        private sealed class SchemaReferenceEqualityComparer : IEqualityComparer<IOpenApiSchema>
        {
            internal static SchemaReferenceEqualityComparer Instance { get; } = new();

            public bool Equals(IOpenApiSchema? x, IOpenApiSchema? y)
                => ReferenceEquals(x, y);

            public int GetHashCode(IOpenApiSchema obj)
                => RuntimeHelpers.GetHashCode(obj);
        }
    }
}
