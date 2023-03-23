// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using JsonSchema = Json.Schema.JsonSchema;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
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
                    o.ExclusiveMaximum(decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture));
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
                    o.ExclusiveMinimum(decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture));
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
                    o.Enum((IEnumerable<JsonNode>)n.CreateListOfAny());
                }
            },
            {
                "type", (o, n) =>
                {
                    if(n is ListNode)
                    {                        
                        o.Type(n.CreateSimpleList(s => ConvertToSchemaValueType(s.GetScalarValue())));
                    }
                    else
                    {
                        o.Type(ConvertToSchemaValueType(n.GetScalarValue()));
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
                "oneOf", (o, n) =>
                {
                    o.OneOf(n.CreateList(LoadSchema));
                }
            },
            {
                "anyOf", (o, n) =>
                {
                    o.AnyOf(n.CreateList(LoadSchema));
                }
            },
            {
                "not", (o, n) =>
                {
                    o.Not(LoadSchema(n));
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
                    o.Default((JsonNode)n.CreateAny());
                }
            },            
            {
                "discriminator", (o, n) =>
                {
                    var discriminator = LoadDiscriminator(n);
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
                "writeOnly", (o, n) =>
                {
                    o.WriteOnly(bool.Parse(n.GetScalarValue()));
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
                "examples", (o, n) =>
                {
                    if(n is ListNode)
                    {
                        o.Examples(n.CreateSimpleList(s => (JsonNode)s.GetScalarValue()));
                    }
                    else
                    {
                        o.Examples((JsonNode)n.CreateAny());
                    }
                }
            },
            {
                "deprecated", (o, n) =>
                {
                    o.Deprecated(bool.Parse(n.GetScalarValue()));
                }
            },
        };

        private static readonly PatternFieldMap<JsonSchemaBuilder> _schemaPatternFields = new PatternFieldMap<JsonSchemaBuilder>
        {
            //{s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static JsonSchema LoadSchema(ParseNode node)
        {
            var mapNode = node.CheckMapNode(OpenApiConstants.Schema);

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                //var description = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Description);
                //var summary = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Summary);

                //return new OpenApiSchema
                //{
                //    UnresolvedReference = true,
                //    Reference = node.Context.VersionService.ConvertToOpenApiReference(pointer, ReferenceType.Schema, summary, description)
                //};
            }

            var builder = new JsonSchemaBuilder();

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(builder, _schemaFixedFields, _schemaPatternFields);
            }

            //OpenApiV31Deserializer.ProcessAnyFields(mapNode, builder, _schemaAnyFields);
            //OpenApiV31Deserializer.ProcessAnyListFields(mapNode, builder, _schemaAnyListFields);

            return builder.Build();
        }

        private static SchemaValueType ConvertToSchemaValueType(string value)
        {
            return value switch
            {
                "string" => SchemaValueType.String,
                "number" => SchemaValueType.Number,
                "integer" => SchemaValueType.Integer,
                "boolean" => SchemaValueType.Boolean,
                "array" => SchemaValueType.Array,
                "object" => SchemaValueType.Object,
                "null" => SchemaValueType.Null,
                _ => throw new NotSupportedException(),
            };
        }
    }

}
