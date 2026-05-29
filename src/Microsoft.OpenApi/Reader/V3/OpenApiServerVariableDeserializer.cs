// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;
using System.Linq;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiServerVariable> _serverVariableFixedFields =
            new()
            {
                {
                    "enum",
                    (o, n, doc, c) => o.Enum = n.CreateSimpleList((s, p) => s.GetScalarValue(), doc, c).OfType<string>().ToList()
                },
                {
                    "default",
                    (o, n, _, c) => o.Default = n.GetScalarValue()
                },
                {
                    "description",
                    (o, n, _, c) => o.Description = n.GetScalarValue()
                },
            };

        private static readonly PatternFieldMap<OpenApiServerVariable> _serverVariablePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static OpenApiServerVariable LoadServerVariable(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("serverVariable", context);

            var serverVariable = new OpenApiServerVariable();

            ParseMap(jsonObject, serverVariable, _serverVariableFixedFields, _serverVariablePatternFields, hostDocument, context);

            return serverVariable;
        }
    }
}
