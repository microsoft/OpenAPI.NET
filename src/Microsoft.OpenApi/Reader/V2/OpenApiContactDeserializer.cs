// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;

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
                (o, n, t, c) => o.Name = n.GetScalarValue()
            },
            {
                "url",
                (o, n, t, c) =>
                {
                    var url = n.GetScalarValue();
                    if (url != null)
                    {
                        o.Url = new(url, UriKind.RelativeOrAbsolute); 
                    }
                }
            },
            {
                "email",
                (o, n, t, c) => o.Email = n.GetScalarValue()
            },
        };

        private static readonly PatternFieldMap<OpenApiContact> _contactPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static OpenApiContact LoadContact(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node as JsonObject;
            var contact = new OpenApiContact();

            ParseMap(jsonObject, contact, _contactFixedFields, _contactPatternFields, hostDocument, context);

            return contact;
        }
    }
}
