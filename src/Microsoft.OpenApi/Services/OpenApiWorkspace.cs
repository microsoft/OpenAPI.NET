// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
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
        private Dictionary<string, OpenApiDocument> _documents = new Dictionary<string, OpenApiDocument>();

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
        public IEnumerable<IOpenApiFragment> Fragments { get; }


        public bool Contains(string location)
        {
            return true;
        }

        public void AddDocument(string location, OpenApiDocument  document) 
        {
            _documents.Add(location, document);
        }

        public void AddFragment(string location, IOpenApiFragment fragment)
        {

        }

        public IOpenApiReferenceable ResolveReference(OpenApiReference reference)
        {
            if (!_documents.TryGetValue(reference.ExternalResource,out var doc))
            {
                return null;
            }
            
            return doc.ResolveReference(reference,true);
        }

    }

    public interface IOpenApiFragment
    {
        IOpenApiReferenceable ResolveReference(OpenApiReference reference);
    }
}
