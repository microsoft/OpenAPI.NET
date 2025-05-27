// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
                    (o, n, t) => o.Name = n.GetScalarValue()
                },
                {
                    "in",
                    ProcessIn
                },
                {
                    "description",
                    (o, n, t) => o.Description = n.GetScalarValue()
                },
                {
                    "required",
                    (o, n, t) =>
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
                    (o, n, t) =>
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
                    (o, n, t) =>
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
                    (o, n, t) =>
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
                    (o, n, t) => GetOrCreateSchema(o).Items = LoadSchema(n, t)
                },
                {
                    "collectionFormat",
                    (o, n, t) =>
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
                    (o, n, t) => GetOrCreateSchema(o).Format = n.GetScalarValue()
                },
                {
                    "minimum",
                    (o, n, t) =>
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
                    (o, n, t) =>
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
                    (o, n, t) =>
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
                    (o, n, t) =>
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
                    (o, n, t) =>
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
                    (o, n, t) => GetOrCreateSchema(o).Default = n.CreateAny()
                },
                {
                    "pattern",
                    (o, n, t) => GetOrCreateSchema(o).Pattern = n.GetScalarValue()
                },
                {
                    "enum",
                    (o, n, t) => GetOrCreateSchema(o).Enum = n.CreateListOfAny()
                },
                {
                    "schema",
                    (o, n, t) => o.Schema = LoadSchema(n, t)
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
                    (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))} 
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

        private static void LoadParameterExamplesExtension(OpenApiParameter parameter, ParseNode node, OpenApiDocument? hostDocument)
        {
            var examples = LoadExamplesExtension(node);
            node.Context.SetTempStorage(TempStorageKeys.Examples, examples, parameter);
        }

        private static OpenApiSchema GetOrCreateSchema(OpenApiParameter p)
        {
            return p.Schema switch {
                OpenApiSchema schema => schema,
                _ => (OpenApiSchema)(p.Schema = new OpenApiSchema()),
            };
        }

        private static void ProcessIn(OpenApiParameter o, ParseNode n, OpenApiDocument hostDocument)
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

        public static IOpenApiParameter? LoadParameter(ParseNode node, OpenApiDocument hostDocument)
        {
            return LoadParameter(node, false, hostDocument);
        }

        public static IOpenApiParameter? LoadParameter(ParseNode node, bool loadRequestBody, OpenApiDocument hostDocument)
        {
            // Reset the local variables every time this method is called.
            node.Context.SetTempStorage(TempStorageKeys.ParameterIsBodyOrFormData, false);

            var mapNode = node.CheckMapNode("parameter");

            var pointer = mapNode.GetReferencePointer();

            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiParameterReference(reference.Item1, hostDocument, reference.Item2);
            }

            var parameter = new OpenApiParameter();

            ParseMap(mapNode, parameter, _parameterFixedFields, _parameterPatternFields, doc: hostDocument);

            var schema = node.Context.GetFromTempStorage<IOpenApiSchema>("schema");
            if (schema != null)
            {
                parameter.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            // load examples from storage and add them to the parameter
            var examples = node.Context.GetFromTempStorage<Dictionary<string, IOpenApiExample>>(TempStorageKeys.Examples, parameter);
            if (examples != null)
            {
                parameter.Examples = examples;
                node.Context.SetTempStorage("examples", null);
            }

            var isBodyOrFormData = false;
            var paramData = node.Context.GetFromTempStorage<object>(TempStorageKeys.ParameterIsBodyOrFormData);
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
