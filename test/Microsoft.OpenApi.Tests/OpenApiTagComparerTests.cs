using System.Collections.Generic;
using System.Linq;
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

    [Fact]
    public void WorksCorrectlyWithHashSetOfTags()
    {
        var tags = new HashSet<OpenApiTag>(_comparer)
        {
            new() { Name = "one" },
            new() { Name = "two" },
            new() { Name = "two" },
            new() { Name = "three" }
        };

        Assert.Equal(["one", "two", "three"], [.. tags.Select(t => t.Name)]);
    }

    [Fact]
    public void SameReferenceInstanceAreEqual()
    {
        var openApiTag = new OpenApiTagReference("tag");
        Assert.True(_comparer.Equals(openApiTag, openApiTag));
    }

    [Fact]
    public void SameReferenceIdsAreEqual()
    {
        var openApiTag1 = new OpenApiTagReference("tag");
        var openApiTag2 = new OpenApiTagReference("tag");
        Assert.True(_comparer.Equals(openApiTag1, openApiTag2));
    }

    [Fact]
    public void SameReferenceIdAreEqualWithValidTagReferences()
    {
        var document = new OpenApiDocument
        {
            Tags = [new() { Name = "tag" }]
        };

        var openApiTag1 = new OpenApiTagReference("tag", document);
        var openApiTag2 = new OpenApiTagReference("tag", document);
        Assert.True(_comparer.Equals(openApiTag1, openApiTag2));
    }

    [Fact]
    public void DifferentReferenceIdAreNotEqualWithValidTagReferences()
    {
        var document = new OpenApiDocument
        {
            Tags =
            [
                new() { Name = "one" },
                new() { Name = "two" },
            ]
        };

        var openApiTag1 = new OpenApiTagReference("one", document);
        var openApiTag2 = new OpenApiTagReference("two", document);
        Assert.False(_comparer.Equals(openApiTag1, openApiTag2));
    }

    [Fact]
    public void DifferentCasingReferenceIdsAreNotEqual()
    {
        var openApiTag1 = new OpenApiTagReference("tag");
        var openApiTag2 = new OpenApiTagReference("TAG");
        Assert.False(_comparer.Equals(openApiTag1, openApiTag2));
    }

    [Fact] // See https://github.com/microsoft/OpenAPI.NET/issues/2319
    public void WorksCorrectlyWithHashSetOfReferences()
    {
        // The document intentionally does not contain the actual tags
        var document = new OpenApiDocument();

        var tags = new HashSet<OpenApiTagReference>(_comparer)
        {
            new("one", document),
            new("two", document),
            new("two", document),
            new("three", document)
        };

        Assert.Equal(["one", "two", "three"], [..tags.Select(t => t.Reference.Id)]);
    }

    [Fact]
    public void WorksCorrectlyWithHashSetOfReferencesToValidTags()
    {
        var document = new OpenApiDocument
        {
            Tags =
            [
                new() { Name = "one" },
                new() { Name = "two" },
                new() { Name = "three" }
            ]
        };

        var tags = new HashSet<OpenApiTagReference>(_comparer)
        {
            new("one", document),
            new("two", document),
            new("two", document),
            new("three", document)
        };

        Assert.Equal(["one", "two", "three"], [.. tags.Select(t => t.Reference.Id)]);
    }
}
