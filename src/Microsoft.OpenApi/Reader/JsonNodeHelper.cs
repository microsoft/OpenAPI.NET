// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader
{
    internal static class JsonNodeHelper
    {
        public static JsonObject CheckMapNode(this JsonNode? node, string nodeName, ParsingContext context)
        {
            if (node is not JsonObject jsonObject)
            {
                throw new OpenApiReaderException($"{nodeName} must be a map/object", context);
            }

            return jsonObject;
        }

        public static List<T> CreateList<T>(this JsonNode? node, Func<JsonNode, OpenApiDocument, ParsingContext, T> map, OpenApiDocument hostDocument, ParsingContext context)
        {
            if (node is not JsonArray jsonArray)
            {
                throw new OpenApiReaderException($"Expected list while parsing {typeof(T).Name}", context);
            }

            return jsonArray
                .OfType<JsonObject>()
                .Select(n => map(n, hostDocument, context))
                .Where(static i => i != null)
                .ToList();
        }

        public static List<JsonNode> CreateListOfAny(this JsonNode? node, ParsingContext context)
        {
            if (node is not JsonArray jsonArray)
            {
                throw new OpenApiReaderException("Cannot create a list from this type of node.", context);
            }

            return jsonArray.OfType<JsonNode>().ToList();
        }

        public static List<T> CreateSimpleList<T>(this JsonNode? node, Func<JsonNode, OpenApiDocument?, T> map, OpenApiDocument? openApiDocument, ParsingContext context)
        {
            if (node is not JsonArray jsonArray)
            {
                throw new OpenApiReaderException($"Expected list while parsing {typeof(T).Name}", context);
            }

            return jsonArray.OfType<JsonNode>().Select(n =>
            {
                if (n is not JsonValue)
                {
                    throw new OpenApiReaderException($"Expected a value while parsing at {context.GetLocation()}.");
                }

                return map(n, openApiDocument);
            }).ToList();
        }

        public static Dictionary<string, T> CreateMap<T>(this JsonNode? node, Func<JsonNode, OpenApiDocument, ParsingContext, T> map, OpenApiDocument hostDocument, ParsingContext context)
        {
            if (node is not JsonObject jsonMap)
            {
                throw new OpenApiReaderException($"Expected map while parsing {typeof(T).Name}", context);
            }

            var nodes = jsonMap.Select(n =>
            {
                var key = n.Key;
                T value;
                try
                {
                    context.StartObject(key);
                    value = n.Value is JsonObject jsonObject
                        ? map(jsonObject, hostDocument, context)
                        : default!;
                }
                finally
                {
                    context.EndObject();
                }

                return new
                {
                    key,
                    value
                };
            });

            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public static Dictionary<string, T> CreateSimpleMap<T>(this JsonNode? node, Func<JsonNode, T> map, ParsingContext context)
        {
            if (node is not JsonObject jsonMap)
            {
                throw new OpenApiReaderException($"Expected map while parsing {typeof(T).Name}", context);
            }

            var nodes = jsonMap.Select(n =>
            {
                var key = n.Key;
                try
                {
                    context.StartObject(key);
                    var jsonNode = n.Value is JsonValue value
                        ? value
                        : throw new OpenApiReaderException($"Expected scalar while parsing {typeof(T).Name}", context);

                    return (key, value: map(jsonNode));
                }
                finally
                {
                    context.EndObject();
                }
            });

            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public static Dictionary<string, HashSet<T>> CreateArrayMap<T>(this JsonNode? node, Func<JsonNode, OpenApiDocument?, T> map, OpenApiDocument? openApiDocument, ParsingContext context)
        {
            if (node is not JsonObject jsonMap)
            {
                throw new OpenApiReaderException($"Expected map while parsing {typeof(T).Name}", context);
            }

            var nodes = jsonMap.Select(n =>
            {
                var key = n.Key;
                try
                {
                    context.StartObject(key);
                    var arrayNode = n.Value is JsonArray value
                        ? value
                        : throw new OpenApiReaderException($"Expected array while parsing {typeof(T).Name}", context);

                    var values = new HashSet<T>(arrayNode.OfType<JsonNode>().Select(item => map(item, openApiDocument)));

                    return (key, values);

                }
                finally
                {
                    context.EndObject();
                }
            });

            return nodes.ToDictionary(kvp => kvp.key, kvp => kvp.values);
        }

        public static string? GetScalarValue(this JsonNode? node)
        {
            if (node.IsJsonNullSentinel())
            {
                return null;
            }

            var scalarNode = node is JsonValue value ? value : throw new OpenApiException("Expected scalar value.");

            return Convert.ToString(scalarNode.GetValue<object>(), CultureInfo.InvariantCulture);
        }

        public static string? GetReferencePointer(this JsonObject jsonObject)
        {
            return jsonObject.TryGetPropertyValue("$ref", out var refNode) ? refNode?.GetScalarValue() : null;
        }

        public static void ValidateReferencePointerFormat(string pointer)
        {
            var hashIndex = pointer.IndexOf('#');
            if (hashIndex >= 0 &&
                pointer.Length > hashIndex + 1 &&
                pointer[hashIndex + 1] != '/')
            {
                throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, pointer));
            }
        }

        /// <summary>
        /// Returns the value of $dynamicRef if $ref is absent. Used to create a schema reference
        /// for bare $dynamicRef schemas (no $ref) so they participate in reference resolution.
        /// </summary>
        public static string? GetDynamicReferencePointer(this JsonObject jsonObject)
        {
            if (jsonObject.TryGetPropertyValue("$ref", out _))
                return null;
            return jsonObject.TryGetPropertyValue("$dynamicRef", out var dynRefNode) ? dynRefNode?.GetScalarValue() : null;
        }

        /// <summary>
        /// Splits a $dynamicRef value on the last '#' and returns the fragment part.
        /// For "#meta" returns "meta". For "https://example.com#meta" returns "meta".
        /// For values with no '#' returns the original string. Returns null for null/empty input.
        /// This is a string operation only — it does not validate or resolve the reference.
        /// </summary>
        public static string? ExtractDynamicAnchorName(string? dynamicRef)
        {
            if (string.IsNullOrEmpty(dynamicRef)) return null;
            var hashIndex = dynamicRef!.LastIndexOf('#');
            return hashIndex >= 0 ? dynamicRef.Substring(hashIndex + 1) : dynamicRef;
        }

        /// <summary>
        /// Determines whether a $dynamicRef value is a fragment-only reference (e.g. "#node")
        /// that targets an anchor within the current document, as opposed to an absolute/relative
        /// URI reference (e.g. "https://example.com/schema#node") that targets another resource.
        /// Per JSON Schema 2020-12, only fragment-only dynamic refs resolve against the local
        /// $dynamicAnchor index; URI-based refs require resolving their target resource first.
        /// </summary>
        public static bool IsFragmentOnlyDynamicRef(string? dynamicRef)
            => !string.IsNullOrEmpty(dynamicRef) && dynamicRef![0] == '#';

        /// <summary>
        /// Extracts the document URI from a $dynamicRef value. Returns null for fragment-only
        /// refs (e.g. "#node"). For URI refs (e.g. "https://example.com/external#node"), returns
        /// the part before the fragment (e.g. "https://example.com/external").
        /// </summary>
        public static string? ExtractDocumentUri(string? dynamicRef)
        {
            if (string.IsNullOrEmpty(dynamicRef)) return null;
            var hashIndex = dynamicRef!.IndexOf('#');
            return hashIndex > 0 ? dynamicRef.Substring(0, hashIndex) : null;
        }

        public static string? GetJsonSchemaIdentifier(this JsonObject jsonObject)
        {
            return jsonObject.TryGetPropertyValue("$id", out var idNode) ? idNode?.GetScalarValue() : null;
        }

        public static void ParseMap<T>(
            this JsonObject? JsonObject,
            T domainObject,
            FixedFieldMap<T> fixedFieldMap,
            PatternFieldMap<T> patternFieldMap,
            OpenApiDocument hostDocument,
            ParsingContext context,
            Action<T, string, JsonNode>? unrecognizedProperty = null)
        {
            if (JsonObject == null)
            {
                return;
            }

            foreach (var propertyNode in JsonObject)
            {
                ParseField(propertyNode.Key, propertyNode.Value ?? JsonNullSentinel.JsonNull, domainObject, fixedFieldMap, patternFieldMap, hostDocument, context, unrecognizedProperty);
            }
        }

        private static void ParseField<T>(
            string name,
            JsonNode value,
            T parentInstance,
            FixedFieldMap<T> fixedFields,
            PatternFieldMap<T> patternFields,
            OpenApiDocument hostDocument,
            ParsingContext context,
            Action<T, string, JsonNode>? unrecognizedProperty)
        {
            if (fixedFields.TryGetValue(name, out var fixedFieldMap))
            {
                try
                {
                    context.StartObject(name);
                    fixedFieldMap(parentInstance, value, hostDocument, context);
                }
                catch (OpenApiReaderException ex)
                {
                    context.Diagnostic.Errors.Add(new(ex));
                }
                catch (OpenApiException ex)
                {
                    ex.Pointer = context.GetLocation();
                    context.Diagnostic.Errors.Add(new(ex));
                }
                finally
                {
                    context.EndObject();
                }
            }
            else
            {
                var map = patternFields.Where(p => p.Key(name)).Select(p => p.Value).FirstOrDefault();
                if (map != null)
                {
                    try
                    {
                        context.StartObject(name);
                        map(parentInstance, name, value, hostDocument, context);
                    }
                    catch (OpenApiReaderException ex)
                    {
                        context.Diagnostic.Errors.Add(new(ex));
                    }
                    catch (OpenApiException ex)
                    {
                        ex.Pointer = context.GetLocation();
                        context.Diagnostic.Errors.Add(new(ex));
                    }
                    finally
                    {
                        context.EndObject();
                    }
                }
                else if (unrecognizedProperty != null)
                {
                    unrecognizedProperty(parentInstance, name, value);
                }
                else
                {
                    var error = new OpenApiError("", $"{name} is not a valid property at {context.GetLocation()}");
                    if ("$schema".Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Diagnostic.Warnings.Add(error);
                    }
                    else
                    {
                        context.Diagnostic.Errors.Add(error);
                    }
                }
            }
        }
    }
}
