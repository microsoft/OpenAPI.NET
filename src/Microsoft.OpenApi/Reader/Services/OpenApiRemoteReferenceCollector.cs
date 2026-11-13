// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Builds a list of all remote references used in an OpenApi document
    /// </summary>
    internal class OpenApiRemoteReferenceCollector : OpenApiVisitorBase
    {
        private readonly Dictionary<string, BaseOpenApiReference> _references = new();
        private readonly HashSet<IOpenApiSchema> _schemas = new(SchemaIdentityComparer.Instance);
        internal List<IOpenApiReferenceHolder> ReferenceHolders { get; } = new();

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
            if (referenceHolder is IOpenApiSchema schema)
            {
                Visit(schema);
                return;
            }

            AddReference(referenceHolder);
        }

        /// <inheritdoc/>
        public override void Visit(IOpenApiSchema schema)
        {
            var pending = new Stack<IOpenApiSchema>();
            pending.Push(schema);
            while (pending.Count > 0)
            {
                var current = pending.Pop();
                if (!_schemas.Add(current))
                {
                    continue;
                }

                IEnumerable<IOpenApiSchema> children;
                OpenApiDiscriminator? discriminator;
                if (current is OpenApiSchemaReference reference)
                {
                    AddReference(reference);
                    children = OpenApiWorkspace.EnumerateChildren(reference.Reference);
                    discriminator = reference.Reference.Discriminator;
                }
                else
                {
                    children = OpenApiWorkspace.EnumerateChildren(current);
                    discriminator = current.Discriminator;
                }
                if (discriminator?.Mapping is { } mapping)
                {
                    foreach (var mappedSchema in mapping.Values)
                    {
                        pending.Push(mappedSchema);
                    }
                }
                if (discriminator?.DefaultMapping is { } defaultMapping)
                {
                    pending.Push(defaultMapping);
                }
                foreach (var child in children.Where(static child => child is not null))
                {
                    pending.Push(child);
                }
            }
        }

        private void AddReference(IOpenApiReferenceHolder referenceHolder)
        {
            ReferenceHolders.Add(referenceHolder);
            var reference = GetReference(referenceHolder);
            AddExternalReferences(reference);
        }

        internal static BaseOpenApiReference GetReference(IOpenApiReferenceHolder referenceHolder)
            => referenceHolder switch
            {
                IOpenApiReferenceHolder<OpenApiReferenceWithDescriptionAndSummary> { Reference: OpenApiReferenceWithDescriptionAndSummary withSummary } => withSummary,
                IOpenApiReferenceHolder<OpenApiReferenceWithDescription> { Reference: OpenApiReferenceWithDescription withDescription } => withDescription,
                IOpenApiReferenceHolder<JsonSchemaReference> { Reference: JsonSchemaReference jsonSchemaReference } => jsonSchemaReference,
                IOpenApiReferenceHolder<BaseOpenApiReference> { Reference: BaseOpenApiReference baseReference } => baseReference,
                _ => throw new OpenApiException($"Unsupported reference holder type: {referenceHolder.GetType().FullName}")
            };

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

        private sealed class SchemaIdentityComparer : IEqualityComparer<IOpenApiSchema>
        {
            public static SchemaIdentityComparer Instance { get; } = new();
            public bool Equals(IOpenApiSchema? x, IOpenApiSchema? y) => ReferenceEquals(x, y);
            public int GetHashCode(IOpenApiSchema obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}
