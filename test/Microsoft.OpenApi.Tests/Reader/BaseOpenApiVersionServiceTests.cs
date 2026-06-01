#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Tests.Reader;

public class BaseOpenApiVersionServiceTests
{
    [Fact]
    public void LoadElementReturnsLoadedElementWhenACompatibleLoaderExists()
    {
        var service = new TestVersionService(new Dictionary<Type, Func<JsonNode, OpenApiDocument, ParsingContext, object?>>
        {
            [typeof(OpenApiInfo)] = static (_, _, _) => new OpenApiInfo { Title = "Sample" }
        });

        var info = service.LoadElement<OpenApiInfo>(new JsonObject(), new OpenApiDocument(), new ParsingContext(new OpenApiDiagnostic()));

        Assert.NotNull(info);
        Assert.Equal("Sample", info.Title);
    }

    [Fact]
    public void LoadElementReturnsNullWhenLoaderReturnsADifferentType()
    {
        var service = new TestVersionService(new Dictionary<Type, Func<JsonNode, OpenApiDocument, ParsingContext, object?>>
        {
            [typeof(OpenApiInfo)] = static (_, _, _) => new OpenApiContact { Name = "Contoso" }
        });

        var info = service.LoadElement<OpenApiInfo>(new JsonObject(), new OpenApiDocument(), new ParsingContext(new OpenApiDiagnostic()));

        Assert.Null(info);
    }

    [Fact]
    public void LoadElementReturnsNullWhenNoLoaderIsRegistered()
    {
        var service = new TestVersionService([]);

        var info = service.LoadElement<OpenApiInfo>(new JsonObject(), new OpenApiDocument(), new ParsingContext(new OpenApiDiagnostic()));

        Assert.Null(info);
    }

    [Theory]
    [InlineData("""{"$ref":"#/components/schemas/Pet"}""", "description", null)]
    [InlineData("""{"$ref":"#/components/schemas/Pet","description":"A pet"}""", "description", "A pet")]
    [InlineData("""{"$ref":"#/components/schemas/Pet","summary":"ignored"}""", "description", null)]
    public void GetReferenceScalarValuesReturnsExpectedScalarValue(string json, string scalarValue, string? expectedValue)
    {
        var service = new TestVersionService([]);
        var jsonObject = JsonNode.Parse(json)!.AsObject();

        var value = service.GetReferenceScalarValues(jsonObject, scalarValue);

        Assert.Equal(expectedValue, value);
    }

    private sealed class TestVersionService(Dictionary<Type, Func<JsonNode, OpenApiDocument, ParsingContext, object?>> loaders)
        : BaseOpenApiVersionService(new OpenApiDiagnostic())
    {
        internal override Dictionary<Type, Func<JsonNode, OpenApiDocument, ParsingContext, object?>> Loaders { get; } = loaders;

        public override OpenApiDocument LoadDocument(JsonNode jsonNode, Uri location, ParsingContext context)
        {
            return new OpenApiDocument
            {
                BaseUri = location
            };
        }
    }
}
