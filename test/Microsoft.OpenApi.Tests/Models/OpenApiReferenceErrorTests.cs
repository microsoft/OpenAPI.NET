using Xunit;

namespace Microsoft.OpenApi.Tests.Models;

public class OpenApiReferenceErrorTests
{
    [Fact]
    public void ConstructorCopiesMessageAndPointerFromException()
    {
        var exception = new OpenApiException("Reference could not be resolved")
        {
            Pointer = "#/components/schemas/Pet"
        };

        var error = new OpenApiReferenceError(exception);

        Assert.Equal(exception.Message, error.Message);
        Assert.Equal(exception.Pointer, error.Pointer);
        Assert.Null(error.Reference);
    }

    [Fact]
    public void ConstructorStoresTheReferenceThatFailedResolution()
    {
        var reference = new BaseOpenApiReference
        {
            Id = "Pet",
            Type = ReferenceType.Schema
        };

        var error = new OpenApiReferenceError(reference, "Missing component");

        Assert.Equal("Missing component", error.Message);
        Assert.Equal(string.Empty, error.Pointer);
        Assert.Same(reference, error.Reference);
    }
}
