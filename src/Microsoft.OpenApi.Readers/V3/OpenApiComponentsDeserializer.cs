// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiComponents> _componentsFixedFields = new()
        {
            {"schemas", (o, n) => o.Schemas =  n.CreateJsonSchemaMapWithReference(ReferenceType.Schema, LoadSchema, OpenApiSpecVersion.OpenApi3_0)},
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
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
            };

        public static OpenApiComponents LoadComponents(ParseNode node)
        {
            var mapNode = node.CheckMapNode("components");
            var components = new OpenApiComponents();

            ParseMap(mapNode, components, _componentsFixedFields, _componentsPatternFields);

            var servers = node.Context.GetFromTempStorage<IList<OpenApiServer>>(TempStorageKeys.Servers);
            var serverUrl = servers?.FirstOrDefault()?.Url;

            // Examples of server url ----> "https://graph.microsoft.com/v1.0", "https://graph.microsoft.com/v1.0-fairfax"
            foreach (var schema in components.Schemas)
            {
                var refPath = !string.IsNullOrEmpty(serverUrl) ? string.Concat(serverUrl, OpenApiConstants.V3ReferencedSchemaPath)
                    : OpenApiConstants.V3ReferenceUri;

                var refUri = new Uri(refPath + schema.Key);
                SchemaRegistry.Global.Register(refUri, schema.Value);
            }

            return components;
        }
    }
}
