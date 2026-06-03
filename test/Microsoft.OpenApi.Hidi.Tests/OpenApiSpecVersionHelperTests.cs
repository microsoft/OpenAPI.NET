#nullable enable
using System;
using Microsoft.OpenApi.Hidi;
using Xunit;

namespace Microsoft.OpenApi.Hidi.Tests;

public class OpenApiSpecVersionHelperTests
{
    [Theory]
    [InlineData("2.0", OpenApiSpecVersion.OpenApi2_0)]
    [InlineData("3.0", OpenApiSpecVersion.OpenApi3_0)]
    [InlineData("3.1", OpenApiSpecVersion.OpenApi3_1)]
    [InlineData("3.2", OpenApiSpecVersion.OpenApi3_2)]
    [InlineData("4.0", OpenApiSpecVersion.OpenApi3_2)]
    public void TryParseOpenApiSpecVersionReturnsExpectedVersion(string version, OpenApiSpecVersion expectedVersion)
    {
        var result = OpenApiSpecVersionHelper.TryParseOpenApiSpecVersion(version);

        Assert.Equal(expectedVersion, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    public void TryParseOpenApiSpecVersionThrowsForInvalidValues(string? version)
    {
        Assert.Throws<InvalidOperationException>(() => OpenApiSpecVersionHelper.TryParseOpenApiSpecVersion(version!));
    }
}
