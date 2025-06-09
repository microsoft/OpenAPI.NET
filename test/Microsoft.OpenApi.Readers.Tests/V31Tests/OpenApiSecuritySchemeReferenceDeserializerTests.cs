using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V31;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests;

public class OpenApiSecuritySchemeReferenceDeserializerTests
{
    [Fact]
    public void ShouldDeserializeSecuritySchemeReferenceAnnotations()
    {
        var json =
        """
        {
            "$ref": "#/components/securitySchemes/MyScheme",
            "description": "This is a security scheme reference"
        }
        """;

        var hostDocument = new OpenApiDocument();
        hostDocument.AddComponent("MyScheme", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "api_key",
            In = ParameterLocation.Header,
            Description = "This is a security scheme description",
        });
        var jsonNode = JsonNode.Parse(json);
        var parseNode = ParseNode.Create(new ParsingContext(new()), jsonNode);

        var result = OpenApiV31Deserializer.LoadSecurityScheme(parseNode, hostDocument);

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiSecuritySchemeReference>(result);

        Assert.Equal("MyScheme", resultReference.Reference.Id);
        Assert.Equal("This is a security scheme reference", resultReference.Description);
        Assert.NotNull(resultReference.Target);
    }
}
