// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;
using System.Linq;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static void ParseMap<T>(
            JsonObject? jsonObject,
            T domainObject,
            FixedFieldMap<T> fixedFieldMap,
            PatternFieldMap<T> patternFieldMap, 
            OpenApiDocument hostDocument,
            ParsingContext context)
        {
            jsonObject.ParseMap(domainObject, fixedFieldMap, patternFieldMap, hostDocument, context);
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
                    context.Diagnostic.Errors.Add(new(exception));
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
                    var mapElements = anyMapFieldMap[anyMapFieldName].PropertyMapGetter(domainObject);
                    if (mapElements is not null)
                    {
                        foreach (var propertyMapElement in mapElements)
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
                    context.Diagnostic.Errors.Add(new(exception));
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
                return new()
                {
                    Expression = RuntimeExpression.Build(value)
                };
            }

            return new()
            {
                Any = node

            };
        }

        public static JsonNodeExtension LoadAny(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            return new JsonNodeExtension(node);
        }

        private static IOpenApiExtension LoadExtension(string name, JsonNode node, ParsingContext context)
        {
            if (context.ExtensionParsers is not null && context.ExtensionParsers.TryGetValue(name, out var parser))
            {
                try
                {
                    var result = parser(node, OpenApiSpecVersion.OpenApi3_0);
                    if (result is { })
                    {
                        return result;
                    }
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

        private static (string, string?) GetReferenceIdAndExternalResource(string pointer)
        {
            JsonNodeHelper.ValidateReferencePointerFormat(pointer);

            var refSegments = pointer.Split('/');
            var refId = refSegments[refSegments.Length - 1];
            var isExternalResource = !refSegments[0].StartsWith("#", StringComparison.OrdinalIgnoreCase);
          
            string? externalResource = null;
            if (isExternalResource)
            {
                externalResource = pointer.Split('#')[0].TrimEnd('#');
            }
            return (refId, externalResource);
        }
    }
}
