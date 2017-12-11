// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using SharpYaml.Serialization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class ValueNode : ParseNode
    {
        private readonly YamlScalarNode node;

        public ValueNode(ParsingContext context, OpenApiDiagnostic diagnostic, YamlScalarNode scalarNode) : base(context, diagnostic)
        {
            node = scalarNode;
        }

        public override string GetScalarValue()
        {
            var scalarNode = node;

            if (scalarNode == null)
            {
                throw new OpenApiException($"Expected scalar at line {node.Start.Line}");
            }

            return scalarNode.Value;
        }

        /// <summary>
        /// Create a <see cref="IOpenApiPrimitive"/>
        /// </summary>
        /// <returns>The created Any object.</returns>
        public override IOpenApiAny CreateAny()
        {
            string value = GetScalarValue();

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

            if (Double.TryParse(value, out var dblValue))
            {
                return new OpenApiDouble(dblValue);
            }

            if (DateTime.TryParse(value, out var datetimeValue))
            {
                return new OpenApiDateTime(datetimeValue);
            }
            // TODO: add more codes to identify each primitive types


            // if we can't identify the type of value, return it as string.
            return new OpenApiString(value);
        }
    }
}
