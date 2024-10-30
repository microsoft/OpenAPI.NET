// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Reader.ParseNodes;

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
                (o, n, _) => GetOrCreateSchema(o).Type = n.GetScalarValue().ToJsonSchemaType()
            },
            {
                "format",
                (o, n, _) => GetOrCreateSchema(o).Format = n.GetScalarValue()
            },
            {
                "items",
                (o, n, _) => GetOrCreateSchema(o).Items = LoadSchema(n)
            },
            {
                "collectionFormat",
                (o, n, _) => LoadStyle(o, n.GetScalarValue())
            },
            {
                "default",
                (o, n, _) => GetOrCreateSchema(o).Default = n.CreateAny()
            },
            {
                "maximum",
                (o, n, _) => GetOrCreateSchema(o).Maximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MaxValue)
            },
            {
                "exclusiveMaximum",
                (o, n, _) => GetOrCreateSchema(o).ExclusiveMaximum = bool.Parse(n.GetScalarValue())
            },
            {
                "minimum",
                (o, n, _) => GetOrCreateSchema(o).Minimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MinValue)
            },
            {
                "exclusiveMinimum",
                (o, n, _) => GetOrCreateSchema(o).ExclusiveMinimum = bool.Parse(n.GetScalarValue())
            },
            {
                "maxLength",
                (o, n, _) => GetOrCreateSchema(o).MaxLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "minLength",
                (o, n, _) => GetOrCreateSchema(o).MinLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "pattern",
                (o, n, _) => GetOrCreateSchema(o).Pattern = n.GetScalarValue()
            },
            {
                "maxItems",
                (o, n, _) => GetOrCreateSchema(o).MaxItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "minItems",
                (o, n, _) => GetOrCreateSchema(o).MinItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "uniqueItems",
                (o, n, _) => GetOrCreateSchema(o).UniqueItems = bool.Parse(n.GetScalarValue())
            },
            {
                "multipleOf",
                (o, n, _) => GetOrCreateSchema(o).MultipleOf = decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
            },
            {
                "enum",
                (o, n, _) => GetOrCreateSchema(o).Enum = n.CreateListOfAny()
            }
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        private static OpenApiSchema GetOrCreateSchema(OpenApiHeader p)
        {
            return p.Schema ??= new();
        }

        public static OpenApiHeader LoadHeader(ParseNode node, OpenApiDocument hostDocument = null)
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
