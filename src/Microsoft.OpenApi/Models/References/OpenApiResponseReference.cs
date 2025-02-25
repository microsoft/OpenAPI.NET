﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Response Object Reference.
    /// </summary>
    public class OpenApiResponseReference : BaseOpenApiReferenceHolder<OpenApiResponse, IOpenApiResponse>, IOpenApiResponse
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
        public OpenApiResponseReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null):base(referenceId, hostDocument, ReferenceType.Response, externalResource)
        {
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="openApiResponseReference">The reference to copy</param>
        private OpenApiResponseReference(OpenApiResponseReference openApiResponseReference):base(openApiResponseReference)
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
        public IDictionary<string, IOpenApiHeader>? Headers { get => Target?.Headers; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiLink>? Links { get => Target?.Links; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public override IOpenApiResponse CopyReferenceAsTargetElementWithOverrides(IOpenApiResponse source)
        {
            return source is OpenApiResponse ? new OpenApiResponse(this) : source;
        }

        /// <inheritdoc/>
        public IOpenApiResponse CreateShallowCopy()
        {
            return new OpenApiResponseReference(this);
        }
    }
}
