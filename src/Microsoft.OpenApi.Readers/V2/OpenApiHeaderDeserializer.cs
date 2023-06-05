// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using System.Linq;
using Json.Schema;
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
                    GetOrCreateSchema(o).Type(SchemaTypeConverter.ConvertToSchemaValueType(n.GetScalarValue())).Build();
                }
            },
            {
                "format", (o, n) =>
                {
                    GetOrCreateSchema(o).Format(n.GetScalarValue()).Build();
                }
            },
            {
                "items", (o, n) =>
                {
                    GetOrCreateSchema(o).Items(LoadSchema(n)).Build();
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
                    GetOrCreateSchema(o).Default(n.CreateAny().Node).Build();
                }
            },
            {
                "maximum", (o, n) =>
                {
                    GetOrCreateSchema(o).Maximum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)).Build();
                }
            },
            {
                "exclusiveMaximum", (o, n) =>
                {
                    GetOrCreateSchema(o).ExclusiveMaximum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)).Build();
                }
            },
            {
                "minimum", (o, n) =>
                {
                    GetOrCreateSchema(o).Minimum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)).Build();
                }
            },
            {
                "exclusiveMinimum", (o, n) =>
                {
                    GetOrCreateSchema(o).ExclusiveMinimum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)).Build();
                }
            },
            {
                "maxLength", (o, n) =>
                {
                    GetOrCreateSchema(o).MaxLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)).Build();
                }
            },
            {
                "minLength", (o, n) =>
                {
                    GetOrCreateSchema(o).MinLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)).Build();
                }
            },
            {
                "pattern", (o, n) =>
                {
                    GetOrCreateSchema(o).Pattern(n.GetScalarValue()).Build();
                }
            },
            {
                "maxItems", (o, n) =>
                {
                    GetOrCreateSchema(o).MaxItems(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)).Build();
                }
            },
            {
                "minItems", (o, n) =>
                {
                    GetOrCreateSchema(o).MinItems(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)).Build();
                }
            },
            {
                "uniqueItems", (o, n) =>
                {
                    GetOrCreateSchema(o).UniqueItems(bool.Parse(n.GetScalarValue())).Build();
                }
            },
            {
                "multipleOf", (o, n) =>
                {
                    GetOrCreateSchema(o).MultipleOf(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)).Build();
                }
            },
            {
                "enum", (o, n) =>
                {
                    GetOrCreateSchema(o).Enum(n.CreateListOfAny().Select(x => x.Node)).Build();
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new PatternFieldMap<OpenApiHeader>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
        };

        //private static readonly AnyFieldMap<OpenApiHeader> _headerAnyFields =
        //    new AnyFieldMap<OpenApiHeader>
        //    {
        //        {
        //            OpenApiConstants.Default,
        //            new AnyFieldMapParameter<OpenApiHeader>(
        //                p => p.Schema31?.GetDefault(),
        //                (p, v) => 
        //                {
        //                    if(p.Schema31 == null) return;
        //                    v = p.Schema31.GetDefault(); 
        //                },
        //                p => p.Schema31)
        //        }
        //    };

        //private static readonly AnyListFieldMap<OpenApiHeader> _headerAnyListFields =
        //    new AnyListFieldMap<OpenApiHeader>
        //    {
        //        {
        //            OpenApiConstants.Enum,
        //            new AnyListFieldMapParameter<OpenApiHeader>(
        //                p => p.Schema31?.GetEnum(),
        //                (p, v) =>
        //                {
        //                    if(p.Schema31 == null) return;
        //                    p.Schema31.Enum = v; 
        //                },
        //                p => p.Schema31)
        //        },
        //    };

        public static OpenApiHeader LoadHeader(ParseNode node)
        {
            var mapNode = node.CheckMapNode("header");
            var header = new OpenApiHeader();
            foreach (var property in mapNode)
            {
                property.ParseField(header, _headerFixedFields, _headerPatternFields);
            }

            var builder = new JsonSchemaBuilder();
            var schema = node.Context.GetFromTempStorage<JsonSchema>("schema");
            if (schema != null)
            {
                builder.Enum(node.CreateAny().Node);
                builder.Default(node.CreateAny().Node);
                schema = builder.Build();

                header.Schema31 = schema;
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
