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
        private static readonly FixedFieldMap<OpenApiComponents> _componentsFixedFields = new()
        {
            {"schemas", (o, n, t, c) => o.Schemas = n.CreateMap(LoadSchema, t, c)},
            {"responses", (o, n, t, c) => o.Responses = n.CreateMap(LoadResponse, t, c)},
            {"parameters", (o, n, t, c) => o.Parameters = n.CreateMap(LoadParameter, t, c)},
            {"examples", (o, n, t, c) => o.Examples = n.CreateMap(LoadExample, t, c)},
            {"requestBodies", (o, n, t, c) => o.RequestBodies = n.CreateMap(LoadRequestBody, t, c)},
            {"headers", (o, n, t, c) => o.Headers = n.CreateMap(LoadHeader, t, c)},
            {"securitySchemes", (o, n, t, c) => o.SecuritySchemes = n.CreateMap(LoadSecurityScheme, t, c)},
            {"links", (o, n, t, c) => o.Links = n.CreateMap(LoadLink, t, c)},
            {"callbacks", (o, n, t, c) => o.Callbacks = n.CreateMap(LoadCallback, t, c)}
        };

        private static readonly PatternFieldMap<OpenApiComponents> _componentsPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static OpenApiComponents LoadComponents(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("components", context);
            var components = new OpenApiComponents();

            ParseMap(jsonObject, components, _componentsFixedFields, _componentsPatternFields, hostDocument, context);
            return components;
        }
    }
}
