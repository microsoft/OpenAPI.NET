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
    public class OpenApiSchemaReference : BaseOpenApiReferenceHolder<OpenApiSchema, IOpenApiSchema, JsonSchemaReference>, IOpenApiSchema, IOpenApiSchemaMissingProperties, IOpenApiSchemaWithUnevaluatedProperties, IOpenApiExtensible
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
            get => string.IsNullOrEmpty(Reference.Description) ? GetFromTarget(static target => target.Description) : Reference.Description;
            set => Reference.Description = value;
        }

        /// <inheritdoc/>
        public string? Title
        {
            get => string.IsNullOrEmpty(Reference.Title) ? GetFromTarget(static target => target.Title) : Reference.Title;
            set => Reference.Title = value;
        }
        /// <inheritdoc/>
        public Uri? Schema { get => Reference.Schema ?? GetFromTarget(static target => target.Schema); set => Reference.Schema = value; }
        /// <inheritdoc/>
        public string? Id { get => string.IsNullOrEmpty(Reference.SchemaId) ? GetFromTarget(static target => target.Id) : Reference.SchemaId; set => Reference.SchemaId = value; }
        /// <inheritdoc/>
        public string? Comment { get => string.IsNullOrEmpty(Reference.Comment) ? GetFromTarget(static target => target.Comment) : Reference.Comment; set => Reference.Comment = value; }
        /// <inheritdoc/>
        public IDictionary<string, bool>? Vocabulary { get => Reference.Vocabulary ?? GetFromTarget(static target => target.Vocabulary); set => Reference.Vocabulary = value; }
        /// <inheritdoc/>
        public string? DynamicRef { get => string.IsNullOrEmpty(Reference.DynamicRef) ? GetFromTarget(static target => target.DynamicRef) : Reference.DynamicRef; set => Reference.DynamicRef = value; }
        /// <inheritdoc/>
        public string? DynamicAnchor { get => string.IsNullOrEmpty(Reference.DynamicAnchor) ? GetFromTarget(static target => target.DynamicAnchor) : Reference.DynamicAnchor; set => Reference.DynamicAnchor = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiSchema>? Definitions { get => Reference.Definitions ?? GetFromTarget(static target => target.Definitions); set => Reference.Definitions = value; }
        /// <inheritdoc/>
        public string? Anchor { get => string.IsNullOrEmpty(Reference.Anchor) ? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.Anchor) : Reference.Anchor; set => Reference.Anchor = value; }
        /// <inheritdoc/>
        public string? ExclusiveMaximum { get => string.IsNullOrEmpty(Reference.ExclusiveMaximum) ? GetFromTarget(static target => target.ExclusiveMaximum) : Reference.ExclusiveMaximum; set => Reference.ExclusiveMaximum = value; }
        /// <inheritdoc/>
        public string? ExclusiveMinimum { get => string.IsNullOrEmpty(Reference.ExclusiveMinimum) ? GetFromTarget(static target => target.ExclusiveMinimum) : Reference.ExclusiveMinimum; set => Reference.ExclusiveMinimum = value; }
        /// <inheritdoc/>
        public JsonSchemaType? Type { get => Reference.SchemaType ?? GetFromTarget(static target => target.Type); set => Reference.SchemaType = value; }
        /// <inheritdoc/>
        public string? Const { get => Reference.WasConstExplicitlySet ? Reference.Const : GetFromTarget(static target => target.Const); set => Reference.Const = value; }
        /// <inheritdoc/>
        public string? Format { get => string.IsNullOrEmpty(Reference.Format) ? GetFromTarget(static target => target.Format) : Reference.Format; set => Reference.Format = value; }
        /// <inheritdoc/>
        public string? Maximum { get => string.IsNullOrEmpty(Reference.Maximum) ? GetFromTarget(static target => target.Maximum) : Reference.Maximum; set => Reference.Maximum = value; }
        /// <inheritdoc/>
        public string? Minimum { get => string.IsNullOrEmpty(Reference.Minimum) ? GetFromTarget(static target => target.Minimum) : Reference.Minimum; set => Reference.Minimum = value; }
        /// <inheritdoc/>
        public int? MaxLength { get => Reference.MaxLength ?? GetFromTarget(static target => target.MaxLength); set => Reference.MaxLength = value; }
        /// <inheritdoc/>
        public int? MinLength { get => Reference.MinLength ?? GetFromTarget(static target => target.MinLength); set => Reference.MinLength = value; }
        /// <inheritdoc/>
        public string? Pattern { get => string.IsNullOrEmpty(Reference.Pattern) ? GetFromTarget(static target => target.Pattern) : Reference.Pattern; set => Reference.Pattern = value; }
        /// <inheritdoc/>
        public decimal? MultipleOf { get => Reference.MultipleOf ?? GetFromTarget(static target => target.MultipleOf); set => Reference.MultipleOf = value; }
        /// <inheritdoc/>
        public JsonNode? Default
        {
            get => Reference.Default ?? GetFromTarget(static target => target.Default);
            set => Reference.Default = value;
        }
        /// <inheritdoc/>
        public bool ReadOnly
        {
            get => Reference.ReadOnly ?? GetFromTarget(static target => target.ReadOnly);
            set => Reference.ReadOnly = value;
        }
        /// <inheritdoc/>
        public bool WriteOnly
        {
            get => Reference.WriteOnly ?? GetFromTarget(static target => target.WriteOnly);
            set => Reference.WriteOnly = value;
        }
        /// <inheritdoc/>
        public IList<IOpenApiSchema>? AllOf { get => Reference.AllOf ?? GetFromTarget(static target => target.AllOf); set => Reference.AllOf = value; }
        /// <inheritdoc/>
        public IList<IOpenApiSchema>? OneOf { get => Reference.OneOf ?? GetFromTarget(static target => target.OneOf); set => Reference.OneOf = value; }
        /// <inheritdoc/>
        public IList<IOpenApiSchema>? AnyOf { get => Reference.AnyOf ?? GetFromTarget(static target => target.AnyOf); set => Reference.AnyOf = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Not { get => Reference.Not ?? GetFromTarget(static target => target.Not); set => Reference.Not = value; }
        /// <inheritdoc/>
        public ISet<string>? Required { get => Reference.Required ?? GetFromTarget(static target => target.Required); set => Reference.Required = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Items { get => Reference.Items ?? GetFromTarget(static target => target.Items); set => Reference.Items = value; }
        /// <inheritdoc/>
        public int? MaxItems { get => Reference.MaxItems ?? GetFromTarget(static target => target.MaxItems); set => Reference.MaxItems = value; }
        /// <inheritdoc/>
        public int? MinItems { get => Reference.MinItems ?? GetFromTarget(static target => target.MinItems); set => Reference.MinItems = value; }
        /// <inheritdoc/>
        public bool? UniqueItems { get => Reference.UniqueItems ?? GetFromTarget(static target => target.UniqueItems); set => Reference.UniqueItems = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Contains { get => Reference.Contains ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.Contains); set => Reference.Contains = value; }
        /// <inheritdoc/>
        public uint? MaxContains { get => Reference.MaxContains ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.MaxContains); set => Reference.MaxContains = value; }
        /// <inheritdoc/>
        public uint? MinContains { get => Reference.MinContains ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.MinContains); set => Reference.MinContains = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiSchema>? Properties { get => Reference.Properties ?? GetFromTarget(static target => target.Properties); set => Reference.Properties = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiSchema>? PatternProperties { get => Reference.PatternProperties ?? GetFromTarget(static target => target.PatternProperties); set => Reference.PatternProperties = value; }
        /// <inheritdoc/>
        public int? MaxProperties { get => Reference.MaxProperties ?? GetFromTarget(static target => target.MaxProperties); set => Reference.MaxProperties = value; }
        /// <inheritdoc/>
        public int? MinProperties { get => Reference.MinProperties ?? GetFromTarget(static target => target.MinProperties); set => Reference.MinProperties = value; }
        /// <inheritdoc/>
        public bool AdditionalPropertiesAllowed { get => Reference.AdditionalPropertiesAllowed ?? GetFromTarget(static target => (bool?)target.AdditionalPropertiesAllowed) ?? true; set => Reference.AdditionalPropertiesAllowed = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? AdditionalProperties { get => Reference.AdditionalProperties ?? GetFromTarget(static target => target.AdditionalProperties); set => Reference.AdditionalProperties = value; }
        /// <inheritdoc/>
        public OpenApiDiscriminator? Discriminator { get => Reference.Discriminator ?? GetFromTarget(static target => target.Discriminator); set => Reference.Discriminator = value; }
        /// <inheritdoc/>
        [Obsolete("Use Examples instead.")]
        public JsonNode? Example { get => Reference.Example ?? GetFromTarget(static target => target.Example); set => Reference.Example = value; }
        /// <inheritdoc/>
        public IList<JsonNode>? Examples
        {
            get => Reference.Examples ?? GetFromTarget(static target => target.Examples);
            set => Reference.Examples = value;
        }
        /// <inheritdoc/>
        public IList<JsonNode>? Enum { get => Reference.Enum ?? GetFromTarget(static target => target.Enum); set => Reference.Enum = value; }
        /// <inheritdoc/>
        public bool UnevaluatedProperties { get => Reference.UnevaluatedProperties ?? GetFromTarget(static target => (bool?)target.UnevaluatedProperties) ?? true; set => Reference.UnevaluatedProperties = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? UnevaluatedPropertiesSchema { get => Reference.UnevaluatedPropertiesSchema ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.UnevaluatedPropertiesSchema); set => Reference.UnevaluatedPropertiesSchema = value; }
        /// <inheritdoc/>
        public string? ContentEncoding { get => string.IsNullOrEmpty(Reference.ContentEncoding) ? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.ContentEncoding) : Reference.ContentEncoding; set => Reference.ContentEncoding = value; }
        /// <inheritdoc/>
        public string? ContentMediaType { get => string.IsNullOrEmpty(Reference.ContentMediaType) ? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.ContentMediaType) : Reference.ContentMediaType; set => Reference.ContentMediaType = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? ContentSchema { get => Reference.ContentSchema ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.ContentSchema); set => Reference.ContentSchema = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? PropertyNames { get => Reference.PropertyNames ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.PropertyNames); set => Reference.PropertyNames = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiSchema>? DependentSchemas { get => Reference.DependentSchemas ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.DependentSchemas); set => Reference.DependentSchemas = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? If { get => Reference.If ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.If); set => Reference.If = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Then { get => Reference.Then ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.Then); set => Reference.Then = value; }
        /// <inheritdoc/>
        public IOpenApiSchema? Else { get => Reference.Else ?? GetFromTarget(static target => (target as IOpenApiSchemaMissingProperties)?.Else); set => Reference.Else = value; }
        /// <inheritdoc/>
        public OpenApiExternalDocs? ExternalDocs { get => Reference.ExternalDocs ?? GetFromTarget(static target => target.ExternalDocs); set => Reference.ExternalDocs = value; }
        /// <inheritdoc/>
        public bool Deprecated
        {
            get => Reference.Deprecated ?? GetFromTarget(static target => target.Deprecated);
            set => Reference.Deprecated = value;
        }
        /// <inheritdoc/>
        public OpenApiXml? Xml { get => Reference.Xml ?? GetFromTarget(static target => target.Xml); set => Reference.Xml = value; }
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions
        {
            get => Reference.Extensions ?? GetFromTarget(static target => target.Extensions);
            set => Reference.Extensions = value;
        }

        /// <inheritdoc/>
        public IDictionary<string, JsonNode>? UnrecognizedKeywords { get => Reference.UnrecognizedKeywords ?? GetFromTarget(static target => target.UnrecognizedKeywords); set => Reference.UnrecognizedKeywords = value; }

        /// <inheritdoc/>
        public IDictionary<string, HashSet<string>>? DependentRequired { get => Reference.DependentRequired ?? GetFromTarget(static target => target.DependentRequired); set => Reference.DependentRequired = value; }

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
        /// <inheritdoc/>
        protected override JsonSchemaReference CopyReference(JsonSchemaReference sourceReference)
        {
            return new JsonSchemaReference(sourceReference);
        }
#pragma warning restore CS0618
    }
}
