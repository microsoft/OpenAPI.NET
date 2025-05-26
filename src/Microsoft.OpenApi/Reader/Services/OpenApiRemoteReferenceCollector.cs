// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Builds a list of all remote references used in an OpenApi document
    /// </summary>
    internal class OpenApiRemoteReferenceCollector : OpenApiVisitorBase
    {
        private readonly Dictionary<string, OpenApiReference> _references = new();

        /// <summary>
        /// List of all external references collected from OpenApiDocument
        /// </summary>
        public IEnumerable<OpenApiReference> References
        {
            get
            {
                return _references.Values;
            }
        }

        /// <inheritdoc/>
        public override void Visit(IOpenApiReferenceHolder referenceHolder)
        {
            AddExternalReferences(referenceHolder.Reference);
        }

        /// <summary>
        /// Collect external references
        /// </summary>
        private void AddExternalReferences(OpenApiReference? reference)
        {
            if (reference is {IsExternal: true} && reference.ExternalResource is {} externalResource&&
                !_references.ContainsKey(externalResource))
            {
                _references.Add(externalResource, reference);
            }
        }
    }
}
