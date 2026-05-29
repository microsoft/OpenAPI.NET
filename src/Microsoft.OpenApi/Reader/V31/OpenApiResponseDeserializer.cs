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
        private static readonly FixedFieldMap<OpenApiResponse> _responseFixedFields = new()
        {
            {
                "description", (o, n, _, c) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "headers", (o, n, t, c) =>
                {
                    o.Headers = n.CreateMap(LoadHeader, t, c);
                }
            },
            {
                "content", (o, n, t, c) =>
                {
                    o.Content = n.CreateMap(LoadMediaType, t, c);
                }
            },
            {
                "links", (o, n, t, c) =>
                {
                    o.Links = n.CreateMap(LoadLink, t, c);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => 
                {
                    if (p.Equals("x-oai-summary", StringComparison.OrdinalIgnoreCase))
                    {
                        o.Summary = n.GetScalarValue();
                    }
                    else
                    {
                        o.AddExtension(p, LoadExtension(p, n, c));
                    }
                }}
            };

        public static IOpenApiResponse LoadResponse(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("response", context);

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var responseReference = new OpenApiResponseReference(reference.Item1, hostDocument, reference.Item2);
                responseReference.Reference.SetMetadataFromJsonObject(jsonObject);
                return responseReference;
            }

            var response = new OpenApiResponse();
            ParseMap(jsonObject, response, _responseFixedFields, _responsePatternFields, hostDocument, context);

            return response;
        }
    }
}
