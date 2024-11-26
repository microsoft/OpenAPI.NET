// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Models.References
{
    /// <summary>
    /// Schema reference object
    /// </summary>
    public class OpenApiSchemaReference : OpenApiSchema
    {
        internal OpenApiSchema _target;
        private readonly OpenApiReference _reference;
        private string _description;
        private JsonNode _default;
        private JsonNode _example;
        private IList<JsonNode> _examples;

        private OpenApiSchema Target
        {
            get
            {
                _target ??= Reference.HostDocument?.ResolveReferenceTo<OpenApiSchema>(_reference);
                OpenApiSchema resolved = new OpenApiSchema(_target);
                if (!string.IsNullOrEmpty(_description)) resolved.Description = _description;
                return resolved;
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
        public OpenApiSchemaReference(string referenceId, OpenApiDocument hostDocument, string externalResource = null)
        {
            Utils.CheckArgumentNullOrEmpty(referenceId);

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                HostDocument = hostDocument,
                Type = ReferenceType.Schema,
                ExternalResource = externalResource
            };

            Reference = _reference;
        }

        internal OpenApiSchemaReference(OpenApiSchema target, string referenceId)
        {
            _target = target;

            _reference = new OpenApiReference()
            {
                Id = referenceId,
                Type = ReferenceType.Schema,
            };
        }

        /// <inheritdoc/>
        public override string Title { get => Target.Title; set => Target.Title = value; }
        /// <inheritdoc/>
        public override string Schema { get => Target.Schema; set => Target.Schema = value; }
        /// <inheritdoc/>
        public override string Id { get => Target.Id; set => Target.Id = value; }
        /// <inheritdoc/>
        public override string Comment { get => Target.Comment; set => Target.Comment = value; }
        /// <inheritdoc/>
        public override IDictionary<string, bool> Vocabulary { get => Target.Vocabulary; set => Target.Vocabulary = value; }
        /// <inheritdoc/>
        public override string DynamicRef { get => Target.DynamicRef; set => Target.DynamicRef = value; }
        /// <inheritdoc/>
        public override string DynamicAnchor { get => Target.DynamicAnchor; set => Target.DynamicAnchor = value; }
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiSchema> Definitions { get => Target.Definitions; set => Target.Definitions = value; }
        /// <inheritdoc/>
        public override decimal? V31ExclusiveMaximum { get => Target.V31ExclusiveMaximum; set => Target.V31ExclusiveMaximum = value; }
        /// <inheritdoc/>
        public override decimal? V31ExclusiveMinimum { get => Target.V31ExclusiveMinimum; set => Target.V31ExclusiveMinimum = value; }
        /// <inheritdoc/>
        public override bool UnEvaluatedProperties { get => Target.UnEvaluatedProperties; set => Target.UnEvaluatedProperties = value; }
        /// <inheritdoc/>
        public override JsonSchemaType? Type { get => Target.Type; set => Target.Type = value; }
        /// <inheritdoc/>
        public override string Const { get => Target.Const; set => Target.Const = value; }
        /// <inheritdoc/>
        public override string Format { get => Target.Format; set => Target.Format = value; }
        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target.Description : _description;
            set => _description = value;
        }
        /// <inheritdoc/>
        public override decimal? Maximum { get => Target.Maximum; set => Target.Maximum = value; }
        /// <inheritdoc/>
        public override bool? ExclusiveMaximum { get => Target.ExclusiveMaximum; set => Target.ExclusiveMaximum = value; }
        /// <inheritdoc/>
        public override decimal? Minimum { get => Target.Minimum; set => Target.Minimum = value; }
        /// <inheritdoc/>
        public override bool? ExclusiveMinimum { get => Target.ExclusiveMinimum; set => Target.ExclusiveMinimum = value; }
        /// <inheritdoc/>
        public override int? MaxLength { get => Target.MaxLength; set => Target.MaxLength = value; }
        /// <inheritdoc/>
        public override int? MinLength { get => Target.MinLength; set => Target.MinLength = value; }
        /// <inheritdoc/>
        public override string Pattern { get => Target.Pattern; set => Target.Pattern = value; }
        /// <inheritdoc/>
        public override decimal? MultipleOf { get => Target.MultipleOf; set => Target.MultipleOf = value; }
        /// <inheritdoc/>
        public override JsonNode Default 
        { 
            get => _default ??= Target.Default; 
            set => _default = value; 
        }
        /// <inheritdoc/>
        public override bool ReadOnly { get => Target.ReadOnly; set => Target.ReadOnly = value; }
        /// <inheritdoc/>
        public override bool WriteOnly { get => Target.WriteOnly; set => Target.WriteOnly = value; }
        /// <inheritdoc/>
        public override IList<OpenApiSchema> AllOf { get => Target.AllOf; set => Target.AllOf = value; }
        /// <inheritdoc/>
        public override IList<OpenApiSchema> OneOf { get => Target.OneOf; set => Target.OneOf = value; }
        /// <inheritdoc/>
        public override IList<OpenApiSchema> AnyOf { get => Target.AnyOf; set => Target.AnyOf = value; }
        /// <inheritdoc/>
        public override OpenApiSchema Not { get => Target.Not; set => Target.Not = value; }
        /// <inheritdoc/>
        public override ISet<string> Required { get => Target.Required; set => Target.Required = value; }
        /// <inheritdoc/>
        public override OpenApiSchema Items { get => Target.Items; set => Target.Items = value; }
        /// <inheritdoc/>
        public override int? MaxItems { get => Target.MaxItems; set => Target.MaxItems = value; }
        /// <inheritdoc/>
        public override int? MinItems { get => Target.MinItems; set => Target.MinItems = value; }
        /// <inheritdoc/>
        public override bool? UniqueItems { get => Target.UniqueItems; set => Target.UniqueItems = value; }
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiSchema> Properties { get => Target.Properties; set => Target.Properties = value; }
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiSchema> PatternProperties { get => Target.PatternProperties; set => Target.PatternProperties = value; }
        /// <inheritdoc/>
        public override int? MaxProperties { get => Target.MaxProperties; set => Target.MaxProperties = value; }
        /// <inheritdoc/>
        public override int? MinProperties { get => Target.MinProperties; set => Target.MinProperties = value; }
        /// <inheritdoc/>
        public override bool AdditionalPropertiesAllowed { get => Target.AdditionalPropertiesAllowed; set => Target.AdditionalPropertiesAllowed = value; }
        /// <inheritdoc/>
        public override OpenApiSchema AdditionalProperties { get => Target.AdditionalProperties; set => Target.AdditionalProperties = value; }
        /// <inheritdoc/>
        public override OpenApiDiscriminator Discriminator { get => Target.Discriminator; set => Target.Discriminator = value; }
        /// <inheritdoc/>
        public override JsonNode Example 
        { 
            get => _example ??= Target.Example; 
            set => _example = value; 
        }
        /// <inheritdoc/>
        public override IList<JsonNode> Examples 
        { 
            get => _examples ??= Target.Examples; 
            set => Target.Examples = value; 
        }
        /// <inheritdoc/>
        public override IList<JsonNode> Enum { get => Target.Enum; set => Target.Enum = value; }
        /// <inheritdoc/>
        public override bool Nullable { get => Target.Nullable; set => Target.Nullable = value; }
        /// <inheritdoc/>
        public override bool UnevaluatedProperties { get => Target.UnevaluatedProperties; set => Target.UnevaluatedProperties = value; }
        /// <inheritdoc/>
        public override OpenApiExternalDocs ExternalDocs { get => Target.ExternalDocs; set => Target.ExternalDocs = value; }
        /// <inheritdoc/>
        public override bool Deprecated { get => Target.Deprecated; set => Target.Deprecated = value; }
        /// <inheritdoc/>
        public override OpenApiXml Xml { get => Target.Xml; set => Target.Xml = value; }
        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => Target.Extensions; set => Target.Extensions = value; }

        /// <inheritdoc/>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(_reference))
            {
                _reference.SerializeAsV31(writer);
                return;
            }
            // If Loop is detected then just Serialize as a reference.
            else if (!writer.GetSettings().LoopDetector.PushLoop<OpenApiSchema>(this))
            {
                writer.GetSettings().LoopDetector.SaveLoop(this);
                _reference.SerializeAsV31(writer);
                return;
            }

            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer));
            writer.GetSettings().LoopDetector.PopLoop<OpenApiSchema>();
        }

        /// <inheritdoc/>
        public override void SerializeAsV3(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(_reference))
            {
                _reference.SerializeAsV3(writer);
                return;
            }
            // If Loop is detected then just Serialize as a reference.
            else if (!writer.GetSettings().LoopDetector.PushLoop<OpenApiSchema>(this))
            {
                writer.GetSettings().LoopDetector.SaveLoop(this);
                _reference.SerializeAsV3(writer);
                return;
            }
               
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer));
            writer.GetSettings().LoopDetector.PopLoop<OpenApiSchema>();
        }

        /// <inheritdoc/>
        public override void SerializeAsV2(IOpenApiWriter writer)
        {
            if (!writer.GetSettings().ShouldInlineReference(_reference))
            {
                _reference.SerializeAsV2(writer);
                return;
            }
            else
            {
                SerializeInternal(writer, (writer, element) => element.SerializeAsV2(writer));
            }
        }

        /// <inheritdoc/>
        private void SerializeInternal(IOpenApiWriter writer,
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            Utils.CheckArgumentNull(writer);
            action(writer, Target);
        }
    }
}
