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
            BaseUrl = new Uri("file://" + Environment.CurrentDirectory + "\\" );
        }

        /// <summary>
        /// Verify if workspace contains a document based on its URL. 
        /// </summary>
        /// <param name="location">A relative or absolute URL of the file.  Use file:// for folder locations.</param>
        /// <returns>Returns true if a matching document is found.</returns>
        public bool Contains(string location)
        {
            Uri key = ToLocationUrl(location);
            return _documents.ContainsKey(key) || _fragments.ContainsKey(key) || _artifacts.ContainsKey(key);
        }

        /// <summary>
        /// Add an OpenApiDocument to the workspace.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="document"></param>
        public void AddDocument(string location, OpenApiDocument  document)
        {
            document.Workspace = this;
            _documents.Add(ToLocationUrl(location), document);
        }

        /// <summary>
        /// Adds a fragment of an OpenApiDocument to the workspace.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="fragment"></param>
        /// <remarks>Not sure how this is going to work.  Does the reference just point to the fragment as a whole, or do we need to 
        /// to be able to point into the fragment.  Keeping it private until we figure it out.
        /// </remarks>
        private void AddFragment(string location, IOpenApiElement fragment)
        {
            _fragments.Add(ToLocationUrl(location), fragment);
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
            if (_documents.TryGetValue(new Uri(BaseUrl,reference.ExternalResource),out var doc))
            {
                return doc.ResolveReference(reference, true); 
            }
            else if (_fragments.TryGetValue(new Uri(BaseUrl, reference.ExternalResource), out var fragment))
            {
                var frag = fragment as IOpenApiReferenceable;
                if (frag != null)
                {
                    return null; // frag.ResolveReference(reference, true); // IOpenApiElement needs to implement ResolveReference
                }
                else
                {
                    return null;
                }
            }
            return null;
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
            return new Uri(BaseUrl, location);
        }
    }
}
