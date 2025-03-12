﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiPaths> _pathsFixedFields = new();

        private static readonly PatternFieldMap<OpenApiPaths> _pathsPatternFields = new()
        {
            {s => s.StartsWith("/", StringComparison.OrdinalIgnoreCase), (o, k, n, t) => o.Add(k, LoadPathItem(n, t))},
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        public static OpenApiPaths LoadPaths(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("Paths");

            var domainObject = new OpenApiPaths();

            ParseMap(mapNode, domainObject, _pathsFixedFields, _pathsPatternFields, doc: hostDocument);

            return domainObject;
        }
    }
}
