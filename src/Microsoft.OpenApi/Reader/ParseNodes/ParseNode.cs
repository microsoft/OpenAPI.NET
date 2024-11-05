// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Reader.ParseNodes
{
    internal abstract class ParseNode
    {
        protected ParseNode(ParsingContext parsingContext, JsonNode jsonNode)
        {
            Context = parsingContext;
            JsonNode = jsonNode;
        }

        public ParsingContext Context { get; }

        public JsonNode JsonNode { get; }

        public MapNode CheckMapNode(string nodeName)
        {
            if (this is not MapNode mapNode)
            {
                throw new OpenApiReaderException($"{nodeName} must be a map/object", Context);
            }

            return mapNode;
        }

        public static ParseNode Create(ParsingContext context, JsonNode node)
        {
            if (node is JsonArray listNode)
            {
                return new ListNode(context, listNode);
            }

            if (node is JsonObject mapNode)
            {
                return new MapNode(context, mapNode);
            }

            return new ValueNode(context, node as JsonValue);
        }

        public virtual List<T> CreateList<T>(Func<MapNode, OpenApiDocument, T> map, OpenApiDocument hostDocument = null)
        {
            throw new OpenApiReaderException("Cannot create list from this type of node.", Context);
        }

        public virtual Dictionary<string, T> CreateMap<T>(Func<MapNode, OpenApiDocument, T> map, OpenApiDocument hostDocument = null)
        {
            throw new OpenApiReaderException("Cannot create map from this type of node.", Context);
        }

        public virtual List<T> CreateSimpleList<T>(Func<ValueNode, OpenApiDocument, T> map)
        {
            throw new OpenApiReaderException("Cannot create simple list from this type of node.", Context);
        }

        public virtual Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
        {
            throw new OpenApiReaderException("Cannot create simple map from this type of node.", Context);
        }

        public virtual JsonNode CreateAny()
        {
            throw new OpenApiReaderException("Cannot create an Any object this type of node.", Context);
        }

        public virtual string GetRaw()
        {
            throw new OpenApiReaderException("Cannot get raw value from this type of node.", Context);
        }

        public virtual string GetScalarValue()
        {
            throw new OpenApiReaderException("Cannot create a scalar value from this type of node.", Context);
        }

        public virtual List<JsonNode> CreateListOfAny()
        {
            throw new OpenApiReaderException("Cannot create a list from this type of node.", Context);
        }      
    }
}
