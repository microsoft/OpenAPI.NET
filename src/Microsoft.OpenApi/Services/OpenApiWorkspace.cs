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

        public IEnumerable<OpenApiDocument> Documents { get; }  

        public IEnumerable<IOpenApiFragment> Fragments { get; }


        public bool Contains(string location)
        {
            return true;
        }
        public void AddDocument(string location, OpenApiDocument  document)
        {

        }

        public void AddFragment(string location, IOpenApiFragment fragment)
        {

        }

        public IOpenApiReferenceable ResolveReference(OpenApiReference reference)
        {
            // Find the doc/fragment
            // Call ResolveReference on it
            return null;
        }

    }

    public interface IOpenApiFragment
    {
        IOpenApiReferenceable ResolveReference(OpenApiReference reference);
    }
}
