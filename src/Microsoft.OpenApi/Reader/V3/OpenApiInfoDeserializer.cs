// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        public static readonly FixedFieldMap<OpenApiInfo> InfoFixedFields = new()
        {
            {
                "title",
                (o, n, _) => o.Title = n.GetScalarValue()
            },
            {
                "version",
                (o, n, _) => o.Version = n.GetScalarValue()
            },
            {
                "description",
                (o, n, _) => o.Description = n.GetScalarValue()
            },
            {
                "termsOfService",
                (o, n, _) => o.TermsOfService = new(n.GetScalarValue(), UriKind.RelativeOrAbsolute)
            },
            {
                "contact",
                (o, n, t) => o.Contact = LoadContact(n, t)
            },
            {
                "license",
                (o, n, t) => o.License = LoadLicense(n, t)
            }
        };

        public static readonly PatternFieldMap<OpenApiInfo> InfoPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, k, n, _) => o.AddExtension(k,LoadExtension(k, n))}
        };

        public static OpenApiInfo LoadInfo(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("Info");
            var info = new OpenApiInfo();
            ParseMap(mapNode, info, InfoFixedFields, InfoPatternFields, hostDocument);

            return info;
        }
    }
}
