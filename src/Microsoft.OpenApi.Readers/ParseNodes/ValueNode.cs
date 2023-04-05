// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml;
using SharpYaml.Serialization;

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

        public override string GetScalarValue() => _node.GetValue<string>();

        /// <summary>
        /// Create a <see cref="IOpenApiPrimitive"/>
        /// </summary>
        /// <returns>The created Any object.</returns>
        public override IOpenApiAny CreateAny()
        {
            var value = GetScalarValue();
            return new OpenApiString(value);// this._node..Style == ScalarStyle.SingleQuoted || this._node.Style == ScalarStyle.DoubleQuoted || this._node.Style == ScalarStyle.Literal || this._node.Style == ScalarStyle.Folded);
        }
    }
}
