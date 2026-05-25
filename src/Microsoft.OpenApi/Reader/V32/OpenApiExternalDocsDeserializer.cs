using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        private static readonly FixedFieldMap<OpenApiExternalDocs> _externalDocsFixedFields =
            new()
            {
                // $ref
                {
                    "description", (o, n, _, c) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
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
            };

        private static readonly PatternFieldMap<OpenApiExternalDocs> _externalDocsPatternFields =
                new()
                {

                    {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
                    };

        public static OpenApiExternalDocs LoadExternalDocs(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("externalDocs", context);

            var externalDocs = new OpenApiExternalDocs();

            ParseMap(JsonObject, externalDocs, _externalDocsFixedFields, _externalDocsPatternFields, hostDocument, context);

            return externalDocs;
        }
    }
}

