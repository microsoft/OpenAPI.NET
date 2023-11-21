// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Readers.Services
{
    /// <summary>
    /// Builds a list of all remote references used in an OpenApi document
    /// </summary>
    internal class OpenApiRemoteReferenceCollector : OpenApiVisitorBase
    {
        private OpenApiDocument _document;
        private Dictionary<string, OpenApiReference> _references = new Dictionary<string, OpenApiReference>();
        public OpenApiRemoteReferenceCollector(OpenApiDocument document)
        {
            _document = document;
        }

        /// <summary>
        /// List of external references collected from OpenApiDocument
        /// </summary>
        public IEnumerable<OpenApiReference> References
        {
            get
            {
                return _references.Values;
            }
        }

        /// <summary>
        /// Collect reference for each reference 
        /// </summary>
        /// <param name="referenceable"></param>
        public override void Visit(IOpenApiReferenceable referenceable)
        {
            AddReference(referenceable.Reference);
        }

        /// <summary>
        /// Collect external reference
        /// </summary>
        private void AddReference(OpenApiReference reference)
        {
            if (reference != null)
            {
                if (reference.IsExternal)
                {
                    if (!_references.ContainsKey(reference.ExternalResource))
                    {
                        _references.Add(reference.ExternalResource, reference);
                    }
                }
            }
        }
    }
}
