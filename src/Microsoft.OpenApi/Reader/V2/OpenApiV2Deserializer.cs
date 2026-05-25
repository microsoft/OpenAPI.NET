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
            JsonObject jsonObject,
            T domainObject,
            AnyFieldMap<T> anyFieldMap,
            ParsingContext context)
        {
            foreach (var anyFieldName in anyFieldMap.Keys.ToList())
            {
                try
                {
                    context.StartObject(anyFieldName);
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
                    exception.Pointer = context.GetLocation();
                    context.Diagnostic.Errors.Add(new(exception));
                }
                finally
                {
                    context.EndObject();
                }
            }
        }

        public static JsonNode LoadAny(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            return node.CreateAny();
        }

        private static IOpenApiExtension LoadExtension(string name, JsonNode node, ParsingContext context)
        {
            if (context.ExtensionParsers is not null && context.ExtensionParsers.TryGetValue(name, out var parser))
            {
                try
                {
                    return parser(node.CreateAny(), OpenApiSpecVersion.OpenApi2_0);
                }
                catch (OpenApiException ex)
                {
                    ex.Pointer = context.GetLocation();
                    context.Diagnostic.Errors.Add(new(ex));
                }
            }

            return new JsonNodeExtension(node.CreateAny());
        }

        private static string? LoadString(JsonNode node)
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
