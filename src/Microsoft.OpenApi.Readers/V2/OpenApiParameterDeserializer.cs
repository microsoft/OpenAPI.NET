// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiParameter> ParameterFixedFields =
            new FixedFieldMap<OpenApiParameter>
            {
                {
                    "name", (o, n) =>
                    {
                        o.Name = n.GetScalarValue();
                    }
                },
                {
                    "in", (o, n) =>
                    {
                        ProcessIn(o, n);
                    }
                },
                {
                    "description", (o, n) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "required", (o, n) =>
                    {
                        o.Required = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "deprecated", (o, n) =>
                    {
                        o.Deprecated = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "allowEmptyValue", (o, n) =>
                    {
                        o.AllowEmptyValue = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "example", (o, n) =>
                    {
                        o.Example = n.GetScalarValue();
                    }
                },
                {
                    "type", (o, n) =>
                    {
                        GetOrCreateSchema(o).Type = n.GetScalarValue();
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
                    "format", (o, n) =>
                    {
                        GetOrCreateSchema(o).Format = n.GetScalarValue();
                    }
                },
                {
                    "minimum", (o, n) =>
                    {
                        GetOrCreateSchema(o).Minimum = decimal.Parse(n.GetScalarValue());
                    }
                },
                {
                    "maximum", (o, n) =>
                    {
                        GetOrCreateSchema(o).Maximum = decimal.Parse(n.GetScalarValue());
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
                    "readOnly", (o, n) =>
                    {
                        GetOrCreateSchema(o).ReadOnly = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "default", (o, n) =>
                    {
                        GetOrCreateSchema(o).Default = new OpenApiString(n.GetScalarValue());
                    }
                },
                {
                    "pattern", (o, n) =>
                    {
                        GetOrCreateSchema(o).Pattern = n.GetScalarValue();
                    }
                },
                {
                    "enum", (o, n) =>
                    {
                        GetOrCreateSchema(o).Enum =
                            n.CreateSimpleList<IOpenApiAny>(l => new OpenApiString(l.GetScalarValue()));
                    }
                },
                {
                    "schema", (o, n) =>
                    {
                        o.Schema = LoadSchema(n);
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiParameter> ParameterPatternFields =
            new PatternFieldMap<OpenApiParameter>
            {
                {s => s.StartsWith("x-"), (o, k, n) => o.Extensions.Add(k, new OpenApiString(n.GetScalarValue()))},
            };

        private static void LoadStyle(OpenApiParameter p, string v)
        {
            switch (v)
            {
                case "csv":
                    p.Style = "simple";
                    return;
                case "ssv":
                    p.Style = "spaceDelimited";
                    return;
                case "pipes":
                    p.Style = "pipeDelimited";
                    return;
                case "tsv":
                    throw new NotSupportedException();
                case "multi":
                    p.Style = "form";
                    p.Explode = true;
                    return;
            }
        }

        private static OpenApiSchema GetOrCreateSchema(OpenApiParameter p)
        {
            if (p.Schema == null)
            {
                p.Schema = new OpenApiSchema();
            }
            return p.Schema;
        }

        private static OpenApiSchema GetOrCreateSchema(OpenApiHeader p)
        {
            if (p.Schema == null)
            {
                p.Schema = new OpenApiSchema();
            }
            return p.Schema;
        }

        private static void ProcessIn(OpenApiParameter o, ParseNode n)
        {
            var value = n.GetScalarValue();
            switch (value)
            {
                case "body":
                    n.Context.SetTempStorage("bodyParameter", o);
                    break;
                case "form":
                    var formParameters = n.Context.GetTempStorage<List<OpenApiParameter>>("formParameters");
                    if (formParameters == null)
                    {
                        formParameters = new List<OpenApiParameter>();
                        n.Context.SetTempStorage("formParameters", formParameters);
                    }
                    formParameters.Add(o);
                    break;
                default:
                    o.In = (ParameterLocation)Enum.Parse(typeof(ParameterLocation), value);
                    break;
            }
        }

        public static OpenApiParameter LoadParameter(ParseNode node)
        {
            var mapNode = node.CheckMapNode("parameter");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiParameter>(refpointer);
            }

            var parameter = new OpenApiParameter();

            ParseMap(mapNode, parameter, ParameterFixedFields, ParameterPatternFields);

            var schema = node.Context.GetTempStorage<OpenApiSchema>("schema");
            if (schema != null)
            {
                parameter.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            if (parameter.In == 0)
            {
                return null; // Don't include Form or Body parameters in OpenApiOperation.Parameters list
            }

            return parameter;
        }
    }
}