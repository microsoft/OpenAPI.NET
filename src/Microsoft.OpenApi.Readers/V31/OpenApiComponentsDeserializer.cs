// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Json.Schema;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiComponents> _componentsFixedFields = new()
        {
        {"schemas", (o, n) => o.Schemas = n.CreateMap(LoadSchema)},
        {"responses", (o, n) => o.Responses = n.CreateMapWithReference(ReferenceType.Response, LoadResponse)},
        {"parameters", (o, n) => o.Parameters = n.CreateMapWithReference(ReferenceType.Parameter, LoadParameter)},
        {"examples", (o, n) => o.Examples = n.CreateMapWithReference(ReferenceType.Example, LoadExample)},
        {"requestBodies", (o, n) => o.RequestBodies = n.CreateMapWithReference(ReferenceType.RequestBody, LoadRequestBody)},
        {"headers", (o, n) => o.Headers = n.CreateMapWithReference(ReferenceType.Header, LoadHeader)},
        {"securitySchemes", (o, n) => o.SecuritySchemes = n.CreateMapWithReference(ReferenceType.SecurityScheme, LoadSecurityScheme)},
        {"links", (o, n) => o.Links = n.CreateMapWithReference(ReferenceType.Link, LoadLink)},
        {"callbacks", (o, n) => o.Callbacks = n.CreateMapWithReference(ReferenceType.Callback, LoadCallback)},
        {"pathItems", (o, n) => o.PathItems = n.CreateMapWithReference(ReferenceType.PathItem, LoadPathItem)}
    };

        private static readonly PatternFieldMap<OpenApiComponents> _componentsPatternFields =
            new()
            {
            {s => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
            };

        public static OpenApiComponents LoadComponents(ParseNode node)
        {
            var mapNode = node.CheckMapNode("components");
            var components = new OpenApiComponents();

            ParseMap(mapNode, components, _componentsFixedFields, _componentsPatternFields);

            var servers = node.Context.GetFromTempStorage<IList<OpenApiServer>>(TempStorageKeys.Servers);
            var serverUrl = servers?.FirstOrDefault()?.Url;

            foreach (var schema in components.Schemas)
            {
                var refPath = serverUrl != null ? string.Concat(serverUrl, OpenApiConstants.V3ReferencedSchemaPath)
                    : OpenApiConstants.V3ReferenceUri;

                var refUri = new Uri(refPath + schema.Key);
                SchemaRegistry.Global.Register(refUri, schema.Value);
            }

            return components;
        }
    }
}
