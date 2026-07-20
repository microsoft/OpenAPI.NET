using System;
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
        var parseNode = jsonNode;

        var result = OpenApiV31Deserializer.LoadSecurityScheme(parseNode, hostDocument, new ParsingContext(new()));

        Assert.NotNull(result);
        var resultReference = Assert.IsType<OpenApiSecuritySchemeReference>(result);

        Assert.Equal("MyScheme", resultReference.Reference.Id);
        Assert.Equal("This is a security scheme reference", resultReference.Description);
        Assert.NotNull(resultReference.Target);
    }

    [Fact]
    public void ShouldDeserializeSecuritySchemeWithXOaiDeprecatedExtension()
    {
        var json =
        """
        {
            "type": "apiKey",
            "description": "This is a deprecated security scheme",
            "name": "api_key",
            "in": "header",
            "x-oai-deprecated": true
        }
        """;

        var hostDocument = new OpenApiDocument();
        var jsonNode = JsonNode.Parse(json);
        var parseNode = jsonNode;

        var result = OpenApiV31Deserializer.LoadSecurityScheme(parseNode, hostDocument, new ParsingContext(new()));

        Assert.NotNull(result);
        var resultScheme = Assert.IsType<OpenApiSecurityScheme>(result);

        Assert.Equal(SecuritySchemeType.ApiKey, resultScheme.Type);
        Assert.Equal("api_key", resultScheme.Name);
        Assert.Equal(ParameterLocation.Header, resultScheme.In);
        Assert.True(resultScheme.Deprecated);
    }

    [Fact]
    public void ShouldDeserializeSecuritySchemeWithOAuth2MetadataUrlExtension()
    {
        var json =
        """
        {
            "type": "oauth2",
            "x-oai-oauth2-metadata-url": "https://idp.example.com/.well-known/oauth-authorization-server",
            "flows": {
                "clientCredentials": {
                    "tokenUrl": "https://idp.example.com/oauth/token",
                    "scopes": {
                        "scope:one": "Scope one"
                    }
                }
            }
        }
        """;

        var result = OpenApiV31Deserializer.LoadSecurityScheme(JsonNode.Parse(json), new OpenApiDocument(), new ParsingContext(new()));

        var resultScheme = Assert.IsType<OpenApiSecurityScheme>(result);
        Assert.Equal(new Uri("https://idp.example.com/.well-known/oauth-authorization-server"), resultScheme.OAuth2MetadataUrl);
        Assert.True(resultScheme.Extensions is null || !resultScheme.Extensions.ContainsKey(OpenApiConstants.OAuth2MetadataUrlExtension));
    }
}
