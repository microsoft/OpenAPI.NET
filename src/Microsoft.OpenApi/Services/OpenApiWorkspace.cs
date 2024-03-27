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
            BaseUrl = new("http://openapi.net/workspace/");
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
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="components"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterComponents(Uri uri, OpenApiComponents components)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (components == null) throw new ArgumentNullException(nameof(components));
            ComponentsRegistry[uri] = components;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterComponents(OpenApiDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (document.Components == null) throw new ArgumentNullException(nameof(document.Components));
            ComponentsRegistry[GetDocumentUri(document)] = document.Components;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public bool TryGetComponents(Uri uri, out OpenApiComponents components)
        {
            if (uri == null)
            {
                components = null;
                return false;
            }

            ComponentsRegistry.TryGetValue(uri, out components);
            return (components != null);
        }
                
        /// <summary>
        /// Verify if workspace contains a document based on its URL.
        /// </summary>
        /// <param name="location">A relative or absolute URL of the file.  Use file:// for folder locations.</param>
        /// <returns>Returns true if a matching document is found.</returns>
        public bool Contains(string location)
        {
            var key = ToLocationUrl(location);
            return _documents.ContainsKey(key) || _fragments.ContainsKey(key) || _artifacts.ContainsKey(key);
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
            _documents.Add(locationUrl, document);
            if (document.Components != null)
            {
                RegisterComponents(locationUrl, document.Components);
            }
        }

        /// <summary>
        /// Add an OpenApiDocument to the workspace.
        /// </summary>
        /// <param name="document">The OpenAPI document.</param>
        public void AddDocument(OpenApiDocument document)
        {
            // document.Workspace = this; TODO

            // Register components in this doc.
            if (document.Components != null)
            {
                RegisterComponents(GetDocumentUri(document), document.Components);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private Uri GetDocumentUri(OpenApiDocument document)
        {
            if (document == null) return null;

            string docUri = (document.Servers.FirstOrDefault() != null) ? document.Servers.First().Url : document.BaseUri.OriginalString;
            if (!Uri.TryCreate(docUri, UriKind.Absolute, out _))
            {
                docUri = $"http://openapi.net/{docUri}";
            }

            return new Uri(docUri);
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
            _schemaFragments.Add(locationUri, fragment);
            var schemaComponent = new OpenApiComponents();
            schemaComponent.Schemas.Add(locationUri.OriginalString, fragment);
            ComponentsRegistry[locationUri] = schemaComponent;
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
        /// Returns the target of an OpenApiReference from within the workspace.
        /// </summary>
        /// <param name="reference">An instance of an OpenApiReference</param>
        /// <returns></returns>
        public IOpenApiReferenceable ResolveReference(OpenApiReference reference)
        {
            var uri = new Uri(BaseUrl, reference.ExternalResource);
            if (_documents.TryGetValue(uri, out var doc))
            {
                // return doc.ResolveReference(reference, false); // TODO: Resolve internally, don't refer to doc.
                return ResolveReference<IOpenApiReferenceable>(reference.Id, reference.Type, doc.Components);
            }
            else if (_fragments.TryGetValue(uri, out var fragment))
            {
                var jsonPointer = new JsonPointer($"/{reference.Id ?? string.Empty}");
                return fragment.ResolveReference(jsonPointer);
            }
            return null;

        }

        
        //public JsonSchema ResolveJsonSchemaReference(Uri reference)
        // {
        //    TryResolveReference<JsonSchema>(reference.OriginalString, ReferenceType.Schema, document.BaseUri, out var resolvedSchema);

        //    if (resolvedSchema != null)
        //    {
        //        var resolvedSchemaBuilder = new JsonSchemaBuilder();
        //        var description = resolvedSchema.GetDescription();
        //        var summary = resolvedSchema.GetSummary();

        //        foreach (var keyword in resolvedSchema.Keywords)
        //        {
        //            resolvedSchemaBuilder.Add(keyword);

        //            // Replace the resolved schema's description with that of the schema reference
        //            if (!string.IsNullOrEmpty(description))
        //            {
        //                resolvedSchemaBuilder.Description(description);
        //            }

        //            // Replace the resolved schema's summary with that of the schema reference
        //            if (!string.IsNullOrEmpty(summary))
        //            {
        //                resolvedSchemaBuilder.Summary(summary);
        //            }
        //        }

        //        return resolvedSchemaBuilder.Build();
        //    }
        //    else
        //    {
        //        var referenceId = reference.OriginalString.Split('/').LastOrDefault();
        //        throw new OpenApiException(string.Format(Properties.SRResource.InvalidReferenceId, referenceId));
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="referenceV3"></param>
        /// <param name="referenceType"></param>
        /// <param name="docBaseUri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="OpenApiException"></exception>
        public bool TryResolveReference<T>(string referenceV3, ReferenceType? referenceType, out T value, Uri docBaseUri = null)
        {
            value = default;
            if (string.IsNullOrEmpty(referenceV3)) return false;

            var referenceId = referenceV3.Split('/').LastOrDefault();

            // The first part of the referenceId before the # should give us our location url
            // if the 1st part is missing, then the reference is in the entry document
            var locationUrl = (referenceV3.Contains('#')) ? referenceV3.Substring(0, referenceV3.IndexOf('#')) : null;

            ComponentsRegistry.TryGetValue(docBaseUri, out var componentsTest);

            OpenApiComponents components;
            if (string.IsNullOrEmpty(locationUrl))
            {
                // Get the entry level document components
                // or the 1st registry component (if entry level has no components)
                components = ComponentsRegistry.FirstOrDefault().Value;
            }
            else
            {
                // Try convert to absolute uri
                Uri uriLocation = ToLocationUrl(locationUrl);                                           

                ComponentsRegistry.TryGetValue(uriLocation, out components);
            }

            if (components == null) return false;

            switch (referenceType)
            {
                case ReferenceType.PathItem:
                    value = (T)(IOpenApiReferenceable)components.PathItems[referenceId];
                    return (value != null);

                case ReferenceType.Response:
                    value = (T)(IOpenApiReferenceable)components.Responses[referenceId];
                    return (value != null);

                case ReferenceType.Parameter:
                    value = (T)(IOpenApiReferenceable)components.Parameters[referenceId];
                    return (value != null);

                case ReferenceType.Example:
                    value = (T)(IOpenApiReferenceable)components.Examples[referenceId];
                    return (value != null);

                case ReferenceType.RequestBody:
                    value = (T)(IOpenApiReferenceable)components.RequestBodies[referenceId];
                    return (value != null);

                case ReferenceType.Header:
                    value = (T)(IOpenApiReferenceable)components.Headers[referenceId];
                    return (value != null);

                case ReferenceType.SecurityScheme:
                    value = (T)(IOpenApiReferenceable)components.SecuritySchemes[referenceId];
                    return (value != null);

                case ReferenceType.Link:
                    value = (T)(IOpenApiReferenceable)components.Links[referenceId];
                    return (value != null);

                case ReferenceType.Callback:
                    value = (T)(IOpenApiReferenceable)components.Callbacks[referenceId];
                    return (value != null);

                case ReferenceType.Schema:
                    value = (T)(IBaseDocument)components.Schemas[referenceId];
                    return (value != null);

                default:
                    throw new OpenApiException(Properties.SRResource.InvalidReferenceType);
            }
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
            // Try convert to absolute uri
            return (Uri.TryCreate(location, UriKind.Absolute, out var uri)) == true ? uri : new Uri(BaseUrl, location);

            //if (Uri.TryCreate(location, UriKind.Absolute, out var uri))
            //   {
            //    locationUri = new Uri(BaseUrl, uri.LocalPath);
            //   }
            //else
            //{
            //    locationUri = new Uri(BaseUrl, location);
            //}
            //return locationUri;

            // return new(BaseUrl, location);
        }

        private static JsonSchema FetchSchemaFromRegistry(Uri reference)
        {
            var resolvedSchema = (JsonSchema)SchemaRegistry.Global.Get(reference);
            return resolvedSchema;
        }
    }
}
