// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        public static readonly FixedFieldMap<OpenApiResponses> ResponsesFixedFields = new();

        public static readonly PatternFieldMap<OpenApiResponses> ResponsesPatternFields = new()
        {
            {s => !s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, t) => o.Add(p, LoadResponse(n, t))},
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiResponses LoadResponses(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("Responses");

            var domainObject = new OpenApiResponses();

            ParseMap(mapNode, domainObject, ResponsesFixedFields, ResponsesPatternFields, hostDocument);

            return domainObject;
        }
    }
}
