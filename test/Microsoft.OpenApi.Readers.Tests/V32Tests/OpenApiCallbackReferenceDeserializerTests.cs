using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

public class OpenApiCallbackReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeCallbackReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/callbacks/MyCallback"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MyCallback", new OpenApiCallback
        {
            // Optionally add a PathItem or similar here if needed
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV32Deserializer.LoadCallback(parseNode, hostDocument);

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiCallbackReference>(result);

        Assert.Equal("MyCallback", resultReference.Reference.Id);
        Assert.NotNull(resultReference.Target);
    }
}

