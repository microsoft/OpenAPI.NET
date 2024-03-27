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
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static JsonSchemaBuilder _parameterJsonSchemaBuilder;
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
                    "type", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().Type(SchemaTypeConverter.ConvertToSchemaValueType(n.GetScalarValue()));
                    }
                },
                {
                    "items", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().Items(LoadSchema(n));
                    }
                },
                {
                    "collectionFormat",
                    (o, n) => LoadStyle(o, n.GetScalarValue())
                },
                {
                    "format", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().Format(n.GetScalarValue());
                    }
                },
                {
                    "minimum", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().Minimum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                    }
                },
                {
                    "maximum", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().Maximum(decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                    }
                },
                {
                    "maxLength", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().MaxLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                    }
                },
                {
                    "minLength", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().MinLength(uint.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture));
                    }
                },
                {
                    "readOnly", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().ReadOnly(bool.Parse(n.GetScalarValue()));
                    }
                },
                {
                    "default", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().Default(n.CreateAny().Node);
                    }
                },
                {
                    "pattern", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().Pattern(n.GetScalarValue());
                    }
                },
                {
                    "enum", (o, n) =>
                    {
                        o.Schema = GetOrCreateParameterSchemaBuilder().Enum(n.CreateListOfAny()).Build();
                    }
                },
                {
                    "schema",
                    (o, n) => o.Schema = LoadSchema(n)
                },
            };

        private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields =
            new()
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
        
        private static JsonSchemaBuilder GetOrCreateParameterSchemaBuilder()
        {
            _parameterJsonSchemaBuilder ??= new JsonSchemaBuilder();
            return _parameterJsonSchemaBuilder;
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

        public static OpenApiParameter LoadParameter(ParseNode node, OpenApiDocument hostDocument = null)
        {
            return LoadParameter(node, false, hostDocument);
        }

        public static OpenApiParameter LoadParameter(ParseNode node, bool loadRequestBody, OpenApiDocument hostDocument)
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
            _parameterJsonSchemaBuilder = null;

            ParseMap(mapNode, parameter, _parameterFixedFields, _parameterPatternFields);

            var schema = node.Context.GetFromTempStorage<JsonSchema>("schema");
            if (schema != null)
            {
                parameter.Schema = schema;
                node.Context.SetTempStorage("schema", null);
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
