// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Any;
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
        private static readonly FixedFieldMap<OpenApiRequestBody> RequestBodyFixedFields =
            new FixedFieldMap<OpenApiRequestBody>
            {
                {
                    "description", (o, n) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "content", (o, n) =>
                    {
                        o.Content = n.CreateMap(LoadMediaType);
                    }
                },
                {
                    "required", (o, n) =>
                    {
                        o.Required = bool.Parse(n.GetScalarValue());
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiRequestBody> RequestBodyPatternFields =
            new PatternFieldMap<OpenApiRequestBody>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
            };

        public static OpenApiRequestBody LoadRequestBody(ParseNode node)
        {
            var mapNode = node.CheckMapNode("requestBody");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiRequestBody>(ReferenceType.RequestBody, pointer);
            }

            var requestBody = new OpenApiRequestBody();
            foreach (var property in mapNode)
            {
                property.ParseField(requestBody, RequestBodyFixedFields, RequestBodyPatternFields);
            }

            return requestBody;
        }
    }
}