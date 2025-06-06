// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Request Body Object Reference.
    /// </summary>
    public class OpenApiRequestBodyReference : BaseOpenApiReferenceHolder<OpenApiRequestBody, IOpenApiRequestBody, BaseOpenApiReference>, IOpenApiRequestBody
    {
        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        /// <param name="externalResource">Optional: External resource in the reference.
        /// It may be:
        /// 1. a absolute/relative file path, for example:  ../commons/pet.json
        /// 2. a Url, for example: http://localhost/pet.json
        /// </param>
        public OpenApiRequestBodyReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null) : base(referenceId, hostDocument, ReferenceType.RequestBody, externalResource)
        {
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="openApiRequestBodyReference">The reference to copy</param>
        private OpenApiRequestBodyReference(OpenApiRequestBodyReference openApiRequestBodyReference) : base(openApiRequestBodyReference)
        {

        }

        /// <inheritdoc/>
        public string? Description
        {
            get => string.IsNullOrEmpty(Reference.Description) ? Target?.Description : Reference.Description;
            set => Reference.Description = value;
        }

        /// <inheritdoc/>
        public IDictionary<string, OpenApiMediaType>? Content { get => Target?.Content; }

        /// <inheritdoc/>
        public bool Required { get => Target?.Required ?? false; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public override IOpenApiRequestBody CopyReferenceAsTargetElementWithOverrides(IOpenApiRequestBody source)
        {
            return source is OpenApiRequestBody ? new OpenApiRequestBody(this) : source;
        }
        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            // doesn't exist in v2
        }
        /// <inheritdoc/>
        public IOpenApiParameter? ConvertToBodyParameter(IOpenApiWriter writer)
        {
            if (writer.GetSettings().ShouldInlineReference(Reference))
            {
                return Target?.ConvertToBodyParameter(writer);
            }

            return Reference.Id is not null ? new OpenApiParameterReference(Reference.Id, Reference.HostDocument) : null;
        }
        /// <inheritdoc/>
        public IEnumerable<IOpenApiParameter>? ConvertToFormDataParameters(IOpenApiWriter writer)
        {
            if (writer.GetSettings().ShouldInlineReference(Reference))
            {
                return Target?.ConvertToFormDataParameters(writer);
            }

            if (Content == null || !Content.Any())
                return [];

            return Content.First().Value.Schema?.Properties?.Select(x => new OpenApiParameterReference(x.Key, Reference.HostDocument));
        }

        /// <inheritdoc/>
        public IOpenApiRequestBody CreateShallowCopy()
        {
            return new OpenApiRequestBodyReference(this);
        }
        /// <inheritdoc/>
        protected override BaseOpenApiReference CopyReference(BaseOpenApiReference sourceReference)
        {
            return new BaseOpenApiReference(sourceReference);
        }
    }
}
