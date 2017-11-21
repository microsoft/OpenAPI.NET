﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

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
        private static readonly FixedFieldMap<OpenApiLink> LinkFixedFields = new FixedFieldMap<OpenApiLink>
        {
            {
                "operationRef", (o, n) =>
                {
                    o.OperationRef = n.GetScalarValue();
                }
            },
            {
                "operationId", (o, n) =>
                {
                    o.OperationId = n.GetScalarValue();
                }
            },
            {
                "parameters", (o, n) =>
                {
                    o.Parameters = n.CreateSimpleMap(LoadRuntimeExpressionAnyWrapper);
                }
            },
            {
                "requestBody", (o, n) =>
                {
                    o.RequestBody = LoadRuntimeExpressionAnyWrapper(n);
                }
            },
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {"server", (o, n) => o.Server = LoadServer(n)}
        };

        private static readonly PatternFieldMap<OpenApiLink> LinkPatternFields = new PatternFieldMap<OpenApiLink>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())},
        };

        public static OpenApiLink LoadLink(ParseNode node)
        {
            var mapNode = node.CheckMapNode("link");
            var link = new OpenApiLink();

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiLink>(ReferenceType.Link, pointer);
            }

            ParseMap(mapNode, link, LinkFixedFields, LinkPatternFields);

            return link;
        }
    }
}