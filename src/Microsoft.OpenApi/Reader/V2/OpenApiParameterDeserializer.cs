// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.OpenApi.Reader.V2
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
                    (o, n, t, c) => o.Name = n.GetScalarValue()
                },
                {
                    "in",
                    ProcessIn
                },
                {
                    "description",
                    (o, n, t, c) => o.Description = n.GetScalarValue()
                },
                {
                    "required",
                    (o, n, t, c) =>
                    {
                        var required = n.GetScalarValue();
                        if (required != null)
                        {
                            o.Required = bool.Parse(required);
                        }
                    }
                },
                {
                    "deprecated",
                    (o, n, t, c) =>
                    {
                        var deprecated = n.GetScalarValue();
                        if (deprecated != null)
                        {
                            o.Deprecated = bool.Parse(deprecated);
                        }
                    }
                },
                {
                    "allowEmptyValue",
                    (o, n, t, c) =>
                    {
                        var allowEmptyValue = n.GetScalarValue();
                        if (allowEmptyValue != null)
                        {
                            o.AllowEmptyValue = bool.Parse(allowEmptyValue);
                        }
                    }
                },
                {
                    "type",
                    (o, n, t, c) =>
                    {
                        var type = n.GetScalarValue();
                        if (type != null)
                        {                            
                            var schema = GetOrCreateSchema(o);
                            schema.Type = type.ToJsonSchemaType();
                            if ("file".Equals(type, StringComparison.OrdinalIgnoreCase))
                            {
                                schema.Format = "binary";
                            }
                        }
                    }
                },
                {
                    "items",
                    (o, n, t, c) => GetOrCreateSchema(o).Items = LoadSchema(n, t, c)
                },
                {
                    "collectionFormat",
                    (o, n, t, c) =>
                    {
                        var collectionFormat = n.GetScalarValue();
                        if (collectionFormat != null)
                        {
                            LoadStyle(o, collectionFormat);
                        }
                    }
                },
                {
                    "format",
                    (o, n, t, c) => GetOrCreateSchema(o).Format = n.GetScalarValue()
                },
                {
                    "minimum",
                    (o, n, t, c) =>
                    {
                        var min = n.GetScalarValue();
                        if (!string.IsNullOrEmpty(min))
                        {
                            GetOrCreateSchema(o).Minimum = min;
                        }
                    }
                },
                {
                    "maximum",
                    (o, n, t, c) =>
                    {
                        var max = n.GetScalarValue();
                        if (!string.IsNullOrEmpty(max))
                        {
                            GetOrCreateSchema(o).Maximum = max;
                        }
                    }
                },
                {
                    "maxLength",
                    (o, n, t, c) =>
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
                    (o, n, t, c) =>
                    {
                        var minLength = n.GetScalarValue();
                        if (minLength != null)
                        {
                            GetOrCreateSchema(o).MinLength = int.Parse(minLength, CultureInfo.InvariantCulture);
                        }
                    }
                },
                {
                    "readOnly",
                    (o, n, t, c) =>
                    {
                        var readOnly = n.GetScalarValue();
                        if (readOnly != null)
                        {
                            GetOrCreateSchema(o).ReadOnly = bool.Parse(readOnly);
                        }
                    }
                },
                {
                    "default",
                    (o, n, t, c) => GetOrCreateSchema(o).Default = n
                },
                {
                    "pattern",
                    (o, n, t, c) => GetOrCreateSchema(o).Pattern = n.GetScalarValue()
                },
                {
                    "enum",
                    (o, n, t, c) => GetOrCreateSchema(o).Enum = n.CreateListOfAny(c)
                },
                {
                    "schema",
                    (o, n, t, c) => o.Schema = LoadSchema(n, t, c)
                },
                {
                    "x-examples",
                    LoadParameterExamplesExtension
                },
            };

        private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase) && !s.Equals(OpenApiConstants.ExamplesExtension, StringComparison.OrdinalIgnoreCase),
                    (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))} 
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

        private static void LoadParameterExamplesExtension(OpenApiParameter parameter, JsonNode node, OpenApiDocument? hostDocument, ParsingContext context)
        {
            var examples = LoadExamplesExtension(node, context);
            context.SetTempStorage(TempStorageKeys.Examples, examples, parameter);
        }

        private static OpenApiSchema GetOrCreateSchema(OpenApiParameter p)
        {
            return p.Schema switch {
                OpenApiSchema schema => schema,
                _ => (OpenApiSchema)(p.Schema = new OpenApiSchema()),
            };
        }

        private static void ProcessIn(OpenApiParameter o, JsonNode n, OpenApiDocument hostDocument, ParsingContext context)
        {
            var value = n.GetScalarValue();
            switch (value)
            {
                case "body":
                    context.SetTempStorage(TempStorageKeys.ParameterIsBodyOrFormData, true);
                    context.SetTempStorage(TempStorageKeys.BodyParameter, o);
                    break;
                case "formData":
                    context.SetTempStorage(TempStorageKeys.ParameterIsBodyOrFormData, true);
                    var formParameters = context.GetFromTempStorage<List<OpenApiParameter>>("formParameters");
                    if (formParameters == null)
                    {
                        formParameters = new();
                        context.SetTempStorage("formParameters", formParameters);
                    }

                    formParameters.Add(o);
                    break;
                case "query":
                case "header":
                case "path":
                    if (value.TryGetEnumFromDisplayName<ParameterLocation>(out var _in))
                    {
                        o.In = _in;
                    }
                    break;
                default:
                    o.In = null;
                    break;
            }
        }

        public static IOpenApiParameter? LoadParameter(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            return LoadParameter(node, false, hostDocument, context);
        }

        public static IOpenApiParameter? LoadParameter(JsonNode node, bool loadRequestBody, OpenApiDocument hostDocument, ParsingContext context)
        {
            // Reset the local variables every time this method is called.
            context.SetTempStorage(TempStorageKeys.ParameterIsBodyOrFormData, false);

            var jsonObject = node.CheckMapNode("parameter", context);

            var pointer = jsonObject.GetReferencePointer();

            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiParameterReference(reference.Item1, hostDocument, reference.Item2);
            }

            var parameter = new OpenApiParameter();

            ParseMap(jsonObject, parameter, _parameterFixedFields, _parameterPatternFields, hostDocument, context);

            var schema = context.GetFromTempStorage<IOpenApiSchema>("schema");
            if (schema != null)
            {
                parameter.Schema = schema;
                context.SetTempStorage("schema", null);
            }

            // load examples from storage and add them to the parameter
            var examples = context.GetFromTempStorage<Dictionary<string, IOpenApiExample>>(TempStorageKeys.Examples, parameter);
            if (examples != null)
            {
                parameter.Examples = examples;
                context.SetTempStorage("examples", null);
            }

            var isBodyOrFormData = false;
            var paramData = context.GetFromTempStorage<object>(TempStorageKeys.ParameterIsBodyOrFormData);
            if (paramData is bool boolValue)
            {
                isBodyOrFormData = boolValue;
            }

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
