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
        private static readonly FixedFieldMap<OpenApiMediaType> _mediaTypeFixedFields =
            new()
            {
                {
                    OpenApiConstants.Schema,
                    (o, n) => o.Schema = LoadSchema(n)
                },
                {
                    OpenApiConstants.Examples,
                    (o, n) => o.Examples = n.CreateMap(LoadExample)
                },
                {
                    OpenApiConstants.Example,
                    (o, n) => o.Example = n.CreateAny()
                },
                {
                    OpenApiConstants.Encoding,
                    (o, n) => o.Encoding = n.CreateMap(LoadEncoding)
                },
            };

        private static readonly PatternFieldMap<OpenApiMediaType> _mediaTypePatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
            };

        private static readonly AnyFieldMap<OpenApiMediaType> _mediaTypeAnyFields = new()
        {
            {
                OpenApiConstants.Example,
                new(
                    s => s.Example,
                    (s, v) => s.Example = v,
                    s => s.Schema)
            }
        };

        private static readonly AnyMapFieldMap<OpenApiMediaType, OpenApiExample> _mediaTypeAnyMapOpenApiExampleFields =
            new()
            {
            {
                OpenApiConstants.Examples,
                new(
                    m => m.Examples,
                    e => e.Value,
                    (e, v) => e.Value = v,
                    m => m.Schema)
            }
        };

        public static OpenApiMediaType LoadMediaType(ParseNode node)
        {
            var mapNode = node.CheckMapNode(OpenApiConstants.Content);

            var mediaType = new OpenApiMediaType();

            ParseMap(mapNode, mediaType, _mediaTypeFixedFields, _mediaTypePatternFields);

            ProcessAnyFields(mapNode, mediaType, _mediaTypeAnyFields);
            ProcessAnyMapFields(mapNode, mediaType, _mediaTypeAnyMapOpenApiExampleFields);

            return mediaType;
        }
    }
}
