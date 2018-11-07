// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiServer> _serverFixedFields = new FixedFieldMap<OpenApiServer>
        {
            {
                "url", (o, n) =>
                {
                    o.Url = n.GetScalarValue();
                }
            },
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "variables", (o, n) =>
                {
                    o.Variables = n.CreateMap(LoadServerVariable);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiServer> _serverPatternFields = new PatternFieldMap<OpenApiServer>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiServer LoadServer(ParseNode node)
        {
            var mapNode = node.CheckMapNode("server");

            var server = new OpenApiServer();

            ParseMap(mapNode, server, _serverFixedFields, _serverPatternFields);

            if (server.Url != null && server.Url.StartsWith("/") && node.Context.BaseUrl != null && !string.IsNullOrEmpty(node.Context.BaseUrl.ToString()))
            {
                string backup = server.Url; //just in case
                try
                {
                    server.Url = new Uri(node.Context.BaseUrl, server.Url).ToString();
                }
                catch (Exception)
                {
                    server.Url = backup;
                }
            }

            return server;
        }
    }
}