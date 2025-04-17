﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Schema reference object
    /// </summary>
    public class OpenApiSchemaReference : BaseOpenApiReferenceHolder<OpenApiSchema, IOpenApiSchema>, IOpenApiSchema
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
        public OpenApiSchemaReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null):base(referenceId, hostDocument, ReferenceType.Schema, externalResource)
        {
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="schema">The schema reference to copy</param>
        private OpenApiSchemaReference(OpenApiSchemaReference schema):base(schema)
        {
        }

        /// <inheritdoc/>
        public string? Description
        {
            get => string.IsNullOrEmpty(Reference.Description) ? Target?.Description : Reference.Description;
            set => Reference.Description = value;
        }

        /// <inheritdoc/>
        public string? Title { get => Target?.Title; }
        /// <inheritdoc/>
        public Uri? Schema { get => Target?.Schema; }
        /// <inheritdoc/>
        public string? Id { get => Target?.Id; }
        /// <inheritdoc/>
        public string? Comment { get => Target?.Comment; }
        /// <inheritdoc/>
        public Dictionary<string, bool>? Vocabulary { get => Target?.Vocabulary; }
        /// <inheritdoc/>
        public string? DynamicRef { get => Target?.DynamicRef; }
        /// <inheritdoc/>
        public string? DynamicAnchor { get => Target?.DynamicAnchor; }
        /// <inheritdoc/>
        public Dictionary<string, IOpenApiSchema>? Definitions { get => Target?.Definitions; }
        /// <inheritdoc/>
        public string? ExclusiveMaximum { get => Target?.ExclusiveMaximum; }
        /// <inheritdoc/>
        public string? ExclusiveMinimum { get => Target?.ExclusiveMinimum; }
        /// <inheritdoc/>
        public JsonSchemaType? Type { get => Target?.Type; }
        /// <inheritdoc/>
        public string? Const { get => Target?.Const; }
        /// <inheritdoc/>
        public string? Format { get => Target?.Format; }
        /// <inheritdoc/>
        public string? Maximum { get => Target?.Maximum; }
        /// <inheritdoc/>
        public string? Minimum { get => Target?.Minimum; }
        /// <inheritdoc/>
        public int? MaxLength { get => Target?.MaxLength; }
        /// <inheritdoc/>
        public int? MinLength { get => Target?.MinLength; }
        /// <inheritdoc/>
        public string? Pattern { get => Target?.Pattern; }
        /// <inheritdoc/>
        public decimal? MultipleOf { get => Target?.MultipleOf; }
        /// <inheritdoc/>
        public JsonNode? Default { get => Target?.Default; }
        /// <inheritdoc/>
        public bool ReadOnly { get => Target?.ReadOnly ?? false; }
        /// <inheritdoc/>
        public bool WriteOnly { get => Target?.WriteOnly ?? false; }
        /// <inheritdoc/>
        public List<IOpenApiSchema>? AllOf { get => Target?.AllOf; }
        /// <inheritdoc/>
        public List<IOpenApiSchema>? OneOf { get => Target?.OneOf; }
        /// <inheritdoc/>
        public List<IOpenApiSchema>? AnyOf { get => Target?.AnyOf; }
        /// <inheritdoc/>
        public IOpenApiSchema? Not { get => Target?.Not; }
        /// <inheritdoc/>
        public HashSet<string>? Required { get => Target?.Required; }
        /// <inheritdoc/>
        public IOpenApiSchema? Items { get => Target?.Items; }
        /// <inheritdoc/>
        public int? MaxItems { get => Target?.MaxItems; }
        /// <inheritdoc/>
        public int? MinItems { get => Target?.MinItems; }
        /// <inheritdoc/>
        public bool? UniqueItems { get => Target?.UniqueItems; }
        /// <inheritdoc/>
        public Dictionary<string, IOpenApiSchema>? Properties { get => Target?.Properties; }
        /// <inheritdoc/>
        public Dictionary<string, IOpenApiSchema>? PatternProperties { get => Target?.PatternProperties; }
        /// <inheritdoc/>
        public int? MaxProperties { get => Target?.MaxProperties; }
        /// <inheritdoc/>
        public int? MinProperties { get => Target?.MinProperties; }
        /// <inheritdoc/>
        public bool AdditionalPropertiesAllowed { get => Target?.AdditionalPropertiesAllowed ?? true; }
        /// <inheritdoc/>
        public IOpenApiSchema? AdditionalProperties { get => Target?.AdditionalProperties; }
        /// <inheritdoc/>
        public OpenApiDiscriminator? Discriminator { get => Target?.Discriminator; }
        /// <inheritdoc/>
        public JsonNode? Example { get => Target?.Example; }
        /// <inheritdoc/>
        public List<JsonNode>? Examples { get => Target?.Examples; }
        /// <inheritdoc/>
        public List<JsonNode>? Enum { get => Target?.Enum; }
        /// <inheritdoc/>
        public bool UnevaluatedProperties { get => Target?.UnevaluatedProperties ?? false; }
        /// <inheritdoc/>
        public OpenApiExternalDocs? ExternalDocs { get => Target?.ExternalDocs; }
        /// <inheritdoc/>
        public bool Deprecated { get => Target?.Deprecated ?? false; }
        /// <inheritdoc/>
        public OpenApiXml? Xml { get => Target?.Xml; }
        /// <inheritdoc/>
        public OpenApiExtensionDictionary? Extensions { get => Target?.Extensions; }

        /// <inheritdoc/>
        public Dictionary<string, JsonNode>? UnrecognizedKeywords { get => Target?.UnrecognizedKeywords; }

        /// <inheritdoc/>
        public Dictionary<string, HashSet<string>>? DependentRequired { get => Target?.DependentRequired; }

        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeAsWithoutLoops(writer, (w, element) => (element is IOpenApiSchema s ? CopyReferenceAsTargetElementWithOverrides(s) : element).SerializeAsV31(w));
        }

        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeAsWithoutLoops(writer, (w, element) => element.SerializeAsV3(w));
        }
        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            SerializeAsWithoutLoops(writer, (w, element) => element.SerializeAsV2(w));
        }
        private void SerializeAsWithoutLoops(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> action)
        {
            if (!writer.GetSettings().ShouldInlineReference(Reference))
            {
                action(writer, Reference);
            }
            // If Loop is detected then just Serialize as a reference.
            else if (!writer.GetSettings().LoopDetector.PushLoop<IOpenApiSchema>(this))
            {
                writer.GetSettings().LoopDetector.SaveLoop<IOpenApiSchema>(this);
                action(writer, Reference);
            }
            else
            {
                SerializeInternal(writer, (w, element) => action(w, element));
                writer.GetSettings().LoopDetector.PopLoop<IOpenApiSchema>();
            }

        }
        /// <inheritdoc/>
        public override IOpenApiSchema CopyReferenceAsTargetElementWithOverrides(IOpenApiSchema source)
        {
            return source is OpenApiSchema ? new OpenApiSchema(this) : source;
        }
        /// <inheritdoc/>
        public IOpenApiSchema CreateShallowCopy()
        {
            return new OpenApiSchemaReference(this);
        }
    }
}
