using System;
using System.IO;
using System.Linq;
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

        Assert.True(result.Document is not null, string.Join(Environment.NewLine, result.Diagnostic.Errors.Select(static error => error.Message)));
        Assert.Equal("Sample API", result.Document.Info.Title);
        Assert.Equal(OpenApiConstants.Yaml, result.Diagnostic.Format);
    }

    [Fact]
    public async Task ReadAsyncHonorsCancellationForMemoryStreams()
    {
        var reader = new OpenApiYamlReader();
        await using var stream = CreateStream(
            """
            openapi: 3.0.1
            info:
              title: Sample API
              version: 1.0.0
            paths: {}
            """);
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => reader.ReadAsync(
                stream,
                DocumentLocation,
                SettingsFixture.ReaderSettings,
                cancellationSource.Token));
    }

    [Fact]
    public async Task ReadAsyncPropagatesCancellationDuringYamlScanning()
    {
        var reader = new OpenApiYamlReader();
        using var cancellationSource = new CancellationTokenSource();
        await using var stream = new CancelingMemoryStream(
            Encoding.UTF8.GetBytes(
                """
                openapi: 3.0.1
                info:
                  title: Sample API
                  version: 1.0.0
                paths: {}
                """),
            cancellationSource);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => reader.ReadAsync(
                stream,
                DocumentLocation,
                SettingsFixture.ReaderSettings,
                cancellationSource.Token));
    }

    [Fact]
    public void ReadReturnsDiagnosticWhenYamlDoesNotContainADocument()
    {
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream(string.Empty);

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.Single(result.Diagnostic.Errors);
        Assert.Equal(OpenApiConstants.Yaml, result.Diagnostic.Format);
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

    [Fact]
    public async Task ReadAsyncThrowsWhenSettingsIsNullWithoutConsumingTheStream()
    {
        var reader = new OpenApiYamlReader();
        var inner = CreateStream("openapi: 3.0.1");
        await using var stream = new NonMemoryStream(inner);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => reader.ReadAsync(stream, DocumentLocation, null!, CancellationToken.None));

        Assert.Equal(0, inner.Position);
    }

    [Fact]
    public void ReadReturnsDiagnosticErrorForExponentialAliasExpansion()
    {
        // A "billion laughs" YAML bomb must surface as a diagnostic error with no document,
        // rather than throwing or exhausting memory.
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream(
            """
            a: &a ["x","x","x","x","x","x","x","x","x"]
            b: &b [*a,*a,*a,*a,*a,*a,*a,*a,*a]
            c: &c [*b,*b,*b,*b,*b,*b,*b,*b,*b]
            d: &d [*c,*c,*c,*c,*c,*c,*c,*c,*c]
            e: &e [*d,*d,*d,*d,*d,*d,*d,*d,*d]
            f: &f [*e,*e,*e,*e,*e,*e,*e,*e,*e]
            g: &g [*f,*f,*f,*f,*f,*f,*f,*f,*f]
            h: &h [*g,*g,*g,*g,*g,*g,*g,*g,*g]
            i: &i [*h,*h,*h,*h,*h,*h,*h,*h,*h]
            """);

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.NotEmpty(result.Diagnostic.Errors);
        Assert.Equal(OpenApiConstants.Yaml, result.Diagnostic.Format);
    }

    [Fact]
    public void ReadRejectsAliasBombBelowTotalNodeLimit()
    {
        const int fanOut = 21;
        var yaml = new StringBuilder();
        yaml.AppendLine($"a: &a [{string.Join(",", Enumerable.Repeat("\"x\"", fanOut))}]");
        var previousAnchor = 'a';
        for (var anchor = 'b'; anchor <= 'e'; anchor++)
        {
            yaml.AppendLine($"{anchor}: &{anchor} [{string.Join(",", Enumerable.Repeat($"*{previousAnchor}", fanOut))}]");
            previousAnchor = anchor;
        }

        var reader = new OpenApiYamlReader();
        using var stream = CreateStream(yaml.ToString());

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("expands aliases", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ReadReturnsDiagnosticForDeepNestingBeforeYamlDomComposition(bool flowStyle)
    {
        const int depth = 5_000;
        var yaml = flowStyle
            ? new string('[', depth) + new string(']', depth)
            : string.Concat(Enumerable.Repeat("- ", depth)) + "value";
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream(yaml);

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("maximum nesting depth", StringComparison.Ordinal));
    }

    [Fact]
    public void ReadHonoursMaxDepthAboveTheUnderlyingParserDefault()
    {
        // SharpYaml applies its own nesting limit, defaulting to 64. Unless the reader forwards
        // MaxDepth to it, every configured value above that default silently has no effect.
        const int depth = 100;
        var yaml = new string('[', depth) + new string(']', depth);
        var reader = new OpenApiYamlReader(new OpenApiYamlReaderSettings { MaxDepth = 200 });
        using var stream = CreateStream(yaml);

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.DoesNotContain(result.Diagnostic.Errors, error => error.Message.Contains("nesting depth", StringComparison.Ordinal));
    }

    [Fact]
    public void ReadHonoursMaxDepthBelowTheUnderlyingParserDefault()
    {
        const int depth = 40;
        var yaml = new string('[', depth) + new string(']', depth);
        var reader = new OpenApiYamlReader(new OpenApiYamlReaderSettings { MaxDepth = 32 });
        using var stream = CreateStream(yaml);

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("nesting depth of 32", StringComparison.Ordinal));
    }

    [Fact]
    public void ReadRejectsAliasExpansionThatExceedsMaxDepth()
    {
        // The anchor and the alias each sit within the depth limit, but expanding the alias grafts
        // the anchored subtree onto an equally deep position, producing a tree twice as deep. The
        // YAML parser cannot catch this because it sees the alias as a single event.
        const int half = 50;
        var yaml =
            $"a: &d {new string('[', half)}{new string(']', half)}\n" +
            $"b: {new string('[', half)}*d{new string(']', half)}\n";
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream(yaml);

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("expands an alias", StringComparison.Ordinal));
    }

    [Fact]
    public void ReadStopsAfterFirstYamlDocument()
    {
        const int depth = 5_000;
        var yaml =
            """
            openapi: 3.0.1
            info:
              title: First document
              version: 1.0.0
            paths: {}
            ---
            """ +
            Environment.NewLine +
            new string('[', depth) +
            new string(']', depth);
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream(yaml);

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.True(result.Document is not null, string.Join(Environment.NewLine, result.Diagnostic.Errors.Select(static error => error.Message)));
        Assert.Equal("First document", result.Document.Info.Title);
        Assert.Empty(result.Diagnostic.Errors);
    }

    [Fact]
    public void ReadReturnsDiagnosticForCyclicAlias()
    {
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream("x-cycle: &cycle [*cycle]");

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("forms a cycle", StringComparison.Ordinal));
    }

    [Fact]
    public void ReadFragmentReturnsDiagnosticForAliasExpansion()
    {
        var reader = new OpenApiYamlReader(new()
        {
            MaxAliasExpansionNodeCount = 1,
        });
        using var stream = CreateStream(
            """
            type: &value string
            title: *value
            description: *value
            """);

        var schema = reader.ReadFragment<OpenApiSchema>(
            stream,
            OpenApiSpecVersion.OpenApi3_0,
            new OpenApiDocument(),
            out var diagnostic);

        Assert.Null(schema);
        Assert.Single(diagnostic.Errors);
        Assert.Equal(OpenApiConstants.Yaml, diagnostic.Format);
    }

    [Fact]
    public void ReadFragmentReturnsDiagnosticWhenInputExceedsByteLimit()
    {
        var reader = new OpenApiYamlReader(new()
        {
            MaxInputByteCount = 10,
        });
        using var stream = CreateStream("type: string");

        var schema = reader.ReadFragment<OpenApiSchema>(
            stream,
            OpenApiSpecVersion.OpenApi3_0,
            new OpenApiDocument(),
            out var diagnostic);

        Assert.Null(schema);
        Assert.Contains(diagnostic.Errors, error => error.Message.Contains("maximum supported size", StringComparison.Ordinal));
        Assert.Equal(OpenApiConstants.Yaml, diagnostic.Format);
    }

    [Fact]
    public void ReadMeasuresOnlyTheRemainingBytesOfAPositionedStream()
    {
        const string padding = "##################################################";
        const string yaml =
            """
            openapi: 3.0.1
            info:
              title: Sample API
              version: 1.0.0
            paths: {}
            """;
        var reader = new OpenApiYamlReader(new()
        {
            MaxInputByteCount = (uint)Encoding.UTF8.GetByteCount(yaml),
        });
        using var stream = CreateStream(padding + yaml);
        stream.Position = padding.Length;

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.True(result.Document is not null, string.Join(Environment.NewLine, result.Diagnostic.Errors.Select(static error => error.Message)));
        Assert.Equal("Sample API", result.Document.Info.Title);
    }

    [Fact]
    public void ReadReturnsDiagnosticWhenInputExceedsByteLimit()
    {
        var reader = new OpenApiYamlReader(new()
        {
            MaxInputByteCount = 10,
        });
        using var stream = CreateStream("openapi: 3.0.1");

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("maximum supported size", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ReadAsyncReturnsDiagnosticWhenNonMemoryStreamExceedsByteLimit()
    {
        var reader = new OpenApiYamlReader(new()
        {
            MaxInputByteCount = 10,
        });
        await using var stream = new NonMemoryStream(CreateStream("openapi: 3.0.1"));

        var result = await reader.ReadAsync(
            stream,
            DocumentLocation,
            SettingsFixture.ReaderSettings,
            CancellationToken.None);

        Assert.Null(result.Document);
        Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("maximum supported size", StringComparison.Ordinal));
    }

    [Fact]
    public void ReadReturnsDiagnosticWhenScalarExceedsLengthLimit()
    {
        var reader = new OpenApiYamlReader(new()
        {
            MaxScalarLength = 3,
        });
        using var stream = CreateStream("value: 1234");

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.Contains(result.Diagnostic.Errors, error => error.Message.Contains("maximum supported length", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("a: 1\na: 2")]
    [InlineData("? [1, 2]: value")]
    [InlineData("[")]
    [InlineData("# comment only")]
    public void ReadReturnsDiagnosticForInvalidYaml(string yaml)
    {
        var reader = new OpenApiYamlReader();
        using var stream = CreateStream(yaml);

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.Null(result.Document);
        Assert.NotEmpty(result.Diagnostic.Errors);
        Assert.Equal(OpenApiConstants.Yaml, result.Diagnostic.Format);
    }

    [Fact]
    public void ReaderCopiesPerInstanceSettings()
    {
        var yamlSettings = new OpenApiYamlReaderSettings
        {
            MaxAliasExpansionNodeCount = 1,
        };
        var reader = new OpenApiYamlReader(yamlSettings);
        yamlSettings.MaxAliasExpansionNodeCount = 100;
        using var stream = CreateStream(
            """
            type: &value string
            title: *value
            description: *value
            """);

        var schema = reader.ReadFragment<OpenApiSchema>(
            stream,
            OpenApiSpecVersion.OpenApi3_0,
            new OpenApiDocument(),
            out var diagnostic);

        Assert.Null(schema);
        Assert.Single(diagnostic.Errors);
    }

    [Theory]
    [InlineData(0u, 5_000_000u, 5_000u, OpenApiYamlReaderSettings.DefaultMaxInputByteCount, OpenApiYamlReaderSettings.DefaultMaxScalarLength)]
    [InlineData(YamlConverter.MaximumAllowedDepth + 1, 5_000_000u, 5_000u, OpenApiYamlReaderSettings.DefaultMaxInputByteCount, OpenApiYamlReaderSettings.DefaultMaxScalarLength)]
    [InlineData(64u, 0u, 5_000u, OpenApiYamlReaderSettings.DefaultMaxInputByteCount, OpenApiYamlReaderSettings.DefaultMaxScalarLength)]
    [InlineData(64u, YamlConverter.MaximumAllowedNodeCount + 1, 5_000u, OpenApiYamlReaderSettings.DefaultMaxInputByteCount, OpenApiYamlReaderSettings.DefaultMaxScalarLength)]
    [InlineData(64u, 5_000_000u, 0u, OpenApiYamlReaderSettings.DefaultMaxInputByteCount, OpenApiYamlReaderSettings.DefaultMaxScalarLength)]
    [InlineData(64u, 5_000_000u, 5_000u, 0u, OpenApiYamlReaderSettings.DefaultMaxScalarLength)]
    [InlineData(64u, 5_000_000u, 5_000u, OpenApiYamlReaderSettings.DefaultMaxInputByteCount, 0u)]
    public void ReaderRejectsInvalidResourceLimits(
        uint maxDepth,
        uint maxNodeCount,
        uint maxAliasExpansionNodeCount,
        uint maxInputByteCount,
        uint maxScalarLength)
    {
        var settings = new OpenApiYamlReaderSettings
        {
            MaxDepth = maxDepth,
            MaxNodeCount = maxNodeCount,
            MaxAliasExpansionNodeCount = maxAliasExpansionNodeCount,
            MaxInputByteCount = maxInputByteCount,
            MaxScalarLength = maxScalarLength,
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => new OpenApiYamlReader(settings));
    }

    [Fact]
    public void ReaderLimitsDefaultToDocumentedValues()
    {
        Assert.Equal(134_217_728u, OpenApiYamlReaderSettings.DefaultMaxInputByteCount);
        Assert.Equal(65_536u, OpenApiYamlReaderSettings.DefaultMaxScalarLength);

        var settings = new OpenApiYamlReaderSettings();

        Assert.Equal(YamlConverter.DefaultMaxDepth, settings.MaxDepth);
        Assert.Equal(YamlConverter.DefaultMaxNodeCount, settings.MaxNodeCount);
        Assert.Equal(YamlConverter.DefaultMaxAliasExpansionNodeCount, settings.MaxAliasExpansionNodeCount);
        Assert.Equal(OpenApiYamlReaderSettings.DefaultMaxInputByteCount, settings.MaxInputByteCount);
        Assert.Equal(OpenApiYamlReaderSettings.DefaultMaxScalarLength, settings.MaxScalarLength);
    }

    [Fact]
    public void ReaderAcceptsMaxNodeCountAtTheSafeCeiling()
    {
        var settings = new OpenApiYamlReaderSettings
        {
            MaxNodeCount = YamlConverter.MaximumAllowedNodeCount,
        };
        var reader = new OpenApiYamlReader(settings);
        using var stream = CreateStream(
            """
            openapi: 3.0.1
            info:
              title: Sample API
              version: 1.0.0
            paths: {}
            """);

        var result = reader.Read(stream, DocumentLocation, SettingsFixture.ReaderSettings);

        Assert.True(result.Document is not null, string.Join(Environment.NewLine, result.Diagnostic.Errors.Select(static error => error.Message)));
        Assert.Equal("Sample API", result.Document.Info.Title);
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

    private sealed class CancelingMemoryStream(byte[] buffer, CancellationTokenSource cancellationSource) : MemoryStream(buffer)
    {
        private bool _canceled;

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = base.Read(buffer, offset, count);
            CancelAfterFirstRead();
            return bytesRead;
        }

        public override int Read(Span<byte> buffer)
        {
            var bytesRead = base.Read(buffer);
            CancelAfterFirstRead();
            return bytesRead;
        }

        private void CancelAfterFirstRead()
        {
            if (!_canceled)
            {
                _canceled = true;
                cancellationSource.Cancel();
            }
        }
    }
}
