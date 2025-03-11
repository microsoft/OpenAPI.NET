﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Microsoft.OpenApi.Exceptions;

namespace Microsoft.OpenApi.Reader.ParseNodes
{
    internal class ValueNode : ParseNode
    {
        private readonly JsonValue _node;

        public ValueNode(ParsingContext context, JsonNode node) : base(
            context, node)
        {
            if (node is not JsonValue scalarNode)
            {
                throw new OpenApiReaderException($"Expected a value while parsing at {Context.GetLocation()}.");
            }
            _node = scalarNode;
        }

        public override string GetScalarValue()
        {
            var scalarValue = _node.GetValue<object>();
            return Convert.ToString(scalarValue, CultureInfo.InvariantCulture) 
                ?? throw new OpenApiReaderException($"Expected a value at {Context.GetLocation()}.");
        }

        /// <summary>
        /// Create a <see cref="JsonNode"/>
        /// </summary>
        /// <returns>The created Any object.</returns>
        public override JsonNode CreateAny()
        {
            return _node;
        }
    }
}
