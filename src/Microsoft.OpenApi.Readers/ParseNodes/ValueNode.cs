// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using Microsoft.OpenApi.Any;
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

            // The NumberStyles below are the default ones based on 
            // https://docs.microsoft.com/en-us/dotnet/api/?view=netframework-4.7.2
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
            {
                return new OpenApiInteger(intValue);
            }

            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
            {
                return new OpenApiLong(longValue); 
            }

            if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
            {
                return new OpenApiDouble(doubleValue); 
            }

            if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
            {
                return new OpenApiDateTime(dateTimeValue);
            }

            // if we can't identify the type of value, return it as string.
            return new OpenApiString(value);
        }
    }
}
