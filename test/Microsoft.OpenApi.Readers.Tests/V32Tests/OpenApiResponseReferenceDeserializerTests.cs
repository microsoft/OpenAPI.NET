using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

public class OpenApiResponseReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeResponseReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/responses/MyResponse",
            "description": "This is a response reference"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MyResponse", new OpenApiResponse
        {
            Description = "This is a response description",
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = jsonNode;

        var result = OpenApiV32Deserializer.LoadResponse(parseNode, hostDocument, new ParsingContext(new()));

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiResponseReference>(result);

        Assert.Equal("MyResponse", resultReference.Reference.Id);
        Assert.Equal("This is a response reference", resultReference.Description);
        Assert.NotNull(resultReference.Target);
    }
}

