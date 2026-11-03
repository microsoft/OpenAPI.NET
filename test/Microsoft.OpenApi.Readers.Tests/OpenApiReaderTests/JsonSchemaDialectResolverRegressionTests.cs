// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectResolverRegressionTests
{
    private const string CustomDialect = "https://example.com/meta";
    private const string MetaSchema = """{ "$schema": "https://json-schema.org/draft/2020-12/schema" }""";

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task ResolvingOneFragmentDoesNotResolveAnotherFragment(string version)
    {
        var loader = new DelegateLoader((uri, _) => Task.FromResult<Stream>(CreateStream(
            uri.AbsoluteUri == CustomDialect + "#a" ? MetaSchema : "{}")));
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var input = CreateDocument(version, CustomDialect + "#a", CustomDialect + "#b");

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result.Document);
        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Equal($"The JSON Schema dialect '{CustomDialect}#b' is not supported.", error.Message);
        Assert.Equal("#/components/schemas/Second", error.Pointer);
        Assert.Equal([CustomDialect + "#a", CustomDialect + "#b"], loader.Requests);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task EmptyFragmentsShareResolutionAndTheMetaSchemaCache(string version)
    {
        var loader = new DelegateLoader((_, _) => Task.FromResult<Stream>(CreateStream(MetaSchema)));
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var input = CreateDocument(version, CustomDialect, CustomDialect + "#");

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result.Document);
        Assert.Empty(result.Diagnostic.Errors);
        Assert.Equal(CustomDialect, Assert.Single(loader.Requests).TrimEnd('#'));
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task LoaderCancellationWithoutCallerCancellationRetainsUnsupportedDialect(string version)
    {
        var loader = new DelegateLoader((_, _) => Task.FromException<Stream>(
            new OperationCanceledException("The loader cancelled its own operation.")));
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var input = CreateDocument(version, CustomDialect);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        AssertUnsupportedDialect(result);
        Assert.Equal(CustomDialect, Assert.Single(loader.Requests));
        Assert.False(TestContext.Current.CancellationToken.IsCancellationRequested);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task HttpClientTimeoutRetainsUnsupportedDialect(string version)
    {
        var handler = new TimeoutMessageHandler();
        using var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMilliseconds(100),
        };
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            HttpClient = client,
        };
        using var input = CreateDocument(version, CustomDialect);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        AssertUnsupportedDialect(result);
        Assert.Equal(1, handler.CallCount);
        Assert.True(handler.RequestCancellationToken.IsCancellationRequested);
        Assert.False(TestContext.Current.CancellationToken.IsCancellationRequested);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task CallerCancellationStillPropagates(string version)
    {
        using var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        var loader = new DelegateLoader((_, token) =>
        {
            cancellationSource.Cancel();
            return Task.FromCanceled<Stream>(token);
        });
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var input = CreateDocument(version, CustomDialect);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            cancellationSource.Token));

        Assert.Equal(CustomDialect, Assert.Single(loader.Requests));
        Assert.True(cancellationSource.IsCancellationRequested);
    }

    private static void AssertUnsupportedDialect(ReadResult result)
    {
        Assert.NotNull(result.Document);
        Assert.Equal(
            $"The JSON Schema dialect '{CustomDialect}' is not supported.",
            Assert.Single(result.Diagnostic.Errors).Message);
    }

    private static MemoryStream CreateDocument(string version, string firstDialect, string secondDialect = null)
    {
        var secondSchema = secondDialect is null ? string.Empty : $$"""
            , "Second": { "$schema": "{{secondDialect}}", "type": "object" }
            """;
        return CreateStream($$"""
            {
              "openapi": "{{version}}",
              "info": { "title": "test", "version": "1.0.0" },
              "components": {
                "schemas": {
                  "First": { "$schema": "{{firstDialect}}", "type": "string" }{{secondSchema}}
                }
              }
            }
            """);
    }

    private static MemoryStream CreateStream(string content) => new(Encoding.UTF8.GetBytes(content));

    private sealed class DelegateLoader(Func<Uri, CancellationToken, Task<Stream>> load) : IStreamLoader
    {
        internal List<string> Requests { get; } = [];

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests.Add(uri.AbsoluteUri);
            return load(uri, cancellationToken);
        }
    }

    private sealed class TimeoutMessageHandler : HttpMessageHandler
    {
        internal int CallCount { get; private set; }
        internal CancellationToken RequestCancellationToken { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            RequestCancellationToken = cancellationToken;
            await Task.Delay(Timeout.Infinite, cancellationToken);
            throw new InvalidOperationException("The request should only finish through timeout cancellation.");
        }
    }
}
