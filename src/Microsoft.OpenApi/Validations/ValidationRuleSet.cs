// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The rule set of the validation.
    /// </summary>
    public sealed class ValidationRuleSet : IEnumerable<ValidationRule>
    {
        private IDictionary<Type, IList<ValidationRule>> _rules = new Dictionary<Type, IList<ValidationRule>>();

        /// <summary>
        /// The default rule set.
        /// </summary>
        public static ValidationRuleSet DefaultRuleSet = new ValidationRuleSet
        {
            OpenApiComponentsRules.KeyMustBeRegularExpression,

            OpenApiDocumentRules.FieldIsRequired,

            OpenApiInfoRules.FieldIsRequired,

            OpenApiLicenseRules.FieldIsRequired,

            OpenApiContactRules.EmailMustBeEmailFormat,

            OpenApiExternalDocsRules.FieldIsRequired,

            OpenApiServerRules.FieldIsRequired,

            OpenApiTagRules.FieldIsRequired,

            OpenApiResponseRules.FieldIsRequired,

            OpenApiOAuthFlowRules.FieldIsRequired,

            OpenApiExtensibleRules.ExtensionNameMustStartWithXDash,
            // add more default rules.
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRuleSet"/> class.
        /// </summary>
        public ValidationRuleSet()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRuleSet"/> class.
        /// </summary>
        /// <param name="rules">Rules to be contained in this ruleset.</param>
        public ValidationRuleSet(IEnumerable<ValidationRule> rules)
        {
            if (rules != null)
            {
                foreach (ValidationRule rule in rules)
                {
                    Add(rule);
                }
            }
        }

        /// <summary>
        /// Gets the rules in this rule set.
        /// </summary>
        public IEnumerable<ValidationRule> Rules
        {
            get
            {
                return _rules.Values.SelectMany(v => v);
            }
        }

        /// <summary>
        /// Add the new rule into rule set.
        /// </summary>
        /// <param name="rule">The rule.</param>
        public void Add(ValidationRule rule)
        {
            IList<ValidationRule> typeRules;
            if (!_rules.TryGetValue(rule.ElementType, out typeRules))
            {
                typeRules = new List<ValidationRule>();
                _rules[rule.ElementType] = typeRules;
            }

            if (typeRules.Contains(rule))
            {
                throw new OpenApiException(SRResource.Validation_RuleAddTwice);
            }

            typeRules.Add(rule);
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<ValidationRule> GetEnumerator()
        {
            foreach (List<ValidationRule> ruleList in _rules.Values)
            {
                foreach (var rule in ruleList)
                {
                    yield return rule;
                }
            }
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
