using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V31;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests;

public class OpenApiTagDeserializerTests
{
    [Fact]
    public void ShouldDeserializeTagWithNewV31Properties()
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

        var result = OpenApiV31Deserializer.LoadTag(parseNode, hostDocument);

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
        Assert.Single(result.Extensions);
        Assert.True(result.Extensions.ContainsKey("x-custom-extension"));
    }

    [Fact] 
    public void ShouldDeserializeTagWithBasicProperties()
    {
        var json =
        """
        {
            "name": "pets",
            "description": "Pet operations",
            "x-oas-summary": "All operations for managing pets"
        }
        """;

        var hostDocument = new OpenApiDocument();
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV31Deserializer.LoadTag(parseNode, hostDocument);

        Assert.NotNull(result);
        Assert.Equal("pets", result.Name);
        Assert.Equal("Pet operations", result.Description);
        Assert.Equal("All operations for managing pets", result.Summary);
        Assert.Null(result.Parent);
        Assert.Null(result.Kind);
    }
}
