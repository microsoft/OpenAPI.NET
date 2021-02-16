using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V2OpenApiDocumentSerializer : IOpenApiElementSerializer<OpenApiDocument>
    {
        private readonly IOpenApiElementSerializer<OpenApiInfo> _infoSerializer;

        private readonly IOpenApiElementSerializer<OpenApiPaths> _pathsSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiSchema> _schemaReferenceSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiParameter> _parameterReferenceSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiResponse> _responseReferenceSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiSecurityScheme> _securitySchemeReferenceSerializer;

        private readonly IOpenApiElementSerializer<OpenApiSecurityRequirement> _securityRequirementSerializer;

        private readonly IOpenApiElementSerializer<OpenApiExternalDocs> _externalDocsSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiTag> _tagSerializer;

        public V2OpenApiDocumentSerializer(
            IOpenApiElementSerializer<OpenApiInfo> infoSerializer,
            IOpenApiElementSerializer<OpenApiPaths> pathsSerializer,
            IOpenApiReferenceElementSerializer<OpenApiSchema> schemaReferenceSerializer,
            IOpenApiReferenceElementSerializer<OpenApiParameter> parameterReferenceSerializer,
            IOpenApiReferenceElementSerializer<OpenApiResponse> responseReferenceSerializer,
            IOpenApiReferenceElementSerializer<OpenApiSecurityScheme> securitySchemeReferenceSerializer,
            IOpenApiElementSerializer<OpenApiSecurityRequirement> securityRequirementSerializer,
            IOpenApiElementSerializer<OpenApiExternalDocs> externalDocsSerializer,
            IOpenApiReferenceElementSerializer<OpenApiTag> tagSerializer)
        {
            _infoSerializer = infoSerializer;
            _pathsSerializer = pathsSerializer;
            _schemaReferenceSerializer = schemaReferenceSerializer;
            _parameterReferenceSerializer = parameterReferenceSerializer;
            _responseReferenceSerializer = responseReferenceSerializer;
            _securitySchemeReferenceSerializer = securitySchemeReferenceSerializer;
            _securityRequirementSerializer = securityRequirementSerializer;
            _externalDocsSerializer = externalDocsSerializer;
            _tagSerializer = tagSerializer;
        }

        public void Serialize(OpenApiDocument element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // swagger
            writer.WriteProperty(OpenApiConstants.Swagger, "2.0");

            // info
            writer.WriteRequiredObject(OpenApiConstants.Info, element.Info, (w, i) => _infoSerializer.Serialize(i, w));

            // host, basePath, schemes, consumes, produces
            WriteHostInfoV2(writer, element.Servers);

            // paths
            writer.WriteRequiredObject(OpenApiConstants.Paths, element.Paths, (w, p) => _pathsSerializer.Serialize(p, w));

            // If references have been inlined we don't need the to render the components section
            // however if they have cycles, then we will need a component rendered
            if (writer.GetSettings().ReferenceInline != ReferenceInlineSetting.DoNotInlineReferences)
            {
                var loops = writer.GetSettings().LoopDetector.Loops;

                if (loops.TryGetValue(typeof(OpenApiSchema), out List<object> schemas))
                {
                    var openApiSchemas = schemas.Cast<OpenApiSchema>().Distinct().ToList()
                        .ToDictionary<OpenApiSchema, string>(k => k.Reference.Id);

                    foreach (var schema in openApiSchemas.Values.ToList())
                    {
                        FindSchemaReferences.ResolveSchemas(element.Components, openApiSchemas);
                    }

                    writer.WriteOptionalMap(
                       OpenApiConstants.Definitions,
                       openApiSchemas,
                       (w, key, component) =>
                       {
                           _schemaReferenceSerializer.SerializeWithoutReference(component, w);
                       });
                }
            }
            else
            {
                // Serialize each referenceable object as full object without reference if the reference in the object points to itself. 
                // If the reference exists but points to other objects, the object is serialized to just that reference.
                // definitions
                writer.WriteOptionalMap(
                    OpenApiConstants.Definitions,
                    element.Components?.Schemas,
                    (w, key, component) =>
                    {
                        if (component.Reference != null &&
                            component.Reference.Type == ReferenceType.Schema &&
                            component.Reference.Id == key)
                        {
                            _schemaReferenceSerializer.SerializeWithoutReference(component, w);
                        }
                        else
                        {
                            _schemaReferenceSerializer.Serialize(component, w);
                        }
                    });
            }
            // parameters
            writer.WriteOptionalMap(
                OpenApiConstants.Parameters,
                element.Components?.Parameters,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Parameter &&
                        component.Reference.Id == key)
                    {
                        _parameterReferenceSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _parameterReferenceSerializer.Serialize(component, w);
                    }
                });

            // responses
            writer.WriteOptionalMap(
                OpenApiConstants.Responses,
                element.Components?.Responses,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Response &&
                        component.Reference.Id == key)
                    {
                        _responseReferenceSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _responseReferenceSerializer.Serialize(component, w);
                    }
                });

            // securityDefinitions
            writer.WriteOptionalMap(
                OpenApiConstants.SecurityDefinitions,
                element.Components?.SecuritySchemes,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.SecurityScheme &&
                        component.Reference.Id == key)
                    {
                        _securitySchemeReferenceSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _securitySchemeReferenceSerializer.Serialize(component, w);
                    }
                });

            // security
            writer.WriteOptionalCollection(
                OpenApiConstants.Security,
                element.SecurityRequirements,
                (w, s) => _securityRequirementSerializer.Serialize(s, w));

            // tags
            writer.WriteOptionalCollection(OpenApiConstants.Tags, element.Tags, (w, t) => _tagSerializer.SerializeWithoutReference(t, w));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, element.ExternalDocs, (w, e) => _externalDocsSerializer.Serialize(e, w));

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        private static void WriteHostInfoV2(IOpenApiWriter writer, IList<OpenApiServer> servers)
        {
            if (servers == null || !servers.Any())
            {
                return;
            }

            // Arbitrarily choose the first server given that V2 only allows 
            // one host, port, and base path.
            var firstServer = servers.First();

            // Divide the URL in the Url property into host and basePath required in OpenAPI V2
            // The Url property cannotcontain path templating to be valid for V2 serialization.
            var firstServerUrl = new Uri(firstServer.Url, UriKind.RelativeOrAbsolute);

            // host
            if (firstServerUrl.IsAbsoluteUri)
            {
                writer.WriteProperty(
                    OpenApiConstants.Host,
                    firstServerUrl.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped));

                // basePath
                if (firstServerUrl.AbsolutePath != "/")
                {
                    writer.WriteProperty(OpenApiConstants.BasePath, firstServerUrl.AbsolutePath);
                }
            }
            else
            {
                var relativeUrl = firstServerUrl.OriginalString;
                if (relativeUrl.StartsWith("//"))
                {
                    var pathPosition = relativeUrl.IndexOf('/', 3);
                    writer.WriteProperty(OpenApiConstants.Host, relativeUrl.Substring(0, pathPosition));
                    relativeUrl = relativeUrl.Substring(pathPosition);
                }
                if (!String.IsNullOrEmpty(relativeUrl) && relativeUrl != "/")
                {
                    writer.WriteProperty(OpenApiConstants.BasePath, relativeUrl);
                }
            }

            // Consider all schemes of the URLs in the server list that have the same
            // host, port, and base path as the first server.
            var schemes = servers.Select(
                    s =>
                    {
                        Uri.TryCreate(s.Url, UriKind.RelativeOrAbsolute, out var url);
                        return url;
                    })
                .Where(
                    u => Uri.Compare(
                            u,
                            firstServerUrl,
                            UriComponents.Host | UriComponents.Port | UriComponents.Path,
                            UriFormat.SafeUnescaped,
                            StringComparison.OrdinalIgnoreCase) ==
                        0 && u.IsAbsoluteUri)
                .Select(u => u.Scheme)
                .Distinct()
                .ToList();

            // schemes
            writer.WriteOptionalCollection(OpenApiConstants.Schemes, schemes, (w, s) => w.WriteValue(s));
        }

        internal class FindSchemaReferences : OpenApiVisitorBase
        {
            private Dictionary<string, OpenApiSchema> Schemas;

            public static void ResolveSchemas(OpenApiComponents components, Dictionary<string, OpenApiSchema> schemas)
            {
                var visitor = new FindSchemaReferences();
                visitor.Schemas = schemas;
                var walker = new OpenApiWalker(visitor);
                walker.Walk(components);
            }

            public override void Visit(IOpenApiReferenceable referenceable)
            {
                switch (referenceable)
                {
                    case OpenApiSchema schema:
                        if (!Schemas.ContainsKey(schema.Reference.Id))
                        {
                            Schemas.Add(schema.Reference.Id, schema);
                        }
                        break;

                    default:
                        break;
                }
                base.Visit(referenceable);
            }

            public override void Visit(OpenApiSchema schema)
            {
                // This is needed to handle schemas used in Responses in components
                if (schema.Reference != null)
                {
                    if (!Schemas.ContainsKey(schema.Reference.Id))
                    {
                        Schemas.Add(schema.Reference.Id, schema);
                    }
                }
                base.Visit(schema);
            }
        }
    }
}
