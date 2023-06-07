// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Path Item Object Reference: to describe the operations available on a single path.
    /// </summary>
    internal class OpenApiPathItemReference : OpenApiPathItem
    {
        private OpenApiPathItem _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiPathItem Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiPathItem>(Reference);
                return _target;
            }
        }

        /// <inheritdoc/>
        public override string Summary { get => Target.Summary; set => Target.Summary = value; }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override IDictionary<OperationType, OpenApiOperation> Operations { get => Target.Operations; set => Target.Operations = value; }

        /// <inheritdoc/>
        public override IList<OpenApiServer> Servers { get => Target.Servers; set => Target.Servers = value; }

        /// <inheritdoc/>
        public override IList<OpenApiParameter> Parameters { get => Target.Parameters; set => Target.Parameters = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override bool UnresolvedReference { get => base.UnresolvedReference; set => base.UnresolvedReference = value; }

        /// <inheritdoc/>
        public override OpenApiReference Reference
        {
            get => base.Reference;
            set
            {
                base.Reference = value;
                _target = null;
            }
        }
    }
}
