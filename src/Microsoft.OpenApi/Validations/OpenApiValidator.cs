// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Class containing dispatchers to execute validation rules on for Open API document.
    /// </summary>
    public class OpenApiValidator : OpenApiVisitorBase, IValidationContext
    {
        private readonly ValidationRuleSet _ruleSet;
        private readonly IList<OpenApiValidatorError> _errors = new List<OpenApiValidatorError>();
        private readonly IList<OpenApiValidatorWarning> _warnings = new List<OpenApiValidatorWarning>();

        /// <summary>
        /// Create a visitor that will validate an OpenAPIDocument
        /// </summary>
        /// <param name="ruleSet"></param>
        public OpenApiValidator(ValidationRuleSet ruleSet)
        {
            _ruleSet = ruleSet;
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        public IEnumerable<OpenApiValidatorError> Errors { get => _errors; }

        /// <summary>
        /// Gets the validation warnings.
        /// </summary>
        public IEnumerable<OpenApiValidatorWarning> Warnings { get => _warnings; }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="error">Error to register.</param>
        public void AddError(OpenApiValidatorError error)
        {
            Utils.CheckArgumentNull(error);

            _errors.Add(error);
        }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="warning">Error to register.</param>
        public void AddWarning(OpenApiValidatorWarning warning)
        {
            Utils.CheckArgumentNull(warning);

            _warnings.Add(warning);
        }

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiDocument"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiDocument item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiInfo"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiInfo item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiContact"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiContact item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiComponents"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiComponents item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiHeader"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiHeader item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiResponse"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiResponse item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiMediaType"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiMediaType item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiResponses"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiResponses item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiExternalDocs"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiExternalDocs item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiLicense"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiLicense item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiOAuthFlow"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiOAuthFlow item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiTag"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiTag item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiParameter"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiParameter item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiSchema"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiSchema item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiServer"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiServer item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiEncoding"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiEncoding item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiCallback"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiCallback item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="IOpenApiExtensible"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IOpenApiExtensible item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="IOpenApiExtension"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IOpenApiExtension item) => Validate(item, item.GetType());

        /// <summary>
        /// Execute validation rules against a list of <see cref="OpenApiExample"/>
        /// </summary>
        /// <param name="items">The object to be validated</param>
        public override void Visit(IList<OpenApiExample> items) => Validate(items, items.GetType());

        /// <summary>
        /// Execute validation rules against a <see cref="OpenApiPathItem"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiPathItem item) => Validate(item);

        /// <summary>
        /// Execute validation rules against a <see cref="OpenApiServerVariable"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiServerVariable item) => Validate(item);

        /// <summary>
        /// Execute validation rules against a <see cref="OpenApiSecurityScheme"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiSecurityScheme item) => Validate(item);

        /// <summary>
        /// Execute validation rules against a <see cref="OpenApiSecurityRequirement"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiSecurityRequirement item) => Validate(item);

        /// <summary>
        /// Execute validation rules against a <see cref="OpenApiRequestBody"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiRequestBody item) => Validate(item);

        /// <summary>
        /// Execute validation rules against a <see cref="OpenApiPaths"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiPaths item) => Validate(item);

        /// <summary>
        /// Execute validation rules against a <see cref="OpenApiLink"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiLink item) => Validate(item);

        /// <summary>
        /// Execute validation rules against a <see cref="OpenApiExample"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiExample item) => Validate(item);

        /// <summary>
        /// Execute validation rules against a <see cref="OpenApiOperation"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiOperation item) => Validate(item);
        /// <summary>
        /// Execute validation rules against a <see cref="IDictionary{OperationType, OpenApiOperation}"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IDictionary<OperationType, OpenApiOperation> item) => Validate(item, item.GetType());
        /// <summary>
        /// Execute validation rules against a <see cref="IDictionary{String, OpenApiHeader}"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IDictionary<string, OpenApiHeader> item) => Validate(item, item.GetType());
        /// <summary>
        /// Execute validation rules against a <see cref="IDictionary{String, OpenApiCallback}"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IDictionary<string, OpenApiCallback> item) => Validate(item, item.GetType());
        /// <summary>
        /// Execute validation rules against a <see cref="IDictionary{String, OpenApiMediaType}"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IDictionary<string, OpenApiMediaType> item) => Validate(item, item.GetType());
        /// <summary>
        /// Execute validation rules against a <see cref="IDictionary{String, OpenApiExample}"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IDictionary<string, OpenApiExample> item) => Validate(item, item.GetType());
        /// <summary>
        /// Execute validation rules against a <see cref="IDictionary{String, OpenApiLink}"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IDictionary<string, OpenApiLink> item) => Validate(item, item.GetType());
        /// <summary>
        /// Execute validation rules against a <see cref="IDictionary{String, OpenApiServerVariable}"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IDictionary<string, OpenApiServerVariable> item) => Validate(item, item.GetType());
        /// <summary>
        /// Execute validation rules against a <see cref="IDictionary{String, OpenApiEncoding}"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IDictionary<string, OpenApiEncoding> item) => Validate(item, item.GetType());

        private void Validate<T>(T item)
        {
            var type = typeof(T);

            Validate(item, type);
        }

        /// <summary>
        /// This overload allows applying rules based on actual object type, rather than matched interface.  This is
        /// needed for validating extensions.
        /// </summary>
        private void Validate(object item, Type type)
        {
            if (item == null)
            {
                return;  // Required fields should be checked by higher level objects
            }

            // Validate unresolved references as references
            if (item is IOpenApiReferenceable {UnresolvedReference: true})
            {
                type = typeof(IOpenApiReferenceable);
            }

            var rules = _ruleSet.FindRules(type);
            foreach (var rule in rules)
            {
                rule.Evaluate(this as IValidationContext, item);
            }
        }
    }
}
