// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Models.Interfaces;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new()
        {
            {
                "description",
                (o, n, _) => o.Description = n.GetScalarValue()
            },
            {
                "type",
                (o, n, _) =>
                {
                    var type = n.GetScalarValue();
                    if (type != null)
                    {
                        GetOrCreateSchema(o).Type = type.ToJsonSchemaType();
                    }
                }
            },
            {
                "format",
                (o, n, _) => GetOrCreateSchema(o).Format = n.GetScalarValue()
            },
            {
                "items",
                (o, n, doc) => GetOrCreateSchema(o).Items = LoadSchema(n, doc)
            },
            {
                "collectionFormat",
                (o, n, _) =>
                {
                    var collectionFormat = n.GetScalarValue();
                    if (collectionFormat != null)
                    {
                        LoadStyle(o, collectionFormat);
                    }
                }
            },
            {
                "default",
                (o, n, _) => GetOrCreateSchema(o).Default = n.CreateAny()
            },
            {
                "maximum",
                (o, n, _) =>
                {
                    var max = n.GetScalarValue();
                    if (max != null)
                    {
                        GetOrCreateSchema(o).Maximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(max, decimal.MaxValue);
                    }
                }
            },
            {
                "exclusiveMaximum",
                (o, n, _) =>
                {
                    var scalarValue = n.GetScalarValue();
                    if (scalarValue is not null)
                    {
                        GetOrCreateSchema(o).ExclusiveMaximum = bool.Parse(scalarValue);
                    }
                }
            },
            {
                "minimum",
                (o, n, _) =>
                {
                    var min = n.GetScalarValue();
                    if (min != null)
                    {
                        GetOrCreateSchema(o).Minimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(min, decimal.MinValue); 
                    } 
                }
            },
            {
                "exclusiveMinimum",
                (o, n, _) =>
                {
                    var exMin = n.GetScalarValue();
                    if (exMin != null)
                    {
                        GetOrCreateSchema(o).ExclusiveMinimum = bool.Parse(exMin);
                    }
                }
            },
            {
                "maxLength",
                (o, n, _) =>
                {
                    var maxLength = n.GetScalarValue();
                    if (maxLength != null)
                    {
                        GetOrCreateSchema(o).MaxLength = int.Parse(maxLength, CultureInfo.InvariantCulture);
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
                        GetOrCreateSchema(o).MinLength = int.Parse(minLength, CultureInfo.InvariantCulture); 
                    }
                }
            },
            {
                "pattern",
                (o, n, _) => GetOrCreateSchema(o).Pattern = n.GetScalarValue()
            },
            {
                "maxItems",
                (o, n, _) =>
                {
                    var maxItems = n.GetScalarValue();
                    if (maxItems != null)
                    {
                        GetOrCreateSchema(o).MaxItems = int.Parse(maxItems, CultureInfo.InvariantCulture);
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
                        GetOrCreateSchema(o).MinItems = int.Parse(minItems, CultureInfo.InvariantCulture);
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
                        GetOrCreateSchema(o).UniqueItems = bool.Parse(uniqueItems);
                    }
                }
            },
            {
                "multipleOf",
                (o, n, _) =>
                {
                    var multipleOf = n.GetScalarValue();
                    if (multipleOf != null)
                    {
                        GetOrCreateSchema(o).MultipleOf = decimal.Parse(multipleOf, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "enum",
                (o, n, _) => GetOrCreateSchema(o).Enum = n.CreateListOfAny()
            }
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        private static OpenApiSchema GetOrCreateSchema(OpenApiHeader p)
        {
            return p.Schema switch {
                OpenApiSchema schema => schema,
                _ => (OpenApiSchema)(p.Schema = new OpenApiSchema()),
            };
        }

        public static IOpenApiHeader LoadHeader(ParseNode node, OpenApiDocument? hostDocument)
        {
            var mapNode = node.CheckMapNode("header");
            var header = new OpenApiHeader();
            
            foreach (var property in mapNode)
            {
                property.ParseField(header, _headerFixedFields, _headerPatternFields, hostDocument);
            }

            var schema = node.Context.GetFromTempStorage<IOpenApiSchema>("schema");
            if (schema != null)
            {
                header.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }            

            return header;
        }

        private static void LoadStyle(OpenApiHeader header, string style)
        {
            switch (style)
            {
                case "csv":
                    header.Style = ParameterStyle.Simple;
                    return;
                case "ssv":
                    header.Style = ParameterStyle.SpaceDelimited;
                    return;
                case "pipes":
                    header.Style = ParameterStyle.PipeDelimited;
                    return;
                case "tsv":
                    throw new NotSupportedException();
                default:
                    throw new OpenApiReaderException("Unrecognized header style: " + style);
            }
        }
    }
}
