using System.Linq;
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
            Assert.NotEmpty(errors);
            Assert.Equivalent(new string[] {error}, errors.Select(e => e.Message).ToArray());
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
            Assert.NotEmpty(errors);
            Assert.Equivalent(new string[] {error}, errors.Select(e => e.Message).ToArray());
        }
        [Fact]
        public void ValidatePathsAreUniqueDoesNotConsiderMultiParametersAsIdentical()
        {
            // Arrange
            var paths = new OpenApiPaths
            {
                {"/drives/{drive-id}/items/{driveItem-id}/workbook/worksheets/{workbookWorksheet-id}/charts/{workbookChart-id}/image(width={width},height={height},fittingMode='{fittingMode}')",new OpenApiPathItem() },
                {"/drives/{drive-id}/items/{driveItem-id}/workbook/worksheets/{workbookWorksheet-id}/charts/{workbookChart-id}/image(width={width},height={height})",new OpenApiPathItem() },
                {"/drives/{drive-id}/items/{driveItem-id}/workbook/worksheets/{workbookWorksheet-id}/charts/{workbookChart-id}/image(width={width})", new OpenApiPathItem() },
            };

            // Act
            var errors = paths.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            Assert.Empty(errors);
        }
        [Fact]
        public void ValidatePathsAreUniqueConsidersMultiParametersAsIdentical()
        {
            // Arrange
            var paths = new OpenApiPaths
            {
                {"/drives/{drive-id}/items/{driveItem-id}/workbook/worksheets/{workbookWorksheet-id}/charts/{workbookChart-id}/image(width={width},height={height})",new OpenApiPathItem() },
                {"/drives/{drive-id}/items/{driveItem-id}/workbook/worksheets/{workbookWorksheet-id}/charts/{workbookChart-id}/image(width={width},height={size})",new OpenApiPathItem() },
            };

            // Act
            var errors = paths.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            Assert.NotEmpty(errors);
        }
    }
}
