// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
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
        private Guid _guid = new();

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
            AddExternalReference(referenceable.Reference);
            AddLocalReference(referenceable.Reference);
        }

        /// <summary>
        /// Collect external reference
        /// </summary>
        private void AddExternalReference(OpenApiReference reference)
        {
            if (reference is {IsExternal: true} &&
                !_references.ContainsKey(reference.ExternalResource))
            {
                _references.Add(reference.ExternalResource, reference);
            }
        }

        private void AddLocalReference(OpenApiReference reference)
        {
            if (reference is { IsExternal: false } &&
                !_references.ContainsKey(reference.ReferenceV3))
            {
                reference.ExternalResource = _guid.ToString();
                _references.Add(reference.ReferenceV3, reference);
            }
        }
    }
}
