// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Reader.ParseNodes
{
    /// <summary>
    /// Abstraction of a Map to isolate semantic parsing from details of JSON DOM
    /// </summary>
    internal class MapNode : ParseNode, IEnumerable<PropertyNode>
    {
        private readonly JsonObject _node;
        private readonly List<PropertyNode> _nodes;

        public MapNode(ParsingContext context, JsonNode node) : base(
            context, node)
        {
            if (node is not JsonObject mapNode)
            {
                throw new OpenApiReaderException("Expected map.", Context);
            }

            _node = mapNode;
            _nodes = _node.Select(p => new PropertyNode(Context, p.Key, p.Value)).ToList();
        }

        public PropertyNode this[string key]
        {
            get
            {
                if (_node.TryGetPropertyValue(key, out var node))
                {
                    return new(Context, key, node);
                }

                return null;
            }
        }

        public override Dictionary<string, T> CreateMap<T>(Func<MapNode, OpenApiDocument, T> map, OpenApiDocument hostDocument = null)
        {
            var jsonMap = _node ?? throw new OpenApiReaderException($"Expected map while parsing {typeof(T).Name}", Context);
            var nodes = jsonMap.Select(
                n =>
                {

                    var key = n.Key;
                    T value;
                    try
                    {
                        Context.StartObject(key);
                        value = n.Value is JsonObject jsonObject
                          ? map(new MapNode(Context, jsonObject), hostDocument)
                          : default;
                    }
                    finally
                    {    
                        Context.EndObject();
                    }
                    return new
                    {
                        key,
                        value
                    };
                });

            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public override Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
        {
            var jsonMap = _node ?? throw new OpenApiReaderException($"Expected map while parsing {typeof(T).Name}", Context);
            var nodes = jsonMap.Select(
                n =>
                {
                    var key = n.Key;
                    try
                    {
                        Context.StartObject(key);
                        JsonValue valueNode = n.Value is JsonValue value ? value
                        : throw new OpenApiReaderException($"Expected scalar while parsing {typeof(T).Name}", Context);

                        return (key, value: map(new ValueNode(Context, valueNode)));
                    }
                    finally
                    {
                        Context.EndObject();
                    }
                });

            return nodes.ToDictionary(k => k.key, v => v.value);
        }

        public IEnumerator<PropertyNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        public override string GetRaw()
        {
            var x = JsonSerializer.Serialize(_node);
            return x;
        }

        public T GetReferencedObject<T>(ReferenceType referenceType, string referenceId, string summary = null, string description = null)
            where T : IOpenApiReferenceable, new()
        {
            return new()
            {
                UnresolvedReference = true,
                Reference = Context.VersionService.ConvertToOpenApiReference(referenceId, referenceType, summary, description)
            };
        }

        public string GetReferencePointer()
        {
            if (!_node.TryGetPropertyValue("$ref", out JsonNode refNode))
            {
                return null;
            }

            return refNode.GetScalarValue();
        }

        public string GetSummaryValue()
        {
            if (!_node.TryGetPropertyValue("summary", out JsonNode summaryNode))
            {
                return null;
            }

            return summaryNode.GetScalarValue();
        }

        public string GetDescriptionValue()
        {
            if (!_node.TryGetPropertyValue("description", out JsonNode descriptionNode))
            {
                return null;
            }

            return descriptionNode.GetScalarValue();
        }

        public string GetScalarValue(ValueNode key)
        {
            var scalarNode = _node[key.GetScalarValue()] is JsonValue jsonValue
                ? jsonValue
                : throw new OpenApiReaderException($"Expected scalar while parsing {key.GetScalarValue()}", Context);

            return Convert.ToString(scalarNode?.GetValue<object>(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Create an <see cref="OpenApiAny"/>
        /// </summary>
        /// <returns>The created Json object.</returns>
        public override JsonNode CreateAny()
        {
            return _node;
        }
    }
}
