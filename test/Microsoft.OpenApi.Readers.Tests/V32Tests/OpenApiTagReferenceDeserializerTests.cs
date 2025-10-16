using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

public class OpenApiTagReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeTagReferenceAnnotations()
    {
        var json =
        """
        {
            "tags" : [
                "MyTag"
            ]
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.Tags ??= new HashSet<OpenApiTag>();
        hostDocument.Tags.Add(new OpenApiTag
        {
            Name = "MyTag",
            Description = "This is a tag description",
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV32Deserializer.LoadOperation(parseNode, hostDocument);
        // this diverges from the other unit tests because Tag References are implemented
        // through the reference infrastructure for convenience, but the behave quite differently

        Assert.NotNull(result);
        Assert.NotNull(result.Tags);
        Assert.Single(result.Tags);
        var resultReference = Assert.IsType<OpenApiTagReference>(result.Tags.First());

        Assert.Equal("MyTag", resultReference.Reference.Id);
        Assert.Equal("This is a tag description", resultReference.Description);
        Assert.NotNull(resultReference.Target);
    }
}

