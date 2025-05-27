// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private readonly Dictionary<Type, List<ValidationRule>> _rulesDictionary;

        public ValidationRuleSetTests()
        {
            _rulesDictionary = new Dictionary<Type, List<ValidationRule>>()
            {
                {typeof(OpenApiContact), [_contactValidationRule] },
                {typeof(OpenApiHeader), [_headerValidationRule] },
                {typeof(OpenApiParameter), [_parameterValidationRule] }
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
            Assert.DoesNotContain(_contactValidationRule, ruleSet.Rules);
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
            Assert.DoesNotContain(_contactValidationRule, rules);
            Assert.Contains(_headerValidationRule, rules);
            Assert.Contains(_parameterValidationRule, rules);
        }

        [Fact]
        public void RemoveRulesGivenAKeyWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);
            var responseValidationRule = new ValidationRule<OpenApiResponse>("ValidateResponses", (context, item) => { });
            ruleSet.Add(typeof(OpenApiResponse), [responseValidationRule]);
            Assert.True(ruleSet.ContainsKey(typeof(OpenApiResponse)));
            Assert.Contains(responseValidationRule, ruleSet.Rules); // guard

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
            ruleSet.Add(typeof(OpenApiResponse), [responseValidationRule]);
            ruleSet.Add(typeof(OpenApiTag), [tagValidationRule]);
            var rulesDictionary = new Dictionary<Type, List<ValidationRule>>()
            {
                {typeof(OpenApiPaths), [pathsValidationRule] }
            };

            ValidationRuleSet.AddValidationRules(ruleSet, rulesDictionary);

            // Assert
            Assert.True(ruleSet.ContainsKey(typeof(OpenApiResponse)));
            Assert.True(ruleSet.ContainsKey(typeof(OpenApiTag)));
            Assert.True(ruleSet.ContainsKey(typeof(OpenApiPaths)));
            Assert.Contains(responseValidationRule, ruleSet.Rules);
            Assert.Contains(tagValidationRule, ruleSet.Rules);
            Assert.Contains(pathsValidationRule, ruleSet.Rules);
        }

        [Fact]
        public void UpdateValidationRuleWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);
            var responseValidationRule = new ValidationRule<OpenApiResponse>("ValidateResponses", (context, item) => { });
            ruleSet.Add(typeof(OpenApiResponse), [responseValidationRule]);

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
            Assert.Contains(_contactValidationRule, validationRules);
        }

        [Fact]
        public void ClearAllRulesWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet();
            var tagValidationRule = new ValidationRule<OpenApiTag>("ValidateTags", (context, item) => { });
            var pathsValidationRule = new ValidationRule<OpenApiPaths>("ValidatePaths", (context, item) => { });
            var rulesDictionary = new Dictionary<Type, List<ValidationRule>>()
            {
                {typeof(OpenApiPaths), [pathsValidationRule] },
                {typeof(OpenApiTag), [tagValidationRule] }
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
