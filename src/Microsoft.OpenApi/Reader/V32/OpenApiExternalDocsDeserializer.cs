using System;

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
                    "description", (o, n, _) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "url",
                    (o, n, t) =>
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

                    {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
                    };

        public static OpenApiExternalDocs LoadExternalDocs(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("externalDocs");

            var externalDocs = new OpenApiExternalDocs();

            ParseMap(mapNode, externalDocs, _externalDocsFixedFields, _externalDocsPatternFields, hostDocument);

            return externalDocs;
        }
    }
}

