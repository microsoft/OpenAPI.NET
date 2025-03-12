﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static void ParseMap<T>(
            MapNode? mapNode,
            T domainObject,
            FixedFieldMap<T> fixedFieldMap,
            PatternFieldMap<T> patternFieldMap, 
            OpenApiDocument hostDocument)
        {
            if (mapNode == null)
            {
                return;
            }

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(domainObject, fixedFieldMap, patternFieldMap, hostDocument);
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
                    mapNode.Context.Diagnostic.Errors.Add(new(exception));
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
                    var mapElements = anyMapFieldMap[anyMapFieldName].PropertyMapGetter(domainObject);
                    if (mapElements is not null)
                    {
                        foreach (var propertyMapElement in mapElements)
                        {
                            mapNode.Context.StartObject(propertyMapElement.Key);

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
                    exception.Pointer = mapNode.Context.GetLocation();
                    mapNode.Context.Diagnostic.Errors.Add(new(exception));
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

            if (value != null && value.StartsWith("$", StringComparison.OrdinalIgnoreCase))
            {
                return new()
                {
                    Expression = RuntimeExpression.Build(value)
                };
            }

            return new()
            {
                Any = node.CreateAny()

            };
        }

        public static OpenApiAny LoadAny(ParseNode node, OpenApiDocument hostDocument)
        {
            return new OpenApiAny(node.CreateAny());
        }

        private static IOpenApiExtension LoadExtension(string name, ParseNode node)
        {
            if (node.Context.ExtensionParsers is not null && node.Context.ExtensionParsers.TryGetValue(name, out var parser) && parser(
                node.CreateAny(), OpenApiSpecVersion.OpenApi3_0) is { } result)
            {
                return result;
            }
            else
            {
                return new OpenApiAny(node.CreateAny());
            }
        }

        private static string? LoadString(ParseNode node)
        {
            return node.GetScalarValue();
        }

        private static (string, string?) GetReferenceIdAndExternalResource(string pointer)
        {
            var refSegments = pointer.Split('/');
            var refId = refSegments[refSegments.Count() -1];
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
