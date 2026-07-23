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
        private readonly bool _overwriteExisting;

        public ReferenceHostDocumentSetter(OpenApiDocument currentDocument, bool overwriteExisting = false)
        {
            _currentDocument = currentDocument;
            _overwriteExisting = overwriteExisting;
        }

        /// <inheritdoc/>
        public override void Visit(IOpenApiReferenceHolder referenceHolder)
        {
            var reference = referenceHolder switch
            {
                IOpenApiReferenceHolder<OpenApiReferenceWithDescriptionAndSummary> { Reference: OpenApiReferenceWithDescriptionAndSummary withSummary } => withSummary,
                IOpenApiReferenceHolder<OpenApiReferenceWithDescription> { Reference: OpenApiReferenceWithDescription withDescription } => withDescription,
                IOpenApiReferenceHolder<JsonSchemaReference> { Reference: JsonSchemaReference jsonSchemaReference } => jsonSchemaReference,
                IOpenApiReferenceHolder<BaseOpenApiReference> { Reference: BaseOpenApiReference baseReference } => baseReference,
                _ => throw new OpenApiException($"Unsupported reference holder type: {referenceHolder.GetType().FullName}")
            };
            if (_overwriteExisting)
            {
                reference.SetHostDocument(_currentDocument);
            }
            else
            {
                reference.EnsureHostDocumentIsSet(_currentDocument);
            }
        }
    }
}
