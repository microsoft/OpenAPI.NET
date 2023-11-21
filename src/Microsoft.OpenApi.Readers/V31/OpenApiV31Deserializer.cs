// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static void ParseMap<T>(
            MapNode mapNode,
            T domainObject,
            FixedFieldMap<T> fixedFieldMap,
            PatternFieldMap<T> patternFieldMap)
        {
            if (mapNode == null)
            {
                return;
            }

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(domainObject, fixedFieldMap, patternFieldMap);
            }

        }

        private static void ProcessAnyFields<T>(
            MapNode mapNode,
            T domainObject,
            AnyFieldMap<T> anyFieldMap)
        {
            foreach (var anyFieldName in anyFieldMap.Keys.ToList())
            {
                try
                {
                    mapNode.Context.StartObject(anyFieldName);

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
                    exception.Pointer = mapNode.Context.GetLocation();
                    mapNode.Context.Diagnostic.Errors.Add(new OpenApiError(exception));
                }
                finally
                {
                    mapNode.Context.EndObject();
                }
            }
        }

        private static void ProcessAnyMapFields<T, U>(
            MapNode mapNode,
            T domainObject,
            AnyMapFieldMap<T, U> anyMapFieldMap)
        {
            foreach (var anyMapFieldName in anyMapFieldMap.Keys.ToList())
            {
                try
                {
                    mapNode.Context.StartObject(anyMapFieldName);
                    var propertyMapGetter = anyMapFieldMap[anyMapFieldName].PropertyMapGetter(domainObject);
                    if (propertyMapGetter != null)
                    {
                        foreach (var propertyMapElement in propertyMapGetter)
                        {
                            mapNode.Context.StartObject(propertyMapElement.Key);

                            if (propertyMapElement.Value != null)
                            {
                                var any = anyMapFieldMap[anyMapFieldName].PropertyGetter(propertyMapElement.Value);

                                anyMapFieldMap[anyMapFieldName].PropertySetter(propertyMapElement.Value, any);
                            }
                        }
                    }
                }
                catch (OpenApiException exception)
                {
                    exception.Pointer = mapNode.Context.GetLocation();
                    mapNode.Context.Diagnostic.Errors.Add(new OpenApiError(exception));
                }
                finally
                {
                    mapNode.Context.EndObject();
                }
            }
        }

        private static RuntimeExpressionAnyWrapper LoadRuntimeExpressionAnyWrapper(ParseNode node)
        {
            var value = node.GetScalarValue();

            if (value != null && value.StartsWith("$"))
            {
                return new RuntimeExpressionAnyWrapper
                {
                    Expression = RuntimeExpression.Build(value)
                };
            }

            return new RuntimeExpressionAnyWrapper
            {
                Any = node.CreateAny()
            };
        }

        public static OpenApiAny LoadAny(ParseNode node)
        {
            return node.CreateAny();
        }

        private static IOpenApiExtension LoadExtension(string name, ParseNode node)
        {
            return node.Context.ExtensionParsers.TryGetValue(name, out var parser)
                ? parser(node.CreateAny(), OpenApiSpecVersion.OpenApi3_1)
                : node.CreateAny();
        }

        private static string LoadString(ParseNode node)
        {
            return node.GetScalarValue();
        }
    }
}
