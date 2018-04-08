// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class ValidationRuleSetTests
    {
        private readonly ITestOutputHelper _output;

        public ValidationRuleSetTests(ITestOutputHelper output)
        {
            _output = output;
        }

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
            Assert.Equal(14, rules.Count);
        }
    }
}
