// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Validations.Rules;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// The rule set of the validation.
    /// </summary>
    public sealed class ValidationRuleSet : IEnumerable<ValidationRule>
    {
        private IDictionary<Type, IList<ValidationRule>> _rules = new Dictionary<Type, IList<ValidationRule>>();

        private static ValidationRuleSet _defaultRuleSet;

        /// <summary>
        /// Gets the default validation rule sets.
        /// </summary>
        public static ValidationRuleSet DefaultRuleSet
        {
            get
            {
                if (_defaultRuleSet == null)
                {
                    _defaultRuleSet = new Lazy<ValidationRuleSet>(() => BuildDefaultRuleSet(), isThreadSafe: true).Value;
                }

                return _defaultRuleSet;
            }
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

        private static ValidationRuleSet BuildDefaultRuleSet()
        {
            ValidationRuleSet ruleSet = new ValidationRuleSet();
            Type validationRuleType = typeof(ValidationRule);

            IEnumerable<PropertyInfo> rules = typeof(ValidationRuleSet).Assembly.GetTypes()
                .Where(t => t.IsClass 
                            && t != typeof(object)
                            && t.GetCustomAttributes(typeof(OpenApiRuleAttribute), false).Any())
                .SelectMany(t2 => t2.GetProperties(BindingFlags.Static | BindingFlags.Public)
                                .Where(p => validationRuleType.IsAssignableFrom(p.PropertyType)));

            foreach (var property in rules)
            {
                    var propertyValue = property.GetValue(null); // static property
                    ValidationRule rule = propertyValue as ValidationRule;
                    if (rule != null)
                    {
                        ruleSet.Add(rule);
                    }
            }

            return ruleSet;
        }
    }
}
