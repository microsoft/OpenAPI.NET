// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
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
            if (referenceHolder is IOpenApiReferenceHolder<BaseOpenApiReference> { Reference: BaseOpenApiReference reference })
            {
                reference.EnsureHostDocumentIsSet(_currentDocument);
            }
            else if (referenceHolder is IOpenApiReferenceHolder<JsonSchemaReference> { Reference: JsonSchemaReference jsonSchemaReference })
            {
                jsonSchemaReference.EnsureHostDocumentIsSet(_currentDocument);
            }
            else if (referenceHolder is IOpenApiReferenceHolder<OpenApiReferenceWithDescriptionAndSummary> { Reference: OpenApiReferenceWithDescriptionAndSummary withSummaryReference })
            {
                withSummaryReference.EnsureHostDocumentIsSet(_currentDocument);
            }
        }
    }
}
