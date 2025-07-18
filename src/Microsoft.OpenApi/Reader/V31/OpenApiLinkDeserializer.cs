﻿using System;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiLink> _linkFixedFields = new()
        {
            {
                "operationRef", (o, n, _) =>
                {
                    o.OperationRef = n.GetScalarValue();
                }
            },
            {
                "operationId", (o, n, _) =>
                {
                    o.OperationId = n.GetScalarValue();
                }
            },
            {
                "parameters", (o, n, _) =>
                {
                    o.Parameters = n.CreateSimpleMap(LoadRuntimeExpressionAnyWrapper);
                }
            },
            {
                "requestBody", (o, n, _) =>
                {
                    o.RequestBody = LoadRuntimeExpressionAnyWrapper(n);
                }
            },
            {
                "description", (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {"server", (o, n, t) => o.Server = LoadServer(n, t)}
        };

        private static readonly PatternFieldMap<OpenApiLink> _linkPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))},
        };

        public static IOpenApiLink LoadLink(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("link");
            var link = new OpenApiLink();

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var linkReference = new OpenApiLinkReference(reference.Item1, hostDocument, reference.Item2);
                linkReference.Reference.SetMetadataFromMapNode(mapNode);
                return linkReference;
            }

            ParseMap(mapNode, link, _linkFixedFields, _linkPatternFields, hostDocument);

            return link;
        }
    }
}
