// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.OpenApiV2Deserializer
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        public static FixedFieldMap<OpenApiLicense> LicenseFixedFields = new FixedFieldMap<OpenApiLicense> {
            { "name", (o,n) => { o.Name = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute); } },
        };

        public static PatternFieldMap<OpenApiLicense> LicensePatternFields = new PatternFieldMap<OpenApiLicense>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new OpenApiString(n.GetScalarValue())) }
        };

        internal static OpenApiLicense LoadLicense(ParseNode node)
        {
            var mapNode = node.CheckMapNode("OpenApiLicense");

            var license = new OpenApiLicense();

            ParseMap(mapNode, license, LicenseFixedFields, LicensePatternFields);

            return license;
        }
    }
}
