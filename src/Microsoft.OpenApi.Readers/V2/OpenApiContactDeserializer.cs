// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Extensions;
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
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
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