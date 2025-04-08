// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OpenApi.Reader.V31
{
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiSchema> _openApiSchemaFixedFields = new()
        {
            {
                "title",
                (o, n, _) => o.Title = n.GetScalarValue()
            },
            {
                "$schema",
                (o, n, _) => { if (n.GetScalarValue() is string {} sSchema && Uri.TryCreate(sSchema, UriKind.Absolute, out var schema)) {o.Schema = schema;}}
            },
            {
                "$id",
                (o, n, _) => o.Id = n.GetScalarValue()
            },
            {
                "$comment",
                (o, n, _) => o.Comment = n.GetScalarValue()
            },
            {
                "$vocabulary",
                (o, n, _) => o.Vocabulary = n.CreateSimpleMap(LoadBool).ToDictionary(kvp => kvp.Key, kvp => kvp.Value ?? false)
            },
            {
                "$dynamicRef",
                (o, n, _) => o.DynamicRef = n.GetScalarValue()
            },
            {
                "$dynamicAnchor",
                (o, n, _) => o.DynamicAnchor = n.GetScalarValue()
            },
            {
                "$defs",
                (o, n, t) => o.Definitions = n.CreateMap(LoadSchema, t)
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
                    if (max != null)
                    {
                        o.Maximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(max, decimal.MaxValue);
                    }
                }
            },
            {
                "exclusiveMaximum",
                (o, n, _) => o.ExclusiveMaximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MaxValue)
            },
            {
                "minimum",
                (o, n, _) =>
                {
                    var min = n.GetScalarValue();
                    if (min != null)
                    {
                        o.Minimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(min, decimal.MinValue);
                    }
                }
            },
            {
                "exclusiveMinimum",
                (o, n, _) => o.ExclusiveMinimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MaxValue)
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
                "unevaluatedProperties",
                (o, n, _) =>
                {
                    var unevaluatedProps = n.GetScalarValue();
                    if (unevaluatedProps != null)
                    {
                        o.UnevaluatedProperties = bool.Parse(unevaluatedProps);
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
                (o, n, doc) => o.Required = new HashSet<string>(n.CreateSimpleList((n2, p) => n2.GetScalarValue(), doc).Where(s => s != null))
            },
            {
                "enum",
                (o, n, _) => o.Enum = n.CreateListOfAny()
            },
            {
                "type",
                (o, n, doc) => 
                {
                    if (n is ValueNode)
                    {
                        o.Type = n.GetScalarValue()?.ToJsonSchemaType();
                    }
                    else
                    {
                        var list = n.CreateSimpleList((n2, p) => n2.GetScalarValue(), doc);
                        JsonSchemaType combinedType = 0;
                        foreach(var type in list)
                        {
                            if (type is not null)
                            {
                                var schemaType = type.ToJsonSchemaType();
                                combinedType |= schemaType;
                            }                            
                        }
                        o.Type = combinedType;
                    }
                }
            },
            {
                "const",
                (o, n, _) => o.Const = n.GetScalarValue()
            },
            {
                "allOf",
                (o, n, t) => o.AllOf = n.CreateList(LoadSchema, t)
            },
            {
                "oneOf",
                (o, n, t) => o.OneOf = n.CreateList(LoadSchema, t)
            },
            {
                "anyOf",
                (o, n, t) => o.AnyOf = n.CreateList(LoadSchema, t)
            },
            {
                "not",
                (o, n, doc) => o.Not = LoadSchema(n, doc)
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
                "patternProperties",
                (o, n, t) => o.PatternProperties = n.CreateMap(LoadSchema, t)
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
                "nullable",
                (o, n, _) => 
                {
                    var value = n.GetScalarValue();
                    if (value is not null)
                    {
                        var nullable = bool.Parse(value);
                        if (nullable) // if nullable, convert type into an array of type(s) and null
                        {
                            o.Type |= JsonSchemaType.Null;
                        }
                    }
                }
            },
            {
                "discriminator",
                (o, n, doc) => o.Discriminator = LoadDiscriminator(n, doc)
            },
            {
                "readOnly",
                (o, n, _) =>
                {
                    var readOnly = n.GetScalarValue();
                    if (readOnly != null)
                    {
                        o.ReadOnly = bool.Parse(readOnly);
                    }
                }
            },
            {
                "writeOnly",
                (o, n, _) =>
                {
                    var writeOnly = n.GetScalarValue();
                    if (writeOnly != null)
                    {
                        o.WriteOnly = bool.Parse(writeOnly);
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
            {
                "examples",
                (o, n, _) => o.Examples = n.CreateListOfAny()
            },
            {
                "deprecated",
                (o, n, t) =>
                {
                    var deprecated = n.GetScalarValue();
                    if (deprecated != null)
                    {
                        o.Deprecated = bool.Parse(deprecated);
                    }
                }
            },
            {
                "dependentRequired",
                (o, n, doc) =>
                {
                    o.DependentRequired = n.CreateArrayMap((n2, p) => n2.GetScalarValue(), doc);
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiSchema> _openApiSchemaPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static IOpenApiSchema LoadSchema(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode(OpenApiConstants.Schema);

            var pointer = mapNode.GetReferencePointer();

            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var result = new OpenApiSchemaReference(reference.Item1, hostDocument, reference.Item2);
                result.Reference.SetSummaryAndDescriptionFromMapNode(mapNode);
                return result;
            }

            var schema = new OpenApiSchema();

            foreach (var propertyNode in mapNode)
            {
                bool isRecognized = _openApiSchemaFixedFields.ContainsKey(propertyNode.Name) ||
                        _openApiSchemaPatternFields.Any(p => p.Key(propertyNode.Name));

                if (isRecognized)
                {
                    propertyNode.ParseField(schema, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument);
                }
                else if (schema.UnrecognizedKeywords is not null && propertyNode.JsonNode is not null)
                {
                    schema.UnrecognizedKeywords[propertyNode.Name] = propertyNode.JsonNode;
                }
            }

            if (schema.Extensions is not null && schema.Extensions.ContainsKey(OpenApiConstants.NullableExtension))
            {
                var type = schema.Type;
                schema.Type = type | JsonSchemaType.Null;
                schema.Extensions.Remove(OpenApiConstants.NullableExtension);
            }

            return schema;
        }
    }
}
