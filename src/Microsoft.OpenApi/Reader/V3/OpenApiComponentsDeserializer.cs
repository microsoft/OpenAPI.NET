// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
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
            {"schemas", (o, n) => o.Schemas =  n.CreateJsonSchemaMap(ReferenceType.Schema, LoadSchema, OpenApiSpecVersion.OpenApi3_0)},
            {"responses", (o, n) => o.Responses = n.CreateMap(LoadResponse)},
            {"parameters", (o, n) => o.Parameters = n.CreateMap(LoadParameter)},
            {"examples", (o, n) => o.Examples = n.CreateMap(LoadExample)},
            {"requestBodies", (o, n) => o.RequestBodies = n.CreateMap(LoadRequestBody)},
            {"headers", (o, n) => o.Headers = n.CreateMap(LoadHeader)},
            {"securitySchemes", (o, n) => o.SecuritySchemes = n.CreateMap(LoadSecurityScheme)},
            {"links", (o, n) => o.Links = n.CreateMap(LoadLink)},
            {"callbacks", (o, n) => o.Callbacks = n.CreateMap(LoadCallback)}
        };

        private static readonly PatternFieldMap<OpenApiComponents> _componentsPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
            };

        public static OpenApiComponents LoadComponents(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("components");
            var components = new OpenApiComponents();

            ParseMap(mapNode, components, _componentsFixedFields, _componentsPatternFields);
            return components;
        }
    }
}
