using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V2;
using Microsoft.OpenApi.YamlReader;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests;

[Collection("DefaultSettings")]
public class OpenApiSecurityRequirementTests
{
    [Fact]
    public void LoadSecurityRequirementResolvesScopesForKnownSchemes()
    {
        var node = LoadYamlNode(
            """
            petstore_auth:
              - write:pets
              - read:pets
            """);
        var hostDocument = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                {
                    ["petstore_auth"] = new OpenApiSecurityScheme { Type = SecuritySchemeType.OAuth2 }
                }
            }
        };
        var context = new ParsingContext(new OpenApiDiagnostic());

        var requirement = OpenApiV2Deserializer.LoadSecurityRequirement(node, hostDocument, context);

        var resolvedScheme = Assert.Single(requirement);
        Assert.Equal("petstore_auth", resolvedScheme.Key.Reference.Id);
        Assert.Equal(["write:pets", "read:pets"], resolvedScheme.Value);
        Assert.Empty(context.Diagnostic.Errors);
    }

    [Fact]
    public void LoadSecurityRequirementCreatesUnresolvedReferenceWhenSchemeIsMissing()
    {
        var node = LoadYamlNode(
            """
            petstore_auth:
              - write:pets
            """);
        var hostDocument = new OpenApiDocument();
        var context = new ParsingContext(new OpenApiDiagnostic());

        var requirement = OpenApiV2Deserializer.LoadSecurityRequirement(node, hostDocument, context);

        var unresolvedScheme = Assert.Single(requirement);
        Assert.Equal("petstore_auth", unresolvedScheme.Key.Reference.Id);
        Assert.True(unresolvedScheme.Key.UnresolvedReference);
        Assert.Equal(["write:pets"], unresolvedScheme.Value);
        Assert.Empty(context.Diagnostic.Errors);
    }

    private static JsonNode LoadYamlNode(string yaml)
    {
        using var reader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(reader);
        return yamlStream.Documents[0].RootNode.ToJsonNode();
    }
}
