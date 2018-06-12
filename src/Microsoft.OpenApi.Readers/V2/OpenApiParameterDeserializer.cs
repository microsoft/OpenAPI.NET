// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
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
        private static ParameterLocation? _in;

        private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields =
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
                        o.Example = n.CreateAny();
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
                        GetOrCreateSchema(o).Default = n.CreateAny();
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
                        GetOrCreateSchema(o).Enum = n.CreateListOfAny();
                    }
                },
                {
                    "schema", (o, n) =>
                    {
                        o.Schema = LoadSchema(n);
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields =
            new PatternFieldMap<OpenApiParameter>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
            };

        private static void LoadStyle(OpenApiParameter p, string v)
        {
            switch (v)
            {
                case "csv":
                    if (p.In == ParameterLocation.Query)
                    {
                        p.Style = ParameterStyle.Form;
                    }
                    else
                    {
                        p.Style = ParameterStyle.Simple;
                    }
                    return;
                case "ssv":
                    p.Style = ParameterStyle.SpaceDelimited;
                    return;
                case "pipes":
                    p.Style = ParameterStyle.PipeDelimited;
                    return;
                case "tsv":
                    throw new NotSupportedException();
                case "multi":
                    p.Style = ParameterStyle.Form;
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
                    n.Context.SetTempStorage(TempStorageKeys.BodyParameter, o);
                    break;
                case "formData":
                    var formParameters = n.Context.GetFromTempStorage<List<OpenApiParameter>>("formParameters");
                    if (formParameters == null)
                    {
                        formParameters = new List<OpenApiParameter>();
                        n.Context.SetTempStorage("formParameters", formParameters);
                    }

                    formParameters.Add(o);
                    break;
                default:
                    _in = value.GetEnumFromDisplayName<ParameterLocation>();
                    o.In = _in;
                    break;
            }
        }

        public static OpenApiParameter LoadParameter(ParseNode node)
        {
            return LoadParameter(node, false);
        }

        public static OpenApiParameter LoadParameter(ParseNode node, bool evenBody)
        {
            // Reset the local variables every time this method is called.
            _in = null;

            var mapNode = node.CheckMapNode("parameter");

            var pointer = mapNode.GetReferencePointer();

            if (pointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiParameter>(ReferenceType.Parameter, pointer);
            }
            
            var parameter = new OpenApiParameter();

            ParseMap(mapNode, parameter, _parameterFixedFields, _parameterPatternFields);

            var schema = node.Context.GetFromTempStorage<OpenApiSchema>("schema");
            if (schema != null)
            {
                parameter.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            if (_in == null && !evenBody)
            {
                return null; // Don't include Form or Body parameters in OpenApiOperation.Parameters list
            }

            return parameter;
        }
    }
}