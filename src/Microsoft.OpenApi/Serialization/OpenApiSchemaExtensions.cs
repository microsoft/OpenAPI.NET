// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiSchema"/> serialization.
    /// </summary>
    internal static class OpenApiSchemaExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiSchema schema, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (schema == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (schema.IsReference())
            {
                schema.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();

                writer.WriteStringProperty("title", schema.Title);
                writer.WriteStringProperty("type", schema.Type);
                writer.WriteStringProperty("format", schema.Format);
                writer.WriteStringProperty("description", schema.Description);

                writer.WriteNumberProperty("maxLength", schema.MaxLength);
                writer.WriteNumberProperty("minLength", schema.MinLength);
                writer.WriteStringProperty("pattern", schema.Pattern);
                writer.WriteStringProperty("default", schema.Default);

                writer.WriteList("required", schema.Required, (nodeWriter, s) => nodeWriter.WriteValue(s));

                writer.WriteNumberProperty("maximum", schema.Maximum);
                writer.WriteBoolProperty("exclusiveMaximum", schema.ExclusiveMaximum, false);
                writer.WriteNumberProperty("minimum", schema.Minimum);
                writer.WriteBoolProperty("exclusiveMinimum", schema.ExclusiveMinimum, false);

                if (schema.AdditionalProperties != null)
                {
                    writer.WritePropertyName("additionalProperties");
                    schema.AdditionalProperties.SerializeV3(writer);
                }

                if (schema.Items != null)
                {
                    writer.WritePropertyName("items");
                    schema.Items.SerializeV3(writer);
                }
                writer.WriteNumberProperty("maxItems", schema.MaxItems);
                writer.WriteNumberProperty("minItems", schema.MinItems);

                if (schema.Properties != null)
                {
                    writer.WritePropertyName("properties");
                    writer.WriteStartObject();
                    foreach (var prop in schema.Properties)
                    {
                        writer.WritePropertyName(prop.Key);
                        if (prop.Value != null)
                        {
                            prop.Value.SerializeV3(writer);
                        }
                        else
                        {
                            writer.WriteValue("null");
                        }
                    }
                    writer.WriteEndObject();
                }
                writer.WriteNumberProperty("maxProperties", schema.MaxProperties);
                writer.WriteNumberProperty("minProperties", schema.MinProperties);

                writer.WriteList("enum", schema.Enum, (nodeWriter, s) => nodeWriter.WriteValue(s));

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiSchema schema, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (schema == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (schema.IsReference())
            {
                schema.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();
                schema.SerializeSchemaProperties(writer);
                writer.WriteEndObject();
            }
        }

        public static void SerializeSchemaProperties(this OpenApiSchema schema, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (schema == null)
            {
                return;
            }

            writer.WriteStringProperty("title", schema.Title);
            writer.WriteStringProperty("type", schema.Type);
            writer.WriteStringProperty("format", schema.Format);
            writer.WriteStringProperty("description", schema.Description);

            writer.WriteNumberProperty("maxLength", schema.MaxLength);
            writer.WriteNumberProperty("minLength", schema.MinLength);
            writer.WriteStringProperty("pattern", schema.Pattern);
            writer.WriteStringProperty("default", schema.Default);

            writer.WriteList("required", schema.Required, (nodeWriter, s) => nodeWriter.WriteValue(s));

            writer.WriteNumberProperty("maximum", schema.Maximum);
            writer.WriteBoolProperty("exclusiveMaximum", schema.ExclusiveMaximum, false);
            writer.WriteNumberProperty("minimum", schema.Minimum);
            writer.WriteBoolProperty("exclusiveMinimum", schema.ExclusiveMinimum, false);

            if (schema.AdditionalProperties != null)
            {
                writer.WritePropertyName("additionalProperties");
                schema.AdditionalProperties.SerializeV2(writer);
            }

            if (schema.Items != null)
            {
                writer.WritePropertyName("items");
                schema.Items.SerializeV2(writer);
            }
            writer.WriteNumberProperty("maxItems", schema.MaxItems);
            writer.WriteNumberProperty("minItems", schema.MinItems);

            if (schema.Properties != null)
            {
                writer.WritePropertyName("properties");
                writer.WriteStartObject();
                foreach (var prop in schema.Properties)
                {
                    writer.WritePropertyName(prop.Key);
                    if (prop.Value != null)
                    {
                        prop.Value.SerializeV2(writer);
                    }
                    else
                    {
                        writer.WriteValue("null");
                    }
                }
                writer.WriteEndObject();
            }
            writer.WriteNumberProperty("maxProperties", schema.MaxProperties);
            writer.WriteNumberProperty("minProperties", schema.MinProperties);

            writer.WriteList("enum", schema.Enum, (nodeWriter, s) => nodeWriter.WriteValue(s));
        }
    }
}
