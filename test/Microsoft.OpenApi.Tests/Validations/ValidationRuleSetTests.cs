// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.OpenApi.Validations.Rules;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class ValidationRuleSetTests
    {
        private readonly ValidationRule _contactValidationRule = new ValidationRule<OpenApiContact>(nameof(_contactValidationRule),
                (context, item) => { });

        private readonly ValidationRule _headerValidationRule = new ValidationRule<OpenApiHeader>(nameof(_headerValidationRule),
            (context, item) => { });

        private readonly ValidationRule _parameterValidationRule = new ValidationRule<OpenApiParameter>(nameof(_parameterValidationRule),
            (context, item) => { });

        private readonly IDictionary<Type, IList<ValidationRule>> _rulesDictionary;

        public ValidationRuleSetTests()
        {
            _rulesDictionary = new Dictionary<Type, IList<ValidationRule>>()
            {
                {typeof(OpenApiContact), new List<ValidationRule> { _contactValidationRule } },
                {typeof(OpenApiHeader), new List<ValidationRule> { _headerValidationRule } },
                {typeof(OpenApiParameter), new List<ValidationRule> { _parameterValidationRule } }
            };
        }

        [Fact]
        public void RuleSetConstructorsReturnsTheCorrectRules()
        {
            // Arrange & Act
            var ruleSet_1 = ValidationRuleSet.GetDefaultRuleSet();
            var ruleSet_2 = new ValidationRuleSet(ValidationRuleSet.GetDefaultRuleSet());
            var ruleSet_3 = new ValidationRuleSet(_rulesDictionary);
            var ruleSet_4 = new ValidationRuleSet();

            // Assert
            Assert.NotNull(ruleSet_1?.Rules);
            Assert.NotNull(ruleSet_2?.Rules);
            Assert.NotNull(ruleSet_3?.Rules);
            Assert.NotNull(ruleSet_4);

            Assert.NotEmpty(ruleSet_1.Rules);
            Assert.NotEmpty(ruleSet_2.Rules);
            Assert.NotEmpty(ruleSet_3.Rules);
            Assert.Empty(ruleSet_4.Rules);

            // Update the number if you add new default rule(s).
            Assert.Equal(19, ruleSet_1.Rules.Count);
            Assert.Equal(19, ruleSet_2.Rules.Count);
            Assert.Equal(3, ruleSet_3.Rules.Count);
        }

        [Fact]
        public void RemoveValidatioRuleGivenTheValidationRuleWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);
            var responseValidationRule = new ValidationRule<OpenApiResponse>("ValidateResponses", (context, item) => { });

            // Act and Assert
            Assert.True(ruleSet.Remove(_contactValidationRule));
            Assert.False(ruleSet.Rules.Contains(_contactValidationRule));
            Assert.False(ruleSet.Remove(_contactValidationRule)); // rule already removed
        }

        [Fact]
        public void RemoveValidationRuleGivenTheKeyAndValidationRuleWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);

            // Act
            ruleSet.Remove(typeof(OpenApiContact), _contactValidationRule);
            ruleSet.Remove("parameter"); // validation rule not in parameter key; shouldn't remove
            ruleSet.Remove("foo"); // key does not exist; shouldn't remove

            var rules = ruleSet.Rules;

            // Assert
            Assert.False(rules.Contains(_contactValidationRule));
            Assert.True(rules.Contains(_headerValidationRule));
            Assert.True(rules.Contains(_parameterValidationRule));
        }

        [Fact]
        public void RemoveRulesGivenAKeyWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);
            var responseValidationRule = new ValidationRule<OpenApiResponse>("ValidateResponses", (context, item) => { });
            ruleSet.Add(typeof(OpenApiResponse), new List<ValidationRule> { responseValidationRule });
            Assert.True(ruleSet.ContainsKey(typeof(OpenApiResponse)));
            Assert.True(ruleSet.Rules.Contains(responseValidationRule)); // guard

            // Act
            ruleSet.Remove(typeof(OpenApiResponse));

            // Assert
            Assert.False(ruleSet.ContainsKey(typeof(OpenApiResponse)));
        }

        [Fact]
        public void AddNewValidationRuleWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);
            var responseValidationRule = new ValidationRule<OpenApiResponse>("ValidateResponses", (context, item) => { });
            var tagValidationRule = new ValidationRule<OpenApiTag>("ValidateTags", (context, item) => { });
            var pathsValidationRule = new ValidationRule<OpenApiPaths>("ValidatePaths", (context, item) => { });

            // Act
            ruleSet.Add(typeof(OpenApiResponse), new List<ValidationRule> { responseValidationRule });
            ruleSet.Add(typeof(OpenApiTag), new List<ValidationRule> { tagValidationRule });
            var rulesDictionary = new Dictionary<Type, IList<ValidationRule>>()
            {
                {typeof(OpenApiPaths), new List<ValidationRule> { pathsValidationRule } }
            };

            ValidationRuleSet.AddValidationRules(ruleSet, rulesDictionary);

            // Assert
            Assert.True(ruleSet.ContainsKey(typeof(OpenApiResponse)));
            Assert.True(ruleSet.ContainsKey(typeof(OpenApiTag)));
            Assert.True(ruleSet.ContainsKey(typeof(OpenApiPaths)));
            Assert.True(ruleSet.Rules.Contains(responseValidationRule));
            Assert.True(ruleSet.Rules.Contains(tagValidationRule));
            Assert.True(ruleSet.Rules.Contains(pathsValidationRule));
        }

        [Fact]
        public void UpdateValidationRuleWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);
            var responseValidationRule = new ValidationRule<OpenApiResponse>("ValidateResponses", (context, item) => { });
            ruleSet.Add(typeof(OpenApiResponse), new List<ValidationRule> { responseValidationRule });

            // Act
            var pathsValidationRule = new ValidationRule<OpenApiPaths>("ValidatePaths", (context, item) => { });
            ruleSet.Update(typeof(OpenApiResponse), pathsValidationRule, responseValidationRule);

            // Assert
            Assert.True(ruleSet.Contains(typeof(OpenApiResponse), pathsValidationRule));
            Assert.False(ruleSet.Contains(typeof(OpenApiResponse), responseValidationRule));
        }

        [Fact]
        public void TryGetValueWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);

            // Act
            ruleSet.TryGetValue(typeof(OpenApiContact), out var validationRules);

            // Assert
            Assert.True(validationRules.Any());
            Assert.True(validationRules.Contains(_contactValidationRule));
        }

        [Fact]
        public void ClearAllRulesWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet();
            var tagValidationRule = new ValidationRule<OpenApiTag>("ValidateTags", (context, item) => { });
            var pathsValidationRule = new ValidationRule<OpenApiPaths>("ValidatePaths", (context, item) => { });
            var rulesDictionary = new Dictionary<Type, IList<ValidationRule>>()
            {
                {typeof(OpenApiPaths), new List<ValidationRule> { pathsValidationRule } },
                {typeof(OpenApiTag), new List<ValidationRule> { tagValidationRule } }
            };

            ValidationRuleSet.AddValidationRules(ruleSet, rulesDictionary);
            Assert.NotEmpty(ruleSet.Rules);

            // Act
            ruleSet.Clear();

            // Assert
            Assert.Empty(ruleSet.Rules);
        }

        [Fact]
        public void GetValidationRuleTypesReturnsAllSupportedTypes()
        {
            var declaredRuleTypeProperties = typeof(ValidationRuleSet).Assembly.GetTypes()
                .Where(t => t.IsClass
                            && t != typeof(object)
                            && t.GetCustomAttributes(typeof(OpenApiRuleAttribute), false).Any())
                .SelectMany(t2 => t2.GetProperties(BindingFlags.Static | BindingFlags.Public))
                .ToList();
            
            var resolvedRuleTypeProperties = ValidationRuleSet.GetValidationRuleTypes();

            foreach (var ruleTypeProperty in resolvedRuleTypeProperties)
            {
                Assert.True(declaredRuleTypeProperties.Remove(ruleTypeProperty));
            }

            Assert.Empty(declaredRuleTypeProperties);
        }
    }
}
