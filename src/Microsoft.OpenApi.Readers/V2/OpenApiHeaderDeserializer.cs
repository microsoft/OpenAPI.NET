// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Any;
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
        private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new FixedFieldMap<OpenApiHeader>
        {
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "type", (o, n) =>
                {
                    GetOrCreateSchema(o).Type = n.GetScalarValue();
                }
            },
            {
                "format", (o, n) =>
                {
                    GetOrCreateSchema(o).Format = n.GetScalarValue();
                }
            },
            {
                "items", (o, n) =>
                {
                    GetOrCreateSchema(o).Items = LoadSchema(n);
                }
            },
            {
                "collectionFormat", (o, n) =>
                {
                    LoadStyle(o, n.GetScalarValue());
                }
            },
            {
                "default", (o, n) =>
                {
                    GetOrCreateSchema(o).Default = n.CreateAny();
                }
            },
                        {
                "maximum", (o, n) =>
                {
                    GetOrCreateSchema(o).Maximum = decimal.Parse(n.GetScalarValue());
                }
            },
            {
                "exclusiveMaximum", (o, n) =>
                {
                    GetOrCreateSchema(o).ExclusiveMaximum = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "minimum", (o, n) =>
                {
                    GetOrCreateSchema(o).Minimum = decimal.Parse(n.GetScalarValue());
                }
            },
            {
                "exclusiveMinimum", (o, n) =>
                {
                    GetOrCreateSchema(o).ExclusiveMinimum = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "maxLength", (o, n) =>
                {
                    GetOrCreateSchema(o).MaxLength = int.Parse(n.GetScalarValue());
                }
            },
            {
                "minLength", (o, n) =>
                {
                    GetOrCreateSchema(o).MinLength = int.Parse(n.GetScalarValue());
                }
            },
            {
                "pattern", (o, n) =>
                {
                    GetOrCreateSchema(o).Pattern = n.GetScalarValue();
                }
            },
            {
                "maxItems", (o, n) =>
                {
                    GetOrCreateSchema(o).MaxItems = int.Parse(n.GetScalarValue());
                }
            },
            {
                "minItems", (o, n) =>
                {
                    GetOrCreateSchema(o).MinItems = int.Parse(n.GetScalarValue());
                }
            },
            {
                "uniqueItems", (o, n) =>
                {
                    GetOrCreateSchema(o).UniqueItems = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "multipleOf", (o, n) =>
                {
                    GetOrCreateSchema(o).MultipleOf = decimal.Parse(n.GetScalarValue());
                }
            },
            {
                "enum", (o, n) =>
                {
                    GetOrCreateSchema(o).Enum = n.CreateListOfAny();
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new PatternFieldMap<OpenApiHeader>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
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