using System;
using System.Text.Json.Nodes;

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
                    "description", (o, n, _, _) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "content", (o, n, t, c) =>
                    {
                        o.Content = n.CreateMap(LoadMediaType, t, c);
                    }
                },
                {
                    "required", (o, n, _, _) =>
                    {
                        var required = n.GetScalarValue();
                        if (required != null)
                        {
                            o.Required = bool.Parse(required);
                        }
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiRequestBody> _requestBodyPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static IOpenApiRequestBody LoadRequestBody(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("requestBody", context);

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var requestBodyReference = new OpenApiRequestBodyReference(reference.Item1, hostDocument, reference.Item2);
                requestBodyReference.Reference.SetMetadataFromJsonObject(jsonObject);
                return requestBodyReference;
            }

            var requestBody = new OpenApiRequestBody();
            ParseMap(jsonObject, requestBody, _requestBodyFixedFields, _requestBodyPatternFields, hostDocument, context);

            return requestBody;
        }
    }
}
