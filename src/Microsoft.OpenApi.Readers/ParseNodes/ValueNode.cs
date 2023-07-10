// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Readers.Exceptions;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class ValueNode : ParseNode
    {
        private readonly JsonValue _node;

        public ValueNode(ParsingContext context, JsonNode node) : base(
            context)
        {
            if (node is not JsonValue scalarNode)
            {
                throw new OpenApiReaderException("Expected a value.", node);
            }
            _node = scalarNode;
        }

        public override string GetScalarValue()
        {
            return Convert.ToString(_node.GetValue<object>(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Create a <see cref="JsonNode"/>
        /// </summary>
        /// <returns>The created Any object.</returns>
        public override OpenApiAny CreateAny()
        {
            return new OpenApiAny(_node);
        }
    }
}
