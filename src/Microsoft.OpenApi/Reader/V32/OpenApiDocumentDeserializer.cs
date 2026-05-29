using System;
using System.Text.Json.Nodes;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
  {
        private static readonly FixedFieldMap<OpenApiDocument> _openApiFixedFields = new()
        {
            {
                "openapi", (o, n, _, c) =>
                {
                } /* Version is valid field but we already parsed it */
            },
            {"info", (o, n, _, c) => o.Info = LoadInfo(n, o, c)},
            {"jsonSchemaDialect", (o, n, _, c) => { if (n.GetScalarValue() is string {} sjsd && Uri.TryCreate(sjsd, UriKind.Absolute, out var jsd)) {o.JsonSchemaDialect = jsd;}} },
            {"$self", (o, n, _, c) => { if (n.GetScalarValue() is string {} sself && Uri.TryCreate(sself, UriKind.Absolute, out var self)) {o.Self = self;}} },
            {"servers", (o, n, _, c) => o.Servers = n.CreateList(LoadServer, o, c)},
            {"paths", (o, n, _, c) => o.Paths = LoadPaths(n, o, c)},
            {"webhooks", (o, n, _, c) => o.Webhooks = n.CreateMap(LoadPathItem, o, c)},
            {"components", (o, n, _, c) => o.Components = LoadComponents(n, o, c)},
            {"tags", (o, n, _, c) => { if (n.CreateList(LoadTag, o, c) is {Count:> 0} tags) {o.Tags = new HashSet<OpenApiTag>(tags, OpenApiTagComparer.Instance); } } },
            {"externalDocs", (o, n, _, c) => o.ExternalDocs = LoadExternalDocs(n, o, c)},
            {"security", (o, n, _, c) => o.Security = n.CreateList(LoadSecurityRequirement, o, c)}
        };

        private static readonly PatternFieldMap<OpenApiDocument> _openApiPatternFields = new()
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static OpenApiDocument LoadOpenApi(JsonNode jsonNode, Uri location, ParsingContext context)
        {
            var openApiDoc = new OpenApiDocument
            {
                BaseUri = location
            };
            var openApiNode = jsonNode.CheckMapNode("OpenAPI", context);

            ParseMap(openApiNode, openApiDoc, _openApiFixedFields, _openApiPatternFields, openApiDoc, context);

            // Register components
            openApiDoc.Workspace?.RegisterComponents(openApiDoc);

            return openApiDoc;
        }
    }
}

