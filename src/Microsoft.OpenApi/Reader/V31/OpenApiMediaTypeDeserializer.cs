using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiMediaType> _mediaTypeFixedFields =
            new()
            {
                {
                    OpenApiConstants.Schema, (o, n, t, c) =>
                    {
                        o.Schema = LoadSchema(n, t, c);
                    }
                },
                {
                    OpenApiConstants.Examples, (o, n, t, c) =>
                    {
                        o.Examples = n.CreateMap(LoadExample, t, c);
                    }
                },
                {
                    OpenApiConstants.Example, (o, n, _, c) =>
                    {
                        o.Example = n;
                    }
                },
                {
                    OpenApiConstants.Encoding, (o, n, t, c) =>
                    {
                        o.Encoding = n.CreateMap(LoadEncoding, t, c);
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiMediaType> _mediaTypePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        private static readonly AnyFieldMap<OpenApiMediaType> _mediaTypeAnyFields = new AnyFieldMap<OpenApiMediaType>
        {
            {
                OpenApiConstants.Example,
                new AnyFieldMapParameter<OpenApiMediaType>(
                    s => s.Example,
                    (s, v) => s.Example = v,
                    s => s.Schema)
            }
        };


        private static readonly AnyMapFieldMap<OpenApiMediaType, IOpenApiExample> _mediaTypeAnyMapOpenApiExampleFields =
            new AnyMapFieldMap<OpenApiMediaType, IOpenApiExample>
        {
            {
                OpenApiConstants.Examples,
                new AnyMapFieldMapParameter<OpenApiMediaType, IOpenApiExample>(
                    m => m.Examples,
                    e => e.Value,
                    (e, v) => {if (e is OpenApiExample ex) {ex.Value = v;}},
                    m => m.Schema)
            }
        };

        public static OpenApiMediaType LoadMediaType(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode(OpenApiConstants.Content, context);

            var mediaType = new OpenApiMediaType();

            ParseMap(JsonObject, mediaType, _mediaTypeFixedFields, _mediaTypePatternFields, hostDocument, context);

            ProcessAnyFields(mediaType, _mediaTypeAnyFields, context);
            ProcessAnyMapFields(mediaType, _mediaTypeAnyMapOpenApiExampleFields, context);

            return mediaType;
        }
    }
}
