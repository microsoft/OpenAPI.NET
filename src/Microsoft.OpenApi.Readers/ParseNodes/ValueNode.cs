// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class ValueNode : ParseNode
    {
        private readonly YamlScalarNode _node;

        public ValueNode(ParsingContext context, OpenApiDiagnostic diagnostic, YamlNode node) : base(
            context,
            diagnostic)
        {
            if (!(node is YamlScalarNode scalarNode))
            {
                throw new OpenApiReaderException("Expected a value.", node);
            }
            _node = scalarNode;
        }

        public override string GetScalarValue()
        {
            return _node.Value;
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

            if (long.TryParse(value, out var longValue))
            {
                return
                    new OpenApiLong(
                        longValue); 
            }

            if (double.TryParse(value, out var dblValue))
            {
                return
                    new OpenApiDouble(
                        dblValue); // Note(darrmi): This may be better as decimal. Further investigation required.
            }

            if (DateTimeOffset.TryParse(value, out var datetimeValue))
            {
                return new OpenApiDateTime(datetimeValue);
            }

            // if we can't identify the type of value, return it as string.
            return new OpenApiString(value);
        }
    }
}
