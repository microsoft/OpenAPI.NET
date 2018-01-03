// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Class containing dispatchers to execute validation rules on for Open API document.
    /// </summary>
    public class OpenApiValidator : OpenApiVisitorBase
    {
        readonly ValidationRuleSet _ruleSet;
        readonly ValidationContext _context;

        /// <summary>
        /// Create a vistor that will validate an OpenAPIDocument
        /// </summary>
        /// <param name="ruleSet"></param>
        public OpenApiValidator(ValidationRuleSet ruleSet = null)
        {
            _ruleSet = ruleSet ?? ValidationRuleSet.DefaultRuleSet;
            _context = new ValidationContext(_ruleSet);
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
        /// Execute validation rules against an <see cref="OpenApiResponse"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiResponse item) => Validate(item);

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
        /// Errors accumulated while validating OpenAPI elements
        /// </summary>
        public IEnumerable<ValidationError> Errors => _context.Errors;

        private void Validate<T>(T item)
        {
            if (item == null) return;  // Required fields should be checked by higher level objects
            var rules = _ruleSet.Where(r => r.ElementType == typeof(T));
            foreach (var rule in rules)
            {
                rule.Evaluate(_context, item);
            }
        }
    }
}