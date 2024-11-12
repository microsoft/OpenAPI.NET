// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V2
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
            List<string> requiredFields = null, 
            OpenApiDocument doc = null)
        {
            if (mapNode == null)
            {
                return;
            }

            var allFields = fixedFieldMap.Keys.Union(mapNode.Select(static x => x.Name));
            foreach (var propertyNode in allFields)
            {
                mapNode[propertyNode]?.ParseField(domainObject, fixedFieldMap, patternFieldMap, doc);
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

        public static JsonNode LoadAny(ParseNode node, OpenApiDocument hostDocument = null)
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
                return new OpenApiAny(node.CreateAny());
            }
        }

        private static string LoadString(ParseNode node)
        {
            return node.GetScalarValue();
        }

        private static (string, string) GetReferenceIdAndExternalResource(string pointer)
        {
            var refSegments = pointer.Split('/');
            var refId = refSegments.Last();
            var isExternalResource = !refSegments.First().StartsWith("#");

            string externalResource = isExternalResource ? $"{refSegments.First()}/{refSegments[1].TrimEnd('#')}" : null;

            return (refId, externalResource);
        }
    }
}
