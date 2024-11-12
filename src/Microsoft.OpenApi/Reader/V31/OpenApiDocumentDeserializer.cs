using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
  {
        private static readonly FixedFieldMap<OpenApiDocument> _openApiFixedFields = new()
        {
            {
                "openapi", (o, n, _) =>
                {
                } /* Version is valid field but we already parsed it */
            },
            {"info", (o, n, _) => o.Info = LoadInfo(n, o)},
            {"jsonSchemaDialect", (o, n, _) => o.JsonSchemaDialect = n.GetScalarValue() },
            {"servers", (o, n, _) => o.Servers = n.CreateList(LoadServer, o)},
            {"paths", (o, n, _) => o.Paths = LoadPaths(n, o)},
            {"webhooks", (o, n, _) => o.Webhooks = n.CreateMap(LoadPathItem, o)},
            {"components", (o, n, _) => o.Components = LoadComponents(n, o)},
            {"tags", (o, n, _) => {o.Tags = n.CreateList(LoadTag, o);
                foreach (var tag in o.Tags)
    {
                    tag.Reference = new OpenApiReference()
                    {
                        Id = tag.Name,
                        Type = ReferenceType.Tag
                    };
    }
            } },
            {"externalDocs", (o, n, _) => o.ExternalDocs = LoadExternalDocs(n, o)},
            {"security", (o, n, _) => o.SecurityRequirements = n.CreateList(LoadSecurityRequirement, o)}
        };

        private static readonly PatternFieldMap<OpenApiDocument> _openApiPatternFields = new()
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        public static OpenApiDocument LoadOpenApi(RootNode rootNode)
        {
            var openApiDoc = new OpenApiDocument();
            var openApiNode = rootNode.GetMap();

            ParseMap(openApiNode, openApiDoc, _openApiFixedFields, _openApiPatternFields, openApiDoc);

            // Register components
            openApiDoc.Workspace.RegisterComponents(openApiDoc);

            return openApiDoc;
        }
    }
}
