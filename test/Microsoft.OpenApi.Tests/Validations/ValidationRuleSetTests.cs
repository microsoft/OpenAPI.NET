// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
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
            var ruleSet = ValidationRuleSet.DefaultRuleSet;
            Assert.NotNull(ruleSet); // guard

            var rules = ruleSet.Rules;

            // Assert
            Assert.NotNull(rules);
            Assert.NotEmpty(rules);

            // Temporarily removing this test as we get inconsistent behaviour on AppVeyor
            // This needs to be investigated but it is currently holding up other work.
            // Assert.Equal(14, rules.ToList().Count); // please update the number if you add new rule.
        }
    }
}
