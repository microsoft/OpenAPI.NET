// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Json.Schema;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Contains a set of OpenApi documents and document fragments that reference each other
    /// </summary>
    public class OpenApiWorkspace
    {
        private readonly Dictionary<Uri, OpenApiDocument> _documents = new();
        private readonly Dictionary<Uri, IOpenApiReferenceable> _fragments = new();
        private readonly Dictionary<Uri, JsonSchema> _schemaFragments = new();
        private readonly Dictionary<Uri, Stream> _artifacts = new();

        /// <summary>
        /// A list of OpenApiDocuments contained in the workspace
        /// </summary>
        public IEnumerable<OpenApiDocument> Documents
        {
            get
            {
                return _documents.Values;
            }
        }

        /// <summary>
        /// A list of document fragments that are contained in the workspace
        /// </summary>
        public IEnumerable<IOpenApiElement> Fragments { get; }

        /// <summary>
        /// The base location from where all relative references are resolved
        /// </summary>
        public Uri BaseUrl { get; }

        /// <summary>
        /// A list of document fragments that are contained in the workspace
        /// </summary>
        public IEnumerable<Stream> Artifacts { get; }

        /// <summary>
        /// Initialize workspace pointing to a base URL to allow resolving relative document locations.  Use a file:// url to point to a folder
        /// </summary>
        /// <param name="baseUrl"></param>
        public OpenApiWorkspace(Uri baseUrl)
        {
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Initialize workspace using current directory as the default location.
        /// </summary>
        public OpenApiWorkspace()
        {
            BaseUrl = new("file://" + Environment.CurrentDirectory + $"{Path.DirectorySeparatorChar}");
        }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiWorkspace"/> object
        /// </summary>
        public OpenApiWorkspace(OpenApiWorkspace workspace) { }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<Uri, OpenApiComponents> ComponentsRegistry { get; } = new Dictionary<Uri, OpenApiComponents>();
                        
        /// <summary>
        /// Verify if workspace contains a document based on its URL.
        /// </summary>
        /// <param name="location">A relative or absolute URL of the file.  Use file:// for folder locations.</param>
        /// <returns>Returns true if a matching document is found.</returns>
        public bool Contains(string location)
        {
            var key = ToLocationUrl(location);
            return _documents.ContainsKey(key) || _fragments.ContainsKey(key) || _artifacts.ContainsKey(key) || _schemaFragments.ContainsKey(key);
        }

        /// <summary>
        /// Add an OpenApiDocument to the workspace.
        /// </summary>
        /// <param name="location">The string location.</param>
        /// <param name="document">The OpenAPI document.</param>
        public void AddDocument(string location, OpenApiDocument document)
        {
            document.Workspace = this;
            var locationUrl = ToLocationUrl(location);

            if (!_documents.ContainsKey(locationUrl))
            {
                _documents.Add(locationUrl, document);
            }
        }

        /// <summary>
        /// Adds a fragment of an OpenApiDocument to the workspace.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="fragment"></param>
        /// <remarks>Not sure how this is going to work.  Does the reference just point to the fragment as a whole, or do we need to
        /// to be able to point into the fragment.  Keeping it private until we figure it out.
        /// </remarks>
        public void AddFragment(string location, IOpenApiReferenceable fragment)
        {
            _fragments.Add(ToLocationUrl(location), fragment);
        }

        /// <summary>
        /// Adds a schema fragment of an OpenApiDocument to the workspace.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="fragment"></param>
        public void AddSchemaFragment(string location, JsonSchema fragment)
        {
            var locationUri = ToLocationUrl(location);
            if (!_schemaFragments.ContainsKey(locationUri))
            {
                _schemaFragments.Add(locationUri, fragment);
            }           
        }

        /// <summary>
        /// Add a stream based artificat to the workspace.  Useful for images, examples, alternative schemas.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="artifact"></param>
        public void AddArtifact(string location, Stream artifact)
        {
            _artifacts.Add(ToLocationUrl(location), artifact);
        }

        /// <summary>
        /// Returns the target of a referenceable item from within the workspace.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        public T ResolveReference<T>(OpenApiReference reference)
        {
            var uri = new Uri(BaseUrl, reference.ExternalResource);
            if (_documents.TryGetValue(uri, out var doc))
            {
                return ResolveReference<T>(reference.Id, reference.Type, doc.Components);
            }
            else if (_fragments.TryGetValue(uri, out var fragment))
            {
                var jsonPointer = new JsonPointer($"/{reference.Id ?? string.Empty}");
                return (T)fragment.ResolveReference(jsonPointer);
            }
            else if (_schemaFragments.TryGetValue(uri, out var schemaFragment))
            {
                return (T)(schemaFragment as IBaseDocument);
            }
            return default;

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="referenceId"></param>
        /// <param name="referenceType"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        /// <exception cref="OpenApiException"></exception>
        public T ResolveReference<T>(string referenceId, ReferenceType? referenceType, OpenApiComponents components)
        {
            if (string.IsNullOrEmpty(referenceId)) return default;
            if (components == null) return default;

            return referenceType switch
            {
                ReferenceType.PathItem => (T)(IOpenApiReferenceable)components.PathItems[referenceId],
                ReferenceType.Response => (T)(IOpenApiReferenceable)components.Responses[referenceId],
                ReferenceType.Parameter => (T)(IOpenApiReferenceable)components.Parameters[referenceId],
                ReferenceType.Example => (T)(IOpenApiReferenceable)components.Examples[referenceId],
                ReferenceType.RequestBody => (T)(IOpenApiReferenceable)components.RequestBodies[referenceId],
                ReferenceType.Header => (T)(IOpenApiReferenceable)components.Headers[referenceId],
                ReferenceType.SecurityScheme => (T)(IOpenApiReferenceable)components.SecuritySchemes[referenceId],
                ReferenceType.Link => (T)(IOpenApiReferenceable)components.Links[referenceId],
                ReferenceType.Callback => (T)(IOpenApiReferenceable)components.Callbacks[referenceId],
                ReferenceType.Schema => (T)(IBaseDocument)components.Schemas[referenceId],
                _ => throw new OpenApiException(Properties.SRResource.InvalidReferenceType),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Stream GetArtifact(string location)
        {
            return _artifacts[ToLocationUrl(location)];
        }

        private Uri ToLocationUrl(string location)
        {
            return new(BaseUrl, location);
        }

        private static JsonSchema FetchSchemaFromRegistry(Uri reference)
        {
            var resolvedSchema = (JsonSchema)SchemaRegistry.Global.Get(reference);
            return resolvedSchema;
        }
    }
}
