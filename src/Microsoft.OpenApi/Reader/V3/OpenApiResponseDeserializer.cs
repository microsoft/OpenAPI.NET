// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiResponse> _responseFixedFields = new()
        {
            {
                "description",
                (o, n, _) => o.Description = n.GetScalarValue()
            },
            {
                "headers",
                (o, n, t) => o.Headers = n.CreateMap(LoadHeader, t)
            },
            {
                "content",
                 (o, n, t) => o.Content = n.CreateMap(LoadMediaType, t)
            },
            {
                "links",
                (o, n, t) => o.Links = n.CreateMap(LoadLink, t)
            }
        };

        private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => 
                {
                    if (p.Equals("x-oai-summary", StringComparison.OrdinalIgnoreCase))
                    {
                        o.Summary = n.GetScalarValue();
                    }
                    else
                    {
                        o.AddExtension(p, LoadExtension(p,n));
                    }
                }}
            };

        public static IOpenApiResponse LoadResponse(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("response");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiResponseReference(reference.Item1, hostDocument, reference.Item2);
            }

            var response = new OpenApiResponse();
            ParseMap(mapNode, response, _responseFixedFields, _responsePatternFields, hostDocument);

            return response;
        }
    }
}
