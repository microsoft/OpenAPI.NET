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
        private static readonly FixedFieldMap<OpenApiComponents> _componentsFixedFields = new()
        {
            {"schemas", (o, n, t) => o.Schemas = n.CreateMap(LoadSchema, t)},
            {"responses", (o, n, t) => o.Responses = n.CreateMap(LoadResponse, t)},
            {"parameters", (o, n, t) => o.Parameters = n.CreateMap(LoadParameter, t)},
            {"examples", (o, n, t) => o.Examples = n.CreateMap(LoadExample, t)},
            {"requestBodies", (o, n, t) => o.RequestBodies = n.CreateMap(LoadRequestBody, t)},
            {"headers", (o, n, t) => o.Headers = n.CreateMap(LoadHeader, t)},
            {"securitySchemes", (o, n, t) => o.SecuritySchemes = n.CreateMap(LoadSecurityScheme, t)},
            {"links", (o, n, t) => o.Links = n.CreateMap(LoadLink, t)},
            {"callbacks", (o, n, t) => o.Callbacks = n.CreateMap(LoadCallback, t)}
        };

        private static readonly PatternFieldMap<OpenApiComponents> _componentsPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
            };

        public static OpenApiComponents LoadComponents(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("components");
            var components = new OpenApiComponents();

            ParseMap(mapNode, components, _componentsFixedFields, _componentsPatternFields, hostDocument);
            return components;
        }
    }
}
