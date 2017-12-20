// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Class containing logic to validate an Open API document object.
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

        public override void Visit(OpenApiDocument item) => Validate(item);
        public override void Visit(OpenApiInfo item) => Validate(item);
        public override void Visit(OpenApiContact item) => Validate(item);
        public override void Visit(OpenApiResponse item) => Validate(item);


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