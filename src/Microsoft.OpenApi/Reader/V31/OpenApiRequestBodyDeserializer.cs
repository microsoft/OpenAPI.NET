using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiRequestBody> _requestBodyFixedFields =
            new()
            {
                {
                    "description", (o, n, _) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "content", (o, n, t) =>
                    {
                        o.Content = n.CreateMap(LoadMediaType, t);
                    }
                },
                {
                    "required", (o, n, _) =>
                    {
                        o.Required = bool.Parse(n.GetScalarValue());
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiRequestBody> _requestBodyPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiRequestBody LoadRequestBody(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("requestBody");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiRequestBodyReference(reference.Item1, hostDocument, reference.Item2);
            }

            var requestBody = new OpenApiRequestBody();
            foreach (var property in mapNode)
            {
                property.ParseField(requestBody, _requestBodyFixedFields, _requestBodyPatternFields);
            }

            return requestBody;
        }
    }
}
