// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiExternalDocs> _externalDocsFixedFields =
            new()
            {
                {
                    OpenApiConstants.Description,
                    (o, n, _) =>
                    {
                        var description = n.GetScalarValue();
                        if (description != null)
                        {
                            o.Description = n.GetScalarValue(); 
                        } 
                    }
                },
                {
                    OpenApiConstants.Url,
                    (o, n, _) =>
                    {
                        var url = n.GetScalarValue();
                        if (url != null)
                        {
                            o.Url = new(url, UriKind.RelativeOrAbsolute); 
                        } 
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiExternalDocs> _externalDocsPatternFields =
                new()
                {
                    {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
                    };

        public static OpenApiExternalDocs LoadExternalDocs(ParseNode node, OpenApiDocument? hostDocument)
        {
            var mapNode = node.CheckMapNode("externalDocs");

            var externalDocs = new OpenApiExternalDocs();

            ParseMap(mapNode, externalDocs, _externalDocsFixedFields, _externalDocsPatternFields, doc: hostDocument);

            return externalDocs;
        }
    }
}
