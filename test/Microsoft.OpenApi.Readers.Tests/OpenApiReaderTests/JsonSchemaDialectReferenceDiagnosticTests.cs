// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectReferenceDiagnosticTests
{
    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public async Task ResolvingTheDialectRetainsInvalidReferenceDiagnosticsAndSiblingSchemas(string version, string format)
    {
        var document = format == OpenApiConstants.Json
            ? $$"""
              {
                "openapi": "{{version}}",
                "info": { "title": "test", "version": "1.0.0" },
                "jsonSchemaDialect": "https://example.com/dialect",
                "components": { "schemas": {
                  "Before": { "type": "string" },
                  "Item": { "$ref": { "custom": true }, "type": "object" },
                  "After": { "type": "integer" }
                } }
              }
              """
            : $$"""
              openapi: {{version}}
              info:
                title: test
                version: 1.0.0
              jsonSchemaDialect: https://example.com/dialect
              components:
                schemas:
                  Before:
                    type: string
                  Item:
                    $ref:
                      custom: true
                    type: object
                  After:
                    type: integer
              """;
        var loader = new DialectLoader();
        var settings = new OpenApiReaderSettings { LoadExternalRefs = true, CustomExternalLoader = loader };
        settings.AddYamlReader();
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(document));

        var result = await OpenApiDocument.LoadAsync(input, format, settings, TestContext.Current.CancellationToken);

        Assert.NotNull(result.Document);
        var schemas = result.Document.Components.Schemas;
        Assert.NotNull(schemas);
        Assert.Equal(3, schemas.Count);
        Assert.Equal(JsonSchemaType.String, schemas["Before"].Type);
        Assert.Equal(JsonSchemaType.Integer, schemas["After"].Type);
        var schema = Assert.IsType<OpenApiSchema>(schemas["Item"]);
        Assert.Equal(JsonSchemaType.Object, schema.Type);
        Assert.True(JsonNode.DeepEquals(JsonNode.Parse("""{ "custom": true }"""), schema.UnrecognizedKeywords["$ref"]));
        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Equal("#/components/schemas/Item/$ref", error.Pointer);
        Assert.DoesNotContain("not supported", error.Message);
        Assert.Equal(1, loader.CallCount);
    }

    private sealed class DialectLoader : IStreamLoader
    {
        internal int CallCount { get; private set; }

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Assert.Equal("https://example.com/dialect", uri.AbsoluteUri);
            CallCount++;
            return Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(
                """{ "$schema": "https://json-schema.org/draft/2020-12/schema" }""")));
        }
    }
}
