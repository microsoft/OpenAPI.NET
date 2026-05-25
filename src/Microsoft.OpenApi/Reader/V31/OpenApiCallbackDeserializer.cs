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
        private static readonly FixedFieldMap<OpenApiCallback> _callbackFixedFields =
            new();

        private static readonly PatternFieldMap<OpenApiCallback> _callbackPatternFields =
            new()
            {
            {s => !s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, t, c) => o.AddPathItem(RuntimeExpression.Build(p), LoadPathItem(n, t, c))},
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))},
            };

        public static IOpenApiCallback LoadCallback(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("callback", context);

            if (JsonObject.GetReferencePointer() is { } pointer)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var callbackReference = new OpenApiCallbackReference(reference.Item1, hostDocument, reference.Item2);
                callbackReference.Reference.SetMetadataFromJsonObject(JsonObject);
                return callbackReference;
            }

            var domainObject = new OpenApiCallback();

            ParseMap(JsonObject, domainObject, _callbackFixedFields, _callbackPatternFields, hostDocument, context);

            return domainObject;
        }
    }
}
