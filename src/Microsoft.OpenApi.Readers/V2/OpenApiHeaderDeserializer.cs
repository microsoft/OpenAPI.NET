// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using System.Linq;
using Json.Schema;
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
                    o.Schema = GetOrCreateSchemaBuilder(o).Type(SchemaTypeConverter.ConvertToSchemaValueType(n.GetScalarValue()));
                }
            },
            {
                "format", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).Format(n.GetScalarValue());
                }
            },
            {
                "items", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).Items(LoadSchema(n));
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
                    o.Schema = GetOrCreateSchemaBuilder(o).Default(n.CreateAny().Node);
                }
            },
            {
                "maximum", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).Maximum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "exclusiveMaximum", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).ExclusiveMaximum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "minimum", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).Minimum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "exclusiveMinimum", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).ExclusiveMinimum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "maxLength", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).MaxLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "minLength", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).MinLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "pattern", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).Pattern(n.GetScalarValue());
                }
            },
            {
                "maxItems", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).MaxItems(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "minItems", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).MinItems(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "uniqueItems", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).UniqueItems(bool.Parse(n.GetScalarValue()));
                }
            },
            {
                "multipleOf", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).MultipleOf(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "enum", (o, n) =>
                {
                    o.Schema = GetOrCreateSchemaBuilder(o).Enum(n.CreateListOfAny()).Build();
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

            var schema = node.Context.GetFromTempStorage<JsonSchemaBuilder>("schema");
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
