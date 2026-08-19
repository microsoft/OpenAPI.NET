// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
            new(nameof(SchemaMismatchedDataType),
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
                        for (var i = 0; i < schema.Enum.Count; i++)
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
            new(nameof(ValidateSchemaDiscriminator),
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
            if (discriminatorName == null)
            {
                return false;
            }

            if (schema.Required?.Contains(discriminatorName) == true)
            {
                return true;
            }

            // Iteratively check nested schema.OneOf, schema.AnyOf or schema.AllOf and their required fields
            // for the discriminator. A reference-identity visited set guards against circular $ref graphs
            // that would otherwise cause unbounded recursion and a StackOverflowException.
            return TraverseSchemaElementsIterative(discriminatorName, GetSchemaCombinators(schema));
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
            return TraverseSchemaElementsIterative(discriminatorName, childSchema);
        }

        private static bool TraverseSchemaElementsIterative(string discriminatorName, IEnumerable<OpenApiSchema> childSchemas)
        {
            if (discriminatorName == null || childSchemas == null)
            {
                return false;
            }

            var schemasToVisit = new Queue<OpenApiSchema>();
            var visitedSchemas = new HashSet<OpenApiSchema>(SchemaReferenceEqualityComparer.Instance);

            EnqueueSchemas(schemasToVisit, childSchemas);

            while (schemasToVisit.Count > 0)
            {
                var childItem = schemasToVisit.Dequeue();

                // Reference identity ensures self-cycles and multi-node cycles terminate without
                // conflating distinct schemas that happen to be structurally equal.
                if (!visitedSchemas.Add(childItem))
                {
                    continue;
                }

                if ((childItem.Properties?.ContainsKey(discriminatorName) == true) ||
                    (childItem.Required?.Contains(discriminatorName) == true))
                {
                    return true;
                }

                EnqueueSchemas(schemasToVisit, GetSchemaCombinators(childItem));
            }

            return false;
        }

        private static IEnumerable<OpenApiSchema> GetSchemaCombinators(OpenApiSchema schema)
        {
            if (schema.OneOf != null)
            {
                foreach (var childSchema in schema.OneOf)
                {
                    yield return childSchema;
                }
            }

            if (schema.AnyOf != null)
            {
                foreach (var childSchema in schema.AnyOf)
                {
                    yield return childSchema;
                }
            }

            if (schema.AllOf != null)
            {
                foreach (var childSchema in schema.AllOf)
                {
                    yield return childSchema;
                }
            }
        }

        private static void EnqueueSchemas(Queue<OpenApiSchema> schemasToVisit, IEnumerable<OpenApiSchema> childSchemas)
        {
            foreach (var childSchema in childSchemas.Where(s => s is not null))
            {
                schemasToVisit.Enqueue(childSchema);
            }
        }

        private sealed class SchemaReferenceEqualityComparer : IEqualityComparer<OpenApiSchema>
        {
            internal static SchemaReferenceEqualityComparer Instance { get; } = new();

            public bool Equals(OpenApiSchema x, OpenApiSchema y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(OpenApiSchema obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}
