// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Validations.Rules;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class ValidationRuleSetTests
    {
        [Fact]
        public void DefaultRuleSetReturnsTheCorrectRules()
        {
            // Arrange
            var ruleSet = new ValidationRuleSet();

            // Act
            var rules = ruleSet.Rules;

            // Assert
            Assert.NotNull(rules);
            Assert.Empty(rules);
        }

        [Fact]
        public void DefaultRuleSetPropertyReturnsTheCorrectRules()
        {
            // Arrange & Act
            var ruleSet = ValidationRuleSet.GetDefaultRuleSet();
            Assert.NotNull(ruleSet); // guard

            var rules = ruleSet.Rules;

            // Assert
            Assert.NotNull(rules);
            Assert.NotEmpty(rules);

            // Update the number if you add new default rule(s).
            Assert.Equal(23, rules.Count);
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
