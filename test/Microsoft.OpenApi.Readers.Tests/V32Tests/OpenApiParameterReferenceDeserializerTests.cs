using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

public class OpenApiParameterReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeParameterReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/parameters/MyParameter",
            "description": "This is a parameter reference"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MyParameter", new OpenApiParameter
        {
            Name = "myParam",
            In = ParameterLocation.Query,
            Description = "This is a parameter description",
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV32Deserializer.LoadParameter(parseNode, hostDocument);

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiParameterReference>(result);

        Assert.Equal("MyParameter", resultReference.Reference.Id);
        Assert.Equal("This is a parameter reference", resultReference.Description);
        Assert.NotNull(resultReference.Target);
    }
}

