// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Tag Object Reference
    /// </summary>
    internal class OpenApiTagReference : OpenApiTag
    {
        private OpenApiTag _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiTag Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiTag>(Reference);
                return _target;
            }
        }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override OpenApiExternalDocs ExternalDocs { get => Target.ExternalDocs; set => Target.ExternalDocs = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override string Name { get => Target.Name; set => Target.Name = value; }

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
