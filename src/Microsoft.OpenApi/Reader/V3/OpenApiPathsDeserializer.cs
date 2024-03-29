﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
        private static readonly FixedFieldMap<OpenApiPaths> _pathsFixedFields = new();

        private static readonly PatternFieldMap<OpenApiPaths> _pathsPatternFields = new()
        {
            {s => s.StartsWith("/"), (o, k, n) => o.Add(k, LoadPathItem(n))},
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiPaths LoadPaths(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Paths");

            var domainObject = new OpenApiPaths();

            ParseMap(mapNode, domainObject, _pathsFixedFields, _pathsPatternFields);

            return domainObject;
        }
    }
}