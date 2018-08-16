// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
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
        private static FixedFieldMap<OpenApiComponents> _componentsFixedFields = new FixedFieldMap<OpenApiComponents>
        {
            {
                "schemas", (o, n) => o.Schemas = n.CreateMapWithReference(ReferenceType.Schema, LoadSchema)
            },
            {"responses", (o, n) => o.Responses = n.CreateMapWithReference(ReferenceType.Response, LoadResponse)},
            {"parameters", (o, n) => o.Parameters = n.CreateMapWithReference(ReferenceType.Parameter, LoadParameter)},
            {"examples", (o, n) => o.Examples = n.CreateMapWithReference(ReferenceType.Example, LoadExample)},
            {"requestBodies", (o, n) => o.RequestBodies = n.CreateMapWithReference(ReferenceType.RequestBody, LoadRequestBody)},
            {"headers", (o, n) => o.Headers = n.CreateMapWithReference(ReferenceType.Header, LoadHeader)},
            {"securitySchemes", (o, n) => o.SecuritySchemes = n.CreateMapWithReference(ReferenceType.SecurityScheme, LoadSecurityScheme)},
            {"links", (o, n) => o.Links = n.CreateMapWithReference(ReferenceType.Link, LoadLink)},
            {"callbacks", (o, n) => o.Callbacks = n.CreateMapWithReference(ReferenceType.Callback, LoadCallback)},
        };


        private static PatternFieldMap<OpenApiComponents> _componentsPatternFields =
            new PatternFieldMap<OpenApiComponents>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
            };

        public static OpenApiComponents LoadComponents(ParseNode node)
        {
            var mapNode = node.CheckMapNode("components");
            var components = new OpenApiComponents();

            ParseMap(mapNode, components, _componentsFixedFields, _componentsPatternFields);

            return components;
        }
    }
}