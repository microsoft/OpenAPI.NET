// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiResponse> ResponseFixedFields = new FixedFieldMap<OpenApiResponse>
        {
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "headers", (o, n) =>
                {
                    o.Headers = n.CreateMap(LoadHeader);
                }
            },
            {
                "examples", (o, n) =>
                {
                    /*o.Examples = ((ListNode)n).Select(s=> new AnyNode(s)).ToList();*/
                }
            },
            {
                "schema", (o, n) =>
                {
                    n.Context.SetTempStorage("operationschema", LoadSchema(n));
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiResponse> ResponsePatternFields =
            new PatternFieldMap<OpenApiResponse>
            {
                {s => s.StartsWith("x-"), (o, k, n) => o.Extensions.Add(k, new OpenApiString(n.GetScalarValue()))},
            };

        public static OpenApiResponse LoadResponse(ParseNode node)
        {
            var mapNode = node.CheckMapNode("response");

            var response = new OpenApiResponse();
            foreach (var property in mapNode)
            {
                property.ParseField(response, ResponseFixedFields, ResponsePatternFields);
            }

            ProcessProduces(response, node.Context);

            return response;
        }

        private static void ProcessProduces(OpenApiResponse response, ParsingContext context)
        {
            var produces = context.GetTempStorage<List<string>>("operationproduces") ??
                context.GetTempStorage<List<string>>("globalproduces") ?? new List<string> {"application/json"};

            response.Content = new Dictionary<string, OpenApiMediaType>();
            foreach (var mt in produces)
            {
                var schema = context.GetTempStorage<OpenApiSchema>("operationschema");
                OpenApiMediaType mediaType = null;
                if (schema != null)
                {
                    mediaType = new OpenApiMediaType
                    {
                        Schema = schema
                    };
                }
                response.Content.Add(mt, mediaType);
            }
        }
    }
}