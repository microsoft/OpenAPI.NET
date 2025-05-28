// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiSchema> _openApiSchemaFixedFields = new()
        {
            {
                "title",
                (o, n, _) => o.Title = n.GetScalarValue()
            },
            {
                "multipleOf",
                (o, n, _) =>
                {
                    var multipleOf = n.GetScalarValue();
                    if (multipleOf != null)
                    {
                        o.MultipleOf = decimal.Parse(multipleOf, NumberStyles.Float, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "maximum",
                (o, n,_) =>
                {
                    var max = n.GetScalarValue();
                    if (!string.IsNullOrEmpty(max))
                    {
                        o.Maximum = max;
                    }
                }
            },
            {
                "exclusiveMaximum",
                (o, n, _) => o.IsExclusiveMaximum = bool.Parse(n.GetScalarValue())
            },
            {
                "minimum",
                (o, n, _) =>
                {
                    var min = n.GetScalarValue();
                    if (!string.IsNullOrEmpty(min))
                    {
                        o.Minimum = min;
                    }
                }
            },
            {
                "exclusiveMinimum",
                (o, n, _) => o.IsExclusiveMinimum = bool.Parse(n.GetScalarValue())
            },
            {
                "maxLength",
                (o, n, _) =>
                {
                    var maxLength = n.GetScalarValue();
                    if (maxLength != null)
                    {
                        o.MaxLength = int.Parse(maxLength, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "minLength",
                (o, n, _) =>
                {
                    var minLength = n.GetScalarValue();
                    if (minLength != null)
                    {
                        o.MinLength = int.Parse(minLength, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "pattern",
                (o, n, _) => o.Pattern = n.GetScalarValue()
            },
            {
                "maxItems",
                (o, n, _) =>
                {
                    var maxItems = n.GetScalarValue();
                    if (maxItems != null)
                    {
                        o.MaxItems = int.Parse(maxItems, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "minItems",
                (o, n, _) =>
                {
                    var minItems = n.GetScalarValue();
                    if (minItems != null)
                    {
                        o.MinItems = int.Parse(minItems, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "uniqueItems",
                (o, n, _) =>
                {
                    var uniqueItems = n.GetScalarValue();
                    if (uniqueItems != null)
                    {
                        o.UniqueItems = bool.Parse(uniqueItems);
                    }
                }
            },
            {
                "maxProperties",
                (o, n, _) => 
                {
                    var maxProps = n.GetScalarValue();
                    if (maxProps != null)
                    {
                        o.MaxProperties = int.Parse(maxProps, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "minProperties",
                (o, n, _) =>
                {
                    var minProps = n.GetScalarValue();
                    if (minProps != null)
                    {
                        o.MinProperties = int.Parse(minProps, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "required",
                (o, n, doc) => 
                { 
                    o.Required = new HashSet<string>(
                        n.CreateSimpleList((n2, p) => 
                        n2.GetScalarValue(), doc)
                        .Where(s => s != null)!); 
                } 
            },
            {
                "enum",
                (o, n, _) => o.Enum = n.CreateListOfAny()
            },

            {
                "type",
                (o, n, _) =>
                {
                    var type = n.GetScalarValue();
                    if (type != null)
                    {
                        o.Type = type.ToJsonSchemaType();
                    }
                }
            },
            {
                "allOf",
                (o, n, t) => o.AllOf = n.CreateList(LoadSchema, t)
            },
            {
                "items",
                (o, n, doc) => o.Items = LoadSchema(n, doc)
            },
            {
                "properties",
                (o, n, t) => o.Properties = n.CreateMap(LoadSchema, t)
            },
            {
                "additionalProperties", (o, n, doc) =>
                {
                    if (n is ValueNode)
                    {
                        var value = n.GetScalarValue();
                        if (value is not null)
                        {
                            o.AdditionalPropertiesAllowed = bool.Parse(value);
                        }
                    }
                    else
                    {
                        o.AdditionalProperties = LoadSchema(n, doc);
                    }
                }
            },
            {
                "description",
                (o, n, _) => o.Description = n.GetScalarValue()
            },
            {
                "format",
                (o, n, _) => o.Format = n.GetScalarValue()
            },
            {
                "default",
                (o, n, _) => o.Default = n.CreateAny()
            },
            {
                "discriminator", (o, n, _) =>
                {
                    o.Discriminator = new()
                    {
                        PropertyName = n.GetScalarValue()
                    };
                }
            },
            {
                "readOnly",
                (o, n, _) =>
                {
                    var readOnly = n.GetScalarValue();
                    if (readOnly is not null)
                    {
                        o.ReadOnly = bool.Parse(readOnly); 
                    }
                }
            },
            {
                "xml",
                (o, n, doc) => o.Xml = LoadXml(n, doc)
            },
            {
                "externalDocs",
                (o, n, doc) => o.ExternalDocs = LoadExternalDocs(n, doc)
            },
            {
                "example",
                (o, n, _) => o.Example = n.CreateAny()
            },
        };

        private static readonly PatternFieldMap<OpenApiSchema> _openApiSchemaPatternFields = new PatternFieldMap<OpenApiSchema>
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        public static IOpenApiSchema LoadSchema(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("schema");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiSchemaReference(reference.Item1, hostDocument, reference.Item2);
            }

            var schema = new OpenApiSchema();
 
            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(schema, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument);
            }

            if (schema.Extensions is not null && schema.Extensions.ContainsKey(OpenApiConstants.NullableExtension))
            {
                if (schema.Type.HasValue)
                    schema.Type |= JsonSchemaType.Null;
                else
                    schema.Type = JsonSchemaType.Null;

                schema.Extensions.Remove(OpenApiConstants.NullableExtension);
            }

            return schema;
        }
    }
}
