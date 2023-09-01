// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Json.Schema;
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
        private static readonly JsonSchemaBuilder builder = new JsonSchemaBuilder();
        private static JsonSchemaBuilder s_HeaderJsonSchemaBuilder;
        private static JsonSchemaBuilder s_ParameterJsonSchemaBuilder;
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
                    "type", (o, n) =>
                    {
                        o.Schema = GetOrCreateSchemaBuilder(o).Type(SchemaTypeConverter.ConvertToSchemaValueType(n.GetScalarValue()));
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
                    "format", (o, n) =>
                    {
                        o.Schema = GetOrCreateSchemaBuilder(o).Format(n.GetScalarValue());
                    }
                },
                {
                    "minimum", (o, n) =>
                    {
                        o.Schema = GetOrCreateSchemaBuilder(o).Minimum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                    }
                },
                {
                    "maximum", (o, n) =>
                    {
                        o.Schema = GetOrCreateSchemaBuilder(o).Maximum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
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
                    "readOnly", (o, n) =>
                    {
                        o.Schema = GetOrCreateSchemaBuilder(o).ReadOnly(bool.Parse(n.GetScalarValue()));
                    }
                },
                {
                    "default", (o, n) =>
                    {
                        o.Schema = GetOrCreateSchemaBuilder(o).Default(n.CreateAny().Node);
                    }
                },
                {
                    "pattern", (o, n) =>
                    {
                        o.Schema = GetOrCreateSchemaBuilder(o).Pattern(n.GetScalarValue());
                    }
                },
                {
                    "enum", (o, n) =>
                    {
                        o.Schema = GetOrCreateSchemaBuilder(o).Enum(n.CreateListOfAny()).Build();
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

        private static readonly AnyFieldMap<OpenApiParameter> _parameterAnyFields =
            new AnyFieldMap<OpenApiParameter>
            {
                {
                    OpenApiConstants.Default,
                    new AnyFieldMapParameter<OpenApiParameter>(
                        p => new OpenApiAny(p.Schema?.GetDefault()),
                        (p, v) => {
                            if (p.Schema != null || v != null)
                            {
                                p.Schema = GetOrCreateSchemaBuilder(p).Default(v.Node);
                            }
                        },
                        p => p.Schema)
                }
            };

        private static readonly AnyListFieldMap<OpenApiParameter> _parameterAnyListFields =
            new AnyListFieldMap<OpenApiParameter>
            {
                {
                    OpenApiConstants.Enum,
                    new AnyListFieldMapParameter<OpenApiParameter>(
                        p => p.Schema?.GetEnum().ToList(),
                        (p, v) => {
                            if (p.Schema != null || v != null && v.Count > 0)
                            {
                                p.Schema = GetOrCreateSchemaBuilder(p).Enum(v);
                            }
                        },
                        p => p.Schema)
                },
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
        private static JsonSchemaBuilder GetOrCreateSchemaBuilder(OpenApiParameter p)
        {
            s_ParameterJsonSchemaBuilder ??= new JsonSchemaBuilder();
            return s_ParameterJsonSchemaBuilder;
        }

        private static JsonSchemaBuilder GetOrCreateSchemaBuilder(OpenApiHeader p)
        {
            s_HeaderJsonSchemaBuilder ??= new JsonSchemaBuilder();
            return s_HeaderJsonSchemaBuilder;
        }        

        private static void ProcessIn(OpenApiParameter o, ParseNode n)
        {
            var value = n.GetScalarValue();
            switch (value)
            {
                case "body":
                    n.Context.SetTempStorage(TempStorageKeys.ParameterIsBodyOrFormData, true);
                    n.Context.SetTempStorage(TempStorageKeys.BodyParameter, o);
                    break;
                case "formData":
                    n.Context.SetTempStorage(TempStorageKeys.ParameterIsBodyOrFormData, true);
                    var formParameters = n.Context.GetFromTempStorage<List<OpenApiParameter>>("formParameters");
                    if (formParameters == null)
                    {
                        formParameters = new List<OpenApiParameter>();
                        n.Context.SetTempStorage("formParameters", formParameters);
                    }

                    formParameters.Add(o);
                    break;
                case "query":
                case "header":
                case "path":
                    o.In = value.GetEnumFromDisplayName<ParameterLocation>();
                    break;
                default:
                    o.In = null;
                    break;
            }
        }

        public static OpenApiParameter LoadParameter(ParseNode node)
        {
            return LoadParameter(node, false);
        }

        public static OpenApiParameter LoadParameter(ParseNode node, bool loadRequestBody)
        {
            // Reset the local variables every time this method is called.
            node.Context.SetTempStorage(TempStorageKeys.ParameterIsBodyOrFormData, false);

            var mapNode = node.CheckMapNode("parameter");

            var pointer = mapNode.GetReferencePointer();

            if (pointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiParameter>(ReferenceType.Parameter, pointer);
            }

            var parameter = new OpenApiParameter();

            ParseMap(mapNode, parameter, _parameterFixedFields, _parameterPatternFields);

            var schema = node.Context.GetFromTempStorage<JsonSchema>("schema");
            if (schema != null)
            {
                parameter.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            bool isBodyOrFormData = (bool)node.Context.GetFromTempStorage<object>(TempStorageKeys.ParameterIsBodyOrFormData);
            if (isBodyOrFormData && !loadRequestBody)
            {
                return null; // Don't include Form or Body parameters when normal parameters are loaded.
            }

            if (loadRequestBody && !isBodyOrFormData)
            {
                return null; // Don't include non-Body or non-Form parameters when request bodies are loaded.
            }

            return parameter;
        }
    }
}
