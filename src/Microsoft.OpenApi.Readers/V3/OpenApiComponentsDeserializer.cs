// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
        public static FixedFieldMap<OpenApiComponents> ComponentsFixedFields = new FixedFieldMap<OpenApiComponents>
        {
            {
                "schemas", (o, n) =>
                {
                    o.Schemas = n.CreateMap(LoadSchema);
                }
            },
            {"responses", (o, n) => o.Responses = n.CreateMap(LoadResponse)},
            {"parameters", (o, n) => o.Parameters = n.CreateMap(LoadParameter)},
            {"examples", (o, n) => o.Examples = n.CreateMap(LoadExample)},
            {"requestBodies", (o, n) => o.RequestBodies = n.CreateMap(LoadRequestBody)},
            {"headers", (o, n) => o.Headers = n.CreateMap(LoadHeader)},
            {"securitySchemes", (o, n) => o.SecuritySchemes = n.CreateMap(LoadSecurityScheme)},
            {"links", (o, n) => o.Links = n.CreateMap(LoadLink)},
            {"callbacks", (o, n) => o.Callbacks = n.CreateMap(LoadCallback)},
        };

        public static PatternFieldMap<OpenApiComponents> ComponentsPatternFields =
            new PatternFieldMap<OpenApiComponents>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
            };

        public static OpenApiComponents LoadComponents(ParseNode node)
        {
            var mapNode = node.CheckMapNode("components");
            var components = new OpenApiComponents();

            ParseMap(mapNode, components, ComponentsFixedFields, ComponentsPatternFields);

            return components;
        }
    }
}