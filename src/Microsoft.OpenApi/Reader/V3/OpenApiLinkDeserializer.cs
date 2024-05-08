// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiLink> _linkFixedFields = new()
        {
            {
                "operationRef",
                (o, n, _) => o.OperationRef = n.GetScalarValue()
            },
            {
                "operationId",
                (o, n, _) => o.OperationId = n.GetScalarValue()
            },
            {
                "parameters",
                (o, n, _) => o.Parameters = n.CreateSimpleMap(LoadRuntimeExpressionAnyWrapper)
            },
            {
                "requestBody",
                (o, n, _) => o.RequestBody = LoadRuntimeExpressionAnyWrapper(n)
            },
            {
                "description",
                (o, n, _) => o.Description = n.GetScalarValue()
            },
            {"server", (o, n, t) => o.Server = LoadServer(n, t)}
        };

        private static readonly PatternFieldMap<OpenApiLink> _linkPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))},
        };

        public static OpenApiLink LoadLink(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("link");
            var link = new OpenApiLink();

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiLinkReference(reference.Item1, hostDocument, reference.Item2);
            }

            ParseMap(mapNode, link, _linkFixedFields, _linkPatternFields, hostDocument);

            return link;
        }
    }
}
