﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiServer> _serverFixedFields = new()
        {
            {
                "url", (o, n, _) =>
                {
                    o.Url = n.GetScalarValue();
                }
            },
            {
                "description", (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "variables", (o, n, t) =>
                {
                    o.Variables = n.CreateMap(LoadServerVariable, t);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiServer> _serverPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiServer LoadServer(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("server");

            var server = new OpenApiServer();

            ParseMap(mapNode, server, _serverFixedFields, _serverPatternFields, hostDocument);

            return server;
        }
    }
}
