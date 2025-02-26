using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests;

public class OpenApiTagComparerTests
{
    private readonly OpenApiTagComparer _comparer = OpenApiTagComparer.Instance;
    [Fact]
    public void Defensive()
    {
        Assert.NotNull(_comparer);

        Assert.True(_comparer.Equals(null, null));
        Assert.False(_comparer.Equals(null, new OpenApiTag()));
        Assert.Equal(0, _comparer.GetHashCode(null));
        Assert.Equal(0, _comparer.GetHashCode(new OpenApiTag()));
    }
    [Fact]
    public void SameNamesAreEqual()
    {
        var openApiTag1 = new OpenApiTag { Name = "tag" };
        var openApiTag2 = new OpenApiTag { Name = "tag" };
        Assert.True(_comparer.Equals(openApiTag1, openApiTag2));
    }
    [Fact]
    public void SameInstanceAreEqual()
    {
        var openApiTag = new OpenApiTag { Name = "tag" };
        Assert.True(_comparer.Equals(openApiTag, openApiTag));
    }

    [Fact]
    public void DifferentCasingAreNotEquals()
    {
        var openApiTag1 = new OpenApiTag { Name = "tag" };
        var openApiTag2 = new OpenApiTag { Name = "TAG" };
        Assert.False(_comparer.Equals(openApiTag1, openApiTag2));
    }
}
