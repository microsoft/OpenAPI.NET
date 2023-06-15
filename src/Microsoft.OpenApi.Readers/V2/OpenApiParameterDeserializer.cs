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
                        o.Schema31 = builder.Type(SchemaTypeConverter.ConvertToSchemaValueType(n.GetScalarValue()));
                    }
                },
                {
                    "items", (o, n) =>
                    {
                        o.Schema31 = builder.Items(LoadSchema(n));
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
                        o.Schema31 = builder.Format(n.GetScalarValue());
                    }
                },
                {
                    "minimum", (o, n) =>
                    {
                        o.Schema31 = builder.Minimum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                    }
                },
                {
                    "maximum", (o, n) =>
                    {
                        o.Schema31 = builder.Maximum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                    }
                },
                {
                    "maxLength", (o, n) =>
                    {
                        o.Schema31 = builder.MaxLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                    }
                },
                {
                    "minLength", (o, n) =>
                    {
                        o.Schema31 = builder.MinLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                    }
                },
                {
                    "readOnly", (o, n) =>
                    {
                        o.Schema31 = builder.ReadOnly(bool.Parse(n.GetScalarValue()));
                    }
                },
                {
                    "default", (o, n) =>
                    {
                        o.Schema31 = builder.Default(n.CreateAny().Node);
                    }
                },
                {
                    "pattern", (o, n) =>
                    {
                        o.Schema31 = builder.Pattern(n.GetScalarValue());
                    }
                },
                {
                    "enum", (o, n) =>
                    {
                        o.Schema31 = builder.Enum(n.CreateListOfAny());
                    }
                },
                {
                    "schema", (o, n) =>
                    {
                        o.Schema31 = LoadSchema(n);
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
                        p => new OpenApiAny(p.Schema31?.GetDefault()),
                        (p, v) => {
                            if (p.Schema31 != null || v != null)
                            {
                                p.Schema31 = builder.Default(v.Node);
                            }
                        },
                        p => p.Schema31)
                }
            };

        private static readonly AnyListFieldMap<OpenApiParameter> _parameterAnyListFields =
            new AnyListFieldMap<OpenApiParameter>
            {
                {
                    OpenApiConstants.Enum,
                    new AnyListFieldMapParameter<OpenApiParameter>(
                        p => p.Schema31?.GetEnum().ToList(),
                        (p, v) => {
                            if (p.Schema31 != null || v != null && v.Count > 0)
                            {
                                p.Schema31 = builder.Enum(v);
                            }
                        },
                        p => p.Schema31)
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

        private static JsonSchema GetOrCreateSchema(OpenApiParameter p)
        {
            p.Schema31 ??= JsonSchema.Empty;
            return p.Schema31;
        }

        private static JsonSchemaBuilder GetOrCreateSchema(OpenApiHeader p)
        {
            p.Schema31 ??= JsonSchema.Empty;
           
            return new JsonSchemaBuilder();
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
            //ProcessAnyFields(mapNode, parameter, _parameterAnyFields);
            //ProcessAnyListFields(mapNode, parameter, _parameterAnyListFields);

            var schema = node.Context.GetFromTempStorage<JsonSchema>("schema");
            if (schema != null)
            {
                parameter.Schema31 = schema;
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
