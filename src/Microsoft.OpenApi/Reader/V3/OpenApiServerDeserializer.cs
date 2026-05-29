// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiServer> _serverFixedFields = new()
        {
            {
                "url",
                (o, n, _, c) => o.Url = n.GetScalarValue()
            },
            {
                "description",
                (o, n, _, c) => o.Description = n.GetScalarValue()
            },
            {
                "variables",
                (o, n, t, c) => o.Variables = n.CreateMap(LoadServerVariable, t, c)
            }
        };

        private static readonly PatternFieldMap<OpenApiServer> _serverPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static OpenApiServer LoadServer(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("server", context);

            var server = new OpenApiServer();

            ParseMap(JsonObject, server, _serverFixedFields, _serverPatternFields, hostDocument, context);

            return server;
        }
    }
}
