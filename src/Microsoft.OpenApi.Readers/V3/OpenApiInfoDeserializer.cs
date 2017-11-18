﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public static FixedFieldMap<OpenApiInfo> InfoFixedFields = new FixedFieldMap<OpenApiInfo>
        {
            {
                "title", (o, n) =>
                {
                    o.Title = n.GetScalarValue();
                }
            },
            {
                "version", (o, n) =>
                {
                    o.Version = n.GetScalarValue();
                }
            },
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "termsOfService", (o, n) =>
                {
                    o.TermsOfService = new Uri(n.GetScalarValue());
                }
            },
            {
                "contact", (o, n) =>
                {
                    o.Contact = LoadContact(n);
                }
            },
            {
                "license", (o, n) =>
                {
                    o.License = LoadLicense(n);
                }
            }
        };

        public static PatternFieldMap<OpenApiInfo> InfoPatternFields = new PatternFieldMap<OpenApiInfo>
        {
            {s => s.StartsWith("x-"), (o, k, n) => o.AddExtension(k, n.CreateAny())}
        };

        public static OpenApiInfo LoadInfo(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Info");

            var info = new OpenApiInfo();
            var required = new List<string> {"title", "version"};

            ParseMap(mapNode, info, InfoFixedFields, InfoPatternFields, required);

            return info;
        }
    }
}