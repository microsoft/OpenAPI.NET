// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
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
                (o, n) => o.Description = n.GetScalarValue()
            },
            {
                "type",
                (o, n) => GetOrCreateSchema(o).Type = n.GetScalarValue()
            },
            {
                "format",
                (o, n) => GetOrCreateSchema(o).Format = n.GetScalarValue()
            },
            {
                "items",
                (o, n) => GetOrCreateSchema(o).Items = LoadSchema(n)
            },
            {
                "collectionFormat",
                (o, n) => LoadStyle(o, n.GetScalarValue())
            },
            {
                "default",
                (o, n) => GetOrCreateSchema(o).Default = n.CreateAny()
            },
            {
                "maximum",
                (o, n) => GetOrCreateSchema(o).Maximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MaxValue)
            },
            {
                "exclusiveMaximum",
                (o, n) => GetOrCreateSchema(o).ExclusiveMaximum = bool.Parse(n.GetScalarValue())
            },
            {
                "minimum",
                (o, n) => GetOrCreateSchema(o).Minimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MinValue)
            },
            {
                "exclusiveMinimum",
                (o, n) => GetOrCreateSchema(o).ExclusiveMinimum = bool.Parse(n.GetScalarValue())
            },
            {
                "maxLength",
                (o, n) => GetOrCreateSchema(o).MaxLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "minLength",
                (o, n) => GetOrCreateSchema(o).MinLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "pattern",
                (o, n) => GetOrCreateSchema(o).Pattern = n.GetScalarValue()
            },
            {
                "maxItems",
                (o, n) => GetOrCreateSchema(o).MaxItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "minItems",
                (o, n) => GetOrCreateSchema(o).MinItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "uniqueItems",
                (o, n) => GetOrCreateSchema(o).UniqueItems = bool.Parse(n.GetScalarValue())
            },
            {
                "multipleOf",
                (o, n) => GetOrCreateSchema(o).MultipleOf = decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "enum",
                (o, n) => GetOrCreateSchema(o).Enum = n.CreateListOfAny()
            }
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
        };

        private static readonly AnyFieldMap<OpenApiHeader> _headerAnyFields =
            new()
            {
                {
                    OpenApiConstants.Default,
                    new(
                        p => p.Schema?.Default,
                        (p, v) =>
                        {
                            if(p.Schema == null) return;
                            p.Schema.Default = v;
                        },
                        p => p.Schema)
                }
            };

        private static readonly AnyListFieldMap<OpenApiHeader> _headerAnyListFields =
            new()
            {
                {
                    OpenApiConstants.Enum,
                    new(
                        p => p.Schema?.Enum,
                        (p, v) =>
                        {
                            if(p.Schema == null) return;
                            p.Schema.Enum = v;
                        },
                        p => p.Schema)
                },
            };

        public static OpenApiHeader LoadHeader(ParseNode node)
        {
            var mapNode = node.CheckMapNode("header");
            var header = new OpenApiHeader();
            foreach (var property in mapNode)
            {
                property.ParseField(header, _headerFixedFields, _headerPatternFields);
            }

            var schema = node.Context.GetFromTempStorage<OpenApiSchema>("schema");
            if (schema != null)
            {
                header.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            ProcessAnyFields(mapNode, header, _headerAnyFields);
            ProcessAnyListFields(mapNode, header, _headerAnyListFields);

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
