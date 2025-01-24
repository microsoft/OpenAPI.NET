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
    /// Parameter Object Reference.
    /// </summary>
    public class OpenApiParameterReference : IOpenApiParameter, IOpenApiReferenceHolder<OpenApiParameter, IOpenApiParameter>
    {
        /// <inheritdoc/>
        public OpenApiReference Reference { get; set; }

        /// <inheritdoc/>
        public bool UnresolvedReference { get; set; }
        internal OpenApiParameter _target;

        /// <summary>
        /// Gets the target parameter.
        /// </summary>
        /// <remarks>
        /// If the reference is not resolved, this will return null.
        /// </remarks>
        public OpenApiParameter Target
        {
            get
            {
                _target ??= Reference.HostDocument.ResolveReferenceTo<OpenApiParameter>(Reference);
                return _target;
            }
        }

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
        public OpenApiParameterReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            Reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Parameter,
                ExternalResource = externalResource
            };
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="parameter">The parameter reference to copy</param>
        public OpenApiParameterReference(OpenApiParameterReference parameter)
        {
            Utils.CheckArgumentNull(parameter);
            Reference = parameter.Reference != null ? new(parameter.Reference) : null;
            UnresolvedReference = parameter?.UnresolvedReference ?? false;
            //no need to copy summary and description as if they are not overridden, they will be fetched from the target
            //if they are, the reference copy will handle it
        }

        internal OpenApiParameterReference(OpenApiParameter target, string referenceId)
        {
            _target = target;

            Reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Parameter,
            };
        }

        /// <inheritdoc/>
        public string Name { get => Target.Name; }

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
        public bool AllowReserved { get => Target?.AllowReserved ?? default; }

        /// <inheritdoc/>
        public OpenApiSchema Schema { get => Target?.Schema; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExample> Examples { get => Target?.Examples; }

        /// <inheritdoc/>
        public JsonNode Example { get => Target?.Example; }

        /// <inheritdoc/>
        public ParameterLocation? In { get => Target?.In; }

        /// <inheritdoc/>
        public ParameterStyle? Style { get => Target?.Style; }
        
        /// <inheritdoc/>
        public bool Explode { get => Target?.Explode ?? default; }

        /// <inheritdoc/>
        public IDictionary<string, OpenApiMediaType> Content { get => Target.Content; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; }
        
        /// <inheritdoc/>
        public void SerializeAsV3(IOpenApiWriter writer)
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
        public void SerializeAsV31(IOpenApiWriter writer)
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
        public void SerializeAsV2(IOpenApiWriter writer)
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
        public IOpenApiParameter CopyReferenceAsTargetElementWithOverrides(IOpenApiParameter source)
        {
            return source is OpenApiParameter ? new OpenApiParameter(this) : source;
        }

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiParameter> action)
        {
            Utils.CheckArgumentNull(writer);
            action(writer, Target);
        }
    }
}
