// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Nodes;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Extensions;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<JsonSchemaBuilder> _schemaFixedFields = new()
        {
            {
                "title", (o, n) =>
                {
                    o.Title(n.GetScalarValue());
                }
            },
            {
                "multipleOf", (o, n) =>
                {
                    o.MultipleOf(decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture));
                }
            },
            {
                "maximum", (o, n) =>
                {
                    o.Maximum(decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture));
                }
            },
            {
                "exclusiveMaximum", (o, n) =>
                {
                    o.ExclusiveMaximum(bool.Parse(n.GetScalarValue()));
                }
            },
            {
                "minimum", (o, n) =>
                {
                    o.Minimum(decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture));
                }
            },
            {
                "exclusiveMinimum", (o, n) =>
                {
                    o.ExclusiveMinimum(bool.Parse(n.GetScalarValue()));
                }
            },
            {
                "maxLength", (o, n) =>
                {
                    o.MaxLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "minLength", (o, n) =>
                {
                    o.MinLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "pattern", (o, n) =>
                {
                    o.Pattern(n.GetScalarValue());
                }
            },
            {
                "maxItems", (o, n) =>
                {
                    o.MaxItems(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "minItems", (o, n) =>
                {
                    o.MinItems(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "uniqueItems", (o, n) =>
                {
                    o.UniqueItems(bool.Parse(n.GetScalarValue()));
                }
            },
            {
                "maxProperties", (o, n) =>
                {
                    o.MaxProperties(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "minProperties", (o, n) =>
                {
                    o.MinProperties(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "required", (o, n) =>
                {
                    o.Required(new HashSet<string>(n.CreateSimpleList(n2 => n2.GetScalarValue())));
                }
            },
            {
                "enum", (o, n) =>
                {
                    o.Enum(n.CreateListOfAny());
                }
            },
            {
                "type", (o, n) =>
                {
                    if(n is ListNode)
                    {
                        o.Type(n.CreateSimpleList(s => SchemaTypeConverter.ConvertToSchemaValueType(s.GetScalarValue())));
                    }
                    else
                    {
                        o.Type(SchemaTypeConverter.ConvertToSchemaValueType(n.GetScalarValue()));
                    }
                }
            },
            {
                "allOf", (o, n) =>
                {
                    o.AllOf(n.CreateList(LoadSchema));
                }
            },
            {
                "items", (o, n) =>
                {
                    o.Items(LoadSchema(n));
                }
            },
            {
                "properties", (o, n) =>
                {
                    o.Properties(n.CreateMap(LoadSchema));
                }
            },
            {
                "additionalProperties", (o, n) =>
                {
                    if (n is ValueNode)
                    {
                        o.AdditionalProperties(bool.Parse(n.GetScalarValue()));
                    }
                    else
                    {
                        o.AdditionalProperties(LoadSchema(n));
                    }
                }
            },
            {
                "description", (o, n) =>
                {
                    o.Description(n.GetScalarValue());
                }
            },
            {
                "format", (o, n) =>
                {
                    o.Format(n.GetScalarValue());
                }
            },
            {
                "default", (o, n) =>
                {
                    o.Default(n.CreateAny().Node);
                }
            },
            {
                "discriminator", (o, n) =>
                {
                    var discriminator = new OpenApiDiscriminator
                    {
                        PropertyName = n.GetScalarValue()
                    };
                    o.Discriminator(discriminator.PropertyName, (IReadOnlyDictionary<string, string>)discriminator.Mapping,
                        (IReadOnlyDictionary<string, JsonNode>)discriminator.Extensions);
                }
            },
            {
                "readOnly", (o, n) =>
                {
                    o.ReadOnly(bool.Parse(n.GetScalarValue()));
                }
            },
            {
                "xml", (o, n) =>
                {
                    var xml = LoadXml(n);
                    o.Xml(xml.Namespace, xml.Name, xml.Prefix, xml.Attribute, xml.Wrapped,
                        (IReadOnlyDictionary<string, JsonNode>)xml.Extensions);
                }
            },
            {
                "externalDocs", (o, n) =>
                {
                   var externalDocs = LoadExternalDocs(n);
                   o.ExternalDocs(externalDocs.Url, externalDocs.Description,
                       (IReadOnlyDictionary<string, JsonNode>)externalDocs.Extensions);
                }
            },
            {
                "example", (o, n) =>
                {
                    o.Example(n.CreateAny().Node);
                }
            },
        };

        private static readonly PatternFieldMap<JsonSchemaBuilder> _schemaPatternFields = new PatternFieldMap<JsonSchemaBuilder>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.Extensions(LoadExtensions(p, LoadExtension(p, n)))}
        };

        public static JsonSchema LoadSchema(ParseNode node)
        {
            var mapNode = node.CheckMapNode(OpenApiConstants.Schema);
            var schemaBuilder = new JsonSchemaBuilder();

            // check for a $ref and if present, add it to the builder as a Ref keyword
            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                builder.Ref(pointer);
            }

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(schemaBuilder, _schemaFixedFields, _schemaPatternFields);
            }

            var schema = schemaBuilder.Build();
            return schema;
        }

        private static Dictionary<string, IOpenApiExtension> LoadExtensions(string value, IOpenApiExtension extension)
        {
            var extensions = new Dictionary<string, IOpenApiExtension>
            {
                { value, extension }
            };
            return extensions;
        }
    }
}
