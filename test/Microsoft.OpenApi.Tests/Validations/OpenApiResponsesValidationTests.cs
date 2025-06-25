// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiResponsesValidationTests
    {
        [Theory]
        [InlineData("200")]
        [InlineData("404")]
        [InlineData("500")]
        [InlineData("1XX")]
        [InlineData("2XX")]
        [InlineData("3XX")]
        [InlineData("4XX")]
        [InlineData("5XX")]
        [InlineData("1xx")]
        [InlineData("2xx")]
        [InlineData("3xx")]
        [InlineData("4xx")]
        [InlineData("5xx")]
        [InlineData("default")]
        public void ValidateResponseKeyIsValid(string responseKey)
        {
            // Arrange
            var responses = new OpenApiResponses
            {
                [responseKey] = new OpenApiResponse { Description = "Test response" }
            };

            // Act
            var errors = responses.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("600")]
        [InlineData("6XX")]
        [InlineData("6xx")]
        [InlineData("1XXX")]
        [InlineData("XX")]
        [InlineData("x")]
        [InlineData("")]
        public void ValidateResponseKeyIsInvalid(string responseKey)
        {
            // Arrange
            var responses = new OpenApiResponses
            {
                [responseKey] = new OpenApiResponse { Description = "Test response" }
            };

            // Act
            var errors = responses.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            Assert.NotEmpty(errors);
            var error = errors.First();
            Assert.Contains("Responses key must be 'default', an HTTP status code", error.Message);
            Assert.Contains("case insensitive", error.Message);
        }

        [Fact]
        public void ValidateResponsesMustContainAtLeastOneResponse()
        {
            // Arrange
            var responses = new OpenApiResponses();

            // Act
            var errors = responses.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            Assert.NotEmpty(errors);
            var error = errors.First();
            Assert.Contains("Responses must contain at least one response", error.Message);
        }

        [Fact]
        public void ValidateMixedCaseResponseKeysAreAllowed()
        {
            // Arrange - Test the specific issue case mentioned in the bug report
            var responses = new OpenApiResponses
            {
                ["4xx"] = new OpenApiResponse { Description = "Client error" },
                ["5XX"] = new OpenApiResponse { Description = "Server error" },
                ["200"] = new OpenApiResponse { Description = "Success" },
                ["default"] = new OpenApiResponse { Description = "Default response" }
            };

            // Act
            var errors = responses.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            Assert.Empty(errors);
        }
    }
}