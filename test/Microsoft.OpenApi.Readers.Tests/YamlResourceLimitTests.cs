// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class YamlResourceLimitTests
    {
        private const string MinimalDocument =
            "openapi: 3.0.0\ninfo:\n  title: test\n  version: 1.0.0\npaths: {}";

        [Fact]
        public void ResourceLimitsDefaultToDocumentedValues()
        {
            var settings = new OpenApiReaderSettings();

            settings.MaxDepth.Should().Be(64);
            settings.MaxNodeCount.Should().Be(5_000_000);
            settings.MaxAliasExpansionNodeCount.Should().Be(5_000);
            settings.MaxInputByteCount.Should().Be(128 * 1024 * 1024);
            settings.MaxScalarLength.Should().Be(64 * 1024);
        }

        [Fact]
        public void ResourceLimitSettersRejectInvalidValuesWithoutChangingEffectiveLimits()
        {
            var settings = new OpenApiReaderSettings();

            Assert.Throws<ArgumentOutOfRangeException>(() => settings.MaxDepth = 0);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => settings.MaxDepth = OpenApiReaderSettings.MaximumAllowedDepth + 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => settings.MaxNodeCount = 0);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => settings.MaxNodeCount = OpenApiReaderSettings.MaximumAllowedNodeCount + 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => settings.MaxAliasExpansionNodeCount = 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => settings.MaxInputByteCount = 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => settings.MaxScalarLength = 0);

            settings.MaxDepth.Should().Be(OpenApiReaderSettings.DefaultMaxDepth);
            settings.MaxNodeCount.Should().Be(OpenApiReaderSettings.DefaultMaxNodeCount);
            settings.MaxAliasExpansionNodeCount.Should().Be(
                OpenApiReaderSettings.DefaultMaxAliasExpansionNodeCount);
            settings.MaxInputByteCount.Should().Be(OpenApiReaderSettings.DefaultMaxInputByteCount);
            settings.MaxScalarLength.Should().Be(OpenApiReaderSettings.DefaultMaxScalarLength);
        }

        [Fact]
        public void StringReaderAllowsInputAtLimitAndRejectsInputAboveLimit()
        {
            var inputByteCount = (uint)Encoding.UTF8.GetByteCount(MinimalDocument);

            var acceptedDocument = new OpenApiStringReader(new()
            {
                MaxInputByteCount = inputByteCount
            }).Read(MinimalDocument, out var acceptedDiagnostic);
            var rejectedDocument = new OpenApiStringReader(new()
            {
                MaxInputByteCount = inputByteCount - 1
            }).Read(MinimalDocument, out var rejectedDiagnostic);

            acceptedDocument.Should().NotBeNull();
            acceptedDiagnostic.Errors.Should().BeEmpty();
            rejectedDocument.Should().NotBeNull();
            rejectedDiagnostic.Errors.Should().ContainSingle();
        }

        [Fact]
        public void TextReaderRejectsOversizedInputAsDiagnostic()
        {
            using var input = new StringReader(MinimalDocument);
            var reader = new OpenApiTextReaderReader(new()
            {
                MaxInputByteCount = 16
            });

            var document = reader.Read(input, out var diagnostic);

            document.Should().NotBeNull();
            diagnostic.Errors.Should().ContainSingle();
        }

        [Fact]
        public async Task AsyncNonSeekableStreamRejectsOversizedInputBeforeUnboundedBuffering()
        {
            using var input = new NonSeekableReadStream(Encoding.UTF8.GetBytes(MinimalDocument));
            var reader = new OpenApiStreamReader(new()
            {
                MaxInputByteCount = 16,
                LeaveStreamOpen = true
            });

            var result = await reader.ReadAsync(input);

            result.OpenApiDocument.Should().BeNull();
            result.OpenApiDiagnostic.Errors.Should().ContainSingle();
            result.OpenApiDiagnostic.Errors[0].Message.Should().Contain("maximum supported size");
            input.CanRead.Should().BeTrue();
        }

        [Fact]
        public void StreamFragmentRejectsOversizedInputAsDiagnostic()
        {
            using var input = new MemoryStream(Encoding.UTF8.GetBytes("type: string"));
            var reader = new OpenApiStreamReader(new()
            {
                MaxInputByteCount = 4
            });

            var schema = reader.ReadFragment<OpenApiSchema>(
                input,
                OpenApiSpecVersion.OpenApi3_0,
                out var diagnostic);

            schema.Should().BeNull();
            diagnostic.Errors.Should().ContainSingle();
            diagnostic.Errors[0].Message.Should().Contain("maximum supported size");
        }

        [Fact]
        public void ScalarLengthAllowsExactLimitAndRejectsLongerScalar()
        {
            var reader = new OpenApiStringReader(new()
            {
                MaxScalarLength = 4
            });

            var accepted = reader.ReadFragment<IOpenApiAny>(
                "key: 1234",
                OpenApiSpecVersion.OpenApi3_0,
                out var acceptedDiagnostic);
            var rejected = reader.ReadFragment<IOpenApiAny>(
                "key: 12345",
                OpenApiSpecVersion.OpenApi3_0,
                out var rejectedDiagnostic);

            accepted.Should().BeOfType<OpenApiObject>();
            acceptedDiagnostic.Errors.Should().BeEmpty();
            rejected.Should().BeNull();
            rejectedDiagnostic.Errors.Should().ContainSingle();
        }

        [Fact]
        public void TotalNodeLimitAllowsExactLimitAndRejectsAdditionalNode()
        {
            var accepted = new OpenApiStringReader(new()
            {
                MaxNodeCount = 3
            }).ReadFragment<IOpenApiAny>(
                "[a, b]",
                OpenApiSpecVersion.OpenApi3_0,
                out var acceptedDiagnostic);
            var rejected = new OpenApiStringReader(new()
            {
                MaxNodeCount = 2
            }).ReadFragment<IOpenApiAny>(
                "[a, b]",
                OpenApiSpecVersion.OpenApi3_0,
                out var rejectedDiagnostic);

            accepted.Should().BeOfType<OpenApiArray>();
            acceptedDiagnostic.Errors.Should().BeEmpty();
            rejected.Should().BeNull();
            rejectedDiagnostic.Errors.Should().ContainSingle();
        }

        [Fact]
        public void AliasExpansionLimitChargesCompleteAnchoredSubtree()
        {
            const string input = "a: &value [x]\nb: *value\nc: *value";

            var accepted = new OpenApiStringReader(new()
            {
                MaxAliasExpansionNodeCount = 4
            }).ReadFragment<IOpenApiAny>(
                input,
                OpenApiSpecVersion.OpenApi3_0,
                out var acceptedDiagnostic);
            var rejected = new OpenApiStringReader(new()
            {
                MaxAliasExpansionNodeCount = 3
            }).ReadFragment<IOpenApiAny>(
                input,
                OpenApiSpecVersion.OpenApi3_0,
                out var rejectedDiagnostic);

            accepted.Should().BeOfType<OpenApiObject>();
            acceptedDiagnostic.Errors.Should().BeEmpty();
            rejected.Should().BeNull();
            rejectedDiagnostic.Errors.Should().ContainSingle();
        }

        [Fact]
        public void AliasExpandedSubtreeCannotExceedEffectiveDepth()
        {
            const string input = "a: &value [[[x]]]\nb: [[[*value]]]";
            var reader = new OpenApiStringReader(new()
            {
                MaxDepth = 6
            });

            var result = reader.ReadFragment<IOpenApiAny>(
                input,
                OpenApiSpecVersion.OpenApi3_0,
                out var diagnostic);

            result.Should().BeNull();
            diagnostic.Errors.Should().ContainSingle();
            diagnostic.Errors[0].Message.Should().Contain("expands an alias");
        }

        [Theory]
        [InlineData("a: &value [*value]", "forms a cycle")]
        [InlineData("a: *missing", "unknown anchor")]
        [InlineData("a: &value x\nb: &value y", "duplicate anchor")]
        [InlineData("a: x\na: y", "duplicate key")]
        [InlineData("? [a, b]\n: value", "mapping keys must be scalar")]
        public void InvalidYamlGraphIsReportedAsDiagnostic(string input, string expectedMessage)
        {
            var result = new OpenApiStringReader().ReadFragment<IOpenApiAny>(
                input,
                OpenApiSpecVersion.OpenApi3_0,
                out var diagnostic);

            result.Should().BeNull();
            diagnostic.Errors.Should().ContainSingle();
            diagnostic.Errors[0].Message.Should().Contain(expectedMessage);
        }

        [Fact]
        public void CommentOnlyInputIsReportedAsDiagnostic()
        {
            var document = new OpenApiStringReader().Read(
                "# no document content",
                out var diagnostic);

            document.Should().NotBeNull();
            diagnostic.Errors.Should().ContainSingle();
        }

        [Fact]
        public void ReaderStopsAfterFirstYamlDocument()
        {
            var hostileSecondDocument = new string('[', 100) + new string(']', 100);
            var input = MinimalDocument + "\n---\n" + hostileSecondDocument;

            var document = new OpenApiStringReader().Read(input, out var diagnostic);

            document.Should().NotBeNull();
            diagnostic.Errors.Should().BeEmpty();
        }

        [Fact]
        public void RaisedDepthLimitAllowsDeepFragment()
        {
            const int depth = 70;
            var input = new string('[', depth) + "value" + new string(']', depth);
            var reader = new OpenApiStringReader(new()
            {
                MaxDepth = 80
            });

            var result = reader.ReadFragment<IOpenApiAny>(
                input,
                OpenApiSpecVersion.OpenApi3_0,
                out var diagnostic);

            result.Should().BeOfType<OpenApiArray>();
            diagnostic.Errors.Should().BeEmpty();
        }

        private sealed class NonSeekableReadStream : Stream
        {
            private readonly MemoryStream _inner;

            public NonSeekableReadStream(byte[] input)
            {
                _inner = new MemoryStream(input);
            }

            public override bool CanRead => _inner.CanRead;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _inner.Read(buffer, offset, count);
            }

            public override Task<int> ReadAsync(
                byte[] buffer,
                int offset,
                int count,
                CancellationToken cancellationToken)
            {
                return _inner.ReadAsync(buffer, offset, count, cancellationToken);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _inner.Dispose();
                }

                base.Dispose(disposing);
            }
        }
    }
}
