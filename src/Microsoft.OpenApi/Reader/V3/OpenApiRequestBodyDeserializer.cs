﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
        private static readonly FixedFieldMap<OpenApiRequestBody> _requestBodyFixedFields =
            new()
            {
                {
                    "description",
                    (o, n, _) => o.Description = n.GetScalarValue()
                },
                {
                    "content",
                    (o, n, t) => o.Content = n.CreateMap(LoadMediaType, t)
                },
                {
                    "required",
                    (o, n, _) =>
                    {
                        var required = n.GetScalarValue();
                        if (required != null)
                        {
                            o.Required = bool.Parse(required);
                        }
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiRequestBody> _requestBodyPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static IOpenApiRequestBody LoadRequestBody(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("requestBody");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiRequestBodyReference(reference.Item1, hostDocument, reference.Item2);
            }

            var requestBody = new OpenApiRequestBody();
            foreach (var property in mapNode)
            {
                property.ParseField(requestBody, _requestBodyFixedFields, _requestBodyPatternFields, hostDocument);
            }

            return requestBody;
        }
    }
}
