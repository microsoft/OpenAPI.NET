// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Reader.Services
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

        /// <summary>
        /// Collect reference for each reference
        /// </summary>
        /// <param name="referenceable"></param>
        public override void Visit(IOpenApiReferenceable referenceable)
        {
            AddExternalReferences(referenceable.Reference);
        }

        /// <summary>
        /// Collect external references
        /// </summary>
        private void AddExternalReferences(OpenApiReference reference)
        {
            if (reference is {IsExternal: true} &&
                !_references.ContainsKey(reference.ExternalResource))
            {
                _references.Add(reference.ExternalResource, reference);
            }
        }
    }
}
