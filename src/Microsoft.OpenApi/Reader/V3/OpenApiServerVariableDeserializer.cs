// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

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
                    (o, n, _) => o.Enum = n.CreateSimpleList((s, p) => s.GetScalarValue())
                },
                {
                    "default",
                    (o, n, _) => o.Default = n.GetScalarValue()
                },
                {
                    "description",
                    (o, n, _) => o.Description = n.GetScalarValue()
                },
            };

        private static readonly PatternFieldMap<OpenApiServerVariable> _serverVariablePatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiServerVariable LoadServerVariable(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("serverVariable");

            var serverVariable = new OpenApiServerVariable();

            ParseMap(mapNode, serverVariable, _serverVariableFixedFields, _serverVariablePatternFields, hostDocument);

            return serverVariable;
        }
    }
}
