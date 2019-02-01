// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
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

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(domainObject, fixedFieldMap, patternFieldMap);
                requiredFields?.Remove(propertyNode.Name);
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

                    var convertedOpenApiAny = OpenApiAnyConverter.GetSpecificOpenApiAny(
                        anyFieldMap[anyFieldName].PropertyGetter(domainObject),
                        anyFieldMap[anyFieldName].SchemaGetter(domainObject));

                    anyFieldMap[anyFieldName].PropertySetter(domainObject, convertedOpenApiAny);
                }
                catch (OpenApiException exception)
                {
                    exception.Pointer = mapNode.Context.GetLocation();
                    mapNode.Diagnostic.Errors.Add(new OpenApiError(exception));
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
                    var newProperty = new List<IOpenApiAny>();

                    mapNode.Context.StartObject(anyListFieldName);

                    var list = anyListFieldMap[anyListFieldName].PropertyGetter(domainObject);
                    if (list != null)
                    {
                        foreach (var propertyElement in list)
                        {
                            newProperty.Add(
                                OpenApiAnyConverter.GetSpecificOpenApiAny(
                                    propertyElement,
                                    anyListFieldMap[anyListFieldName].SchemaGetter(domainObject)));
                        }
                    }

                    anyListFieldMap[anyListFieldName].PropertySetter(domainObject, newProperty);
                }
                catch (OpenApiException exception)
                {
                    exception.Pointer = mapNode.Context.GetLocation();
                    mapNode.Diagnostic.Errors.Add(new OpenApiError(exception));
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
                    var newProperty = new List<IOpenApiAny>();

                    mapNode.Context.StartObject(anyMapFieldName);

                    foreach (var propertyMapElement in anyMapFieldMap[anyMapFieldName].PropertyMapGetter(domainObject))
                    {
                        if (propertyMapElement.Value != null)
                        {
                            mapNode.Context.StartObject(propertyMapElement.Key);

                            var any = anyMapFieldMap[anyMapFieldName].PropertyGetter(propertyMapElement.Value);

                            var newAny = OpenApiAnyConverter.GetSpecificOpenApiAny(
                                    any,
                                    anyMapFieldMap[anyMapFieldName].SchemaGetter(domainObject));

                            anyMapFieldMap[anyMapFieldName].PropertySetter(propertyMapElement.Value, newAny);
                        }
                    }
                }
                catch (OpenApiException exception)
                {
                    exception.Pointer = mapNode.Context.GetLocation();
                    mapNode.Diagnostic.Errors.Add(new OpenApiError(exception));
                }
                finally
                {
                    mapNode.Context.EndObject();
                }
            }
        }

        public static IOpenApiAny LoadAny(ParseNode node)
        {
            return OpenApiAnyConverter.GetSpecificOpenApiAny(node.CreateAny());
        }

        private static IOpenApiExtension LoadExtension(string name, ParseNode node)
        {
            if (node.Context.ExtensionParsers.TryGetValue(name, out var parser))
            {
                return parser(
                    OpenApiAnyConverter.GetSpecificOpenApiAny(node.CreateAny()),
                    OpenApiSpecVersion.OpenApi2_0);
            }
            else
            {
                return OpenApiAnyConverter.GetSpecificOpenApiAny(node.CreateAny());
            }
        }

        private static string LoadString(ParseNode node)
        {
            return node.GetScalarValue();
        }
    }
}