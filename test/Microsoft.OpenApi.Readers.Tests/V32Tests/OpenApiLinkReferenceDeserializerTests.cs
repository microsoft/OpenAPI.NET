using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

public class OpenApiLinkReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeLinkReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/links/MyLink",
            "description": "This is a link reference"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MyLink", new OpenApiLink
        {
            Description = "This is a link description",
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV32Deserializer.LoadLink(parseNode, hostDocument);

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiLinkReference>(result);

        Assert.Equal("MyLink", resultReference.Reference.Id);
        Assert.Equal("This is a link reference", resultReference.Description);
        Assert.NotNull(resultReference.Target);
    }
}

