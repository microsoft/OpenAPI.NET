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
        public static FixedFieldMap<OpenApiPaths> PathsFixedFields = new FixedFieldMap<OpenApiPaths>();

        public static PatternFieldMap<OpenApiPaths> PathsPatternFields = new PatternFieldMap<OpenApiPaths>
        {
            {s => s.StartsWith("/"), (o, k, n) => o.Add(k, LoadPathItem(n))},
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
        };

        public static OpenApiPaths LoadPaths(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Paths");

            var domainObject = new OpenApiPaths();

            ParseMap(mapNode, domainObject, PathsFixedFields, PathsPatternFields);

            return domainObject;
        }
    }
}