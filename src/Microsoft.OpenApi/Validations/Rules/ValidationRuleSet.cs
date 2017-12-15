// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ValidationRuleSet : IEnumerable<ValidationRule>
    {
        private IDictionary<Type, IList<ValidationRule>> _rules = new Dictionary<Type, IList<ValidationRule>>();

        private static ValidationRuleSet DefaultRuleSet;

        /// <summary>
        /// The default rule set.
        /// </summary>
        public static ValidationRuleSet CreateDefaultRuleSet()
        {
            if (DefaultRuleSet != null)
            {
                return DefaultRuleSet;
            }

            DefaultRuleSet = new ValidationRuleSet
            {
                OpenApiComponentsRules.KeyMustBeRegularExpression,

                OpenApiDocumentRules.FieldIsRequired,

                OpenApiInfoRules.FieldIsRequired,

                OpenApiContactRules.EmailMustBeEmailFormat,

                OpenApiTagRules.NameIsRequired,

                OpenApiExternalDocsRules.UrlIsRequired,

                OpenApiExtensibleRules.ExtensionNameMustStartWithXDash,

                OpenApiLicenseRules.FieldIsRequired,

                OpenApiServerRules.FieldIsRequired,

                OpenApiPathsRules.PathNameMustBeginWithSlash,
                // add more default rules.
            };

            return DefaultRuleSet;
        }

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
            if (!_rules.TryGetValue(rule.ValidatedType, out typeRules))
            {
                typeRules = new List<ValidationRule>();
                _rules[rule.ValidatedType] = typeRules;
            }

            if (typeRules.Contains(rule))
            {
                throw new OpenApiException("The same rule cannot be in the same rule set twice.");
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
