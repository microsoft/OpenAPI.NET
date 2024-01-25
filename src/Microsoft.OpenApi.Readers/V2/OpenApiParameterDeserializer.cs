// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields =
            new()
            {
                {
                    "name",
                    (o, n) => o.Name = n.GetScalarValue()
                },
                {
                    "in",
                    ProcessIn
                },
                {
                    "description",
                    (o, n) => o.Description = n.GetScalarValue()
                },
                {
                    "required",
                    (o, n) => o.Required = bool.Parse(n.GetScalarValue())
                },
                {
                    "deprecated",
                    (o, n) => o.Deprecated = bool.Parse(n.GetScalarValue())
                },
                {
                    "allowEmptyValue",
                    (o, n) => o.AllowEmptyValue = bool.Parse(n.GetScalarValue())
                },
                {
                    "type",
                    (o, n) => GetOrCreateSchema(o).Type = n.GetScalarValue()
                },
                {
                    "items",
                    (o, n) => GetOrCreateSchema(o).Items = LoadSchema(n)
                },
                {
                    "collectionFormat",
                    (o, n) => LoadStyle(o, n.GetScalarValue())
                },
                {
                    "format",
                    (o, n) => GetOrCreateSchema(o).Format = n.GetScalarValue()
                },
                {
                    "minimum",
                    (o, n) => GetOrCreateSchema(o).Minimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MinValue)
                },
                {
                    "maximum",
                    (o, n) => GetOrCreateSchema(o).Maximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MaxValue)
                },
                {
                    "maxLength",
                    (o, n) => GetOrCreateSchema(o).MaxLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
                },
                {
                    "minLength",
                    (o, n) => GetOrCreateSchema(o).MinLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture)
                },
                {
                    "readOnly",
                    (o, n) => GetOrCreateSchema(o).ReadOnly = bool.Parse(n.GetScalarValue())
                },
                {
                    "default",
                    (o, n) => GetOrCreateSchema(o).Default = n.CreateAny()
                },
                {
                    "pattern",
                    (o, n) => GetOrCreateSchema(o).Pattern = n.GetScalarValue()
                },
                {
                    "enum",
                    (o, n) => GetOrCreateSchema(o).Enum = n.CreateListOfAny()
                },
                {
                    "schema",
                    (o, n) => o.Schema = LoadSchema(n)
                },
                {
                    "x-examples",
                    LoadParameterExamplesExtension
                },
            };

        private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields =
            new()
            {
                {s => s.StartsWith("x-") && !s.Equals(OpenApiConstants.ExamplesExtension, StringComparison.OrdinalIgnoreCase),
                    (o, p, n) => o.AddExtension(p, LoadExtension(p, n))} 
            };

        private static readonly AnyFieldMap<OpenApiParameter> _parameterAnyFields =
            new()
            {
                {
                    OpenApiConstants.Default,
                    new(
                        p => p.Schema?.Default,
                        (p, v) => {
                            if (p.Schema != null || v != null)
                            {
                                GetOrCreateSchema(p).Default = v;
                            }
                        },
                        p => p.Schema)
                }
            };

        private static readonly AnyListFieldMap<OpenApiParameter> _parameterAnyListFields =
            new()
            {
                {
                    OpenApiConstants.Enum,
                    new(
                        p => p.Schema?.Enum,
                        (p, v) => {
                            if (p.Schema != null || v is {Count: > 0})
                            {
                                GetOrCreateSchema(p).Enum = v;
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

        private static void LoadParameterExamplesExtension(OpenApiParameter parameter, ParseNode node)
        {
            var examples = LoadExamplesExtension(node);
            node.Context.SetTempStorage(TempStorageKeys.Examples, examples, parameter);
        }

        private static OpenApiSchema GetOrCreateSchema(OpenApiParameter p)
        {
            if (p.Schema == null)
            {
                p.Schema = new();
            }

            return p.Schema;
        }

        private static OpenApiSchema GetOrCreateSchema(OpenApiHeader p)
        {
            if (p.Schema == null)
            {
                p.Schema = new();
            }

            return p.Schema;
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
                        formParameters = new();
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

            ProcessAnyFields(mapNode, parameter, _parameterAnyFields);
            ProcessAnyListFields(mapNode, parameter, _parameterAnyListFields);

            var schema = node.Context.GetFromTempStorage<OpenApiSchema>("schema");
            if (schema != null)
            {
                parameter.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            // load examples from storage and add them to the parameter
            var examples = node.Context.GetFromTempStorage<Dictionary<string, OpenApiExample>>(TempStorageKeys.Examples, parameter);
            if (examples != null)
            {
                parameter.Examples = examples;
                node.Context.SetTempStorage("examples", null);
            }

            var isBodyOrFormData = (bool)node.Context.GetFromTempStorage<object>(TempStorageKeys.ParameterIsBodyOrFormData);
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
