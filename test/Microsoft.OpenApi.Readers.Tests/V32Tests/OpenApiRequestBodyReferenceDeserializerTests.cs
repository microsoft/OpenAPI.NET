using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

public class OpenApiRequestBodyReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeRequestBodyReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/requestBodies/MyRequestBody",
            "description": "This is a request body reference"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MyRequestBody", new OpenApiRequestBody
        {
            Description = "This is a request body description",
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV32Deserializer.LoadRequestBody(parseNode, hostDocument);

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiRequestBodyReference>(result);

        Assert.Equal("MyRequestBody", resultReference.Reference.Id);
        Assert.Equal("This is a request body reference", resultReference.Description);
        Assert.NotNull(resultReference.Target);
    }
}

