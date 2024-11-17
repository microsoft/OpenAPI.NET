// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Writers;

#nullable enable

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Describes an OpenAPI object (OpenAPI document). See: https://spec.openapis.org
    /// </summary>
    public class OpenApiDocument : IOpenApiSerializable, IOpenApiExtensible, IOpenApiAnnotatable
    {
        /// <summary>
        /// Related workspace containing components that are referenced in a document
        /// </summary>
        public OpenApiWorkspace? Workspace { get; set; }

        /// <summary>
        /// REQUIRED. Provides metadata about the API. The metadata MAY be used by tooling as required.
        /// </summary>
        public OpenApiInfo Info { get; set; }

        /// <summary>
        /// The default value for the $schema keyword within Schema Objects contained within this OAS document. This MUST be in the form of a URI.
        /// </summary>
        public string? JsonSchemaDialect { get; set; }

        /// <summary>
        /// An array of Server Objects, which provide connectivity information to a target server.
        /// </summary>
        public IList<OpenApiServer>? Servers { get; set; } = new List<OpenApiServer>();

        /// <summary>
        /// REQUIRED. The available paths and operations for the API.
        /// </summary>
        public OpenApiPaths Paths { get; set; }

        /// <summary>
        /// The incoming webhooks that MAY be received as part of this API and that the API consumer MAY choose to implement.
        /// A map of requests initiated other than by an API call, for example by an out of band registration. 
        /// The key name is a unique string to refer to each webhook, while the (optionally referenced) Path Item Object describes a request that may be initiated by the API provider and the expected responses
        /// </summary>
        public IDictionary<string, OpenApiPathItem>? Webhooks { get; set; } = new Dictionary<string, OpenApiPathItem>();

        /// <summary>
        /// An element to hold various schemas for the specification.
        /// </summary>
        public OpenApiComponents? Components { get; set; }

        /// <summary>
        /// A declaration of which security mechanisms can be used across the API.
        /// </summary>
        public IList<OpenApiSecurityRequirement>? SecurityRequirements { get; set; } =
            new List<OpenApiSecurityRequirement>();

        /// <summary>
        /// A list of tags used by the specification with additional metadata.
        /// </summary>
        public IList<OpenApiTag>? Tags { get; set; } = new List<OpenApiTag>();

        /// <summary>
        /// Additional external documentation.
        /// </summary>
        public OpenApiExternalDocs? ExternalDocs { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// The unique hash code of the generated OpenAPI document
        /// </summary>
        public string HashCode => GenerateHashValue(this);

        /// <inheritdoc />
        public IDictionary<string, object>? Annotations { get; set; }

        /// <summary>
        /// Implements IBaseDocument
        /// </summary>
        public Uri BaseUri { get; }

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiDocument() 
        {
            Workspace = new OpenApiWorkspace();
            BaseUri = new(OpenApiConstants.BaseRegistryUri + Guid.NewGuid());
            Info = new OpenApiInfo();
            Paths = new OpenApiPaths();
        }
                
        /// <summary>
        /// Initializes a copy of an an <see cref="OpenApiDocument"/> object
        /// </summary>
        public OpenApiDocument(OpenApiDocument? document)
        {
            Workspace = document?.Workspace != null ? new(document?.Workspace) : null;
            Info = document?.Info != null ? new(document?.Info) : new OpenApiInfo();
            JsonSchemaDialect = document?.JsonSchemaDialect ?? JsonSchemaDialect;
            Servers = document?.Servers != null ? new List<OpenApiServer>(document.Servers) : null;
            Paths = document?.Paths != null ? new(document?.Paths) : new OpenApiPaths();
            Webhooks = document?.Webhooks != null ? new Dictionary<string, OpenApiPathItem>(document.Webhooks) : null;
            Components = document?.Components != null ? new(document?.Components) : null;
            SecurityRequirements = document?.SecurityRequirements != null ? new List<OpenApiSecurityRequirement>(document.SecurityRequirements) : null;
            Tags = document?.Tags != null ? new List<OpenApiTag>(document.Tags) : null;
            ExternalDocs = document?.ExternalDocs != null ? new(document?.ExternalDocs) : null;
            Extensions = document?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(document.Extensions) : null;
            Annotations = document?.Annotations != null ? new Dictionary<string, object>(document.Annotations) : null;
            BaseUri = document?.BaseUri != null ? document.BaseUri : new(OpenApiConstants.BaseRegistryUri + Guid.NewGuid());
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to Open API v3.1 document.
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // openApi;
            writer.WriteProperty(OpenApiConstants.OpenApi, "3.1.1");

            // jsonSchemaDialect
            writer.WriteProperty(OpenApiConstants.JsonSchemaDialect, JsonSchemaDialect);

            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (w, element) => element.SerializeAsV31(w));

            // webhooks
            writer.WriteOptionalMap(
            OpenApiConstants.Webhooks,
            Webhooks,
            (w, key, component) =>
            {
                if (component is OpenApiPathItemReference reference)
                {
                    reference.SerializeAsV31(w);
                }
                else
                {
                    component.SerializeAsV31(w);
                }
            });

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to the latest patch of OpenAPI object V3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // openapi
            writer.WriteProperty(OpenApiConstants.OpenApi, "3.0.4");
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (w, element) => element.SerializeAsV3(w));
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="version"></param>
        /// <param name="callback"></param>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            // info
            writer.WriteRequiredObject(OpenApiConstants.Info, Info, callback);

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, Servers, callback);

            // paths            
            writer.WriteRequiredObject(OpenApiConstants.Paths, Paths, callback);

            // components
            writer.WriteOptionalObject(OpenApiConstants.Components, Components, callback);

            // security
            writer.WriteOptionalCollection(
                OpenApiConstants.Security,
                SecurityRequirements,
                callback);

            // tags
            writer.WriteOptionalCollection(OpenApiConstants.Tags, Tags, (w, t) => callback(w, t));

            // external docs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, callback);

            // extensions
            writer.WriteExtensions(Extensions, version);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to OpenAPI object V2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // swagger
            writer.WriteProperty(OpenApiConstants.Swagger, "2.0");

            // info
            writer.WriteRequiredObject(OpenApiConstants.Info, Info, (w, i) => i.SerializeAsV2(w));

            // host, basePath, schemes, consumes, produces
            WriteHostInfoV2(writer, Servers);

            // paths
            writer.WriteRequiredObject(OpenApiConstants.Paths, Paths, (w, p) => p.SerializeAsV2(w));

            // If references have been inlined we don't need to render the components section
            // however if they have cycles, then we will need a component rendered
            if (writer.GetSettings().InlineLocalReferences)
            {
                var loops = writer.GetSettings().LoopDetector.Loops;

                if (loops.TryGetValue(typeof(OpenApiSchema), out List<object> schemas))
                {
                    var openApiSchemas = schemas.Cast<OpenApiSchema>().Distinct().ToList()
                         .ToDictionary<OpenApiSchema, string>(k => k.Reference.Id);

                    foreach (var schema in openApiSchemas.Values.ToList())
                    {
                        FindSchemaReferences.ResolveSchemas(Components, openApiSchemas);
                    }

                    writer.WriteOptionalMap(
                       OpenApiConstants.Definitions,
                       openApiSchemas,
                       (w, _, component) => component.SerializeAsV2(w));
                }
            }
            else
            {
                // Serialize each referenceable object as full object without reference if the reference in the object points to itself.
                // If the reference exists but points to other objects, the object is serialized to just that reference.
                // definitions
                writer.WriteOptionalMap(
                    OpenApiConstants.Definitions,
                    Components?.Schemas,
                    (w, key, component) =>
                    {
                        if (component is OpenApiSchemaReference reference)
                        {
                            reference.SerializeAsV2(w);
                        }
                        else
                        {
                            component.SerializeAsV2(w);
                        }
                    });

                // parameters
                var parameters = Components?.Parameters != null
                    ? new Dictionary<string, OpenApiParameter>(Components.Parameters)
                    : new Dictionary<string, OpenApiParameter>();

                if (Components?.RequestBodies != null)
                {
                    foreach (var requestBody in Components.RequestBodies.Where(b => !parameters.ContainsKey(b.Key)))
                    {
                        parameters.Add(requestBody.Key, requestBody.Value.ConvertToBodyParameter());
                    }
                }
                writer.WriteOptionalMap(
                    OpenApiConstants.Parameters,
                    parameters,
                    (w, key, component) =>
                    {
                        if (component is OpenApiParameterReference reference)
                        {
                            reference.SerializeAsV2(w);
                        }
                        else
                        {
                            component.SerializeAsV2(w);
                        }
                    });

                // responses
                writer.WriteOptionalMap(
                    OpenApiConstants.Responses,
                    Components?.Responses,
                    (w, key, component) =>
                    {
                        if (component is OpenApiResponseReference reference)
                        {
                            reference.SerializeAsV2(w);
                        }
                        else
                        {
                            component.SerializeAsV2(w);
                        }
                    });

                // securityDefinitions
                writer.WriteOptionalMap(
                    OpenApiConstants.SecurityDefinitions,
                    Components?.SecuritySchemes,
                    (w, key, component) =>
                    {
                        if (component is OpenApiSecuritySchemeReference reference)
                        {
                            reference.SerializeAsV2(w);
                        }
                        else
                        {
                            component.SerializeAsV2(w);
                        }
                    });

                // security
                writer.WriteOptionalCollection(
                    OpenApiConstants.Security,
                    SecurityRequirements,
                    (w, s) => s.SerializeAsV2(w));

                // tags
                writer.WriteOptionalCollection(OpenApiConstants.Tags, Tags, (w, t) => t.SerializeAsV2(w));

                // externalDocs
                writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.SerializeAsV2(w));

                // extensions
                writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

                writer.WriteEndObject();
            }
        }

        private static string ParseServerUrl(OpenApiServer server)
        {
            return server.ReplaceServerUrlVariables(new Dictionary<string, string>(0));
        }

        private static void WriteHostInfoV2(IOpenApiWriter writer, IList<OpenApiServer>? servers)
        {
            if (servers == null || !servers.Any())
            {
                return;
            }

            // Arbitrarily choose the first server given that V2 only allows
            // one host, port, and base path.
            var serverUrl = ParseServerUrl(servers.First());

            // Divide the URL in the Url property into host and basePath required in OpenAPI V2
            // The Url property cannot contain path templating to be valid for V2 serialization.
            var firstServerUrl = new Uri(serverUrl, UriKind.RelativeOrAbsolute);

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
                        Uri.TryCreate(ParseServerUrl(s), UriKind.RelativeOrAbsolute, out var url);
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

        /// <summary>
        /// Walks the OpenApiDocument and sets the host document for all IOpenApiReferenceable objects
        /// </summary>
        public void SetReferenceHostDocument()
        {
            var resolver = new ReferenceHostDocumentSetter(this);
            var walker = new OpenApiWalker(resolver);
            walker.Walk(this);
        }

        /// <summary>
        /// Load the referenced <see cref="IOpenApiReferenceable"/> object from a <see cref="OpenApiReference"/> object
        /// </summary>
        internal T? ResolveReferenceTo<T>(OpenApiReference reference) where T : class, IOpenApiReferenceable
        {
            if (reference.IsExternal)
            {
                return ResolveReference(reference, true) as T;
            }
            else
            {
                return ResolveReference(reference, false) as T;
            }
        }

        /// <summary>
        /// Takes in an OpenApi document instance and generates its hash value
        /// </summary>
        /// <param name="doc">The OpenAPI description to hash.</param>
        /// <returns>The hash value.</returns>
        public static string GenerateHashValue(OpenApiDocument doc)
        {
            using HashAlgorithm sha = SHA512.Create();
            using var cryptoStream = new CryptoStream(Stream.Null, sha, CryptoStreamMode.Write);
            using var streamWriter = new StreamWriter(cryptoStream);

            var openApiJsonWriter = new OpenApiJsonWriter(streamWriter, new() { Terse = true });
            doc.SerializeAsV3(openApiJsonWriter);
            openApiJsonWriter.Flush();

            cryptoStream.FlushFinalBlock();
            var hash = sha.Hash;

            return ConvertByteArrayToString(hash);
        }

        private static string ConvertByteArrayToString(byte[] hash)
        {
            // Build the final string by converting each byte
            // into hex and appending it to a StringBuilder
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Load the referenced <see cref="IOpenApiReferenceable"/> object from a <see cref="OpenApiReference"/> object
        /// </summary>
        internal IOpenApiReferenceable? ResolveReference(OpenApiReference? reference, bool useExternal)
        {
            if (reference == null)
            {
                return null;
            }

            if (!reference.Type.HasValue)
            {
                throw new ArgumentException(Properties.SRResource.LocalReferenceRequiresType);
            }

            // Special case for Tag
            if (reference.Type == ReferenceType.Tag)
            {
                foreach (var tag in this.Tags ?? Enumerable.Empty<OpenApiTag>())
                {
                    if (tag.Name == reference.Id)
                    {
                        tag.Reference = reference;
                        return tag;
                    }
                }

                return null;
            }

            string uriLocation;
            if (reference.Id.Contains("/")) // this means its a URL reference
            {
                uriLocation = reference.Id;
            }
            else
            {
                string relativePath = OpenApiConstants.ComponentsSegment + reference.Type.GetDisplayName() + "/" + reference.Id;

                uriLocation = useExternal
                    ? Workspace?.GetDocumentId(reference.ExternalResource)?.OriginalString + relativePath
                    : BaseUri + relativePath;
            }

            return Workspace?.ResolveReference<IOpenApiReferenceable>(uriLocation);
        }

        /// <summary>
        /// Parses a local file path or Url into an Open API document.
        /// </summary>
        /// <param name="url"> The path to the OpenAPI file.</param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static ReadResult Load(string url, OpenApiReaderSettings? settings = null)
        {
            return OpenApiModelFactory.Load(url, settings);
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="stream">Stream containing OpenAPI description to parse.</param>
        /// <param name="format">The OpenAPI format to use during parsing.</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns></returns>
        public static ReadResult Load(Stream stream,
                                      string format,
                                      OpenApiReaderSettings? settings = null)
        {
            return OpenApiModelFactory.Load(stream, format, settings);
        }

        /// <summary>
        /// Reads the text reader content and parses it into an Open API document.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="format"> The OpenAPI format to use during parsing.</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns></returns>
        public static ReadResult Load(TextReader input,
                                           string format,
                                           OpenApiReaderSettings? settings = null)
        {
            return OpenApiModelFactory.Load(input, format, settings);
        }

        /// <summary>
        /// Parses a local file path or Url into an Open API document.
        /// </summary>
        /// <param name="url"> The path to the OpenAPI file.</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings? settings = null)
        {
            return await OpenApiModelFactory.LoadAsync(url, settings);
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="stream">Stream containing OpenAPI description to parse.</param>
        /// <param name="format">The OpenAPI format to use during parsing.</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates information about operation cancelling.</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(Stream stream, string format, OpenApiReaderSettings? settings = null, CancellationToken cancellationToken = default)
        {
            return await OpenApiModelFactory.LoadAsync(stream, format, settings, cancellationToken);
        }

        /// <summary>
        /// Reads the text reader content and parses it into an Open API document.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="format"> The OpenAPI format to use during parsing.</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(TextReader input, string format, OpenApiReaderSettings? settings = null)
        {
            return await OpenApiModelFactory.LoadAsync(input, format, settings);
        }

        /// <summary>
        /// Parses a string into a <see cref="OpenApiDocument"/> object.
        /// </summary>
        /// <param name="input"> The string input.</param>
        /// <param name="format"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static ReadResult Parse(string input,
                                       string? format = null,
                                       OpenApiReaderSettings? settings = null)
        {
            return OpenApiModelFactory.Parse(input, format, settings);
        }
    }

    internal class FindSchemaReferences : OpenApiVisitorBase
    {
        private Dictionary<string, OpenApiSchema> Schemas = new();

        public static void ResolveSchemas(OpenApiComponents? components, Dictionary<string, OpenApiSchema> schemas)
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
            if (schema.Reference != null && !Schemas.ContainsKey(schema.Reference.Id))
            {
                Schemas.Add(schema.Reference.Id, schema);
            }
            base.Visit(schema);
        }
    }
}
