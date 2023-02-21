// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using JsonSchema = Json.Schema.JsonSchema;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<JsonSchemaBuilder> _schemaFixedFields = new FixedFieldMap<JsonSchemaBuilder>
        {
            {
                "title", (o, n) =>
                {
                    o.Title(o.Get<TitleKeyword>().Value);
                }
            },
            {
                "multipleOf", (o, n) =>
                {
                    o.MultipleOf(o.Get<MultipleOfKeyword>().Value);
                }
            },
            {
                "maximum", (o, n) =>
                {
                    o.Maximum(o.Get<MaximumKeyword>().Value);
                }
            },
            {
                "exclusiveMaximum", (o, n) =>
                {
                    o.ExclusiveMaximum(o.Get<ExclusiveMaximumKeyword>().Value);
                }
            },
            {
                "minimum", (o, n) =>
                {
                    o.Minimum(o.Get<MinimumKeyword>().Value);
                }
            },
            {
                "exclusiveMinimum", (o, n) =>
                {
                    o.ExclusiveMinimum(o.Get<ExclusiveMaximumKeyword>().Value);
                }
            },
            {
                "maxLength", (o, n) =>
                {
                    o.MaxLength(o.Get<MaxLengthKeyword>().Value);
                }
            },
            {
                "minLength", (o, n) =>
                {
                    o.MinLength(o.Get<MinLengthKeyword>().Value);
                }
            },
            {
                "pattern", (o, n) =>
                {
                    o.Pattern(o.Get<PatternKeyword>().Value);
                }
            },
            {
                "maxItems", (o, n) =>
                {
                    o.MaxItems(o.Get<MaxItemsKeyword>().Value);
                }
            },
            {
                "minItems", (o, n) =>
                {
                    o.MinItems(o.Get<MinItemsKeyword>().Value);
                }
            },
            {
                "uniqueItems", (o, n) =>
                {
                    o.UniqueItems(o.Get<UniqueItemsKeyword>().Value);
                }
            },
            {
                "maxProperties", (o, n) =>
                {
                    o.MaxProperties(o.Get<MaxPropertiesKeyword>().Value);
                }
            },
            {
                "minProperties", (o, n) =>
                {
                    o.MinProperties(o.Get<MinPropertiesKeyword>().Value);
                }
            },
            {
                "required", (o, n) =>
                {
                    o.Required(o.Get<RequiredKeyword>().Properties);
                }
            },
            {
                "enum", (o, n) =>
                {
                    o.Enum(o.Get<EnumKeyword>().Values);
                }
            },
            {
                "type", (o, n) =>
                {
                    o.Type(o.Get<TypeKeyword>().Type);
                }
            },
            {
                "allOf", (o, n) =>
                {
                    o.AllOf(o.Get<AllOfKeyword>().Schemas);
                }
            },
            {
                "oneOf", (o, n) =>
                {
                    o.OneOf(o.Get<OneOfKeyword>().Schemas);
                }
            },
            {
                "anyOf", (o, n) =>
                {
                    o.AnyOf(o.Get<AnyOfKeyword>().Schemas);
                }
            },
            {
                "not", (o, n) =>
                {
                    o.Not(o.Get<NotKeyword>().Schema);
                }
            },
            {
                "items", (o, n) =>
                {
                    o.Items(o.Get<ItemsKeyword>().SingleSchema);
                }
            },
            {
                "properties", (o, n) =>
                {
                    o.Properties(o.Get<PropertiesKeyword>().Properties);
                }
            },
            {
                "additionalProperties", (o, n) =>
                {
                    o.AdditionalProperties(o.Get<AdditionalPropertiesKeyword>().Schema);
                }
            },
            {
                "description", (o, n) =>
                {
                    o.Description(o.Get<DescriptionKeyword>().Value);
                }
            },
            {
                "format", (o, n) =>
                {
                    o.Format(o.Get<FormatKeyword>().Value);
                }
            },
            {
                "default", (o, n) =>
                {
                    o.Default(o.Get<DefaultKeyword>().Value);
                }
            },            
            {
                "discriminator", (o, n) =>
                {
                    //o.Discriminator(o.Get<DiscriminatorKeyword>().Mapping);
                }
            },
            {
                "readOnly", (o, n) =>
                {
                    o.ReadOnly(o.Get<ReadOnlyKeyword>().Value);
                }
            },
            {
                "writeOnly", (o, n) =>
                {
                    o.WriteOnly(o.Get<WriteOnlyKeyword>().Value);
                }
            },
            {
                "xml", (o, n) =>
                {
                    //o.Xml(o.Get<XmlKeyword>());
                }
            },
            {
                "externalDocs", (o, n) =>
                {
                   // o.ExternalDocs(o.Get<ExternalDocsKeyword>());
                }
            },
            {
                "example", (o, n) =>
                {
                    o.Example(o.Get<ExampleKeyword>().Value);
                }
            },
            {
                "deprecated", (o, n) =>
                {
                    o.Deprecated(o.Get<DeprecatedKeyword>().Value);
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiSchema> _schemaPatternFields = new PatternFieldMap<OpenApiSchema>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
        };

        private static readonly AnyFieldMap<OpenApiSchema> _schemaAnyFields = new AnyFieldMap<OpenApiSchema>
        {
            {
                OpenApiConstants.Default,
                new AnyFieldMapParameter<OpenApiSchema>(
                    s => s.Default,
                    (s, v) => s.Default = v,
                    s => s)
            },
            {
                 OpenApiConstants.Example,
                new AnyFieldMapParameter<OpenApiSchema>(
                    s => s.Example,
                    (s, v) => s.Example = v,
                    s => s)
            }
        };

        private static readonly AnyListFieldMap<OpenApiSchema> _schemaAnyListFields = new AnyListFieldMap<OpenApiSchema>
        {
            {
                OpenApiConstants.Enum,
                new AnyListFieldMapParameter<OpenApiSchema>(
                    s => s.Enum,
                    (s, v) => s.Enum = v,
                    s => s)
            }
        };

        public static JsonSchema LoadSchema(ParseNode node)
        {
            var mapNode = node.CheckMapNode(OpenApiConstants.Schema);

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var description = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Description);
                var summary = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Summary);

                return new OpenApiSchema
                {
                    UnresolvedReference = true,
                    Reference = node.Context.VersionService.ConvertToOpenApiReference(pointer, ReferenceType.Schema, summary, description)
                };
            }

            //var schema = new OpenApiSchema();
            var builder = new JsonSchemaBuilder();

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(builder, _schemaFixedFields, _schemaPatternFields);
            }

            OpenApiV3Deserializer.ProcessAnyFields(mapNode, builder, _schemaAnyFields);
            OpenApiV3Deserializer.ProcessAnyListFields(mapNode, builder, _schemaAnyListFields);

            return builder.Build();
        }
    }
}
