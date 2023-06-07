// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Response Object Reference.
    /// </summary>
    internal class OpenApiResponseReference : OpenApiResponse
    {
        private OpenApiResponse _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiResponse Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiResponse>(Reference);
                return _target;
            }
        }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiMediaType> Content { get => Target.Content; set => Target.Content = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiHeader> Headers { get => Target.Headers; set => Target.Headers = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiLink> Links { get => Target.Links; set => Target.Links = value; }

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
