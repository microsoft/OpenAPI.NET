// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Header Object Reference.
    /// </summary>
    public class OpenApiHeaderReference : BaseOpenApiReferenceHolder<OpenApiHeader, IOpenApiHeader>, IOpenApiHeader
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
        public OpenApiHeaderReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null):base(referenceId, hostDocument, ReferenceType.Header, externalResource)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="header">The <see cref="OpenApiHeaderReference"/> object to copy</param>
        public OpenApiHeaderReference(OpenApiHeaderReference header):base(header)
        {
        }

        internal OpenApiHeaderReference(OpenApiHeader target, string referenceId):base(target, referenceId, ReferenceType.Header)
        {
        }

        /// <inheritdoc/>
        public string Description
        {
            get => string.IsNullOrEmpty(Reference?.Description) ? Target?.Description : Reference.Description;
            set 
            {
                if (Reference is not null)
                {
                    Reference.Description = value;
                }
            }
        }

        /// <inheritdoc/>
        public bool Required { get => Target?.Required ?? default; }

        /// <inheritdoc/>
        public bool Deprecated { get => Target?.Deprecated ?? default; }

        /// <inheritdoc/>
        public bool AllowEmptyValue { get => Target?.AllowEmptyValue ?? default; }

        /// <inheritdoc/>
        public OpenApiSchema Schema { get => Target?.Schema; }

        /// <inheritdoc/>
        public ParameterStyle? Style { get => Target?.Style; }

        /// <inheritdoc/>
        public bool Explode { get => Target?.Explode ?? default; }

        /// <inheritdoc/>
        public bool AllowReserved { get => Target?.AllowReserved ?? default; }

        /// <inheritdoc/>
        public JsonNode Example { get => Target?.Example; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExample> Examples { get => Target?.Examples; }

        /// <inheritdoc/>
        public IDictionary<string, OpenApiMediaType> Content { get => Target?.Content; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension> Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV31(writer);
            }
            else
            {
                SerializeInternal(writer, (writer, element) => CopyReferenceAsTargetElementWithOverrides(element).SerializeAsV31(writer));
            }
        }

        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV3(writer);
            }
            else
            {
                SerializeInternal(writer, (writer, element) => CopyReferenceAsTargetElementWithOverrides(element).SerializeAsV3(writer));
            }
        }

        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                Reference.SerializeAsV2(writer);
            }
            else
            {
                SerializeInternal(writer, (writer, element) => CopyReferenceAsTargetElementWithOverrides(element).SerializeAsV2(writer));
            }
        }
        /// <inheritdoc/>
        public override IOpenApiHeader CopyReferenceAsTargetElementWithOverrides(IOpenApiHeader source)
        {
            return source is OpenApiHeader ? new OpenApiHeader(this) : source;
        }

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiHeader> action)
        {
            Utils.CheckArgumentNull(writer);
            action(writer, Target);
        }
    }
}
