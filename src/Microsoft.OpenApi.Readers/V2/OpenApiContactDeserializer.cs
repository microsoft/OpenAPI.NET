// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        public static FixedFieldMap<OpenApiContact> ContactFixedFields = new FixedFieldMap<OpenApiContact>
        {
            {
                "name", (o, n) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                "url", (o, n) =>
                {
                    o.Url = new Uri(n.GetScalarValue());
                }
            },
            {
                "email", (o, n) =>
                {
                    o.Email = n.GetScalarValue();
                }
            },
        };

        public static PatternFieldMap<OpenApiContact> ContactPatternFields = new PatternFieldMap<OpenApiContact>
        {
            {s => s.StartsWith("x-"), (o, k, n) => o.Extensions.Add(k, new OpenApiString(n.GetScalarValue()))}
        };

        public static OpenApiContact LoadContact(ParseNode node)
        {
            var mapNode = node as MapNode;
            var contact = new OpenApiContact();

            ParseMap(mapNode, contact, ContactFixedFields, ContactPatternFields);

            return contact;
        }
    }
}