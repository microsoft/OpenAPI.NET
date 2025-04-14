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
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Describes an OpenAPI object (OpenAPI document). See: https://spec.openapis.org
    /// </summary>
    public class OpenApiDocument : IOpenApiSerializable, IOpenApiExtensible, IMetadataContainer
    {
        /// <summary>
        /// Register components in the document to the workspace
        /// </summary>
        public void RegisterComponents()
        {
            Workspace?.RegisterComponents(this);
        }
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
        public Uri? JsonSchemaDialect { get; set; }

        /// <summary>
        /// An array of Server Objects, which provide connectivity information to a target server.
        /// </summary>
        public List<OpenApiServer>? Servers { get; set; } = [];

        /// <summary>
        /// REQUIRED. The available paths and operations for the API.
        /// </summary>
        public OpenApiPaths Paths { get; set; }

        /// <summary>
        /// The incoming webhooks that MAY be received as part of this API and that the API consumer MAY choose to implement.
        /// A map of requests initiated other than by an API call, for example by an out of band registration. 
        /// The key name is a unique string to refer to each webhook, while the (optionally referenced) Path Item Object describes a request that may be initiated by the API provider and the expected responses
        /// </summary>
        public Dictionary<string, IOpenApiPathItem>? Webhooks { get; set; }

        /// <summary>
        /// An element to hold various schemas for the specification.
        /// </summary>
        public OpenApiComponents? Components { get; set; }

        /// <summary>
        /// A declaration of which security mechanisms can be used across the API.
        /// </summary>
        public List<OpenApiSecurityRequirement>? Security { get; set; }

        private HashSet<OpenApiTag>? _tags;
        /// <summary>
        /// A list of tags used by the specification with additional metadata.
        /// </summary>
        public HashSet<OpenApiTag>? Tags 
        { 
            get
            {
                return _tags;
            }
            set
            {
                if (value is null)
                {
                    return;
                }
                _tags = value is HashSet<OpenApiTag> tags && tags.Comparer is OpenApiTagComparer ?
                        tags :
                        new HashSet<OpenApiTag>(value, OpenApiTagComparer.Instance);
            }
        }

        /// <summary>
        /// Additional external documentation.
        /// </summary>
        public OpenApiExternalDocs? ExternalDocs { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public Dictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <inheritdoc />
        public Dictionary<string, object>? Metadata { get; set; }

        /// <summary>
        /// Absolute location of the document or a generated placeholder if location is not given
        /// </summary>
        public Uri BaseUri { get; internal set; }

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
            Workspace = document?.Workspace != null ? new(document.Workspace) : null;
            Info = document?.Info != null ? new(document.Info) : new OpenApiInfo();
            JsonSchemaDialect = document?.JsonSchemaDialect ?? JsonSchemaDialect;
            Servers = document?.Servers != null ? [.. document.Servers] : null;
            Paths = document?.Paths != null ? new(document.Paths) : [];
            Webhooks = document?.Webhooks != null ? new Dictionary<string, IOpenApiPathItem>(document.Webhooks) : null;
            Components = document?.Components != null ? new(document?.Components) : null;
            Security = document?.Security != null ? [.. document.Security] : null;
            Tags = document?.Tags != null ? new HashSet<OpenApiTag>(document.Tags, OpenApiTagComparer.Instance) : null;
            ExternalDocs = document?.ExternalDocs != null ? new(document.ExternalDocs) : null;
            Extensions = document?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(document.Extensions) : null;
            Metadata = document?.Metadata != null ? new Dictionary<string, object>(document.Metadata) : null;
            BaseUri = document?.BaseUri != null ? document.BaseUri : new(OpenApiConstants.BaseRegistryUri + Guid.NewGuid());
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to an Open API document using the specified version.
        /// </summary>
        /// <param name="version">The Open API specification version to serialize the document as.</param>
        /// <param name="writer">The <see cref="IOpenApiWriter"/> to serialize the document to.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="version"/> is not a supported Open API specification version.
        /// </exception>
        public void SerializeAs(OpenApiSpecVersion version, IOpenApiWriter writer)
        {
            switch (version)
            {
                case OpenApiSpecVersion.OpenApi2_0:
                    SerializeAsV2(writer);
                    break;

                case OpenApiSpecVersion.OpenApi3_0:
                    SerializeAsV3(writer);
                    break;

                case OpenApiSpecVersion.OpenApi3_1:
                    SerializeAsV31(writer);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, string.Format(Properties.SRResource.OpenApiSpecVersionNotSupported, version));
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to Open API v3.1 document.
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // openApi
            writer.WriteProperty(OpenApiConstants.OpenApi, "3.1.1");

            // jsonSchemaDialect
            writer.WriteProperty(OpenApiConstants.JsonSchemaDialect, JsonSchemaDialect?.ToString());

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
                Security,
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

                if (loops.TryGetValue(typeof(IOpenApiSchema), out var schemas))
                {
                    var openApiSchemas = schemas.Cast<IOpenApiSchema>()
                        .Distinct()
                        .OfType<OpenApiSchemaReference>()
                        .Where(k => k.Reference?.Id is not null)
                        .ToDictionary<OpenApiSchemaReference, string, IOpenApiSchema>(
                            k => k.Reference?.Id!,
                            v => v
                        );


                    foreach (var schema in openApiSchemas.Values.ToList())
                    {
                        FindSchemaReferences.ResolveSchemas(Components, openApiSchemas!);
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
                    ? new Dictionary<string, IOpenApiParameter>(Components.Parameters)
                    : [];

                if (Components?.RequestBodies != null)
                {
                    foreach (var requestBody in Components.RequestBodies.Where(b => !parameters.ContainsKey(b.Key)))
                    {
                        var paramValue = requestBody.Value.ConvertToBodyParameter(writer);
                        if (paramValue is not null)
                            parameters.Add(requestBody.Key, paramValue);
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
                    Security,
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

        private static string? ParseServerUrl(OpenApiServer server)
        {
            return server.ReplaceServerUrlVariables([]);
        }

        private static void WriteHostInfoV2(IOpenApiWriter writer, List<OpenApiServer>? servers)
        {
            if (servers == null || !servers.Any())
            {
                return;
            }

            // Arbitrarily choose the first server given that V2 only allows
            // one host, port, and base path.
            var serverUrl = ParseServerUrl(servers[0]);

            if (serverUrl != null)
            {
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
                    if (relativeUrl.StartsWith("//", StringComparison.OrdinalIgnoreCase))
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
                        u => u is not null &&
                            Uri.Compare(
                                u,
                                firstServerUrl,
                                UriComponents.Host | UriComponents.Port | UriComponents.Path,
                                UriFormat.SafeUnescaped,
                                StringComparison.OrdinalIgnoreCase) ==
                            0 && u.IsAbsoluteUri)
                    .Select(u => u!.Scheme)
                    .Distinct()
                    .ToList();

                // schemes
                writer.WriteOptionalCollection(OpenApiConstants.Schemes, schemes, (w, s) => 
                {
                    if(!string.IsNullOrEmpty(s) && s is not null)
                    {
                        w.WriteValue(s);
                    }
                });
            }            
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
        internal T? ResolveReferenceTo<T>(OpenApiReference reference) where T : IOpenApiReferenceable
        {

            if (ResolveReference(reference, reference.IsExternal) is T result)
            {
                return result;
            }
            return default;
        }

        /// <summary>
        /// Takes in an OpenApi document instance and generates its hash value
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>The hash value.</returns>
        public async Task<string> GetHashCodeAsync(CancellationToken cancellationToken = default)
        {
            using HashAlgorithm sha = SHA512.Create();
            using var cryptoStream = new CryptoStream(Stream.Null, sha, CryptoStreamMode.Write);
            using var streamWriter = new StreamWriter(cryptoStream);

            var openApiJsonWriter = new OpenApiJsonWriter(streamWriter, new() { Terse = true });
            SerializeAsV3(openApiJsonWriter);
            await openApiJsonWriter.FlushAsync(cancellationToken).ConfigureAwait(false);

#if NET5_0_OR_GREATER
            await cryptoStream.FlushFinalBlockAsync(cancellationToken).ConfigureAwait(false);
#else
            cryptoStream.FlushFinalBlock();
#endif
            return ConvertByteArrayToString(sha.Hash ?? []);
        }

        private static string ConvertByteArrayToString(byte[] hash)
        {
            // Build the final string by converting each byte
            // into hex and appending it to a StringBuilder
#if NET5_0_OR_GREATER
            return Convert.ToHexString(hash);
#else
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
#endif
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

            string uriLocation;
            var id = reference.Id;
            if (!string.IsNullOrEmpty(id) && id!.Contains("/")) // this means its a URL reference
            {
                uriLocation = id;
            }
            else
            {
                string relativePath = $"#{OpenApiConstants.ComponentsSegment}{reference.Type.GetDisplayName()}/{id}";
                Uri? externalResourceUri = useExternal ? Workspace?.GetDocumentId(reference.ExternalResource) : null;

                uriLocation = useExternal && externalResourceUri is not null
                    ? externalResourceUri.AbsoluteUri + relativePath
                    : BaseUri + relativePath;
            }

            return Workspace?.ResolveReference<IOpenApiReferenceable>(new Uri(uriLocation).AbsoluteUri);
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="stream">Stream containing OpenAPI description to parse.</param>
        /// <param name="format">The OpenAPI format to use during parsing.</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns></returns>
        public static ReadResult Load(MemoryStream stream,
                                      string? format = null,
                                      OpenApiReaderSettings? settings = null)
        {
            return OpenApiModelFactory.Load(stream, format, settings);
        }

        /// <summary>
        /// Parses a local file path or Url into an Open API document.
        /// </summary>
        /// <param name="url"> The path to the OpenAPI file.</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings? settings = null, CancellationToken token = default)
        {
            return await OpenApiModelFactory.LoadAsync(url, settings, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="stream">Stream containing OpenAPI description to parse.</param>
        /// <param name="format">The OpenAPI format to use during parsing.</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates information about operation cancelling.</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(Stream stream, string? format = null, OpenApiReaderSettings? settings = null, CancellationToken cancellationToken = default)
        {
            return await OpenApiModelFactory.LoadAsync(stream, format, settings, cancellationToken).ConfigureAwait(false);
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
        /// <summary>
        /// Adds a component to the components object of the current document and registers it to the underlying workspace.
        /// </summary>
        /// <param name="componentToRegister">The component to add</param>
        /// <param name="id">The id for the component</param>
        /// <typeparam name="T">The type of the component</typeparam>
        /// <returns>Whether the component was added to the components.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the component is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the id is null or empty.</exception>
        public bool AddComponent<T>(string id, T componentToRegister)
        {
            Utils.CheckArgumentNull(componentToRegister);
            Utils.CheckArgumentNullOrEmpty(id);
            Components ??= new();
            switch (componentToRegister)
            {
                case IOpenApiSchema openApiSchema:
                    Components.Schemas ??= [];
                    Components.Schemas.Add(id, openApiSchema);
                    break;
                case IOpenApiParameter openApiParameter:
                    Components.Parameters ??= [];
                    Components.Parameters.Add(id, openApiParameter);
                    break;
                case IOpenApiResponse openApiResponse:
                    Components.Responses ??= [];
                    Components.Responses.Add(id, openApiResponse);
                    break;
                case IOpenApiRequestBody openApiRequestBody:
                    Components.RequestBodies ??= [];
                    Components.RequestBodies.Add(id, openApiRequestBody);
                    break;
                case IOpenApiLink openApiLink:
                    Components.Links ??= [];
                    Components.Links.Add(id, openApiLink);
                    break;
                case IOpenApiCallback openApiCallback:
                    Components.Callbacks ??= [];
                    Components.Callbacks.Add(id, openApiCallback);
                    break;
                case IOpenApiPathItem openApiPathItem:
                    Components.PathItems ??= [];
                    Components.PathItems.Add(id, openApiPathItem);
                    break;
                case IOpenApiExample openApiExample:
                    Components.Examples ??= [];
                    Components.Examples.Add(id, openApiExample);
                    break;
                case IOpenApiHeader openApiHeader:
                    Components.Headers ??= [];
                    Components.Headers.Add(id, openApiHeader);
                    break;
                case IOpenApiSecurityScheme openApiSecurityScheme:
                    Components.SecuritySchemes ??= [];
                    Components.SecuritySchemes.Add(id, openApiSecurityScheme);
                    break;
                default:
                    throw new ArgumentException($"Component type {componentToRegister!.GetType().Name} is not supported.");
            }
            return Workspace?.RegisterComponentForDocument(this, componentToRegister, id) ?? false;
        }
    }

    internal class FindSchemaReferences : OpenApiVisitorBase
    {
        private Dictionary<string, IOpenApiSchema> Schemas = new(StringComparer.Ordinal);

        public static void ResolveSchemas(OpenApiComponents? components, Dictionary<string, IOpenApiSchema> schemas)
        {
            var visitor = new FindSchemaReferences();
            visitor.Schemas = schemas;
            var walker = new OpenApiWalker(visitor);
            walker.Walk(components);
        }

        /// <inheritdoc/>
        public override void Visit(IOpenApiReferenceHolder referenceHolder)
        {
            switch (referenceHolder)
            {
                case OpenApiSchemaReference schema:
                    var id = schema.Reference?.Id;
                    if (id is not null && Schemas is not null && !Schemas.ContainsKey(id))
                    {
                        Schemas.Add(id, schema);
                    }
                    break;

                default:
                    break;
            }
            base.Visit(referenceHolder);
        }

        public override void Visit(IOpenApiSchema schema)
        {
            // This is needed to handle schemas used in Responses in components
            if (schema is OpenApiSchemaReference { Reference: not null } schemaReference)
            {
                var id = schemaReference.Reference?.Id;
                if (id is not null && Schemas is not null && !Schemas.ContainsKey(id))
                {
                    Schemas.Add(id, schema);
                }
            } 
            base.Visit(schema);
        }
    }
}
