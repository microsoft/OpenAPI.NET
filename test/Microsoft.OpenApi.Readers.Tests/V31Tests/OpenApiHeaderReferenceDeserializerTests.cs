
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V31;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests;

public class OpenApiHeaderReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/headers/MyHeader",
            "description": "This is a header reference"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MyHeader", new OpenApiHeader
        {
            Description = "This is a header"
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV31Deserializer.LoadHeader(parseNode, hostDocument);

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiHeaderReference>(result);

        Assert.Equal("MyHeader", resultReference.Reference.Id);
        Assert.Equal("This is a header reference", resultReference.Description);
        Assert.NotNull(resultReference.Target);
    }
}
