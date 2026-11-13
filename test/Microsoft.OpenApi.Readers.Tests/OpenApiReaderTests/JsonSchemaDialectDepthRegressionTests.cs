// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectDepthRegressionTests
{
    private const string KnownDialect = "https://json-schema.org/draft/2020-12/schema";

    [Theory]
    [InlineData("3.1.1", false)]
    [InlineData("3.1.1", true)]
    [InlineData("3.2.0", false)]
    [InlineData("3.2.0", true)]
    public async Task ResolvingASuffixFirstDoesNotBypassTheParentChainsDepthLimit(string version, bool suffixFirst)
    {
        var length = JsonSchemaDialectResolver.MaxResolutionDepth + 1;
        var loader = new DialectLoader(CreateChain("chain", length));
        using var input = CreateDocument(version, suffixFirst,
            ("TooDeep", GetDialectUri("chain", 0)),
            ("AtLimit", GetDialectUri("chain", 1)));

        var result = await LoadAsync(input, loader);

        AssertUnsupportedSchema(result, "TooDeep", GetDialectUri("chain", 0));
        Assert.Equal(length, loader.TotalCallCount);
        AssertLoadedOnce(loader, "chain", length);
    }

    [Theory]
    [InlineData("3.1.1", 15)]
    [InlineData("3.1.1", 16)]
    [InlineData("3.1.1", 17)]
    [InlineData("3.2.0", 15)]
    [InlineData("3.2.0", 16)]
    [InlineData("3.2.0", 17)]
    public async Task EveryParentLinkCountsAtTheDepthBoundary(string version, int length)
    {
        var loader = new DialectLoader(CreateChain("boundary", length));
        var dialect = GetDialectUri("boundary", 0);
        using var input = CreateDocument(version, false, ("Item", dialect));

        var result = await LoadAsync(input, loader);

        Assert.NotNull(result.Document);
        if (length > JsonSchemaDialectResolver.MaxResolutionDepth)
        {
            AssertUnsupportedSchema(result, "Item", dialect);
        }
        else
        {
            Assert.Empty(result.Diagnostic.Errors);
        }

        var loaded = Math.Min(length, JsonSchemaDialectResolver.MaxResolutionDepth);
        Assert.Equal(loaded, loader.TotalCallCount);
        AssertLoadedOnce(loader, "boundary", loaded);
        if (length > loaded)
        {
            Assert.Equal(0, loader.GetCallCount(GetDialectUri("boundary", loaded)));
        }
    }

    [Theory]
    [InlineData("3.1.1", false)]
    [InlineData("3.1.1", true)]
    [InlineData("3.2.0", false)]
    [InlineData("3.2.0", true)]
    public async Task SharedTailKeepsEachRootsDepthAndIsLoadedOnlyOnce(string version, bool longerFirst)
    {
        var tailLength = JsonSchemaDialectResolver.MaxResolutionDepth - 1;
        var documents = CreateChain("tail", tailLength);
        var atLimit = GetDialectUri("short", 0);
        var tooDeep = GetDialectUri("long", 0);
        var intermediate = GetDialectUri("long", 1);
        documents[atLimit] = MetaSchema(GetDialectUri("tail", 0));
        documents[tooDeep] = MetaSchema(intermediate);
        documents[intermediate] = MetaSchema(GetDialectUri("tail", 0));
        var loader = new DialectLoader(documents);
        using var input = CreateDocument(version, longerFirst, ("AtLimit", atLimit), ("TooDeep", tooDeep));

        var result = await LoadAsync(input, loader);

        AssertUnsupportedSchema(result, "TooDeep", tooDeep);
        Assert.Equal(tailLength + 3, loader.TotalCallCount);
        AssertLoadedOnce(loader, "tail", tailLength);
        Assert.Equal(1, loader.GetCallCount(atLimit));
        Assert.Equal(1, loader.GetCallCount(tooDeep));
        Assert.Equal(1, loader.GetCallCount(intermediate));
    }

    [Theory]
    [InlineData("3.1.1", false)]
    [InlineData("3.1.1", true)]
    [InlineData("3.2.0", false)]
    [InlineData("3.2.0", true)]
    public async Task SharedCyclicTailRemainsUnsupportedWithoutRepeatedLoads(string version, bool secondFirst)
    {
        var first = GetDialectUri("root", 0);
        var second = GetDialectUri("root", 1);
        var cycleStart = GetDialectUri("cycle", 0);
        var cycleEnd = GetDialectUri("cycle", 1);
        var loader = new DialectLoader(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [first] = MetaSchema(cycleStart),
            [second] = MetaSchema(cycleStart),
            [cycleStart] = MetaSchema(cycleEnd),
            [cycleEnd] = MetaSchema(cycleStart),
        });
        using var input = CreateDocument(version, secondFirst, ("First", first), ("Second", second));

        var result = await LoadAsync(input, loader);

        AssertBothUnsupported(result, first, second);
        Assert.Equal(4, loader.TotalCallCount);
        AssertLoadedOnce(loader, "root", 2);
        AssertLoadedOnce(loader, "cycle", 2);
    }

    [Theory]
    [InlineData("3.1.1", false)]
    [InlineData("3.1.1", true)]
    [InlineData("3.2.0", false)]
    [InlineData("3.2.0", true)]
    public async Task SharedMissingParentRemainsUnsupportedWithoutRepeatedLoads(string version, bool secondFirst)
    {
        var first = GetDialectUri("root", 0);
        var second = GetDialectUri("root", 1);
        var missing = GetDialectUri("missing", 0);
        var loader = new DialectLoader(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [first] = MetaSchema(missing),
            [second] = MetaSchema(missing),
        });
        using var input = CreateDocument(version, secondFirst, ("First", first), ("Second", second));

        var result = await LoadAsync(input, loader);

        AssertBothUnsupported(result, first, second);
        Assert.Equal(3, loader.TotalCallCount);
        AssertLoadedOnce(loader, "root", 2);
        Assert.Equal(1, loader.GetCallCount(missing));
    }

    private static Task<ReadResult> LoadAsync(Stream input, IStreamLoader loader) => OpenApiDocument.LoadAsync(
        input,
        OpenApiConstants.Json,
        new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        },
        TestContext.Current.CancellationToken);

    private static void AssertUnsupportedSchema(ReadResult result, string name, string dialect)
    {
        Assert.NotNull(result.Document);
        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Equal($"The JSON Schema dialect '{dialect}' is not supported.", error.Message);
        Assert.Equal($"#/components/schemas/{name}", error.Pointer);
    }

    private static void AssertBothUnsupported(ReadResult result, string first, string second)
    {
        Assert.NotNull(result.Document);
        Assert.Equal(2, result.Diagnostic.Errors.Count);
        Assert.Contains(result.Diagnostic.Errors,
            error => error.Pointer == "#/components/schemas/First" &&
                     error.Message == $"The JSON Schema dialect '{first}' is not supported.");
        Assert.Contains(result.Diagnostic.Errors,
            error => error.Pointer == "#/components/schemas/Second" &&
                     error.Message == $"The JSON Schema dialect '{second}' is not supported.");
    }

    private static void AssertLoadedOnce(DialectLoader loader, string prefix, int length)
    {
        for (var index = 0; index < length; index++)
        {
            Assert.Equal(1, loader.GetCallCount(GetDialectUri(prefix, index)));
        }
    }

    private static Dictionary<string, string> CreateChain(string prefix, int length)
    {
        var documents = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 0; index < length; index++)
        {
            var parent = index + 1 == length ? KnownDialect : GetDialectUri(prefix, index + 1);
            documents[GetDialectUri(prefix, index)] = MetaSchema(parent);
        }
        return documents;
    }

    private static string MetaSchema(string parent) => $$"""{ "$schema": "{{parent}}" }""";

    private static string GetDialectUri(string prefix, int index) => $"https://example.com/{prefix}-{index}";

    private static MemoryStream CreateDocument(string version, bool reverseOrder, params (string Name, string Dialect)[] schemas)
    {
        if (reverseOrder)
        {
            Array.Reverse(schemas);
        }
        var entries = new List<string>();
        foreach (var (name, dialect) in schemas)
        {
            entries.Add($$"""
                "{{name}}": { "$schema": "{{dialect}}", "type": "object" }
                """);
        }
        return new MemoryStream(Encoding.UTF8.GetBytes($$"""
            {
              "openapi": "{{version}}",
              "info": { "title": "test", "version": "1.0.0" },
              "components": { "schemas": { {{string.Join(",", entries)}} } }
            }
            """));
    }

    private sealed class DialectLoader(IReadOnlyDictionary<string, string> documents) : IStreamLoader
    {
        private readonly Dictionary<string, int> _callCounts = new(StringComparer.Ordinal);

        internal int TotalCallCount { get; private set; }

        internal int GetCallCount(string uri) => _callCounts.TryGetValue(uri, out var count) ? count : 0;

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var key = uri.AbsoluteUri;
            _callCounts[key] = GetCallCount(key) + 1;
            TotalCallCount++;
            return documents.TryGetValue(key, out var document)
                ? Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(document)))
                : Task.FromException<Stream>(new FileNotFoundException("The meta-schema was not found."));
        }
    }
}
