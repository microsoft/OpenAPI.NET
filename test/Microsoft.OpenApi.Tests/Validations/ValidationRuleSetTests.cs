// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class ValidationRuleSetTests
    {
        private readonly ValidationRule  _contactValidationRule = new ValidationRule<OpenApiContact>(
                (context, item) => { });

        private readonly ValidationRule _headerValidationRule = new ValidationRule<OpenApiHeader>(
            (context, item) => { });

        private readonly ValidationRule _parameterValidationRule = new ValidationRule<OpenApiParameter>(
            (context, item) => { });

        private readonly IDictionary<string, IList<ValidationRule>> _rulesDictionary; 

        public ValidationRuleSetTests()
        {
            _rulesDictionary = new Dictionary<string, IList<ValidationRule>>()
            {
                {"contact", new List<ValidationRule> { _contactValidationRule } },
                {"header", new List<ValidationRule> { _headerValidationRule } },
                {"parameter", new List<ValidationRule> { _parameterValidationRule } }
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
            Assert.Equal(22, ruleSet_1.Rules.Count);
            Assert.Equal(22, ruleSet_2.Rules.Count);
            Assert.Equal(3, ruleSet_3.Rules.Count);
        }

        [Fact]
        public void RemoveValidatioRuleGivenTheValidationRuleWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);

            // Act
            ruleSet.Remove(_contactValidationRule);
            var rules = ruleSet.Rules;

            // Assert
            Assert.False(rules.Contains(_contactValidationRule));
        }

        [Fact]
        public void RemoveValidationRuleGivenTheKeyAndValidationRuleWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);

            // Act
            ruleSet.Remove("contact", _contactValidationRule);
            ruleSet.Remove("parameter", _headerValidationRule); // validation rule not in parameter key; shouldn't remove
            ruleSet.Remove("foo", _parameterValidationRule); // key does not exist; shouldn't remove

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
            var responseValidationRule = new ValidationRule<OpenApiResponse>((context, item) => { });
            ruleSet.Add("response", new List<ValidationRule> { responseValidationRule });
            Assert.True(ruleSet.ContainsKey("response"));
            Assert.True(ruleSet.Rules.Contains(responseValidationRule)); // guard

            // Act
            ruleSet.Remove("response");

            // Assert
            Assert.False(ruleSet.ContainsKey("response"));
        }

        [Fact]
        public void AddNewValidationRuleWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);
            var responseValidationRule = new ValidationRule<OpenApiResponse>((context, item) => { });
            var tagValidationRule = new ValidationRule<OpenApiTag>((context, item) => { });
            var pathsValidationRule = new ValidationRule<OpenApiPaths>((context, item) => { });

            // Act
            ruleSet.Add("response", new List<ValidationRule> { responseValidationRule });
            ruleSet.Add("tag", new List<ValidationRule> { tagValidationRule });
            var rulesDictionary = new Dictionary<string, IList<ValidationRule>>()
            {
                {"paths", new List<ValidationRule> { pathsValidationRule } }
            };

            ValidationRuleSet.AddValidationRules(ruleSet, rulesDictionary);
            
            // Assert
            Assert.True(ruleSet.ContainsKey("response"));
            Assert.True(ruleSet.ContainsKey("tag"));
            Assert.True(ruleSet.ContainsKey("paths"));
            Assert.True(ruleSet.Rules.Contains(responseValidationRule));
            Assert.True(ruleSet.Rules.Contains(tagValidationRule));
            Assert.True(ruleSet.Rules.Contains(pathsValidationRule));
        }

        [Fact]
        public void UpdateValidationRuleWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);
            var responseValidationRule = new ValidationRule<OpenApiResponse>((context, item) => { });
            ruleSet.Add("response", new List<ValidationRule> { responseValidationRule });

            // Act
            var pathsValidationRule = new ValidationRule<OpenApiPaths>((context, item) => { });
            ruleSet.Update("response", pathsValidationRule, responseValidationRule);

            // Assert
            Assert.True(ruleSet.Contains("response", pathsValidationRule));
            Assert.False(ruleSet.Contains("response", responseValidationRule));
        }

        [Fact]
        public void TryGetValueWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet(_rulesDictionary);

            // Act
            ruleSet.TryGetValue("contact", out var validationRules); 
            
            // Assert
            Assert.True(validationRules.Any());
            Assert.True(validationRules.Contains(_contactValidationRule));
        }

        [Fact]
        public void ClearAllRulesWorks()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet();
            var tagValidationRule = new ValidationRule<OpenApiTag>((context, item) => { });
            var pathsValidationRule = new ValidationRule<OpenApiPaths>((context, item) => { });
            var rulesDictionary = new Dictionary<string, IList<ValidationRule>>()
            {
                {"paths", new List<ValidationRule> { pathsValidationRule } },
                {"tag", new List<ValidationRule> { tagValidationRule } }
            };

            ValidationRuleSet.AddValidationRules(ruleSet, rulesDictionary);
            Assert.NotEmpty(ruleSet.Rules);

            // Act
            ruleSet.Clear();

            // Assert
            Assert.Empty(ruleSet.Rules);
        }
    }
}
