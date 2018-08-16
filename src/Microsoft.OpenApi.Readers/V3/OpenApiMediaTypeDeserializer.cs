// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Linq;
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
            new FixedFieldMap<OpenApiMediaType>
            {
                {
                    OpenApiConstants.Schema, (o, n) =>
                    {
                        o.Schema = LoadSchema(n);
                    }
                },
                {
                    OpenApiConstants.Examples, (o, n) =>
                    {
                        o.Examples = n.CreateMap(LoadExample);
                    }
                },
                {
                    OpenApiConstants.Example, (o, n) =>
                    {
                        o.Example = n.CreateAny();
                    }
                },
                {
                    OpenApiConstants.Encoding, (o, n) =>
                    {
                        o.Encoding = n.CreateMap(LoadEncoding);
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiMediaType> _mediaTypePatternFields =
            new PatternFieldMap<OpenApiMediaType>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiMediaType LoadMediaType(ParseNode node)
        {
            var mapNode = node.CheckMapNode(OpenApiConstants.Content);

            if (!mapNode.Any())
            {
                return null;
            }

            var mediaType = new OpenApiMediaType();

            ParseMap(mapNode, mediaType, _mediaTypeFixedFields, _mediaTypePatternFields);

            return mediaType;
        }
    }
}