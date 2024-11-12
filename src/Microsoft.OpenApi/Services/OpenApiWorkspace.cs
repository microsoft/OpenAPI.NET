// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly Dictionary<string, Uri> _documentsIdRegistry = new();
        private readonly Dictionary<Uri, Stream> _artifactsRegistry = new();        
        private readonly Dictionary<Uri, IOpenApiReferenceable> _IOpenApiReferenceableRegistry = new();

        /// <summary>
        /// The base location from where all relative references are resolved
        /// </summary>
        public Uri BaseUrl { get; }
       
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
            BaseUrl = new Uri(OpenApiConstants.BaseRegistryUri);
        }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiWorkspace"/> object
        /// </summary>
        public OpenApiWorkspace(OpenApiWorkspace workspace) { }

        /// <summary>
        /// Returns the total count of all the components in the workspace registry
        /// </summary>
        /// <returns></returns>
        public int ComponentsCount()
        {
            return _IOpenApiReferenceableRegistry.Count + _artifactsRegistry.Count;
        }

        /// <summary>
        /// Registers a document's components into the workspace
        /// </summary>
        /// <param name="document"></param>
        public void RegisterComponents(OpenApiDocument document)
        {
            if (document?.Components == null) return;

            string baseUri = document.BaseUri + OpenApiConstants.ComponentsSegment;
            string location;

            // Register Schema
            foreach (var item in document.Components.Schemas)
            {
                location = item.Value.Id ?? baseUri + ReferenceType.Schema.GetDisplayName() + "/" + item.Key;

                RegisterComponent(location, item.Value);
            }

            // Register Parameters
            foreach (var item in document.Components.Parameters)
            {
                location = baseUri + ReferenceType.Parameter.GetDisplayName() + "/" + item.Key;
                RegisterComponent(location, item.Value);
            }

            // Register Responses
            foreach (var item in document.Components.Responses)
            {
                location = baseUri + ReferenceType.Response.GetDisplayName() + "/" + item.Key;
                RegisterComponent(location, item.Value);
            }

            // Register RequestBodies
            foreach (var item in document.Components.RequestBodies)
            {
                location = baseUri + ReferenceType.RequestBody.GetDisplayName() + "/" + item.Key;
                RegisterComponent(location, item.Value);
            }

            // Register Links
            foreach (var item in document.Components.Links)
            {
                location = baseUri + ReferenceType.Link.GetDisplayName() + "/" + item.Key;
                RegisterComponent(location, item.Value);
            }

            // Register Callbacks
            foreach (var item in document.Components.Callbacks)
            {
                location = baseUri + ReferenceType.Callback.GetDisplayName() + "/" + item.Key;
                RegisterComponent(location, item.Value);
            }

            // Register PathItems
            foreach (var item in document.Components.PathItems)
            {
                location = baseUri + ReferenceType.PathItem.GetDisplayName() + "/" + item.Key;
                RegisterComponent(location, item.Value);
            }

            // Register Examples
            foreach (var item in document.Components.Examples)
            {
                location = baseUri + ReferenceType.Example.GetDisplayName() + "/" + item.Key;
                RegisterComponent(location, item.Value);
            }

            // Register Headers
            foreach (var item in document.Components.Headers)
            {
                location = baseUri + ReferenceType.Header.GetDisplayName() + "/" + item.Key;
                RegisterComponent(location, item.Value);
            }

            // Register SecuritySchemes
            foreach (var item in document.Components.SecuritySchemes)
            {
                location = baseUri + ReferenceType.SecurityScheme.GetDisplayName() + "/" + item.Key;
                RegisterComponent(location, item.Value);
            }
        }


        /// <summary>
        /// Registers a component in the component registry.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="component"></param>
        /// <returns>true if the component is successfully registered; otherwise false.</returns>
        public bool RegisterComponent<T>(string location, T component)
        {
            var uri = ToLocationUrl(location);
            if (component is IOpenApiReferenceable referenceable)
            {
                if (!_IOpenApiReferenceableRegistry.ContainsKey(uri))
                {
                    _IOpenApiReferenceableRegistry[uri] = referenceable;
                    return true;
                }
            }
            else if (component is Stream stream)
            {
                if (!_artifactsRegistry.ContainsKey(uri))
                {
                    _artifactsRegistry[uri] = stream;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a document id to the dictionaries of document locations and their ids.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddDocumentId(string key, Uri value)
        {
            if (!_documentsIdRegistry.ContainsKey(key))
            {
                _documentsIdRegistry[key] = value;
            }
        }

        /// <summary>
        /// Retrieves the document id given a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The document id of the given key.</returns>
        public Uri GetDocumentId(string key)
        {
            if (_documentsIdRegistry.TryGetValue(key, out var id))
            {
                return id;
            }
            return null;
        }

        /// <summary>
        /// Verify if workspace contains a component based on its URL.
        /// </summary>
        /// <param name="location">A relative or absolute URL of the file.  Use file:// for folder locations.</param>
        /// <returns>Returns true if a matching document is found.</returns>
        public bool Contains(string location)
        {
            var key = ToLocationUrl(location);
            return _IOpenApiReferenceableRegistry.ContainsKey(key) || _artifactsRegistry.ContainsKey(key);
        }

#nullable enable
        /// <summary>
        /// Resolves a reference given a key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <returns>The resolved reference.</returns>
        public T? ResolveReference<T>(string location)
        {
            if (string.IsNullOrEmpty(location)) return default;

            var uri = ToLocationUrl(location);            
            if (_IOpenApiReferenceableRegistry.TryGetValue(uri, out var referenceableValue))
            {
                return (T)referenceableValue;
            }
            else if (_artifactsRegistry.TryGetValue(uri, out var artifact))
            {
                return (T)(object)artifact;
            }

            return default;
        }
#nullable restore

        private Uri ToLocationUrl(string location)
        {
            return new(BaseUrl, location);
        }
    }
}
