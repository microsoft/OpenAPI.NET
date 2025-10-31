using System;

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
                    OpenApiConstants.Schema, (o, n, t) =>
                    {
                        o.Schema = LoadSchema(n, t);
                    }
                },
                {
                    OpenApiConstants.ItemSchema, (o, n, t) =>
                    {
                        o.ItemSchema = LoadSchema(n, t);
                    }
                },
                {
                    OpenApiConstants.Examples, (o, n, t) =>
                    {
                        o.Examples = n.CreateMap(LoadExample, t);
                    }
                },
                {
                    OpenApiConstants.Example, (o, n, _) =>
                    {
                        o.Example = n.CreateAny();
                    }
                },
                {
                    OpenApiConstants.Encoding, (o, n, t) =>
                    {
                        o.Encoding = n.CreateMap(LoadEncoding, t);
                    }
                },
                {
                    OpenApiConstants.ItemEncoding, (o, n, t) =>
                    {
                        o.ItemEncoding = LoadEncoding(n, t);
                    }
                },
                {
                    OpenApiConstants.PrefixEncoding, (o, n, t) =>
                    {
                        o.PrefixEncoding = n.CreateList(LoadEncoding, t);
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiMediaType> _mediaTypePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
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

        public static IOpenApiMediaType LoadMediaType(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode(OpenApiConstants.Content);

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var mediaTypeReference = new OpenApiMediaTypeReference(reference.Item1, hostDocument, reference.Item2);
                mediaTypeReference.Reference.SetMetadataFromMapNode(mapNode);
                return mediaTypeReference;
            }

            var mediaType = new OpenApiMediaType();

            ParseMap(mapNode, mediaType, _mediaTypeFixedFields, _mediaTypePatternFields, hostDocument);

            ProcessAnyFields(mapNode, mediaType, _mediaTypeAnyFields);
            ProcessAnyMapFields(mapNode, mediaType, _mediaTypeAnyMapOpenApiExampleFields);

            return mediaType;
        }
    }
}

