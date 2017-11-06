// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Any;
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
        private static readonly FixedFieldMap<OpenApiServer> ServerFixedFields = new FixedFieldMap<OpenApiServer>
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

        private static readonly PatternFieldMap<OpenApiServer> ServerPatternFields = new PatternFieldMap<OpenApiServer>
        {
            {s => s.StartsWith("x-"), (o, k, n) => o.Extensions.Add(k, new OpenApiString(n.GetScalarValue()))}
        };

        public static OpenApiServer LoadServer(ParseNode node)
        {
            var mapNode = node.CheckMapNode("server");

            var server = new OpenApiServer();

            ParseMap(mapNode, server, ServerFixedFields, ServerPatternFields);

            return server;
        }
    }
}