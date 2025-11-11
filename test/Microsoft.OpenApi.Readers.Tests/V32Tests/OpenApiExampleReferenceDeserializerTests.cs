using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

public class OpenApiExampleReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeExampleReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/examples/MyExample",
            "description": "This is an example reference",
            "summary": "Example Summary reference"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MyExample", new OpenApiExample
        {
            Summary = "This is an example",
            Description = "This is an example description",
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV32Deserializer.LoadExample(parseNode, hostDocument);

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiExampleReference>(result);

        Assert.Equal("MyExample", resultReference.Reference.Id);
        Assert.Equal("This is an example reference", resultReference.Description);
        Assert.Equal("Example Summary reference", resultReference.Summary);
        Assert.NotNull(resultReference.Target);
    }
}

