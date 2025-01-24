// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// This class is used to walk an OpenApiDocument and sets the host document of IOpenApiReferenceable objects
    /// </summary>
    internal class ReferenceHostDocumentSetter : OpenApiVisitorBase
    {
        private readonly OpenApiDocument _currentDocument;

        public ReferenceHostDocumentSetter(OpenApiDocument currentDocument)
        {
            _currentDocument = currentDocument;
        }

        /// <inheritdoc/>
        public override void Visit(IOpenApiReferenceHolder referenceHolder)
        {
            if (referenceHolder.Reference != null)
            {
                referenceHolder.Reference.HostDocument = _currentDocument;
            }
        }
    }
}
