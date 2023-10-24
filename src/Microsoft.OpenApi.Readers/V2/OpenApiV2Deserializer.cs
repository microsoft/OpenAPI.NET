// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
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

                    var convertedOpenApiAny = OpenApiAnyConverter.GetSpecificOpenApiAny(
                        anyFieldMap[anyFieldName].PropertyGetter(domainObject),
                        anyFieldMap[anyFieldName].SchemaGetter(domainObject));

                    anyFieldMap[anyFieldName].PropertySetter(domainObject, convertedOpenApiAny);
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
                    var newProperty = new List<IOpenApiAny>();

                    mapNode.Context.StartObject(anyListFieldName);
                    if (anyListFieldMap.TryGetValue(anyListFieldName, out var fieldName))
                    {
                        var list = fieldName.PropertyGetter(domainObject);
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
