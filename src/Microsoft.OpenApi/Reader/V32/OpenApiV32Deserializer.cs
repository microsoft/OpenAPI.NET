// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        private static void ParseMap<T>(
            JsonObject? jsonObject,
            T domainObject,
            FixedFieldMap<T> fixedFieldMap,            
            PatternFieldMap<T> patternFieldMap, 
            OpenApiDocument doc,
            ParsingContext context)
        {
            jsonObject.ParseMap(domainObject, fixedFieldMap, patternFieldMap, doc, context);
        }

        private static void ProcessAnyFields<T>(
            T domainObject,
            AnyFieldMap<T> anyFieldMap,
            ParsingContext context)
        {
            foreach (var anyFieldName in anyFieldMap.Keys.ToList())
            {
                try
                {
                    context.StartObject(anyFieldName);

                    var any = anyFieldMap[anyFieldName].PropertyGetter(domainObject);

                    if (any == null)
                    {
                        anyFieldMap[anyFieldName].PropertySetter(domainObject, null);
                    }
                    else
                    {
                        anyFieldMap[anyFieldName].PropertySetter(domainObject, any);
                    }
                }
                catch (OpenApiException exception)
                {
                    exception.Pointer = context.GetLocation();
                    context.Diagnostic.Errors.Add(new OpenApiError(exception));
                }
                finally
                {
                    context.EndObject();
                }
            }
        }

        private static void ProcessAnyMapFields<T, U>(
            T domainObject,
            AnyMapFieldMap<T, U> anyMapFieldMap,
            ParsingContext context)
        {
            foreach (var anyMapFieldName in anyMapFieldMap.Keys.ToList())
            {
                try
                {
                    context.StartObject(anyMapFieldName);
                    var propertyMapGetter = anyMapFieldMap[anyMapFieldName].PropertyMapGetter(domainObject);
                    if (propertyMapGetter != null)
                    {
                        foreach (var propertyMapElement in propertyMapGetter)
                        {
                            context.StartObject(propertyMapElement.Key);

                            if (propertyMapElement.Value != null)
                            {
                                var any = anyMapFieldMap[anyMapFieldName].PropertyGetter(propertyMapElement.Value);
                                if (any is not null)
                                {
                                    anyMapFieldMap[anyMapFieldName].PropertySetter(propertyMapElement.Value, any);
                                }
                            }
                        }
                    }
                }
                catch (OpenApiException exception)
                {
                    exception.Pointer = context.GetLocation();
                    context.Diagnostic.Errors.Add(new OpenApiError(exception));
                }
                finally
                {
                    context.EndObject();
                }
            }
        }

        private static RuntimeExpressionAnyWrapper LoadRuntimeExpressionAnyWrapper(JsonNode node)
        {
            var value = node.GetScalarValue();

            if (value != null && value.StartsWith("$", StringComparison.OrdinalIgnoreCase))
            {
                return new RuntimeExpressionAnyWrapper
                {
                    Expression = RuntimeExpression.Build(value)
                };
            }

            return new RuntimeExpressionAnyWrapper
            {
                Any = node
            };
        }

        public static JsonNode LoadAny(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            return node;
        }

        private static IOpenApiExtension LoadExtension(string name, JsonNode node, ParsingContext context)
        {
            if (context.ExtensionParsers is not null && context.ExtensionParsers.TryGetValue(name, out var parser))
            {
                try
                {
                    return parser(node, OpenApiSpecVersion.OpenApi3_2);
                }
                catch (OpenApiException ex)
                {
                    ex.Pointer = context.GetLocation();
                    context.Diagnostic.Errors.Add(new(ex));
                }
            }

            return new JsonNodeExtension(node);
        }

        private static string? LoadString(JsonNode node)
        {
            return node.GetScalarValue();
        }

        private static bool LoadBool(JsonNode node)
        {
            return node.GetScalarBoolValue();
        }

        private static (string, string?) GetReferenceIdAndExternalResource(string pointer)
        {
            /* Check whether the reference pointer is a URL
             * (id keyword allows you to supply a URL for the schema as a target for referencing)
             * E.g. $ref: 'https://example.com/schemas/resource.json' 
             * or its a normal json pointer fragment syntax
             * E.g. $ref: '#/components/schemas/pet'
             */
            var refSegments = pointer.Split('/');
            string refId = !pointer.Contains('#') ? pointer : refSegments[refSegments.Count()-1];

            var isExternalResource = !refSegments[0].StartsWith("#", StringComparison.OrdinalIgnoreCase);
            string? externalResource = null;
            if (isExternalResource && pointer.Contains('#'))
            {
                externalResource = pointer.Split('#')[0].TrimEnd('#');
            }

            return (refId, externalResource);
        }
    }
}
