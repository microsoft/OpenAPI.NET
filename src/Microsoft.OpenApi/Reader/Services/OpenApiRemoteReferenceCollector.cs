// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Json.Schema;
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
        /// Collect external OpenApiReferences references.
        /// </summary>
        /// <param name="referenceable">The referenceable object to be visited.</param>
        public override void Visit(IOpenApiReferenceable referenceable)
        {
            AddExternalReferences(referenceable.Reference);
        }
        
        /// <summary>
        /// Collect external JsonSchema references.
        /// </summary>
        /// <param name="value">The JsonSchema to be visited.</param>
        public override void Visit(ref JsonSchema value)
        {
            AddExternalJsonSchemaReferences(value);
        }

        /// <summary>
        /// Collect external OpenApiReferences references
        /// </summary>
        private void AddExternalReferences(OpenApiReference reference)
        {
            if (reference is {IsExternal: true} &&
                !_references.ContainsKey(reference.ExternalResource))
            {
                _references.Add(reference.ExternalResource, reference);
            }
        }

        /// <summary>
        /// Collect external JsonSchema references
        /// </summary>
        /// <param name="schema"></param>
        private void AddExternalJsonSchemaReferences(JsonSchema schema)
        {
            Uri jsonRef = schema.GetRef();
            if (jsonRef != null && schema.Keywords.Count == 1)
            {
                var externalResource = jsonRef.OriginalString.Split('#').FirstOrDefault();
                if (!string.IsNullOrEmpty(externalResource) && !_references.ContainsKey(externalResource))
                {
                    var reference = new OpenApiReference()
                    {
                        ExternalResource = externalResource
                    };

                    _references.Add(externalResource, reference);
                }
            }
        }
    }
}
