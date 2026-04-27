// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader
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
        /// Attempts to get the underlying value directly as the specified type without string conversion.
        /// </summary>
        /// <typeparam name="T">The type to retrieve the value as.</typeparam>
        /// <param name="value">The retrieved value if successful.</param>
        /// <returns>True if the value was successfully converted to the specified type; otherwise, false.</returns>
        public bool TryGetValue<T>(out T? value)
        {
            return _node.TryGetValue(out value);
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
