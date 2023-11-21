// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Readers.Exceptions;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class ListNode : ParseNode, IEnumerable<ParseNode>
    {
        private readonly JsonArray _nodeList;

        public ListNode(ParsingContext context, JsonArray jsonArray) : base(
            context, jsonArray)
        {
            _nodeList = jsonArray;
        }

        public override List<T> CreateList<T>(Func<MapNode, T> map)
        {
            if (_nodeList == null)
            {
                throw new OpenApiReaderException($"Expected list while parsing {typeof(T).Name}", _nodeList);
            }

            return _nodeList?.Select(n => map(new MapNode(Context, n as JsonObject)))
                .Where(i => i != null)
                .ToList();
        }

        public override List<JsonNode> CreateListOfAny()
        {

            var list = _nodeList.Select(n => Create(Context, n).CreateAny().Node)
                .Where(i => i != null)
                .ToList();

            return list;
        }

        public override List<T> CreateSimpleList<T>(Func<ValueNode, T> map)
        {
            if (_nodeList == null)
            {
                throw new OpenApiReaderException($"Expected list while parsing {typeof(T).Name}", _nodeList);
            }

            return _nodeList.Select(n => map(new ValueNode(Context, n))).ToList();
        }

        public IEnumerator<ParseNode> GetEnumerator()
        {
            return _nodeList.Select(n => Create(Context, n)).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Create a <see cref="JsonArray"/>
        /// </summary>
        /// <returns>The created Any object.</returns>
        public override OpenApiAny CreateAny()
        {
            return new OpenApiAny(_nodeList);
        }
    }
}
