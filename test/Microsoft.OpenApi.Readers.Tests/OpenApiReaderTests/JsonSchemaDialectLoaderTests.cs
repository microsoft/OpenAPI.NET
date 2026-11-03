// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectLoaderTests
{
    private const string CustomDialect = "https://example.com/custom-dialect";
    private const string KnownDialect = "https://json-schema.org/draft/2020-12/schema";
    private const string MetaSchema = """{ "$schema": "https://json-schema.org/draft/2020-12/schema" }""";

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task MemoryStreamDocumentsResolveDialectsThroughTheDefaultHttpLoader(string version)
    {
        var handler = new DialectMessageHandler();
        using var client = new HttpClient(handler);
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

        Assert.Empty(result.Diagnostic.Errors);
        Assert.Equal(new Uri(CustomDialect), Assert.Single(handler.Requests));
        Assert.Equal(JsonSchemaType.String, result.Document.Components.Schemas["First"].Type);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task DisabledExternalLoadingDoesNotRequestUnknownDialects(string version)
    {
        var handler = new DialectMessageHandler();
        using var client = new HttpClient(handler);
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = false,
            HttpClient = client,
        };
        using var input = CreateDocument(version, CustomDialect);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Empty(handler.Requests);
        Assert.Equal(
            $"The JSON Schema dialect '{CustomDialect}' is not supported.",
            Assert.Single(result.Diagnostic.Errors).Message);
    }

    [Theory]
    [InlineData("3.1.1", KnownDialect)]
    [InlineData("3.1.1", "https://spec.openapis.org/oas/3.1/dialect/base")]
    [InlineData("3.2.0", KnownDialect)]
    [InlineData("3.2.0", "https://spec.openapis.org/oas/3.1/dialect/base")]
    public async Task KnownDialectsDoNotRequireExternalLoading(string version, string dialect)
    {
        var handler = new DialectMessageHandler();
        using var client = new HttpClient(handler);
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            HttpClient = client,
        };
        using var input = CreateDocument(version, dialect);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Empty(result.Diagnostic.Errors);
        Assert.Empty(handler.Requests);
    }

    [Theory]
    [InlineData("3.1.1", 0)]
    [InlineData("3.1.1", 1)]
    [InlineData("3.2.0", 0)]
    [InlineData("3.2.0", 1)]
    public async Task NonSeekableMetaSchemasRespectTheResponseSizeLimit(string version, int extraBytes)
    {
        var length = JsonSchemaDialectResolver.MaxMetaSchemaSize + extraBytes;
        using var metaSchemaStream = new NonSeekableMetaSchemaStream(length);
        var loader = new DelegateLoader((_, _) => Task.FromResult<Stream>(metaSchemaStream));
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

        if (extraBytes == 0)
        {
            Assert.Empty(result.Diagnostic.Errors);
        }
        else
        {
            Assert.Equal(
                $"The JSON Schema dialect '{CustomDialect}' is not supported.",
                Assert.Single(result.Diagnostic.Errors).Message);
        }
        Assert.Equal(1, loader.CallCount);
        Assert.Equal(length, metaSchemaStream.BytesRead);
        Assert.True(metaSchemaStream.WasDisposed);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task CancellationDuringMetaSchemaLoadingPropagates(string version)
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

        Assert.Equal(1, loader.CallCount);
        Assert.True(cancellationSource.IsCancellationRequested);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task CancellationDuringMetaSchemaReadingDisposesTheStream(string version)
    {
        using var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        using var metaSchemaStream = new NonSeekableMetaSchemaStream(MetaSchema.Length, cancellationSource);
        var loader = new DelegateLoader((_, _) => Task.FromResult<Stream>(metaSchemaStream));
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

        Assert.Equal(1, loader.CallCount);
        Assert.True(metaSchemaStream.BytesRead > 0);
        Assert.True(metaSchemaStream.WasDisposed);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task ReusingSettingsDoesNotReuseDialectResultsAcrossReads(string version)
    {
        var responseCount = 0;
        var loader = new DelegateLoader((_, _) =>
        {
            var response = ++responseCount == 1 ? MetaSchema : "{}";
            return Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(response)));
        });
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        using var firstInput = CreateDocument(version, CustomDialect);
        using var secondInput = CreateDocument(version, CustomDialect);

        var firstResult = await OpenApiDocument.LoadAsync(
            firstInput,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);
        var secondResult = await OpenApiDocument.LoadAsync(
            secondInput,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Empty(firstResult.Diagnostic.Errors);
        Assert.Equal(
            $"The JSON Schema dialect '{CustomDialect}' is not supported.",
            Assert.Single(secondResult.Diagnostic.Errors).Message);
        Assert.Equal(2, loader.CallCount);
    }

    private static MemoryStream CreateDocument(string version, string dialect)
    {
        var document = $$"""
            {
              "openapi": "{{version}}",
              "info": { "title": "test", "version": "1.0.0" },
              "jsonSchemaDialect": "{{dialect}}",
              "components": {
                "schemas": {
                  "First": { "type": "string" },
                  "Second": { "type": "object" }
                }
              }
            }
            """;
        return new MemoryStream(Encoding.UTF8.GetBytes(document));
    }

    private sealed class DialectMessageHandler : HttpMessageHandler
    {
        internal List<Uri> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests.Add(request.RequestUri);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(MetaSchema, Encoding.UTF8, "application/schema+json"),
            });
        }
    }

    private sealed class DelegateLoader(Func<Uri, CancellationToken, Task<Stream>> load) : IStreamLoader
    {
        internal int CallCount { get; private set; }

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return load(uri, cancellationToken);
        }
    }

    private sealed class NonSeekableMetaSchemaStream(int length, CancellationTokenSource cancelAfterRead = null) : Stream
    {
        private static readonly byte[] Prefix = Encoding.UTF8.GetBytes(MetaSchema);
        private int _position;

        internal int BytesRead { get; private set; }
        internal bool WasDisposed { get; private set; }
        public override bool CanRead => !WasDisposed;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = Math.Min(count, length - _position);
            for (var index = 0; index < bytesRead; index++, _position++)
            {
                buffer[offset + index] = _position < Prefix.Length ? Prefix[_position] : (byte)' ';
            }
            BytesRead += bytesRead;
            return bytesRead;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var bytesRead = Read(buffer, offset, count);
            if (bytesRead > 0)
            {
                cancelAfterRead?.Cancel();
                cancellationToken.ThrowIfCancellationRequested();
            }
            return Task.FromResult(bytesRead);
        }

        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            WasDisposed = true;
            base.Dispose(disposing);
        }
    }
}
