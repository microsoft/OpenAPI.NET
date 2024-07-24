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
        private Dictionary<Type, IList<ValidationRule>> _rules = new();

        private static ValidationRuleSet _defaultRuleSet;

        private List<ValidationRule> _emptyRules = new();

        /// <summary>
        /// Retrieve the rules that are related to a specific type
        /// </summary>
        /// <param name="type">The type that is to be validated</param>
        /// <returns>Either the rules related to the type, or an empty list.</returns>
        public IList<ValidationRule> FindRules(Type type)
        {
            _rules.TryGetValue(type, out var results);
            return results ?? _emptyRules;
        }

        /// <summary>
        /// Gets the default validation rule sets.
        /// </summary>
        /// <remarks>
        /// This is a method instead of a property to signal that a new default rule-set object is created
        /// per call. Making this a property may be misleading callers to think the returned rule-sets from multiple calls
        /// are the same objects.
        /// </remarks>
        public static ValidationRuleSet GetDefaultRuleSet()
        {
            // Reflection can be an expensive operation, so we cache the default rule set that has already been built.
            if (_defaultRuleSet == null)
            {
                _defaultRuleSet = BuildDefaultRuleSet();
            }

            // We create a new instance of ValidationRuleSet per call as a safeguard
            // against unintentional modification of the private _defaultRuleSet.
            return new(_defaultRuleSet);
        }

        /// <summary>
        /// Return <see cref="ValidationRuleSet"/> with no rules
        /// </summary>
        public static ValidationRuleSet GetEmptyRuleSet()
        {
            // We create a new instance of ValidationRuleSet per call as a safeguard
            // against unintentional modification of the private _defaultRuleSet.
            return new();
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
        /// <param name="ruleSet">Rule set to be copied from.</param>
        public ValidationRuleSet(ValidationRuleSet ruleSet)
        {
            if (ruleSet == null)
            {
                return;
            }

            foreach (var rule in ruleSet)
            {
                Add(rule);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRuleSet"/> class.
        /// </summary>
        /// <param name="rules">Rules to be contained in this ruleset.</param>
        public ValidationRuleSet(IEnumerable<ValidationRule> rules)
        {
            if (rules == null)
            {
                return;
            }

            foreach (var rule in rules)
            {
                Add(rule);
            }
        }

        /// <summary>
        /// Gets the rules in this rule set.
        /// </summary>
        public IList<ValidationRule> Rules
        {
            get => _rules.Values.SelectMany(v => v).ToList();
        }

        /// <summary>
        /// Add the new rule into the rule set.
        /// </summary>
        /// <param name="rule">The rule.</param>
        public void Add(ValidationRule rule)
        {
            if (!_rules.TryGetValue(rule.ElementType, out var item))
            {
                _rules[rule.ElementType] = new List<ValidationRule> {rule};
                return;
            }

            if (item.Contains(rule))
            {
                throw new OpenApiException(SRResource.Validation_RuleAddTwice);
            }

            item.Add(rule);
        }

        /// <summary>
        /// Remove a rule by its name from all types it is used by.
        /// </summary>        
        /// <param name="ruleName">Name of the rule.</param>
        public void Remove(string ruleName)
        {
            foreach (KeyValuePair<Type, IList<ValidationRule>> rule in _rules)
            {
                _rules[rule.Key] = rule.Value.Where(vr => !vr.Name.Equals(ruleName, StringComparison.Ordinal)).ToList();                
            }

            // Remove types with no rule
            _rules = _rules.Where(r => r.Value.Any()).ToDictionary(r => r.Key, r => r.Value);
        }

        /// <summary>
        /// Remove a rule by element type.
        /// </summary>        
        /// <param name="type">Type of the rule.</param>
        public void Remove(Type type)
        {
            _rules.Remove(type);
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<ValidationRule> GetEnumerator()
        {
            foreach (var ruleList in _rules.Values)
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
            var ruleSet = new ValidationRuleSet();
            var validationRuleType = typeof(ValidationRule);

            var rules = typeof(ValidationRuleSet).Assembly.GetTypes()
                .Where(t => t.IsClass
                            && t != typeof(object)
                            && t.GetCustomAttributes(typeof(OpenApiRuleAttribute), false).Any())
                .SelectMany(t2 => t2.GetProperties(BindingFlags.Static | BindingFlags.Public)
                                .Where(p => validationRuleType.IsAssignableFrom(p.PropertyType)));

            foreach (var property in rules)
            {
                var propertyValue = property.GetValue(null); // static property
                if (propertyValue is ValidationRule rule)
                {
                    ruleSet.Add(rule);
                }
            }

            return ruleSet;
        }
    }
}
