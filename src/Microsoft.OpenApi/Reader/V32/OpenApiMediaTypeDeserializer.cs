using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
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
                    OpenApiConstants.ItemSchema, (o, n, t, c) =>
                    {
                        o.ItemSchema = LoadSchema(n, t, c);
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
                {
                    OpenApiConstants.ItemEncoding, (o, n, t, c) =>
                    {
                        o.ItemEncoding = LoadEncoding(n, t, c);
                    }
                },
                {
                    OpenApiConstants.PrefixEncoding, (o, n, t, c) =>
                    {
                        o.PrefixEncoding = n.CreateList(LoadEncoding, t, c);
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

        public static IOpenApiMediaType LoadMediaType(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode(OpenApiConstants.Content, context);

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var mediaTypeReference = new OpenApiMediaTypeReference(reference.Item1, hostDocument, reference.Item2);
                mediaTypeReference.Reference.SetMetadataFromJsonObject(jsonObject);
                return mediaTypeReference;
            }

            var mediaType = new OpenApiMediaType();

            ParseMap(jsonObject, mediaType, _mediaTypeFixedFields, _mediaTypePatternFields, hostDocument, context);

            ProcessAnyFields(mediaType, _mediaTypeAnyFields, context);
            ProcessAnyMapFields(mediaType, _mediaTypeAnyMapOpenApiExampleFields, context);

            return mediaType;
        }
    }
}
