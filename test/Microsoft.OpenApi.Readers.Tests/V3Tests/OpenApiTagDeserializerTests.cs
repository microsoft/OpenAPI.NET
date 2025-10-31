using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V3;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests;

public class OpenApiTagDeserializerTests
{
    [Fact]
    public void ShouldDeserializeTagWithNewPropertiesAsExtensions()
    {
        var json =
        """
        {
            "name": "store",
            "description": "Store operations",
            "x-oas-summary": "Operations related to the pet store",
            "x-oas-parent": "pet",
            "x-oas-kind": "operational",
            "externalDocs": {
                "description": "Find more info here",
                "url": "https://example.com/"
            },
            "x-custom-extension": "test-value"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.Tags ??= new HashSet<OpenApiTag>();
        hostDocument.Tags.Add(new OpenApiTag
        {
            Name = "pet",
            Description = "Parent tag for pets operations",
        });

        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV3Deserializer.LoadTag(parseNode, hostDocument);

        Assert.NotNull(result);
        Assert.Equal("store", result.Name);
        Assert.Equal("Store operations", result.Description);
        Assert.Equal("Operations related to the pet store", result.Summary);
        Assert.Equal("operational", result.Kind);
        Assert.NotNull(result.Parent);
        Assert.Equal("pet", result.Parent.Reference.Id);
        Assert.NotNull(result.ExternalDocs);
        Assert.Equal("Find more info here", result.ExternalDocs.Description);
        Assert.Equal("https://example.com/", result.ExternalDocs.Url?.ToString());
        Assert.NotNull(result.Extensions);
        Assert.Single(result.Extensions); // Only the custom extension should remain
        Assert.True(result.Extensions.ContainsKey("x-custom-extension"));
    }

    [Fact]
    public void ShouldIgnoreNativeV32PropertiesInV30()
    {
        var json =
        """
        {
            "name": "mixed",
            "description": "Mixed format tag",
            "summary": "Native summary should be ignored",
            "parent": "should-be-ignored",
            "kind": "should-be-ignored",
            "x-oas-summary": "Extension summary should be used",
            "x-oas-parent": "actual-parent",
            "x-oas-kind": "actual-kind"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.Tags ??= new HashSet<OpenApiTag>();
        hostDocument.Tags.Add(new OpenApiTag
        {
            Name = "actual-parent",
            Description = "Actual parent tag",
        });

        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV3Deserializer.LoadTag(parseNode, hostDocument);

        Assert.NotNull(result);
        Assert.Equal("mixed", result.Name);
        Assert.Equal("Mixed format tag", result.Description);
        Assert.Equal("Extension summary should be used", result.Summary);
        Assert.NotNull(result.Parent);
        Assert.Equal("actual-parent", result.Parent.Reference.Id);
        Assert.Equal("actual-kind", result.Kind);
    }

    [Fact]
    public void ShouldDeserializeBasicTagWithoutNewProperties()
    {
        var json =
        """
        {
            "name": "basic",
            "description": "Basic tag without new properties",
            "externalDocs": {
                "url": "https://example.com/"
            }
        }
        """;

        var hostDocument = new OpenApiDocument();
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV3Deserializer.LoadTag(parseNode, hostDocument);

        Assert.NotNull(result);
        Assert.Equal("basic", result.Name);
        Assert.Equal("Basic tag without new properties", result.Description);
        Assert.Null(result.Summary);
        Assert.Null(result.Parent);
        Assert.Null(result.Kind);
        Assert.NotNull(result.ExternalDocs);
        Assert.Equal("https://example.com/", result.ExternalDocs.Url?.ToString());
    }
}
