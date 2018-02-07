// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Contains a set of OpenApi documents that reference each other
    /// </summary>
    public class OpenApiWorkspace
    {
        /// <summary>
        /// Load OpenApiDocuments and IOpenApiElements referenced
        /// </summary>
        /// <param name="remoteReferences">List of remote references to load</param>
        public async Task LoadAsync(List<OpenApiReference> remoteReferences)
        {
            //TODO: Load remote documents
            return;
        }
    }
}
