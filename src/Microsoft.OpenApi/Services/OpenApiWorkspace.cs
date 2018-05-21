// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Contains a set of OpenApi documents and document fragments that reference each other
    /// </summary>
    public class OpenApiWorkspace
    {
        private Dictionary<Uri, OpenApiDocument> _documents = new Dictionary<Uri, OpenApiDocument>();
        private Dictionary<Uri, IOpenApiElement> _fragments = new Dictionary<Uri, IOpenApiElement>();
        private Dictionary<Uri, Stream> _artifacts = new Dictionary<Uri, Stream>();

        /// <summary>
        /// A list of OpenApiDocuments contained in the workspace
        /// </summary>
        public IEnumerable<OpenApiDocument> Documents {
            get {
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

        public bool Contains(string location)
        {
            return _documents.ContainsKey(ToLocationUrl(location));
        }

        public void AddDocument(string location, OpenApiDocument  document)
        {
            document.Workspace = this;
            _documents.Add(ToLocationUrl(location), document);
        }

        public void AddFragment(string location, IOpenApiElement fragment)
        {
            _fragments.Add(ToLocationUrl(location), fragment);
        }

        public void AddArtifact(string location, Stream artifact)
        {
            _artifacts.Add(ToLocationUrl(location), artifact);
        }

        public IOpenApiReferenceable ResolveReference(OpenApiReference reference)
        {
            if (!_documents.TryGetValue(new Uri(reference.ExternalResource,UriKind.RelativeOrAbsolute),out var doc))
            {
                return null;
            }
            
            return doc.ResolveReference(reference,true);
        }

        private Uri ToLocationUrl(string location)
        {
            return new Uri(BaseUrl, location);
        }
    }
}
