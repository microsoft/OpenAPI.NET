using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.YamlReader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests;

public class OpenApiYamlReaderTests
{
    private static readonly Uri DocumentLocation = new("https://contoso.test/openapi.yaml");

    [Fact]
    public async Task ReadAsyncParsesDocumentsFromNonMemoryStreams()
    {
        var reader = new OpenApiYamlReader();
        await using var stream = new NonMemoryStream(CreateStream(
            """
            openapi: 3.0.1
            info:
              title: Sample API
              version: 1.0.0
            paths: {}
            """));

        var result = await reader.ReadAsync(stream, DocumentLocation, SettingsFixture.ReaderSettings, CancellationToken.None);

        Assert.NotNull(result.Document);
        Assert.Equal("Sample API", result.Document.Info.Title);
        Assert.Equal(OpenApiConstants.Yaml, result.Diagnostic.Format);
    }

    [Fact]
    public void ReadThrowsWhenYamlDoesNotContainADocument()
    {
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream(string.Empty);

        var exception = Assert.Throws<InvalidOperationException>(() => reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings));

        Assert.Equal("No documents found in the YAML stream.", exception.Message);
    }

    [Fact]
    public void ReadFragmentParsesSchemaFragments()
    {
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream(
            """
            type: string
            description: A reusable schema
            """);

        var schema = reader.ReadFragment<OpenApiSchema>(
            stream,
            OpenApiSpecVersion.OpenApi3_0,
            new OpenApiDocument(),
            out var diagnostic);

        Assert.NotNull(schema);
        Assert.Empty(diagnostic.Errors);
        Assert.Equal(JsonSchemaType.String, schema.Type);
        Assert.Equal("A reusable schema", schema.Description);
    }

    [Fact]
    public void ReadThrowsWhenSettingsIsNull()
    {
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream("openapi: 3.0.1");

        Assert.Throws<ArgumentNullException>(() => reader.Read(stream, DocumentLocation, null!));
    }

    private static MemoryStream CreateStream(string yaml)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(yaml));
    }

    private sealed class NonMemoryStream(Stream innerStream) : Stream
    {
        public override bool CanRead => innerStream.CanRead;
        public override bool CanSeek => innerStream.CanSeek;
        public override bool CanWrite => innerStream.CanWrite;
        public override long Length => innerStream.Length;
        public override long Position
        {
            get => innerStream.Position;
            set => innerStream.Position = value;
        }

        public override void Flush() => innerStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => innerStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => innerStream.Seek(offset, origin);
        public override void SetLength(long value) => innerStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => innerStream.Write(buffer, offset, count);
        public override ValueTask DisposeAsync() => innerStream.DisposeAsync();
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                innerStream.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
