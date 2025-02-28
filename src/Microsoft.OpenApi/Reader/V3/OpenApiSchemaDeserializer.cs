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

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiSchema> _openApiSchemaFixedFields = new()
        {
            {
                "title",
                (o, n, _) => o.Title = n.GetScalarValue()
            },
            {
                "multipleOf",
                (o, n, _) => o.MultipleOf = decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture)
            },
            {
                "maximum",
                (o, n, _) => o.Maximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MaxValue)
            },
            {
                "exclusiveMaximum",
                (o, n, _) => o.IsExclusiveMaximum = bool.Parse(n.GetScalarValue())
            },
            {
                "minimum",
                (o, n, _) => o.Minimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MinValue)
            },
            {
                "exclusiveMinimum",
                (o, n, _) => o.IsExclusiveMinimum = bool.Parse(n.GetScalarValue())
            },
            {
                "maxLength",
                (o, n, _) => o.MaxLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "minLength",
                (o, n, _) => o.MinLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "pattern",
                (o, n, _) => o.Pattern = n.GetScalarValue()
            },
            {
                "maxItems",
                (o, n, _) => o.MaxItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "minItems",
                (o, n, _) => o.MinItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "uniqueItems",
                (o, n, _) => o.UniqueItems = bool.Parse(n.GetScalarValue())
            },
            {
                "maxProperties",
                (o, n, _) => o.MaxProperties = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "minProperties",
                (o, n, _) => o.MinProperties = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "required",
                (o, n, doc) => o.Required = new HashSet<string>(n.CreateSimpleList((n2, p) => n2.GetScalarValue(), doc))
            },
            {
                "enum",
                (o, n, _) => o.Enum = n.CreateListOfAny()
            },
            {
                "type",
                (o, n, _) => {
                    var type = n.GetScalarValue().ToJsonSchemaType();
                    // so we don't loose the value from nullable
                    if (o.Type.HasValue)
                        o.Type |= type;
                    else
                        o.Type = type;
                }
            },
            {
                "allOf",
                (o, n, t) => o.AllOf = n.CreateList(LoadSchema, t)
            },
            {
                "oneOf",
                (o, n, doc) => o.OneOf = n.CreateList(LoadSchema, doc)
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
                "additionalProperties", (o, n, doc) =>
                {
                    if (n is ValueNode)
                    {
                        o.AdditionalPropertiesAllowed = bool.Parse(n.GetScalarValue());
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
                    if (bool.TryParse(n.GetScalarValue(), out var parsed) && parsed)
                    {
                        if (o.Type.HasValue)
                            o.Type |= JsonSchemaType.Null;
                        else
                            o.Type = JsonSchemaType.Null;
                    }
                }
            },
            {
                "discriminator",
                (o, n, doc) => o.Discriminator = LoadDiscriminator(n, doc)
            },
            {
                "readOnly",
                (o, n, _) => o.ReadOnly = bool.Parse(n.GetScalarValue())
            },
            {
                "writeOnly",
                (o, n, _) => o.WriteOnly = bool.Parse(n.GetScalarValue())
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
                "deprecated",
                (o, n, _) => o.Deprecated = bool.Parse(n.GetScalarValue())
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
                return new OpenApiSchemaReference(reference.Item1, hostDocument, reference.Item2);
            }

            var schema = new OpenApiSchema();

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(schema, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument);
            }

            return schema;
        }
    }
}
