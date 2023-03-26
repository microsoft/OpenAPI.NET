using System;
using Json.Schema;

namespace Microsoft.OpenApi.Draft4Support
{
    public static class JsonSchemaKeywordExtensions
    {
        public static JsonSchemaBuilder ExclusiveMaximum(this JsonSchemaBuilder builder, bool value)
        {
            builder.Add(new Draft4ExclusiveMaximumKeyword(value));
            return builder;
        }

        public static JsonSchemaBuilder ExclusiveMinimum(this JsonSchemaBuilder builder, bool value)
        {
            builder.Add(new Draft4ExclusiveMinimumKeyword(value));
            return builder;
        }

        public static JsonSchemaBuilder OasId(this JsonSchemaBuilder builder, Uri id)
        {
            builder.Add(new Draft4IdKeyword(id));
            return builder;
        }

        public static JsonSchemaBuilder OasId(this JsonSchemaBuilder builder, string id)
        {
            builder.Add(new Draft4IdKeyword(new Uri(id, UriKind.RelativeOrAbsolute)));
            return builder;
        }

        public static JsonSchemaBuilder OasType(this JsonSchemaBuilder builder, SchemaValueType type)
        {
            builder.Add(new Draft4TypeKeyword(type));
            return builder;
        }

        public static JsonSchemaBuilder Nullable(this JsonSchemaBuilder builder, bool value)
        {
            builder.Add(new NullableKeyword(value));
            return builder;
        }
    }
}
