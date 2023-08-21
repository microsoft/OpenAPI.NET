using System;
using System.Collections.Generic;
using System.Text;
using Json.Schema;
using Json.Schema.OpenApi;

namespace Microsoft.OpenApi.Extensions
{
    public static class JsonSchemaExtensions
    {
        /// <summary>
        /// Gets the `discriminator` keyword if it exists.
        /// </summary>
        public static DiscriminatorKeyword? GetOpenApiDiscriminator(this JsonSchema schema)
        {
            return schema.TryGetKeyword<DiscriminatorKeyword>(DiscriminatorKeyword.Name, out var k) ? k! : null;
        }
        
    }
}
