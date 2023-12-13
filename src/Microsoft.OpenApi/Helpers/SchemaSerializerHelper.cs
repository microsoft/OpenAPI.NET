// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Json.Schema;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Helpers
{
    internal static class SchemaSerializerHelper
    {
        internal static void WriteAsItemsProperties(JsonSchema schema,
                                                    IOpenApiWriter writer,
                                                    IDictionary<string, IOpenApiExtension> extensions,
                                                    OpenApiSpecVersion version)
        {
            // type
            if (schema.GetJsonType() != null)
            {
                writer.WritePropertyName(OpenApiConstants.Type);
                var type = schema.GetJsonType().Value;
                writer.WriteValue(OpenApiTypeMapper.ConvertSchemaValueTypeToString(type));
            }

            // format
            var format = schema.GetFormat()?.Key;
            if (string.IsNullOrEmpty(format))
            {
                format = RetrieveFormatFromNestedSchema(schema.GetAllOf()) ?? RetrieveFormatFromNestedSchema(schema.GetOneOf())
                    ?? RetrieveFormatFromNestedSchema(schema.GetAnyOf());
            }
            writer.WriteProperty(OpenApiConstants.Format, format);

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, schema.GetItems(),
                (w, s) => w.WriteJsonSchema(s, version));

            // collectionFormat
            // We need information from style in parameter to populate this.
            // The best effort we can make is to pull this information from the first parameter
            // that leverages this schema. However, that in itself may not be as simple
            // as the schema directly under parameter might be referencing one in the Components,
            // so we will need to do a full scan of the object before we can write the value for
            // this property. This is not supported yet, so we will skip this property at the moment.

            // default
            if (schema.GetDefault() != null)
            {
                writer.WritePropertyName(OpenApiConstants.Default);
                writer.WriteValue(schema.GetDefault());
            }

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, schema.GetMaximum());

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, schema.GetExclusiveMaximum());

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, schema.GetMinimum());

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, schema.GetExclusiveMinimum());

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, schema.GetMaxLength());

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, schema.GetMinLength());

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, schema.GetPattern()?.ToString());

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, schema.GetMaxItems());

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, schema.GetMinItems());

            // enum
            if (schema.GetEnum() != null)
            {
                writer.WritePropertyName(OpenApiConstants.Enum);
                writer.WriteValue(schema.GetEnum());
            }

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, schema.GetMultipleOf());

            // extensions
            writer.WriteExtensions(extensions, OpenApiSpecVersion.OpenApi2_0);
        }

        private static string RetrieveFormatFromNestedSchema(IReadOnlyCollection<JsonSchema> schema)
        {
            if (schema != null)
            {
                return schema
                    .Where(item => !string.IsNullOrEmpty(item.GetFormat()?.Key))
                    .Select(item => item.GetFormat().Key)
                    .FirstOrDefault();
            }

            return null;
        }
    }
}
