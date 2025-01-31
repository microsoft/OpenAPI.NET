// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Tag Object Reference
    /// </summary>
    public class OpenApiTagReference : BaseOpenApiReferenceHolder<OpenApiTag, IOpenApiTag>, IOpenApiTag
    {
        /// <summary>
        /// Resolved target of the reference.
        /// </summary>
        public override OpenApiTag Target
        {
            get
            {
                _target ??= Reference.HostDocument?.Tags.FirstOrDefault(t => StringComparer.Ordinal.Equals(t.Name, Reference.Id));
                return _target;
            }
        }

        /// <summary>
        /// Constructor initializing the reference object.
        /// </summary>
        /// <param name="referenceId">The reference Id.</param>
        /// <param name="hostDocument">The host OpenAPI document.</param>
        public OpenApiTagReference(string referenceId, OpenApiDocument hostDocument):base(referenceId, hostDocument, ReferenceType.Tag)
        {
        }

        internal OpenApiTagReference(OpenApiTag target, string referenceId):base(target, referenceId, ReferenceType.Tag)
        {
        }

        /// <inheritdoc/>
        public string Description
        {
            get => string.IsNullOrEmpty(Reference?.Description) ? Target?.Description : Reference.Description;
        }

        /// <inheritdoc/>
        public OpenApiExternalDocs ExternalDocs { get => Target?.ExternalDocs; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension> Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public string Name { get => Target?.Name; }
        /// <inheritdoc/>
        public override IOpenApiTag CopyReferenceAsTargetElementWithOverrides(IOpenApiTag source)
        {
            return source is OpenApiTag ? new OpenApiTag(this) : source;
        }
    }
}
