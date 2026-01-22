// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Security Scheme Object Reference.
    /// </summary>
    public class OpenApiSecuritySchemeReference : BaseOpenApiReferenceHolder<OpenApiSecurityScheme, IOpenApiSecurityScheme, OpenApiReferenceWithDescription>, IOpenApiSecurityScheme
    {
        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        /// <param name="externalResource">The externally referenced file.</param>
        public OpenApiSecuritySchemeReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null) : base(referenceId, hostDocument, ReferenceType.SecurityScheme, externalResource)
        {
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="openApiSecuritySchemeReference">The reference to copy</param>
        private OpenApiSecuritySchemeReference(OpenApiSecuritySchemeReference openApiSecuritySchemeReference) : base(openApiSecuritySchemeReference)
        {

        }

        /// <inheritdoc/>
        public string? Description
        {
            get => string.IsNullOrEmpty(Reference.Description) ? Target?.Description : Reference.Description;
            set => Reference.Description = value;
        }

        /// <inheritdoc/>
        public string? Name { get => Target?.Name; }

        /// <inheritdoc/>
        public ParameterLocation? In { get => Target?.In; }

        /// <inheritdoc/>
        public string? Scheme { get => Target?.Scheme; }

        /// <inheritdoc/>
        public string? BearerFormat { get => Target?.BearerFormat; }

        /// <inheritdoc/>
        public OpenApiOAuthFlows? Flows { get => Target?.Flows; }

        /// <inheritdoc/>
        public Uri? OpenIdConnectUrl { get => Target?.OpenIdConnectUrl; }

        /// <inheritdoc/>
        public Uri? OAuth2MetadataUrl { get => Target?.OAuth2MetadataUrl; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public SecuritySchemeType? Type { get => Target?.Type; }

        /// <inheritdoc/>
        public bool Deprecated { get => Target?.Deprecated ?? default; }

        /// <inheritdoc/>
        public override IOpenApiSecurityScheme CopyReferenceAsTargetElementWithOverrides(IOpenApiSecurityScheme source)
        {
            return source is OpenApiSecurityScheme ? new OpenApiSecurityScheme(this) : source;
        }

        /// <inheritdoc/>
        public IOpenApiSecurityScheme CreateShallowCopy()
        {
            return new OpenApiSecuritySchemeReference(this);
        }
        /// <inheritdoc/>
        protected override OpenApiReferenceWithDescription CopyReference(OpenApiReferenceWithDescription sourceReference)
        {
            return new OpenApiReferenceWithDescription(sourceReference);
        }
    }
}
