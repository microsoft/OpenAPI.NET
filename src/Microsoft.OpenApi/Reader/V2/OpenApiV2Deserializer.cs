// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static void ParseMap<T>(
            MapNode? mapNode,
            T domainObject,
            FixedFieldMap<T> fixedFieldMap,
            PatternFieldMap<T> patternFieldMap,
            OpenApiDocument doc)
        {
            if (mapNode == null)
            {
                return;
            }

            var mapNodeFields = mapNode.ToDictionary(static x => x.Name, static x => x);
            var allFields = fixedFieldMap.Keys.Union(mapNodeFields.Keys);
            foreach (var propertyNodeName in allFields)
            {
                if (!mapNodeFields.TryGetValue(propertyNodeName, out var propertyNode))
                {
                    continue;
                }
                propertyNode.ParseField(domainObject, fixedFieldMap, patternFieldMap, doc);
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

        public static JsonNode LoadAny(ParseNode node, OpenApiDocument hostDocument)
        {
            return node.CreateAny();
        }

        private static IOpenApiExtension LoadExtension(string name, ParseNode node)
        {
            if (node.Context.ExtensionParsers is not null && node.Context.ExtensionParsers.TryGetValue(name, out var parser))
            {
                try
                {
                    return parser(node.CreateAny(), OpenApiSpecVersion.OpenApi2_0);
                }
                catch (OpenApiException ex)
                {
                    ex.Pointer = node.Context.GetLocation();
                    node.Context.Diagnostic.Errors.Add(new(ex));
                }
            }

            return new JsonNodeExtension(node.CreateAny());
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

            string? externalResource = isExternalResource ? $"{refSegments[0]}/{refSegments[1].TrimEnd('#')}" : null;

            return (refId, externalResource);
        }
    }
}
