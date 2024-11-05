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
        private static readonly FixedFieldMap<OpenApiContact> _contactFixedFields = new()
        {
            {
                "name",
                (o, n, t) => o.Name = n.GetScalarValue()
            },
            {
                "url",
                (o, n, t) => o.Url = new(n.GetScalarValue(), UriKind.RelativeOrAbsolute)
            },
            {
                "email",
                (o, n, t) => o.Email = n.GetScalarValue()
            },
        };

        private static readonly PatternFieldMap<OpenApiContact> _contactPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        public static OpenApiContact LoadContact(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node as MapNode;
            var contact = new OpenApiContact();

            ParseMap(mapNode, contact, _contactFixedFields, _contactPatternFields);

            return contact;
        }
    }
}
