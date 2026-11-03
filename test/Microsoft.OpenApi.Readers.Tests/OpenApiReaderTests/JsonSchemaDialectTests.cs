// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectTests
{
    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task UnknownDialectIsResolvedThroughItsMetaSchema(string version)
    {
        const string customDialect = "https://example.com/dialect";
        var loader = new DialectLoader(new Dictionary<string, string>
        {
            [customDialect] = """
                {
                  "$schema": "https://json-schema.org/draft/2020-12/schema"
                }
                """,
        });
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var input = CreateDocument(version, customDialect);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Empty(result.Diagnostic.Errors);
        Assert.Equal(1, loader.GetCallCount(customDialect));
        var schema = result.Document.Components.Schemas["First"];
        Assert.True(schema.UnrecognizedKeywords.ContainsKey("customKeyword"));

        var outputVersion = version.StartsWith("3.2", StringComparison.Ordinal)
            ? OpenApiSpecVersion.OpenApi3_2
            : OpenApiSpecVersion.OpenApi3_1;
        var serialized = JsonNode.Parse(await result.Document.SerializeAsJsonAsync(
            outputVersion,
            TestContext.Current.CancellationToken));
        Assert.Equal(customDialect, serialized[OpenApiConstants.JsonSchemaDialect].GetValue<string>());
        Assert.True(serialized["components"]["schemas"]["First"][OpenApiConstants.UnrecognizedKeywords]["customKeyword"].GetValue<bool>());
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task UnknownDialectIsResolvedForYamlDocuments(string version)
    {
        const string customDialect = "https://example.com/dialect";
        var loader = new DialectLoader(new Dictionary<string, string>
        {
            [customDialect] = """{ "$schema": "https://json-schema.org/draft/2020-12/schema" }""",
        });
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        settings.AddYamlReader();
        var document = $$"""
            openapi: {{version}}
            info:
              title: test
              version: 1.0.0
            jsonSchemaDialect: {{customDialect}}
            components:
              schemas:
                Item:
                  type: object
            """;
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(document));

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Yaml,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Empty(result.Diagnostic.Errors);
        Assert.Equal(OpenApiConstants.Yaml, result.Diagnostic.Format);
        Assert.Equal(1, loader.GetCallCount(customDialect));
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task SharedDialectChainsAreLoadedOnce(string version)
    {
        const string documentDialect = "https://example.com/document-dialect";
        const string schemaDialect = "https://example.com/schema-dialect";
        var loader = new DialectLoader(new Dictionary<string, string>
        {
            [documentDialect] = $$"""{ "$schema": "{{schemaDialect}}" }""",
            [schemaDialect] = """{ "$schema": "https://json-schema.org/draft/2020-12/schema" }""",
        });
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var input = CreateDocument(version, documentDialect, schemaDialect);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Empty(result.Diagnostic.Errors);
        Assert.Equal(1, loader.GetCallCount(documentDialect));
        Assert.Equal(1, loader.GetCallCount(schemaDialect));

        var outputVersion = version.StartsWith("3.2", StringComparison.Ordinal)
            ? OpenApiSpecVersion.OpenApi3_2
            : OpenApiSpecVersion.OpenApi3_1;
        var serialized = JsonNode.Parse(await result.Document.SerializeAsJsonAsync(
            outputVersion,
            TestContext.Current.CancellationToken));
        Assert.Equal(schemaDialect, serialized["components"]["schemas"]["Second"][OpenApiConstants.DollarSchema].GetValue<string>());
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task CyclicDialectMetaSchemasRemainUnsupported(string version)
    {
        const string firstDialect = "https://example.com/dialect-a";
        const string secondDialect = "https://example.com/dialect-b";
        var loader = new DialectLoader(new Dictionary<string, string>
        {
            [firstDialect] = $$"""{ "$schema": "{{secondDialect}}" }""",
            [secondDialect] = $$"""{ "$schema": "{{firstDialect}}" }""",
        });
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var input = CreateDocument(version, firstDialect);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Equal($"The JSON Schema dialect '{firstDialect}' is not supported.", error.Message);
        Assert.Equal(1, loader.GetCallCount(firstDialect));
        Assert.Equal(1, loader.GetCallCount(secondDialect));
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task OversizedDialectMetaSchemaRemainsUnsupported(string version)
    {
        const string customDialect = "https://example.com/dialect";
        var loader = new DialectLoader(new Dictionary<string, string>
        {
            [customDialect] = new string(' ', JsonSchemaDialectResolver.MaxMetaSchemaSize + 1),
        });
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var input = CreateDocument(version, customDialect);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Equal($"The JSON Schema dialect '{customDialect}' is not supported.", error.Message);
        Assert.Equal(1, loader.GetCallCount(customDialect));
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task DialectResolutionStopsAtTheMaximumDepth(string version)
    {
        var documents = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 0; index <= JsonSchemaDialectResolver.MaxResolutionDepth; index++)
        {
            var current = GetDialectUri(index);
            var parent = index == JsonSchemaDialectResolver.MaxResolutionDepth
                ? JsonSchemaDialectService.JsonSchemaDraft202012.AbsoluteUri
                : GetDialectUri(index + 1);
            documents[current] = $$"""{ "$schema": "{{parent}}" }""";
        }

        var loader = new DialectLoader(documents);
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var input = CreateDocument(version, GetDialectUri(0));

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Single(result.Diagnostic.Errors);
        Assert.Equal(JsonSchemaDialectResolver.MaxResolutionDepth, loader.TotalCallCount);
        Assert.Equal(0, loader.GetCallCount(GetDialectUri(JsonSchemaDialectResolver.MaxResolutionDepth)));
    }

    private static MemoryStream CreateDocument(string version, string dialect, string secondSchemaDialect = null)
    {
        var schemaDialect = secondSchemaDialect is null ? string.Empty : $"\"$schema\": \"{secondSchemaDialect}\",";
        var document = $$"""
            {
              "openapi": "{{version}}",
              "info": {
                "title": "test",
                "version": "1.0.0"
              },
              "jsonSchemaDialect": "{{dialect}}",
              "components": {
                "schemas": {
                  "First": {
                    "customKeyword": true,
                    "type": "object"
                  },
                  "Second": {
                    {{schemaDialect}}
                    "type": "string"
                  }
                }
              }
            }
            """;
        return new MemoryStream(Encoding.UTF8.GetBytes(document));
    }

    private static string GetDialectUri(int index) => $"https://example.com/dialect-{index}";

    private sealed class DialectLoader : IStreamLoader
    {
        private readonly IReadOnlyDictionary<string, string> _documents;
        private readonly Dictionary<string, int> _callCounts = new(StringComparer.Ordinal);

        internal DialectLoader(IReadOnlyDictionary<string, string> documents)
        {
            _documents = documents;
        }

        internal int GetCallCount(string uri)
        {
            return _callCounts.TryGetValue(uri, out var count) ? count : 0;
        }

        internal int TotalCallCount { get; private set; }

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var key = uri.AbsoluteUri;
            _callCounts[key] = GetCallCount(key) + 1;
            TotalCallCount++;
            return Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(_documents[key])));
        }
    }
}
