// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text.Json.Nodes;

using System;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields =
            new()
            {
                {
                    "type", (o, n, _, c) =>
                    {
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<SecuritySchemeType>(c, out var type))
                        {
                            return;
                        }
                        o.Type = type;
                    }
                },
                {
                    "description", (o, n, _, c) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "name", (o, n, _, c) =>
                    {
                        o.Name = n.GetScalarValue();
                    }
                },
                {
                    "in", (o, n, _, c) =>
                    {
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterLocation>(c, out var _in))
                        {
                            return;
                        }
                        o.In = _in;
                    }
                },
                {
                    "scheme", (o, n, _, c) =>
                    {
                        o.Scheme = n.GetScalarValue();
                    }
                },
                {
                    "bearerFormat", (o, n, _, c) =>
                    {
                        o.BearerFormat = n.GetScalarValue();
                    }
                },
                {
                    "openIdConnectUrl", (o, n, _, c) =>
                    {
                        var connectUrl = n.GetScalarValue();
                        if (connectUrl != null)
                        {
                            o.OpenIdConnectUrl = new(connectUrl, UriKind.RelativeOrAbsolute);
                        }
                    }
                },
                {
                    "oauth2MetadataUrl", (o, n, _, c) =>
                    {
                        var metadataUrl = n.GetScalarValue();
                        if (metadataUrl != null)
                        {
                            o.OAuth2MetadataUrl = new(metadataUrl, UriKind.RelativeOrAbsolute);
                        }
                    }
                },
                {
                    "flows", (o, n, t, c) =>
                    {
                        o.Flows = LoadOAuthFlows(n, t, c);
                    }
                },
                {
                    "deprecated", (o, n, _, c) =>
                    {
                        var deprecated = n.GetScalarValue();
                        if (deprecated != null)
                        {
                            o.Deprecated = bool.Parse(deprecated);
                        }
                    }
                }
            };

        private static readonly PatternFieldMap<OpenApiSecurityScheme> _securitySchemePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static IOpenApiSecurityScheme LoadSecurityScheme(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("securityScheme", context);

            var pointer = JsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var securitySchemeReference = new OpenApiSecuritySchemeReference(reference.Item1, hostDocument, reference.Item2);
                securitySchemeReference.Reference.SetMetadataFromJsonObject(JsonObject);
                return securitySchemeReference;
            }

            var securityScheme = new OpenApiSecurityScheme();
            ParseMap(JsonObject, securityScheme, _securitySchemeFixedFields, _securitySchemePatternFields, hostDocument, context);

            return securityScheme;
        }
    }
}

