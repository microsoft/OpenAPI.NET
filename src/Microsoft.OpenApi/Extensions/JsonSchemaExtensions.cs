using System.Collections.Generic;
using Json.Schema;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Specifies Extension methods to be applied on a JSON schema instance
    /// </summary>
    public static class JsonSchemaExtensions
    {
        /// <summary>
        /// Gets the `discriminator` keyword if it exists.
        /// </summary>
        public static DiscriminatorKeyword GetOpenApiDiscriminator(this JsonSchema schema)
        {
            return schema.TryGetKeyword<DiscriminatorKeyword>(DiscriminatorKeyword.Name, out var k) ? k! : null;
        }

        /// <summary>
        /// Gets the 'externalDocs' keyword if it exists.
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static OpenApiExternalDocs GetOpenApiExternalDocs(this JsonSchema schema)
        {
            return schema.TryGetKeyword<ExternalDocsKeyword>(ExternalDocsKeyword.Name, out var k) ? k.Value! : null;
        }

        /// <summary>
        /// Gets the `summary` keyword if it exists.
        /// </summary>
        public static string GetSummary(this JsonSchema schema)
        {
            return schema.TryGetKeyword<SummaryKeyword>(SummaryKeyword.Name, out var k) ? k.Summary! : null;
        }

        /// <summary>
        /// Gets the nullable value if it exists
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static bool? GetNullable(this JsonSchema schema)
        {
            return schema.TryGetKeyword<NullableKeyword>(NullableKeyword.Name, out var k) ? k.Value! : null;
        }

        /// <summary>
        /// Gets the minimum value if it exists
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static double? GetOpenApiMinimum(this JsonSchema schema)
        {
            return schema.TryGetKeyword<MinimumKeyword>(MinimumKeyword.Name, out var k) ? k.Value! : null;
        }

        /// <summary>
        /// Gets the maximum value if it exists
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static double? GetOpenApiMaximum(this JsonSchema schema)
        {
            return schema.TryGetKeyword<MaximumKeyword>(MaximumKeyword.Name, out var k) ? k.Value! : null;
        }

        /// <summary>
        /// Gets the multipleOf value if it exists
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static double? GetOpenApiMultipleOf(this JsonSchema schema)
        {
            return schema.TryGetKeyword<MultipleOfKeyword>(MultipleOfKeyword.Name, out var k) ? k.Value! : null;
        }

        /// <summary>
        /// Gets the additional properties value if it exists
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static bool? GetAdditionalPropertiesAllowed(this JsonSchema schema)
        {
            return schema.TryGetKeyword<AdditionalPropertiesAllowedKeyword>(AdditionalPropertiesAllowedKeyword.Name, out var k) ? k.AdditionalPropertiesAllowed! : null;
        }

        /// <summary>
        /// Gets the exclusive maximum value if it exists
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static bool? GetOpenApiExclusiveMaximum(this JsonSchema schema)
        {
            return schema.TryGetKeyword<Draft4ExclusiveMaximumKeyword>(Draft4ExclusiveMaximumKeyword.Name, out var k) ? k.MaxValue! : null;
        }

        /// <summary>
        /// Gets the exclusive minimum value if it exists
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static bool? GetOpenApiExclusiveMinimum(this JsonSchema schema)
        {
            return schema.TryGetKeyword<Draft4ExclusiveMinimumKeyword>(Draft4ExclusiveMinimumKeyword.Name, out var k) ? k.MinValue! : null;
        }

        /// <summary>
        /// Gets the custom extensions if it exists
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public static IDictionary<string, IOpenApiExtension> GetExtensions(this JsonSchema schema)
        {
            return schema.TryGetKeyword<ExtensionsKeyword>(ExtensionsKeyword.Name, out var k) ? k.Extensions! : null;
        }
    }
}
