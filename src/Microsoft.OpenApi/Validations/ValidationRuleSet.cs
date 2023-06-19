// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Validations.Rules;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// The rule set of the validation.
    /// </summary>
    public sealed class ValidationRuleSet
    {
        private readonly IDictionary<string, IList<ValidationRule>> _rules = new Dictionary<string, IList<ValidationRule>>();

        private static ValidationRuleSet _defaultRuleSet;

        private readonly IList<ValidationRule> _emptyRules = new List<ValidationRule>();

        /// <summary>
        /// Gets the keys in this rule set.
        /// </summary>
        public ICollection<string> Keys => _rules.Keys;

        /// <summary>
        /// Gets the rules in this rule set.
        /// </summary>
        public IList<ValidationRule> Rules => _rules.Values.SelectMany(v => v).ToList();

        /// <summary>
        /// Gets the number of elements contained in this rule set.
        /// </summary>
        public int Count => _rules.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRuleSet"/> class.
        /// </summary>
        public ValidationRuleSet()
        {
        }

        /// <summary>
        /// Retrieve the rules that are related to a specific key.
        /// </summary>
        /// <param name="key">The key of the rules to search for.</param>
        /// <returns>Either the rules related to the given key, or an empty list.</returns>
        public IList<ValidationRule> FindRules(string key)
        {
            _rules.TryGetValue(key, out var results);
            return results ?? _emptyRules;
        }

        /// <summary>
        /// Gets the default validation rule sets.
        /// </summary>
        /// <remarks>
        /// This is a method instead of a property to signal that a new default ruleset object is created
        /// per call. Making this a property may be misleading callers to think the returned rulesets from multiple calls
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
            return new ValidationRuleSet(_defaultRuleSet);
        }

        /// <summary>
        /// Return Ruleset with no rules
        /// </summary>
        public static ValidationRuleSet GetEmptyRuleSet()
        {
            // We create a new instance of ValidationRuleSet per call as a safeguard
            // against unintentional modification of the private _defaultRuleSet.
            return new ValidationRuleSet();
        }

        /// <summary>
        /// Add validation rules to the rule set.
        /// </summary>
        /// <param name="ruleSet">The rule set to add validation rules to.</param>
        /// <param name="rules">The validation rules to be added to the rules set.</param>
        /// <exception cref="OpenApiException">Throws a null argument exception if the arguments are null.</exception>
        public static void AddValidationRules(ValidationRuleSet ruleSet, IDictionary<string, IList<ValidationRule>> rules)
        {
            if (ruleSet == null || rules == null)
            {
                throw new OpenApiException(SRResource.ArgumentNull);
            }

            foreach (var rule in rules)
            {
                ruleSet.Add(rule.Key, rule.Value);
            }
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

            foreach (ValidationRule rule in ruleSet)
            {
                Add(rule.ElementType.Name, rule);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRuleSet"/> class.
        /// </summary>
        /// <param name="rules">Rules to be contained in this ruleset.</param>
        public ValidationRuleSet(IDictionary<string, IList<ValidationRule>> rules)
        {
            if (rules == null)
            {
                return;
            }

            foreach (var rule in rules)
            {
                Add(rule.Key, rule.Value);
            }
        }

        /// <summary>
        /// Add the new rule into the rule set.
        /// </summary>
        /// <param name="key">The key for the rule.</param>
        /// <param name="rules">The list of rules.</param>
        public void Add(string key, IList<ValidationRule> rules)
        {
            foreach (var rule in rules)
            {
                Add(key, rule);
            }
        }

        /// <summary>
        /// Add a new rule into the rule set.
        /// </summary>
        /// <param name="key">The key for the rule.</param>
        /// <param name="rule">The rule.</param>
        /// <exception cref="OpenApiException">Exception thrown when rule already exists.</exception>
        public void Add(string key, ValidationRule rule)
        {
            if (!_rules.ContainsKey(key))
            {
                _rules[key] = new List<ValidationRule>();
            }

            if (_rules[key].Contains(rule))
            {
                throw new OpenApiException(SRResource.Validation_RuleAddTwice);
            }

            _rules[key].Add(rule);
        }

        /// <summary>
        /// Updates an existing rule with a new one.
        /// </summary>
        /// <param name="key">The key of the existing rule.</param>
        /// <param name="newRule">The new rule.</param>
        /// <param name="oldRule">The old rule.</param>
        /// <returns>true, if the update was successful; otherwise false.</returns>
        public bool Update(string key, ValidationRule newRule, ValidationRule oldRule)
        {
            if (!_rules.ContainsKey(key))
            {
                return false;
            }
            else
            {
                _rules[key].Remove(oldRule);
                _rules[key].Add(newRule);
                return true;
            }
        }

        /// <summary>
        /// Removes a collection of rules.
        /// </summary>
        /// <param name="key">The key of the collection of rules to be removed.</param>
        /// <returns>true if the collection of rules with the provided key is removed; otherwise, false.</returns>
        public bool Remove(string key)
        {
            return _rules.Remove(key);
        }

        /// <summary>
        /// Removes a rule by key.
        /// </summary>
        /// <param name="key">The key of the rule to be removed.</param>
        /// <param name="rule">The rule to be removed.</param>
        /// <returns>true if the rule is successfully removed; otherwise, false.</returns>
        public bool Remove(string key, ValidationRule rule)
        {
            if (_rules.TryGetValue(key, out IList<ValidationRule> validationRules))
            {
                return validationRules.Remove(rule);
            }

            return false;
        }

        /// <summary>
        /// Removes a rule from the list of rules.
        /// </summary>
        /// <param name="rule">The rule to be removed.</param>
        /// <returns>true if the rule is successfully removed; otherwise, false.</returns>
        public bool Remove(ValidationRule rule)
        {
            return _rules.Values.Remove(rule);
        }

        /// <summary>
        /// Clears all rules in this rule set.
        /// </summary>
        public void Clear()
        {
            _rules.Clear();
        }

        /// <summary>
        /// Determines whether the rule set contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the rule set.</param>
        /// <returns>true if the rule set contains an element with the key; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            return _rules.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the provided rule is contained in the specified key in the rule set.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="rule">The rule to locate.</param>
        /// <returns></returns>
        public bool Contains(string key, ValidationRule rule)
        {
            return _rules.TryGetValue(key, out IList<ValidationRule> validationRules) && validationRules.Contains(rule);
        }

        /// <summary>
        /// Gets the rules associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose rules to get.</param>
        /// <param name="rules">When this method returns, the rules associated with the specified key, if the
        ///  key is found; otherwise, an empty <see cref="IList{ValidationRule}"/> object.
        ///  This parameter is passed uninitialized.</param>
        /// <returns>true if the specified key has rules.</returns>
        public bool TryGetValue(string key, out IList<ValidationRule> rules)
        {
            return _rules.TryGetValue(key, out rules);
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
                    ruleSet.Add(rule.ElementType.Name, rule);
                }
            }

            return ruleSet;
        }
    }
}
