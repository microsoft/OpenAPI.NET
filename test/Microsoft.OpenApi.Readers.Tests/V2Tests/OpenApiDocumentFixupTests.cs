using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.YamlReader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests;

public class OpenApiDocumentFixupTests
{
    private static readonly Uri DocumentLocation = new("https://contoso.test/swagger.yaml");

    [Fact]
    public void ReadCreatesServersFromHostBasePathAndSchemes()
    {
        var result = ReadDocument(
            """
            swagger: "2.0"
            info:
              title: Sample API
              version: "1.0"
            host: api.contoso.com:443
            basePath: /v1/
            schemes:
              - https
            paths: {}
            """);

        Assert.NotNull(result.Document);
        Assert.Empty(result.Diagnostic.Errors);
        var server = Assert.Single(result.Document.Servers);
        Assert.Equal("https://api.contoso.com/v1", server.Url);
    }

    [Theory]
    [InlineData("https://api.contoso.com")]
    [InlineData("api contoso com")]
    public void ReadAddsDiagnosticErrorWhenHostIsInvalid(string host)
    {
        var result = ReadDocument(
            $$"""
            swagger: "2.0"
            info:
              title: Sample API
              version: "1.0"
            host: {{host}}
            paths: {}
            """);

        Assert.NotNull(result.Document);
        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Contains("Invalid host", error.Message, StringComparison.Ordinal);
        Assert.Empty(result.Document.Servers);
    }

    [Fact]
    public void ReadMovesReferencedBodyParametersToRequestBodies()
    {
        var result = ReadDocument(
            """
            swagger: "2.0"
            info:
              title: Sample API
              version: "1.0"
            paths:
              /pets:
                post:
                  parameters:
                    - $ref: '#/parameters/PetBody'
                  responses:
                    '200':
                      description: ok
            parameters:
              PetBody:
                name: pet
                in: body
                required: true
                schema:
                  type: object
                  properties:
                    name:
                      type: string
            """);

        Assert.NotNull(result.Document);
        Assert.Empty(result.Diagnostic.Errors);
        var requestBody = Assert.IsType<OpenApiRequestBodyReference>(result.Document.Paths["/pets"].Operations[HttpMethod.Post].RequestBody);
        Assert.Equal("PetBody", requestBody.Reference.Id);
        Assert.NotNull(result.Document.Components);
        var componentRequestBody = Assert.IsType<OpenApiRequestBody>(result.Document.Components.RequestBodies["PetBody"]);
        Assert.True(componentRequestBody.Required);
        Assert.Contains("application/json", componentRequestBody.Content.Keys);
        Assert.Empty(result.Document.Paths["/pets"].Operations[HttpMethod.Post].Parameters);
    }

    private static ReadResult ReadDocument(string yaml)
    {
        var settings = new OpenApiReaderSettings();
        settings.AddYamlReader();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

        return new OpenApiYamlReader().Read(stream, DocumentLocation, settings);
    }
}
