using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Extensions
{
    public static class OpenApiSchemaExtensions
    {
        public static SchemaTypeEnum GetSchemaType(this OpenApiSchema schema)
        {
            if (schema == null)
                return SchemaTypeEnum.Schema;

            if (schema.Items != null)
                return SchemaTypeEnum.ArraySchema;

            if (!schema.AnyOf.IsNullOrEmpty() || !schema.OneOf.IsNullOrEmpty() || !schema.AllOf.IsNullOrEmpty())
                return SchemaTypeEnum.ComposedSchema;

            return SchemaTypeEnum.Schema;
        }
    }
}
