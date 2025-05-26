using System;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiPaths> _pathsFixedFields = new();

        private static readonly PatternFieldMap<OpenApiPaths> _pathsPatternFields = new()
        {
            {s => s.StartsWith("/", StringComparison.OrdinalIgnoreCase), (o, k, n, t) => o.Add(k, LoadPathItem(n, t))},
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiPaths LoadPaths(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("Paths");

            var domainObject = new OpenApiPaths();

            ParseMap(mapNode, domainObject, _pathsFixedFields, _pathsPatternFields, hostDocument);

            return domainObject;
        }
    }
}
