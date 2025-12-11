using Xunit;
using Microsoft.OpenApi.Reader;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Threading;

namespace Microsoft.OpenApi.Tests.Reader;

public class OpenApiModelFactoryTests
{
    [Fact]
    public async Task UsesSettingsBaseUrl()
    {
        var tempFilePathReferee = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
        await File.WriteAllTextAsync(tempFilePathReferee,
"""
{
    "openapi": "3.1.2",
    "info": {
        "title": "OData Service for namespace microsoft.graph",
        "description": "This OData service is located at https://graph.microsoft.com/v1.0",
        "version": "1.0.1"
    },
    "servers": [
        {
            "url": "https://graph.microsoft.com/v1.0"
        }
    ],
    "paths": {
        "/placeholder": {
            "get": {
                "responses": {
                    "200": {
                        "content": {
                            "application/json": {
                                "schema": {
                                    "type": "string"
                                }
                            }
                        }
                    }
                }
            }
        }
    },
    "components": {
        "schemas": {
            "MySchema": {
                "type": "object",
                "properties": {
                    "id": {
                        "type": "string"
                    }
                }
            }
        }
    }
}
""");
        var tempFilePathReferrer = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
        await File.WriteAllTextAsync(tempFilePathReferrer,
$$$"""
{
    "openapi": "3.1.2",
    "info": {
        "title": "OData Service for namespace microsoft.graph",
        "description": "This OData service is located at https://graph.microsoft.com/v1.0",
        "version": "1.0.1"
    },
    "servers": [
        {
            "url": "https://graph.microsoft.com/v1.0"
        }
    ],
    "paths": {
        "/placeholder": {
            "get": {
                "responses": {
                    "200": {
                        "content": {
                            "application/json": {
                                "schema": {
                                    "$ref": "./{{{Path.GetFileName(tempFilePathReferee)}}}#/components/schemas/MySchema"
                                }
                            }
                        }
                    }
                }
            }
        }
    },
    "components": {
        "schemas": {
            "MySchema": {
                "type": "object",
                "properties": {
                    "id": {
                        "type": "string"
                    }
                }
            }
        }
    }
}
""");
        // read referrer document to a memory stream
        using var stream = new MemoryStream();
        using var reader = new StreamReader(tempFilePathReferrer);
        await reader.BaseStream.CopyToAsync(stream);
        stream.Position = 0;
        var baseUri = new Uri(tempFilePathReferrer);
        var settings = new OpenApiReaderSettings
        {
            BaseUrl = baseUri, 
        };
        var readResult = await OpenApiDocument.LoadAsync(stream, settings: settings);
        Assert.Equal(OpenApiConstants.Json, readResult.Diagnostic.Format);
        Assert.NotNull(readResult.Document);
        Assert.NotNull(readResult.Document.Components);
        Assert.Equal(baseUri, readResult.Document.BaseUri);
    }
    private readonly string documentJson =
"""
{
    "openapi": "3.1.0",
    "info": {
        "title": "Sample API",
        "version": "1.0.0"
    },
    "paths": {}
}
""";
    private readonly string documentYaml =
"""
openapi: 3.1.0
info:
  title: Sample API
  version: 1.0.0
paths: {}
""";
    [Fact]
    public async Task CanLoadANonSeekableStreamInJsonAndDetectFormat()
    {
        // Given
        using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(documentJson));
        using var nonSeekableStream = new NonSeekableStream(memoryStream);
    
        // When
        var (document, _) = await OpenApiDocument.LoadAsync(nonSeekableStream);
    
        // Then
        Assert.NotNull(document);
        Assert.Equal("Sample API", document.Info.Title);
    }

    [Fact]
    public async Task CanLoadANonSeekableStreamInYamlAndDetectFormat()
    {
        // Given
        using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(documentYaml));
        using var nonSeekableStream = new NonSeekableStream(memoryStream);
        var settings = new OpenApiReaderSettings();
        settings.AddYamlReader();
    
        // When
        var (document, _) = await OpenApiDocument.LoadAsync(nonSeekableStream, settings: settings);
    
        // Then
        Assert.NotNull(document);
        Assert.Equal("Sample API", document.Info.Title);
    }

    [Fact]
    public async Task CanLoadAnAsyncOnlyStreamInJsonAndDetectFormat()
    {
        // Given
        await using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(documentJson));
        await using var nonSeekableStream = new AsyncOnlyStream(memoryStream);
    
        // When
        var (document, _) = await OpenApiDocument.LoadAsync(nonSeekableStream);
    
        // Then
        Assert.NotNull(document);
        Assert.Equal("Sample API", document.Info.Title);
    }

    [Fact]
    public async Task CanLoadAnAsyncOnlyStreamInYamlAndDetectFormat()
    {
        // Given
        await using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(documentYaml));
        await using var nonSeekableStream = new AsyncOnlyStream(memoryStream);
        var settings = new OpenApiReaderSettings();
        settings.AddYamlReader();
    
        // When
        var (document, _) = await OpenApiDocument.LoadAsync(nonSeekableStream, settings: settings);
    
        // Then
        Assert.NotNull(document);
        Assert.Equal("Sample API", document.Info.Title);
    }

    public sealed class AsyncOnlyStream : Stream
    {
        private readonly Stream _innerStream;
        public AsyncOnlyStream(Stream stream) : base()
        {
            _innerStream = stream;
        }
        public override bool CanSeek => _innerStream.CanSeek;

        public override long Position { get => _innerStream.Position; set => throw new NotSupportedException("Blocking operations are not supported"); }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override void Flush()
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }
        protected override void Dispose(bool disposing)
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }

        public override async ValueTask DisposeAsync()
        {
            await _innerStream.DisposeAsync();
            await base.DisposeAsync();
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override bool CanTimeout => _innerStream.CanTimeout;

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }

        public override void Close()
        {
            _innerStream.Close();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return _innerStream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _innerStream.EndWrite(asyncResult);
        }

        public override int ReadByte()
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _innerStream.FlushAsync(cancellationToken);
        }

        public override int Read(Span<byte> buffer)
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _innerStream.ReadAsync(buffer, cancellationToken);
        }

        public override int ReadTimeout { get => _innerStream.ReadTimeout; set => _innerStream.ReadTimeout = value; }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            throw new NotSupportedException("Blocking operations are not supported.");
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _innerStream.WriteAsync(buffer, cancellationToken);
        }

        public override int WriteTimeout { get => _innerStream.WriteTimeout; set => _innerStream.WriteTimeout = value; }

    }

    public sealed class NonSeekableStream : Stream
    {
        private readonly Stream _innerStream;
        public NonSeekableStream(Stream stream) : base()
        {
            _innerStream = stream;
        }
        public override bool CanSeek => false;

        public override long Position { get => _innerStream.Position; set => throw new NotSupportedException("Seeking is not supported."); }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seeking is not supported.");
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }
        protected override void Dispose(bool disposing)
        {
            _innerStream.Dispose();
            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            await _innerStream.DisposeAsync();
            await base.DisposeAsync();
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override bool CanTimeout => _innerStream.CanTimeout;

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            _innerStream.CopyTo(destination, bufferSize);
        }

        public override void Close()
        {
            _innerStream.Close();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return _innerStream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _innerStream.EndWrite(asyncResult);
        }

        public override int ReadByte()
        {
            return _innerStream.ReadByte();
        }

        public override void WriteByte(byte value)
        {
            _innerStream.WriteByte(value);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _innerStream.FlushAsync(cancellationToken);
        }

        public override int Read(Span<byte> buffer)
        {
            return _innerStream.Read(buffer);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _innerStream.ReadAsync(buffer, cancellationToken);
        }

        public override int ReadTimeout { get => _innerStream.ReadTimeout; set => _innerStream.ReadTimeout = value; }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _innerStream.Write(buffer);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _innerStream.WriteAsync(buffer, cancellationToken);
        }

        public override int WriteTimeout { get => _innerStream.WriteTimeout; set => _innerStream.WriteTimeout = value; }

    }
}
