using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V31;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests;

public class OpenApiPathItemReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializePathItemReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/pathItems/MyPathItem",
            "description": "This is a path item reference",
            "summary": "PathItem Summary reference"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MyPathItem", new OpenApiPathItem
        {
            Summary = "This is a path item",
            Description = "This is a path item description",
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV31Deserializer.LoadPathItem(parseNode, hostDocument);

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiPathItemReference>(result);

        Assert.Equal("MyPathItem", resultReference.Reference.Id);
        Assert.Equal("This is a path item reference", resultReference.Description);
        Assert.Equal("PathItem Summary reference", resultReference.Summary);
        Assert.NotNull(resultReference.Target);
    }
}
