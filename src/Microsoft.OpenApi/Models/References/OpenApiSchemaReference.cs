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
        #nullable enable
        private OpenApiSchema? _target;
        private readonly OpenApiReference _reference;
        private string? _description;
        private JsonNode? _default;
        private JsonNode? _example;
        private IList<JsonNode>? _examples;
        private bool? _nullable;
        private IDictionary<string, OpenApiSchema>? _properties;
        private string? _title;
        private string? _schema;
        private string? _comment;
        private string? _id;
        private string? _dynamicRef;
        private string? _dynamicAnchor;
        private IDictionary<string, bool>? _vocabulary;
        private IDictionary<string, OpenApiSchema>? _definitions;
        private decimal? _v31ExclusiveMaximum;
        private decimal? _v31ExclusiveMinimum;
        private bool? _unEvaluatedProperties;
        private JsonSchemaType? _type;
        private string? _const;
        private string? _format;
        private decimal? _maximum;
        private bool? _exclusiveMaximum;
        private decimal? _minimum;
        private bool? _exclusiveMinimum;
        private int? _maxLength;
        private int? _minLength;
        private string? _pattern;
        private decimal? _multipleOf;
        private bool? _readOnly;
        private bool? _writeOnly;
        private IList<OpenApiSchema>? _allOf;
        private IList<OpenApiSchema>? _oneOf;
        private IList<OpenApiSchema>? _anyOf;
        private OpenApiSchema? _not;
        private ISet<string>? _required;
        private OpenApiSchema _items;
        private int? _maxItems;
        private int? _minItems;
        private bool? _uniqueItems;
        private IDictionary<string, OpenApiSchema>? _patternProperties;
        private int? _maxProperties;
        private int? _minProperties;
        private bool? _additionalPropertiesAllowed;
        private OpenApiSchema? _additionalProperties;
        private OpenApiDiscriminator? _discriminator;
        private OpenApiExternalDocs? _externalDocs;
        private bool? _deprecated;
        private OpenApiXml? _xml;
        private IDictionary<string, IOpenApiExtension>? _extensions;
        private bool? _unevaluatedProperties;
        private IList<JsonNode>? _enum;

        private OpenApiSchema? Target
        #nullable restore
        {
            get
            {
                _target ??= Reference.HostDocument?.ResolveReferenceTo<OpenApiSchema>(_reference);
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
        public override string Title { get => string.IsNullOrEmpty(_title) ? Target.Title : _title; set => _title = value; }
        /// <inheritdoc/>
        public override string Schema { get => string.IsNullOrEmpty(_schema) ? Target.Schema : _schema; set => _schema = value; }
        /// <inheritdoc/>
        public override string Id { get => string.IsNullOrEmpty(_id) ? Target.Id : _id; set => _id = value; }
        /// <inheritdoc/>
        public override string Comment { get => string.IsNullOrEmpty(_comment) ? Target.Comment : _comment; set => _comment = value; }
        /// <inheritdoc/>
        public override IDictionary<string, bool> Vocabulary { get => _vocabulary is not null ? _vocabulary : Target.Vocabulary; set => _vocabulary = value; }
        /// <inheritdoc/>
        public override string DynamicRef { get => string.IsNullOrEmpty(_dynamicRef) ? Target.DynamicRef : _dynamicRef; set => _dynamicRef = value; }
        /// <inheritdoc/>
        public override string DynamicAnchor { get => string.IsNullOrEmpty(_dynamicAnchor) ? Target.DynamicAnchor : _dynamicAnchor; set => _dynamicAnchor = value; }
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiSchema> Definitions { get => _definitions is not null ? _definitions : Target.Definitions; set => _definitions = value; }
        /// <inheritdoc/>
        public override decimal? V31ExclusiveMaximum { get => _v31ExclusiveMaximum is not null ? _v31ExclusiveMaximum.Value : Target.V31ExclusiveMaximum; set => _v31ExclusiveMaximum = value; }
        /// <inheritdoc/>
        public override decimal? V31ExclusiveMinimum { get => _v31ExclusiveMinimum is not null ? _v31ExclusiveMinimum.Value : Target.V31ExclusiveMinimum; set => _v31ExclusiveMinimum = value; }
        /// <inheritdoc/>
        public override bool UnEvaluatedProperties { get => _unEvaluatedProperties is not null ? _unEvaluatedProperties.Value : Target.UnEvaluatedProperties; set => _unEvaluatedProperties = value; }
        /// <inheritdoc/>
        public override JsonSchemaType? Type { get => _type is not null ? _type.Value : Target.Type; set => _type = value; }
        /// <inheritdoc/>
        public override string Const { get => string.IsNullOrEmpty(_const) ? Target.Const : _const; set => _const = value; }
        /// <inheritdoc/>
        public override string Format { get => string.IsNullOrEmpty(_format) ? Target.Format : _format; set => _format = value; }
        /// <inheritdoc/>
        public override string Description
        {
            get => string.IsNullOrEmpty(_description) ? Target.Description : _description;
            set => _description = value;
        }
        /// <inheritdoc/>
        public override decimal? Maximum { get => _maximum is not null ? _maximum : Target.Maximum; set => _maximum = value; }
        /// <inheritdoc/>
        public override bool? ExclusiveMaximum { get => _exclusiveMaximum is not null ? _exclusiveMaximum : Target.ExclusiveMaximum; set => _exclusiveMaximum = value; }
        /// <inheritdoc/>
        public override decimal? Minimum { get => _minimum is not null ? _minimum : Target.Minimum; set => _minimum = value; }
        /// <inheritdoc/>
        public override bool? ExclusiveMinimum { get => _exclusiveMinimum is not null ? _exclusiveMinimum : Target.ExclusiveMinimum; set => _exclusiveMinimum = value; }
        /// <inheritdoc/>
        public override int? MaxLength { get => _maxLength is not null ? _maxLength : Target.MaxLength; set => _maxLength = value; }
        /// <inheritdoc/>
        public override int? MinLength { get => _minLength is not null ? _minLength : Target.MinLength; set => _minLength = value; }
        /// <inheritdoc/>
        public override string Pattern { get => string.IsNullOrEmpty(_pattern) ? Target.Pattern : _pattern; set => _pattern = value; }
        /// <inheritdoc/>
        public override decimal? MultipleOf { get => _multipleOf is not null ? _multipleOf : Target.MultipleOf; set => _multipleOf = value; }
        /// <inheritdoc/>
        public override JsonNode Default 
        { 
            get => _default ??= Target.Default; //TODO normalize like other properties
            set => _default = value; 
        }
        /// <inheritdoc/>
        public override bool ReadOnly { get => _readOnly is not null ? _readOnly.Value : Target.ReadOnly; set => _readOnly = value; }
        /// <inheritdoc/>
        public override bool WriteOnly { get => _writeOnly is not null ? _writeOnly.Value : Target.WriteOnly; set => _writeOnly = value; }
        /// <inheritdoc/>
        public override IList<OpenApiSchema> AllOf { get => _allOf is not null ? _allOf : Target.AllOf; set => _allOf = value; }
        /// <inheritdoc/>
        public override IList<OpenApiSchema> OneOf { get => _oneOf is not null ? _oneOf : Target.OneOf; set => _oneOf = value; }
        /// <inheritdoc/>
        public override IList<OpenApiSchema> AnyOf { get => _anyOf is not null ? _anyOf : Target.AnyOf; set => _anyOf = value; }
        /// <inheritdoc/>
        public override OpenApiSchema Not { get => _not is not null ? _not : Target.Not; set => _not = value; }
        /// <inheritdoc/>
        public override ISet<string> Required { get => _required is not null ? _required : Target.Required; set => _required = value; }
        /// <inheritdoc/>
        public override OpenApiSchema Items { get => _items is not null ? _items : Target.Items; set => _items = value; }
        /// <inheritdoc/>
        public override int? MaxItems { get => _maxItems is not null ? _maxItems : Target.MaxItems; set => _maxItems = value; }
        /// <inheritdoc/>
        public override int? MinItems { get => _minItems is not null ? _minItems : Target.MinItems; set => _minItems = value; }
        /// <inheritdoc/>
        public override bool? UniqueItems { get => _uniqueItems is not null ? _uniqueItems : Target.UniqueItems; set => _uniqueItems = value; }
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiSchema> Properties { get => _properties is not null ? _properties : Target.Properties ; set => _properties = value; }
        /// <inheritdoc/>
        public override IDictionary<string, OpenApiSchema> PatternProperties { get => _patternProperties is not null ? _patternProperties : Target.PatternProperties; set => _patternProperties = value; }
        /// <inheritdoc/>
        public override int? MaxProperties { get => _maxProperties is not null ? _maxProperties : Target.MaxProperties; set => _maxProperties = value; }
        /// <inheritdoc/>
        public override int? MinProperties { get => _minProperties is not null ? _minProperties : Target.MinProperties; set => _minProperties = value; }
        /// <inheritdoc/>
        public override bool AdditionalPropertiesAllowed { get => _additionalPropertiesAllowed is not null ? _additionalPropertiesAllowed.Value : Target.AdditionalPropertiesAllowed; set => _additionalPropertiesAllowed = value; }
        /// <inheritdoc/>
        public override OpenApiSchema AdditionalProperties { get => _additionalProperties is not null ? _additionalProperties : Target.AdditionalProperties; set => _additionalProperties = value; }
        /// <inheritdoc/>
        public override OpenApiDiscriminator Discriminator { get => _discriminator is not null ? _discriminator : Target.Discriminator; set => _discriminator = value; }
        /// <inheritdoc/>
        public override JsonNode Example 
        { 
            get => _example ??= Target.Example; //TODO normalize like other properties
            set => _example = value; 
        }
        /// <inheritdoc/>
        public override IList<JsonNode> Examples 
        { 
            get => _examples ??= Target.Examples; //TODO normalize like other properties
            set => Target.Examples = value; 
        }
        /// <inheritdoc/>
        public override IList<JsonNode> Enum { get => _enum is not null ? _enum : Target.Enum; set => _enum = value; }
        /// <inheritdoc/>
        public override bool Nullable { get => _nullable is null ? Target.Nullable : _nullable.Value; set => _nullable = value; }
        /// <inheritdoc/>
        public override bool UnevaluatedProperties { get => _unevaluatedProperties is not null ? _unevaluatedProperties.Value : Target.UnevaluatedProperties; set => _unevaluatedProperties = value; }
        /// <inheritdoc/>
        public override OpenApiExternalDocs ExternalDocs { get => _externalDocs is not null ? _externalDocs : Target.ExternalDocs; set => _externalDocs = value; }
        /// <inheritdoc/>
        public override bool Deprecated { get => _deprecated is not null ? _deprecated.Value : Target.Deprecated; set => _deprecated = value; }
        /// <inheritdoc/>
        public override OpenApiXml Xml { get => _xml is not null ? _xml : Target.Xml; set => _xml = value; }
        /// <inheritdoc/>
        public override IDictionary<string, IOpenApiExtension> Extensions { get => _extensions is not null ? _extensions : Target.Extensions; set => _extensions = value; }

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
