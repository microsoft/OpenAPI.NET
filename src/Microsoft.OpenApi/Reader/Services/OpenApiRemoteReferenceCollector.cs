// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Builds a list of all remote references used in an OpenApi document
    /// </summary>
    internal class OpenApiRemoteReferenceCollector : OpenApiVisitorBase
    {
        private readonly Dictionary<string, BaseOpenApiReference> _references = new();

        /// <summary>
        /// List of all external references collected from OpenApiDocument
        /// </summary>
        public IEnumerable<BaseOpenApiReference> References
        {
            get
            {
                return _references.Values;
            }
        }

        /// <inheritdoc/>
        public override void Visit(IOpenApiReferenceHolder referenceHolder)
        {
            if (referenceHolder is IOpenApiReferenceHolder<BaseOpenApiReference> { Reference: BaseOpenApiReference reference })
            {
                AddExternalReferences(reference);
            }
            else if (referenceHolder is IOpenApiReferenceHolder<JsonSchemaReference> { Reference: JsonSchemaReference jsonSchemaReference })
            {
                AddExternalReferences(jsonSchemaReference);
            }
            else if (referenceHolder is IOpenApiReferenceHolder<OpenApiReferenceWithSummary> { Reference: OpenApiReferenceWithSummary withSummaryReference })
            {
                AddExternalReferences(withSummaryReference);
            }
        }

        /// <summary>
        /// Collect external references
        /// </summary>
        private void AddExternalReferences(BaseOpenApiReference? reference)
        {
            if (reference is {IsExternal: true} && reference.ExternalResource is {} externalResource&&
                !_references.ContainsKey(externalResource))
            {
                _references.Add(externalResource, reference);
            }
        }
    }
}
