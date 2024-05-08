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
        private static readonly FixedFieldMap<OpenApiInfo> _infoFixedFields = new()
        {
            {
                "title",
                (o, n, _) => o.Title = n.GetScalarValue()
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
            },
            {
                "version",
                (o, n, _) => o.Version = n.GetScalarValue()
            }
        };

        private static readonly PatternFieldMap<OpenApiInfo> _infoPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        public static OpenApiInfo LoadInfo(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("Info");

            var info = new OpenApiInfo();

            ParseMap(mapNode, info, _infoFixedFields, _infoPatternFields);

            return info;
        }
    }
}
