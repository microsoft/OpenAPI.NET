// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using Microsoft.OpenApi.Any;
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
        private static readonly FixedFieldMap<OpenApiMediaType> MediaTypeFixedFields =
            new FixedFieldMap<OpenApiMediaType>
            {
                {
                    "schema", (o, n) =>
                    {
                        o.Schema = LoadSchema(n);
                    }
                },
                {
                    "examples", (o, n) =>
                    {
                        o.Examples = n.CreateMap(LoadExample);
                    }
                },
                {
                    "example", (o, n) =>
                    {
                        o.Example = new OpenApiString(n.GetScalarValue());
                    }
                },
                //Encoding
            };

        private static readonly PatternFieldMap<OpenApiMediaType> MediaTypePatternFields =
            new PatternFieldMap<OpenApiMediaType>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
            };

        public static OpenApiMediaType LoadMediaType(ParseNode node)
        {
            var mapNode = node.CheckMapNode("contentType");

            if (!mapNode.Any())
            {
                return null;
            }

            var contentType = new OpenApiMediaType();

            ParseMap(mapNode, contentType, MediaTypeFixedFields, MediaTypePatternFields);

            return contentType;
        }
    }
}