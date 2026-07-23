// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
#pragma warning disable CS0618
    /// <summary>
    /// Schema reference object.
    /// Convenience getters return <c>$ref</c>-sibling keyword values authored on
    /// <see cref="IOpenApiReferenceHolder{V}.Reference"/> before falling back to resolved values from <see cref="Target"/>.
    /// These getters are object-model conveniences and do not represent JSON Schema
    /// evaluation semantics.
    /// </summary>
    public class OpenApiSchemaReference : BaseOpenApiReferenceHolder<OpenApiSchema, IOpenApiSchema, JsonSchemaReference>, IOpenApiSchema, IOpenApiSchemaMissingProperties, IOpenApiSchemaWithUnevaluatedProperties, IOpenApiExtensible, IDeepCopyable<IOpenApiSchema>
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
        public OpenApiSchemaReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null) : base(referenceId, hostDocument, ReferenceType.Schema, externalResource)
        {
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="schema">The schema reference to copy</param>
        private OpenApiSchemaReference(OpenApiSchemaReference schema) : base(schema)
        {
        }

        /// <summary>
        /// Resolves the target schema. For $dynamicRef-only references, delegates to
        /// <see cref="OpenApiWorkspace.ResolveDynamicRef"/> which resolves via per-document
        /// $dynamicAnchor and $anchor registries. Returns null when anchors are ambiguous
        /// (multiple candidates require dynamic-scope tracking this library does not perform).
        /// </summary>
        public override IOpenApiSchema? Target
        {
            get
            {
                if (Reference.IsDynamicRefOnly
                    && Reference.HostDocument is { } doc
                    && doc.Workspace is { } ws)
                {
                    return ws.ResolveDynamicRef(doc, Reference.DynamicRef!);
                }
                return base.Target;
            }
        }

        /// <inheritdoc/>
        public string? Description
        {
            get => string.IsNullOrEmpty(Reference.Description) ? Target?.Description : Reference.Description;
            set => Reference.Description = value;
        }

        /// <inheritdoc/>
        public string? Title
        {
            get => string.IsNullOrEmpty(Reference.Title) ? Target?.Title : Reference.Title;
            set => Reference.Title = value;
        }
        /// <inheritdoc/>
        public Uri? Schema { get => Reference.Schema ?? Target?.Schema; set => Reference.Schema = value; }
        /// <inheritdoc/>
        public string? Id { get => string.IsNullOrEmpty(Reference.SchemaId) ? Target?.Id : Reference.SchemaId; set => Reference.SchemaId = value; }
        /// <inheritdoc/>
        public string? Comment { get => string.IsNullOrEmpty(Reference.Comment) ? Target?.Comment : Reference.Comment; set => Reference.Comment = value; }
        /// <inheritdoc/>
        public IDictionary<string, bool>? Vocabulary { get => Reference.Vocabulary ?? Target?.Vocabulary; set => Reference.Vocabulary = value; }
        /// <inheritdoc/>
        public string? DynamicRef { get => string.IsNullOrEmpty(Reference.DynamicRef) ? Target?.DynamicRef : Reference.DynamicRef; set => Reference.DynamicRef = value; }
        /// <inheritdoc/>
        public string? DynamicAnchor { get => string.IsNullOrEmpty(Reference.DynamicAnchor) ? Target?.DynamicAnchor : Reference.DynamicAnchor; set => Reference.DynamicAnchor = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiSchema>? Definitions { get => Reference.Definitions ?? Target?.Definitions; set => Reference.Definitions = value; }
        /// <inheritdoc/>
        public string? Anchor { get => string.IsNullOrEmpty(Reference.Anchor) ? (Target as IOpenApiSchemaMissingProperties)?.Anchor : Reference.Anchor; set => Reference.Anchor = value; }
        /// <inheritdoc/>
        public string? ExclusiveMaximum { get => string.IsNullOrEmpty(Reference.ExclusiveMaximum) ? Target?.ExclusiveMaximum : Reference.ExclusiveMaximum; set => Reference.ExclusiveMaximum = value; }
        /// <inheritdoc/>
        public string? ExclusiveMinimum { get => string.IsNullOrEmpty(Reference.ExclusiveMinimum) ? Target?.ExclusiveMinimum : Reference.ExclusiveMinimum; set => Reference.ExclusiveMinimum = value; }
        /// <inheritdoc/>
        public JsonSchemaType? Type { get => Reference.SchemaType ?? Target?.Type; set => Reference.SchemaType = value; }
        /// <inheritdoc/>
        public string? Const { get => Reference.WasConstExplicitlySet ? Reference.Const : Target?.Const; set => Reference.Const = value; }
        /// <inheritdoc/>
        public string? Format { get => string.IsNullOrEmpty(Reference.Format) ? Target?.Format : Reference.Format; set => Reference.Format = value; }
        /// <inheritdoc/>
        public string? Maximum { get => string.IsNullOrEmpty(Reference.Maximum) ? Target?.Maximum : Reference.Maximum; set => Reference.Maximum = value; }
        /// <inheritdoc/>
        public string? Minimum { get => string.IsNullOrEmpty(Reference.Minimum) ? Target?.Minimum : Reference.Minimum; set => Reference.Minimum = value; }
        /// <inheritdoc/>
        public int? MaxLength { get => Reference.MaxLength ?? Target?.MaxLength; set => Reference.MaxLength = value; }
        /// <inheritdoc/>
        public int? MinLength { get => Reference.MinLength ?? Target?.MinLength; set => Reference.MinLength = value; }
        /// <inheritdoc/>
        public string? Pattern { get => string.IsNullOrEmpty(Reference.Pattern) ? Target?.Pattern : Reference.Pattern; set => Reference.Pattern = value; }
        /// <inheritdoc/>
        public decimal? MultipleOf { get => Reference.MultipleOf ?? Target?.MultipleOf; set => Reference.MultipleOf = value; }
        /// <inheritdoc/>
        public JsonNode? Default
        {
            get => Reference.Default ?? Target?.Default;
            set => Reference.Default = value;
        }
        /// <inheritdoc/>
        public bool ReadOnly
        {
            get => Reference.ReadOnly ?? Target?.ReadOnly ?? false;
            set => Reference.ReadOnly = value;
        }
        /// <inheritdoc/>
        public bool WriteOnly
        {
            get => Reference.WriteOnly ?? Target?.WriteOnly ?? false;
            set => Reference.WriteOnly = value;
        }
        /// <inheritdoc/>
        public IList<IOpenApiSchema>? AllOf { get => Reference.AllOf ?? Target?.AllOf; set => Reference.AllOf = value; }
        /// <inheritdoc/>
        public IList<IOpenApiSchema>? OneOf { get => Reference.OneOf ?? Target?.OneOf; set => Reference.OneOf = value; }
        /// <inheritdoc/>
        public IList<IOpenApiSchema>? AnyOf { get => Reference.AnyOf ?? Target?.AnyOf; set => Reference.AnyOf = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Not { get => Reference.Not ?? Target?.Not; set => Reference.Not = value; }
        /// <inheritdoc/>
        public ISet<string>? Required { get => Reference.Required ?? Target?.Required; set => Reference.Required = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Items { get => Reference.Items ?? Target?.Items; set => Reference.Items = value; }
        /// <inheritdoc/>
        public int? MaxItems { get => Reference.MaxItems ?? Target?.MaxItems; set => Reference.MaxItems = value; }
        /// <inheritdoc/>
        public int? MinItems { get => Reference.MinItems ?? Target?.MinItems; set => Reference.MinItems = value; }
        /// <inheritdoc/>
        public bool? UniqueItems { get => Reference.UniqueItems ?? Target?.UniqueItems; set => Reference.UniqueItems = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Contains { get => Reference.Contains ?? (Target as IOpenApiSchemaMissingProperties)?.Contains; set => Reference.Contains = value; }
        /// <inheritdoc/>
        public uint? MaxContains { get => Reference.MaxContains ?? (Target as IOpenApiSchemaMissingProperties)?.MaxContains; set => Reference.MaxContains = value; }
        /// <inheritdoc/>
        public uint? MinContains { get => Reference.MinContains ?? (Target as IOpenApiSchemaMissingProperties)?.MinContains; set => Reference.MinContains = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiSchema>? Properties { get => Reference.Properties ?? Target?.Properties; set => Reference.Properties = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiSchema>? PatternProperties { get => Reference.PatternProperties ?? Target?.PatternProperties; set => Reference.PatternProperties = value; }
        /// <inheritdoc/>
        public int? MaxProperties { get => Reference.MaxProperties ?? Target?.MaxProperties; set => Reference.MaxProperties = value; }
        /// <inheritdoc/>
        public int? MinProperties { get => Reference.MinProperties ?? Target?.MinProperties; set => Reference.MinProperties = value; }
        /// <inheritdoc/>
        public bool AdditionalPropertiesAllowed { get => Reference.AdditionalPropertiesAllowed ?? Target?.AdditionalPropertiesAllowed ?? true; set => Reference.AdditionalPropertiesAllowed = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? AdditionalProperties { get => Reference.AdditionalProperties ?? Target?.AdditionalProperties; set => Reference.AdditionalProperties = value; }
        /// <inheritdoc/>
        public OpenApiDiscriminator? Discriminator { get => Reference.Discriminator ?? Target?.Discriminator; set => Reference.Discriminator = value; }
        /// <inheritdoc/>
        [Obsolete("Use Examples instead.")]
        public JsonNode? Example { get => Reference.Example ?? Target?.Example; set => Reference.Example = value; }
        /// <inheritdoc/>
        public IList<JsonNode>? Examples
        {
            get => Reference.Examples ?? Target?.Examples;
            set => Reference.Examples = value;
        }
        /// <inheritdoc/>
        public IList<JsonNode>? Enum { get => Reference.Enum ?? Target?.Enum; set => Reference.Enum = value; }
        /// <inheritdoc/>
        public bool UnevaluatedProperties { get => Reference.UnevaluatedProperties ?? Target?.UnevaluatedProperties ?? true; set => Reference.UnevaluatedProperties = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? UnevaluatedPropertiesSchema { get => Reference.UnevaluatedPropertiesSchema ?? (Target as IOpenApiSchemaMissingProperties)?.UnevaluatedPropertiesSchema; set => Reference.UnevaluatedPropertiesSchema = value; }
        /// <inheritdoc/>
        public string? ContentEncoding { get => string.IsNullOrEmpty(Reference.ContentEncoding) ? (Target as IOpenApiSchemaMissingProperties)?.ContentEncoding : Reference.ContentEncoding; set => Reference.ContentEncoding = value; }
        /// <inheritdoc/>
        public string? ContentMediaType { get => string.IsNullOrEmpty(Reference.ContentMediaType) ? (Target as IOpenApiSchemaMissingProperties)?.ContentMediaType : Reference.ContentMediaType; set => Reference.ContentMediaType = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? ContentSchema { get => Reference.ContentSchema ?? (Target as IOpenApiSchemaMissingProperties)?.ContentSchema; set => Reference.ContentSchema = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? PropertyNames { get => Reference.PropertyNames ?? (Target as IOpenApiSchemaMissingProperties)?.PropertyNames; set => Reference.PropertyNames = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiSchema>? DependentSchemas { get => Reference.DependentSchemas ?? (Target as IOpenApiSchemaMissingProperties)?.DependentSchemas; set => Reference.DependentSchemas = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? If { get => Reference.If ?? (Target as IOpenApiSchemaMissingProperties)?.If; set => Reference.If = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Then { get => Reference.Then ?? (Target as IOpenApiSchemaMissingProperties)?.Then; set => Reference.Then = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Else { get => Reference.Else ?? (Target as IOpenApiSchemaMissingProperties)?.Else; set => Reference.Else = value; }
        /// <inheritdoc/>
        public OpenApiExternalDocs? ExternalDocs { get => Reference.ExternalDocs ?? Target?.ExternalDocs; set => Reference.ExternalDocs = value; }
        /// <inheritdoc/>
        public bool Deprecated
        {
            get => Reference.Deprecated ?? Target?.Deprecated ?? false;
            set => Reference.Deprecated = value;
        }
        /// <inheritdoc/>
        public OpenApiXml? Xml { get => Reference.Xml ?? Target?.Xml; set => Reference.Xml = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions
        {
            get => Reference.Extensions ?? Target?.Extensions;
            set => Reference.Extensions = value;
        }

        /// <inheritdoc/>
        public IDictionary<string, JsonNode>? UnrecognizedKeywords { get => Reference.UnrecognizedKeywords ?? Target?.UnrecognizedKeywords; set => Reference.UnrecognizedKeywords = value; }

        /// <inheritdoc/>
        public IDictionary<string, HashSet<string>>? DependentRequired { get => Reference.DependentRequired ?? Target?.DependentRequired; set => Reference.DependentRequired = value; }

        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeAsWithoutLoops(writer, (w, element) => (element is IOpenApiSchema s ? CopyReferenceAsTargetElementWithOverrides(s) : element).SerializeAsV31(w));
        }

        /// <inheritdoc/>
        public override void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeAsWithoutLoops(writer, (w, element) => (element is IOpenApiSchema s ? CopyReferenceAsTargetElementWithOverrides(s) : element).SerializeAsV32(w));
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
        /// <inheritdoc/>
        public IOpenApiSchema CreateDeepCopy()
        {
            return new OpenApiDeepCopyContext().Copy(this);
        }
        /// <inheritdoc/>
        protected override JsonSchemaReference CopyReference(JsonSchemaReference sourceReference)
        {
            return new JsonSchemaReference(sourceReference)!;
        }
#pragma warning restore CS0618
    }
}
