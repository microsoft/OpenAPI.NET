// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static void ParseMap<T>(
            MapNode mapNode,
            T domainObject,
            FixedFieldMap<T> fixedFieldMap,
            PatternFieldMap<T> patternFieldMap,
            List<string> requiredFields = null)
        {
            if (mapNode == null)
            {
                return;
            }

            var allFields = fixedFieldMap.Keys.Union(mapNode.Select(static x => x.Name));
            foreach (var propertyNode in allFields)
            {
                mapNode[propertyNode]?.ParseField(domainObject, fixedFieldMap, patternFieldMap);
                requiredFields?.Remove(propertyNode);
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
                    var anyFieldValue = anyFieldMap[anyFieldName].PropertyGetter(domainObject);

                    if (anyFieldValue == null)
                    {
                        anyFieldMap[anyFieldName].PropertySetter(domainObject, null);
                    }
                    else
                    {
                        anyFieldMap[anyFieldName].PropertySetter(domainObject, anyFieldValue);
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

        private static void ProcessAnyListFields<T>(
            MapNode mapNode,
            T domainObject,
            AnyListFieldMap<T> anyListFieldMap)
        {
            foreach (var anyListFieldName in anyListFieldMap.Keys.ToList())
            {
                try
                {
                    var newProperty = new List<JsonNode>();

                    mapNode.Context.StartObject(anyListFieldName);
                    if (anyListFieldMap.TryGetValue(anyListFieldName, out var fieldName))
                    {
                        var list = fieldName.PropertyGetter(domainObject);
                        if (list != null)
                        {
                            newProperty.Add(propertyElement);
                        }
                    }

                    anyListFieldMap[anyListFieldName].PropertySetter(domainObject, newProperty);
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

                    foreach (var propertyMapElement in anyMapFieldMap[anyMapFieldName].PropertyMapGetter(domainObject))
                    {
                        if (propertyMapElement.Value != null)
                        {
                            mapNode.Context.StartObject(propertyMapElement.Key);

                            var any = anyMapFieldMap[anyMapFieldName].PropertyGetter(propertyMapElement.Value);

                            anyMapFieldMap[anyMapFieldName].PropertySetter(propertyMapElement.Value, any);
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

        public static OpenApiAny LoadAny(ParseNode node)
        {
            return node.CreateAny();
        }

        private static IOpenApiExtension LoadExtension(string name, ParseNode node)
        {
            if (node.Context.ExtensionParsers.TryGetValue(name, out var parser))
            {
                return parser(node.CreateAny(), OpenApiSpecVersion.OpenApi2_0);
            }
            else
            {
                return node.CreateAny();
            }
        }

        private static string LoadString(ParseNode node)
        {
            return node.GetScalarValue();
        }
    }
}
