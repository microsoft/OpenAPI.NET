using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

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
                "description", (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "headers", (o, n, t) =>
                {
                    o.Headers = n.CreateMap(LoadHeader, t);
                }
            },
            {
                "content", (o, n, t) =>
                {
                    o.Content = n.CreateMap(LoadMediaType, t);
                }
            },
            {
                "links", (o, n, t) =>
                {
                    o.Links = n.CreateMap(LoadLink, t);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiResponse LoadResponse(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("response");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiResponseReference(reference.Item1, hostDocument, reference.Item2);
            }

            var response = new OpenApiResponse();
            ParseMap(mapNode, response, _responseFixedFields, _responsePatternFields, hostDocument);

            return response;
        }
    }
}
