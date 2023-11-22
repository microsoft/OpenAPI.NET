using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiPathsValidationTests
    {
        [Fact]
        public void ValidatePathsMustBeginWithSlash()
        {
            // Arrange
            var error = string.Format(SRResource.Validation_PathItemMustBeginWithSlash, "pets/{petId}");
            var paths = new OpenApiPaths
            {
                {"pets/{petId}",new OpenApiPathItem() }
            };

            // Act
            var errors = paths.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            errors.Should().NotBeEmpty();
            errors.Select(e => e.Message).Should().BeEquivalentTo(error);
        }

        [Fact]
        public void ValidatePathsAreUnique()
        {
            // Arrange
            var error = string.Format(SRResource.Validation_PathSignatureMustBeUnique, "/pets/{}");
            var paths = new OpenApiPaths
            {
                {"/pets/{petId}",new OpenApiPathItem() },
                {"/pets/{name}",new OpenApiPathItem() }
            };

            // Act
            var errors = paths.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            errors.Should().NotBeEmpty();
            errors.Select(e => e.Message).Should().BeEquivalentTo(error);
        }
    }
}
