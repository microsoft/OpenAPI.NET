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
        private Dictionary<Type, IList<ValidationRule>> _rulesDictionary = new();

        private static ValidationRuleSet _defaultRuleSet;

        private List<ValidationRule> _emptyRules = new();


        /// <summary>
        /// Gets the rules in this rule set.
        /// </summary>
        public IList<ValidationRule> Rules => _rulesDictionary.Values.SelectMany(v => v).ToList();

        /// <summary>
        /// Gets the number of elements contained in this rule set.
        /// </summary>
        public int Count => _rulesDictionary.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRuleSet"/> class.
        /// </summary>
        public ValidationRuleSet()
        {
        }

        /// <summary>
        /// Retrieve the rules that are related to a specific type
        /// </summary>
        /// <param name="type">The type that is to be validated</param>
        /// <returns>Either the rules related to the type, or an empty list.</returns>
        public IList<ValidationRule> FindRules(Type type)
        {
            _rulesDictionary.TryGetValue(type, out var results);
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
        /// Add validation rules to the rule set.
        /// </summary>
        /// <param name="ruleSet">The rule set to add validation rules to.</param>
        /// <param name="rules">The validation rules to be added to the rules set.</param>
        /// <exception cref="OpenApiException">Throws a null argument exception if the arguments are null.</exception>
        public static void AddValidationRules(ValidationRuleSet ruleSet, IDictionary<Type, IList<ValidationRule>> rules)
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

            foreach (var rule in ruleSet)
            {
                Add(rule.ElementType, rule);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRuleSet"/> class.
        /// </summary>
        /// <param name="rules">Rules to be contained in this ruleset.</param>
        public ValidationRuleSet(IDictionary<Type, IList<ValidationRule>> rules)
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
        public void Add(Type key, IList<ValidationRule> rules)
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
        public void Add(Type key, ValidationRule rule)
        {
            if (!_rulesDictionary.ContainsKey(key))
            {
                _rulesDictionary[key] = new List<ValidationRule>();
            }

            if (_rulesDictionary[key].Contains(rule))
            {
                throw new OpenApiException(SRResource.Validation_RuleAddTwice);
            }

            _rulesDictionary[key].Add(rule);
        }

        /// <summary>
        /// Updates an existing rule with a new one.
        /// </summary>
        /// <param name="key">The key of the existing rule.</param>
        /// <param name="newRule">The new rule.</param>
        /// <param name="oldRule">The old rule.</param>
        /// <returns>true, if the update was successful; otherwise false.</returns>
        public bool Update(Type key, ValidationRule newRule, ValidationRule oldRule)
        {
            if (_rulesDictionary.TryGetValue(key, out var currentRules))
            {
                currentRules.Add(newRule);
                return currentRules.Remove(oldRule);
            }
            return false;
        }

        /// <summary>
        /// Removes a collection of rules.
        /// </summary>
        /// <param name="key">The key of the collection of rules to be removed.</param>
        /// <returns>true if the collection of rules with the provided key is removed; otherwise, false.</returns>
        public bool Remove(Type key)
        {
            return _rulesDictionary.Remove(key);
        }

        /// <summary>
        /// Remove a rule by its name from all types it is used by.
        /// </summary>        
        /// <param name="ruleName">Name of the rule.</param>
        public void Remove(string ruleName)
        {
            foreach (KeyValuePair<Type, IList<ValidationRule>> rule in _rulesDictionary)
            {
                _rulesDictionary[rule.Key] = rule.Value.Where(vr => !vr.Name.Equals(ruleName, StringComparison.Ordinal)).ToList();
            }

            // Remove types with no rule
            _rulesDictionary = _rulesDictionary.Where(r => r.Value.Any()).ToDictionary(r => r.Key, r => r.Value);
        }

        /// <summary>
        /// Removes a rule by key.
        /// </summary>
        /// <param name="key">The key of the rule to be removed.</param>
        /// <param name="rule">The rule to be removed.</param>
        /// <returns>true if the rule is successfully removed; otherwise, false.</returns>
        public bool Remove(Type key, ValidationRule rule)
        {
            if (_rulesDictionary.TryGetValue(key, out IList<ValidationRule> validationRules))
            {
                return validationRules.Remove(rule);
            }

            return false;
        }

        /// <summary>
        /// Removes the first rule  that matches the provided rule from the list of rules.
        /// </summary>
        /// <param name="rule">The rule to be removed.</param>
        /// <returns>true if the rule is successfully removed; otherwise, false.</returns>
        public bool Remove(ValidationRule rule)
        {
            return _rulesDictionary.Values.FirstOrDefault(x => x.Remove(rule)) is not null;
        }

        /// <summary>
        /// Clears all rules in this rule set.
        /// </summary>
        public void Clear()
        {
            _rulesDictionary.Clear();
        }

        /// <summary>
        /// Determines whether the rule set contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the rule set.</param>
        /// <returns>true if the rule set contains an element with the key; otherwise, false.</returns>
        public bool ContainsKey(Type key)
        {
            return _rulesDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the provided rule is contained in the specified key in the rule set.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="rule">The rule to locate.</param>
        /// <returns></returns>
        public bool Contains(Type key, ValidationRule rule)
        {
            return _rulesDictionary.TryGetValue(key, out IList<ValidationRule> validationRules) && validationRules.Contains(rule);
        }

        /// <summary>
        /// Gets the rules associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose rules to get.</param>
        /// <param name="rules">When this method returns, the rules associated with the specified key, if the
        ///  key is found; otherwise, an empty <see cref="IList{ValidationRule}"/> object.
        ///  This parameter is passed uninitialized.</param>
        /// <returns>true if the specified key has rules.</returns>
        public bool TryGetValue(Type key, out IList<ValidationRule> rules)
        {
            return _rulesDictionary.TryGetValue(key, out rules);
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<ValidationRule> GetEnumerator()
        {
            foreach (var ruleList in _rulesDictionary.Values)
            {
                foreach (var rule in ruleList)
                {
                    yield return rule;
                }
            }
        }

        private static ValidationRuleSet BuildDefaultRuleSet()
        {
            var ruleSet = new ValidationRuleSet();
            var validationRuleType = typeof(ValidationRule);
            
            var ruleTypeProperties = GetValidationRuleTypes();

            foreach (var property in ruleTypeProperties)
            {
                if (!validationRuleType.IsAssignableFrom(property.PropertyType))
                {
                    continue;
                }
                var propertyValue = property.GetValue(null); // static property
                if (propertyValue is ValidationRule rule)
                {
                    ruleSet.Add(rule.ElementType, rule);
                }
            }

            return ruleSet;
        }

        internal static PropertyInfo[] GetValidationRuleTypes()
        {
            return [
                ..typeof(OpenApiComponentsRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiContactRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiDocumentRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiExtensibleRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiExternalDocsRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiInfoRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiLicenseRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiOAuthFlowRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiServerRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiResponseRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiResponsesRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiSchemaRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiTagRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiPathsRules).GetProperties(BindingFlags.Static | BindingFlags.Public),
                ..typeof(OpenApiParameterRules).GetProperties(BindingFlags.Static | BindingFlags.Public)
                ];
        }
    }
}
