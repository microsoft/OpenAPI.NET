﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class ValueNode : ParseNode
    {
        private readonly YamlScalarNode _node;

        public ValueNode(ParsingContext context, OpenApiDiagnostic diagnostic, YamlScalarNode scalarNode) : base(
            context,
            diagnostic)
        {
            _node = scalarNode;
        }

        public override string GetScalarValue()
        {
            var scalarNode = _node;

            if (scalarNode == null)
            {
                throw new OpenApiException($"Expected scalar at line {_node.Start.Line}");
            }

            return scalarNode.Value;
        }

        /// <summary>
        /// Create a <see cref="IOpenApiPrimitive"/>
        /// </summary>
        /// <returns>The created Any object.</returns>
        public override IOpenApiAny CreateAny()
        {
            var value = GetScalarValue();

            if (value == null || value == "null")
            {
                return new OpenApiNull();
            }

            if (value == "true")
            {
                return new OpenApiBoolean(true);
            }

            if (value == "false")
            {
                return new OpenApiBoolean(false);
            }

            if (int.TryParse(value, out var intValue))
            {
                return new OpenApiInteger(intValue);
            }

            if (double.TryParse(value, out var dblValue))
            {
                return
                    new OpenApiDouble(
                        dblValue); // Note(darrmi): This may be better as decimal.  Further investigation required.
            }

            if (DateTime.TryParse(value, out var datetimeValue))
            {
                return new OpenApiDateTime(datetimeValue);
            }

            // if we can't identify the type of value, return it as string.
            return new OpenApiString(value);
        }
    }
}