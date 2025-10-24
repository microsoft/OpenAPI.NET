using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

public class OpenApiSchemaReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeSchemaReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/schemas/MySchema",
            "description": "This is a schema reference",
            "default": "foo",
            "readOnly": true,
            "writeOnly": true,
            "deprecated": true,
            "title": "This is a schema reference",
            "examples": ["example reference value"]
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MySchema", new OpenApiSchema
        {
            Title = "This is a schema",
            Description = "This is a schema description",
            Default = "bar",
            Type = JsonSchemaType.String,
            ReadOnly = false,
            WriteOnly = false,
            Deprecated = false,
            Examples = new List<JsonNode> { "example value" },
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV32Deserializer.LoadSchema(parseNode, hostDocument);

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiSchemaReference>(result);

        Assert.Equal("MySchema", resultReference.Reference.Id);
        Assert.Equal("This is a schema reference", resultReference.Description);
        Assert.Equal("foo", resultReference.Default?.ToString());
        Assert.True(resultReference.ReadOnly);
        Assert.True(resultReference.WriteOnly);
        Assert.True(resultReference.Deprecated);
        Assert.Equal("This is a schema reference", resultReference.Title);
        Assert.NotNull(resultReference.Examples);
        Assert.Single(resultReference.Examples);
        Assert.Equal("example reference value", resultReference.Examples[0]?.ToString());
        Assert.NotNull(resultReference.Target);
    }
}

