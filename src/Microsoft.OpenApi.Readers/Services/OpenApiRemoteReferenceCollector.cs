// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
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
        private Dictionary<string, OpenApiReference> _references;
        public OpenApiRemoteReferenceCollector(OpenApiDocument document)
        {
            _document = document;
        }

        // TODO PathItem
        // TODO Example

        /// <summary>
        /// List of external references collected from OpenApiDocument
        /// </summary>
        public IEnumerable<OpenApiReference> References
        {
            get {
                return _references.Values;
            }
        }

        /// <summary>
        /// Collect external reference
        /// </summary>
        public override void Visit(OpenApiParameter parameter)
        {
            AddReference(parameter.Reference);
        }

        /// <summary>
        /// Collect external reference
        /// </summary>
        public override void Visit(OpenApiCallback callback)
        {
            AddReference(callback.Reference);

        }

        /// <summary>
        /// Collect external reference
        /// </summary>
        public override void Visit(OpenApiLink link)
        {
            AddReference(link.Reference);
        }

        /// <summary>
        /// Collect external reference
        /// </summary>
        public override void Visit(OpenApiRequestBody requestBody)
        {
            AddReference(requestBody.Reference);
        }

        /// <summary>
        /// Collect external reference
        /// </summary>
        public override void Visit(OpenApiResponse header)
        {
            AddReference(header.Reference);
        }

        /// <summary>
        /// Collect external reference
        /// </summary>
        public override void Visit(OpenApiHeader header)
        {
            AddReference(header.Reference);
        }

        /// <summary>
        /// Collect external reference
        /// </summary>
        public override void Visit(OpenApiSchema schema)
        {
             AddReference(schema.Reference);
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
