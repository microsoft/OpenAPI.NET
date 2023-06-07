// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Request Body Object Reference.
    /// </summary>
    internal class OpenApiRequestBodyReference : OpenApiRequestBody
    {
        private OpenApiRequestBody _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiRequestBody Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiRequestBody>(Reference);
                return _target;
            }
        }

        public OpenApiRequestBodyReference(OpenApiDocument hostDocument)
        {
            _hostDocument = hostDocument;
        }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override IDictionary<string, OpenApiMediaType> Content { get => Target.Content; set => Target.Content = value; }

        /// <inheritdoc/>
        public override bool Required { get => Target.Required; set => Target.Required = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override bool UnresolvedReference { get => base.UnresolvedReference; set => base.UnresolvedReference = value; }

        /// <inheritdoc/>
        public override OpenApiReference Reference
        {
            get => Target.Reference;
            set
            {
                Target.Reference = value;
                _target = null;
            }
        }
    }
}
