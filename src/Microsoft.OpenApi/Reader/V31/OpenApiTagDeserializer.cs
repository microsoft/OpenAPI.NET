// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text.Json.Nodes;

using System;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiTag> _tagFixedFields = new()
        {
            {
                OpenApiConstants.Name, (o, n, _, c) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                OpenApiConstants.Description, (o, n, _, c) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                OpenApiConstants.ExternalDocs, (o, n, t, c) =>
                {
                    o.ExternalDocs = LoadExternalDocs(n, t, c);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiTag> _tagPatternFields = new()
        {
            {
                s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase),
                (o, p, n, doc, c) =>
                {
                    if (p.Equals("x-oas-summary", StringComparison.OrdinalIgnoreCase))
                    {
                        o.Summary = n.GetScalarValue();
                    }
                    else if (p.Equals("x-oas-parent", StringComparison.OrdinalIgnoreCase))
                    {
                        var tagName = n.GetScalarValue();
                        if (tagName != null)
                        {
                            o.Parent = LoadTagByReference(tagName, doc);
                        }
                    }
                    else if (p.Equals("x-oas-kind", StringComparison.OrdinalIgnoreCase))
                    {
                        o.Kind = n.GetScalarValue();
                    }
                    else
                    {
                        o.AddExtension(p, LoadExtension(p, n, c));
                    }
                }
            }
        };

        public static OpenApiTag LoadTag(JsonNode n, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = n.CheckMapNode("tag", context);

            var domainObject = new OpenApiTag();

            ParseMap(jsonObject, domainObject, _tagFixedFields, _tagPatternFields, hostDocument, context);

            return domainObject;
        }
    }
}
