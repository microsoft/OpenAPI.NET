// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Example Object Reference.
    /// </summary>
    internal class OpenApiExampleReference : OpenApiExample
    {
        private OpenApiExample _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiExample Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiExample>(Reference);
                return _target;
            }
        }

        /// <inheritdoc/>
        public override string Description { get => Target.Description; set => Target.Description = value; }

        /// <inheritdoc/>
        public override string Summary { get => Target.Summary; set => Target.Summary = value; }

        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override string ExternalValue { get => Target.ExternalValue; set => Target.ExternalValue = value; }

        /// <inheritdoc/>
        public override IOpenApiAny Value { get => Target.Value; set => Target.Value = value; }

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
