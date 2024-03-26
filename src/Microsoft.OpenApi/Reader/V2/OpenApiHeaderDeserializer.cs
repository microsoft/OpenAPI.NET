// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using Json.Schema;
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
        private static JsonSchemaBuilder _headerJsonSchemaBuilder;
        private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new()
        {
            {
                "description",
                (o, n) => o.Description = n.GetScalarValue()
            },
            {
                "type", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().Type(SchemaTypeConverter.ConvertToSchemaValueType(n.GetScalarValue()));
                }
            },
            {
                "format", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().Format(n.GetScalarValue());
                }
            },
            {
                "items", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().Items(LoadSchema(n));
                }
            },
            {
                "collectionFormat",
                (o, n) => LoadStyle(o, n.GetScalarValue())
            },
            {
                "default", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().Default(n.CreateAny().Node);
                }
            },
            {
                "maximum", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().Maximum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "exclusiveMaximum", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().ExclusiveMaximum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "minimum", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().Minimum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "exclusiveMinimum", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().ExclusiveMinimum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "maxLength", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().MaxLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "minLength", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().MinLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "pattern", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().Pattern(n.GetScalarValue());
                }
            },
            {
                "maxItems", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().MaxItems(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "minItems", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().MinItems(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "uniqueItems", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().UniqueItems(bool.Parse(n.GetScalarValue()));
                }
            },
            {
                "multipleOf", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().MultipleOf(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                }
            },
            {
                "enum", (o, n) =>
                {
                    o.Schema = GetOrCreateHeaderSchemaBuilder().Enum(n.CreateListOfAny()).Build();
                }
            }            
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
        };

        private static JsonSchemaBuilder GetOrCreateHeaderSchemaBuilder()
        {
            _headerJsonSchemaBuilder ??= new JsonSchemaBuilder();
            return _headerJsonSchemaBuilder;
        }

        public static OpenApiHeader LoadHeader(ParseNode node)
        {
            var mapNode = node.CheckMapNode("header");
            var header = new OpenApiHeader();
            _headerJsonSchemaBuilder = null;
            
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
