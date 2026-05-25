// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;
using System.Globalization;

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
                (o, n, _, c) => o.Description = n.GetScalarValue()
            },
            {
                "type",
                (o, n, _, c) =>
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
                (o, n, _, c) => GetOrCreateSchema(o).Format = n.GetScalarValue()
            },
            {
                "items",
                (o, n, doc, c) => GetOrCreateSchema(o).Items = LoadSchema(n, doc, c)
            },
            {
                "collectionFormat",
                (o, n, _, c) =>
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
                (o, n, _, c) => GetOrCreateSchema(o).Default = n.CreateAny()
            },
            {
                "maximum",
                (o, n, _, c) =>
                {
                    var max = n.GetScalarValue();
                    if (!string.IsNullOrEmpty(max))
                    {
                        GetOrCreateSchema(o).Maximum = max;
                    }
                }
            },
            {
                "exclusiveMaximum",
                (o, n, _, c) => GetOrCreateSchema(o).IsExclusiveMaximum = bool.Parse(n.GetScalarValue()!)
            },
            {
                "minimum",
                (o, n, _, c) =>
                {
                    var min = n.GetScalarValue();
                    if (!string.IsNullOrEmpty(min))
                    {
                        GetOrCreateSchema(o).Minimum = min; 
                    } 
                }
            },
            {
                "exclusiveMinimum",
                (o, n, _, c) => GetOrCreateSchema(o).IsExclusiveMinimum = bool.Parse(n.GetScalarValue()!)
            },
            {
                "maxLength",
                (o, n, _, c) =>
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
                (o, n, _, c) =>
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
                (o, n, _, c) => GetOrCreateSchema(o).Pattern = n.GetScalarValue()
            },
            {
                "maxItems",
                (o, n, _, c) =>
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
                (o, n, _, c) =>
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
                (o, n, _, c) =>
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
                (o, n, _, c) =>
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
                (o, n, _, c) => GetOrCreateSchema(o).Enum = n.CreateListOfAny(c)
            }
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        private static OpenApiSchema GetOrCreateSchema(OpenApiHeader p)
        {
            return p.Schema switch {
                OpenApiSchema schema => schema,
                _ => (OpenApiSchema)(p.Schema = new OpenApiSchema()),
            };
        }

        public static IOpenApiHeader LoadHeader(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("header", context);
            var header = new OpenApiHeader();
            
            ParseMap(JsonObject, header, _headerFixedFields, _headerPatternFields, hostDocument, context);

            var schema = context.GetFromTempStorage<IOpenApiSchema>("schema");
            if (schema != null)
            {
                header.Schema = schema;
                context.SetTempStorage("schema", null);
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
