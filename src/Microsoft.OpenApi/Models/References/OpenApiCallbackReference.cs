// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Callback Object Reference: A reference to a map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallbackReference : OpenApiCallback
    {
        private OpenApiCallback _target;
        private readonly OpenApiDocument _hostDocument;

        private OpenApiCallback Target
        {
            get
            {
                _target ??= _hostDocument.ResolveReferenceTo<OpenApiCallback>(Reference);
                return _target;
            }
        }
        
        /// <inheritdoc/>
        public override Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get => Target.PathItems; set => Target.PathItems = value; }

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
