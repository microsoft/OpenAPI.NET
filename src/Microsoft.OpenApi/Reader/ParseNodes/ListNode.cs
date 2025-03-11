// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Reader.ParseNodes
{
    internal class ListNode : ParseNode, IEnumerable<ParseNode>
    {
        private readonly JsonArray _nodeList;

        public ListNode(ParsingContext context, JsonArray jsonArray) : base(
            context, jsonArray)
        {
            _nodeList = jsonArray;
        }

        public override List<T> CreateList<T>(Func<MapNode, OpenApiDocument?, T> map, OpenApiDocument? hostDocument)
        {
            if (_nodeList == null)
            {
                throw new OpenApiReaderException($"Expected list while parsing {typeof(T).Name}");
            }

            var list = _nodeList
                .OfType<JsonObject>()
                .Select(n => map(new MapNode(Context, n), hostDocument))
                .Where(i => i != null)
                .ToList();
            return list;
        }

        public override List<JsonNode> CreateListOfAny()
        {

            var list = _nodeList.OfType<JsonNode>().Select(n => Create(Context, n).CreateAny())
                .Where(i => i != null)
                .ToList();

            return list;
        }

        public override List<T> CreateSimpleList<T>(Func<ValueNode, OpenApiDocument?, T> map, OpenApiDocument? openApiDocument)
        {
            if (_nodeList == null)
            {
                throw new OpenApiReaderException($"Expected list while parsing {typeof(T).Name}");
            }

            return _nodeList.OfType<JsonNode>().Select(n => map(new(Context, n), openApiDocument)).ToList();
        }

        public IEnumerator<ParseNode> GetEnumerator()
        {
            return _nodeList.OfType<JsonNode>().Select(n => Create(Context, n)).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Create a <see cref="JsonArray"/>
        /// </summary>
        /// <returns>The created Any object.</returns>
        public override JsonNode CreateAny()
        {
            return _nodeList;
        }
    }
}
