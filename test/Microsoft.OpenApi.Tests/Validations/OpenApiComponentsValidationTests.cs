// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiComponentsValidationTests
    {
        [Fact]
        public void ValidateKeyMustMatchRegularExpressionInComponents()
        {
            // Arrange
            const string key = "%@abc";

            var components = new OpenApiComponents
            {
                Responses = new Dictionary<string, IOpenApiResponse>
                {
                    { key, new OpenApiResponse { Description = "any" } }
                }
            };

            var errors = components.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Act
            var result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            var error = Assert.Single(errors);
            Assert.Equal(string.Format(SRResource.Validation_ComponentsKeyMustMatchRegularExpr, key, "responses", @"^[a-zA-Z0-9\.\-_]+$"),
                error.Message);
        }

        [Theory]
        [InlineData("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.-_", true)]
        [InlineData("", false)]
        [InlineData("a b", false)]
        [InlineData("a/b", false)]
        [InlineData("é", false)]
        [InlineData("１２", false)]
        [InlineData("a\n", true)]
        [InlineData("\n", false)]
        [InlineData("a\n\n", false)]
        [InlineData("a\r\n", false)]
        [InlineData("a\nb", false)]
        [InlineData("a\0", false)]
        public void ValidateComponentKeyPreservesRegexBehavior(string key, bool isValid)
        {
            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    { key, new OpenApiSchema() }
                }
            };

            var rules = new ValidationRuleSet();
            rules.Add(typeof(OpenApiComponents), OpenApiComponentsRules.KeyMustBeRegularExpression);
            var errors = components.Validate(rules);

            Assert.Equal(isValid, OpenApiComponentsRules.IsValidKey(key));
            Assert.Equal(isValid, !errors.Any());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task LoadAsyncValidatesLongComponentKeys(bool isValid)
        {
            var key = new string('a', 1_000_000) + (isValid ? string.Empty : "!");
            Assert.Equal(isValid, OpenApiComponentsRules.IsValidKey(key));
            var json = """
                {"openapi":"3.1.0","info":{"title":"Test","version":"1.0"},"paths":{},"components":{"schemas":{
                """ + JsonSerializer.Serialize(key) + ":{\"type\":\"string\"}}}}";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var result = await OpenApiDocument.LoadAsync(stream, cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(result.Document);
            Assert.NotNull(result.Diagnostic);
            if (isValid)
            {
                Assert.Empty(result.Diagnostic.Errors);
            }
            else
            {
                var error = Assert.Single(result.Diagnostic.Errors);
                Assert.Equal(string.Format(SRResource.Validation_ComponentsKeyMustMatchRegularExpr,
                    key, "schemas", @"^[a-zA-Z0-9\.\-_]+$"), error.Message);
            }
        }
    }
}
