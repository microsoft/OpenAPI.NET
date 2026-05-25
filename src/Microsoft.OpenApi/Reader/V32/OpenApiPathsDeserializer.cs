using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        private static readonly FixedFieldMap<OpenApiPaths> _pathsFixedFields = new();

        private static readonly PatternFieldMap<OpenApiPaths> _pathsPatternFields = new()
        {
            {s => s.StartsWith("/", StringComparison.OrdinalIgnoreCase), (o, k, n, t, c) => o.Add(k, LoadPathItem(n, t, c))},
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static OpenApiPaths LoadPaths(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("Paths", context);

            var domainObject = new OpenApiPaths();

            ParseMap(JsonObject, domainObject, _pathsFixedFields, _pathsPatternFields, hostDocument, context);

            return domainObject;
        }
    }
}

