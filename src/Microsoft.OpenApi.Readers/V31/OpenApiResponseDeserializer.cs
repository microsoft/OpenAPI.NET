using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiResponse> _responseFixedFields = new FixedFieldMap<OpenApiResponse>
        {
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "headers", (o, n) =>
                {
                    o.Headers = n.CreateMap(LoadHeader);
                }
            },
            {
                "content", (o, n) =>
                {
                    o.Content = n.CreateMap(LoadMediaType);
                }
            },
            {
                "links", (o, n) =>
                {
                    o.Links = n.CreateMap(LoadLink);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields =
            new PatternFieldMap<OpenApiResponse>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiResponse LoadResponse(ParseNode node)
        {
            var mapNode = node.CheckMapNode("response");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {

                var description = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Description);
                var summary = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Summary);

                return mapNode.GetReferencedObject<OpenApiResponse>(ReferenceType.Response, pointer, summary, description);
            }

            var response = new OpenApiResponse();
            ParseMap(mapNode, response, _responseFixedFields, _responsePatternFields);

            return response;
        }
    }
}
