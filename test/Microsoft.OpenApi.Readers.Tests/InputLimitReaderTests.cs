// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Readers.Exceptions;
using Microsoft.OpenApi.Readers.ParseNodes;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class InputLimitReaderTests
    {
        [Fact]
        public void TextReaderPeekDoesNotConsumeOrChargeInput()
        {
            using var inner = new StringReader("a");
            var reader = new InputLimitTextReader(inner, 1);

            var peeked = reader.Peek();
            var read = reader.Read();
            var end = reader.Read();

            peeked.Should().Be('a');
            read.Should().Be('a');
            end.Should().Be(-1);
        }

        [Fact]
        public void TextReaderBufferReadChargesCharactersAndReportsEndOfInput()
        {
            using var inner = new StringReader("abc");
            var reader = new InputLimitTextReader(inner, 3);
            var buffer = new char[3];

            var charsRead = reader.Read(buffer, 0, buffer.Length);
            var endCharsRead = reader.Read(buffer, 0, buffer.Length);

            charsRead.Should().Be(3);
            buffer.Should().Equal('a', 'b', 'c');
            endCharsRead.Should().Be(0);
        }

        [Fact]
        public void TextReaderAcceptsAllUtf8CharacterWidthsAtExactLimit()
        {
            const string input = "\u007F\u0080\u0800\U0001F600";
            using var inner = new StringReader(input);
            var reader = new InputLimitTextReader(inner, 10);
            var buffer = new char[input.Length];

            var charsRead = reader.Read(buffer, 0, buffer.Length);
            var endCharsRead = reader.Read(buffer, 0, buffer.Length);

            charsRead.Should().Be(input.Length);
            new string(buffer).Should().Be(input);
            endCharsRead.Should().Be(0);
        }

        [Fact]
        public void TextReaderRejectsSurrogatePairAboveLimit()
        {
            using var inner = new StringReader("\U0001F600");
            var reader = new InputLimitTextReader(inner, 3);

            reader.Read().Should().Be('\uD83D');

            Assert.Throws<OpenApiReaderException>(() => reader.Read());
        }

        [Fact]
        public void TextReaderChargesUnpairedLowSurrogateAsReplacementCharacter()
        {
            using var inner = new StringReader("\uDC00");
            var reader = new InputLimitTextReader(inner, 2);

            Assert.Throws<OpenApiReaderException>(() => reader.Read());
        }

        [Fact]
        public void TextReaderChargesPendingHighSurrogateBeforeFollowingCharacter()
        {
            using var inner = new StringReader("\uD800a");
            var reader = new InputLimitTextReader(inner, 3);

            reader.Read().Should().Be('\uD800');

            Assert.Throws<OpenApiReaderException>(() => reader.Read());
        }

        [Fact]
        public void TextReaderChargesPendingHighSurrogateAtEndOfInput()
        {
            using var inner = new StringReader("\uD800");
            var reader = new InputLimitTextReader(inner, 2);

            reader.Read().Should().Be('\uD800');

            Assert.Throws<OpenApiReaderException>(() => reader.Read());
        }

        [Fact]
        public void StreamExposesSupportedPropertiesAndOperations()
        {
            using var inner = new MemoryStream(new byte[] { 1 });
            using var stream = new InputLimitStream(inner, 1, true);

            stream.Flush();

            stream.CanRead.Should().BeTrue();
            stream.CanSeek.Should().BeTrue();
            stream.CanWrite.Should().BeFalse();
            stream.Length.Should().Be(1);
            stream.Position.Should().Be(0);
        }

        [Fact]
        public void StreamRejectsUnsupportedOperations()
        {
            using var inner = new MemoryStream(new byte[] { 1 });
            using var stream = new InputLimitStream(inner, 1, true);

            Assert.Throws<NotSupportedException>(() => stream.Position = 0);
            Assert.Throws<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));
            Assert.Throws<NotSupportedException>(() => stream.SetLength(0));
            Assert.Throws<NotSupportedException>(() => stream.Write(new byte[1], 0, 1));
        }

        [Fact]
        public async Task StreamZeroLengthReadsReturnImmediately()
        {
            using var inner = new MemoryStream(new byte[] { 1 });
            using var stream = new InputLimitStream(inner, 1, true);
            var buffer = new byte[1];

            var syncBytesRead = stream.Read(buffer, 0, 0);
            var asyncBytesRead = await stream.ReadAsync(buffer, 0, 0, CancellationToken.None);

            syncBytesRead.Should().Be(0);
            asyncBytesRead.Should().Be(0);
            stream.Position.Should().Be(0);
        }

        [Fact]
        public void SeekableStreamReturnsEndOfInputAtExactLimit()
        {
            using var inner = new MemoryStream(new byte[] { 1, 2 });
            using var stream = new InputLimitStream(inner, 2, true);
            var buffer = new byte[2];

            var bytesRead = stream.Read(buffer, 0, buffer.Length);
            var endBytesRead = stream.Read(buffer, 0, 1);

            bytesRead.Should().Be(2);
            endBytesRead.Should().Be(0);
        }

        [Fact]
        public async Task SeekableStreamAsyncReadReturnsEndOfInputAtExactLimit()
        {
            using var inner = new MemoryStream(new byte[] { 1, 2 });
            using var stream = new InputLimitStream(inner, 2, true);
            var buffer = new byte[2];

            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, CancellationToken.None);
            var endBytesRead = await stream.ReadAsync(buffer, 0, 1, CancellationToken.None);

            bytesRead.Should().Be(2);
            endBytesRead.Should().Be(0);
        }

        [Fact]
        public void NonSeekableStreamReturnsEndOfInputAtExactLimit()
        {
            using var inner = new NonSeekableReadStream(new byte[] { 1, 2 });
            using var stream = new InputLimitStream(inner, 2, true);
            var buffer = new byte[2];

            var bytesRead = stream.Read(buffer, 0, buffer.Length);
            var endBytesRead = stream.Read(buffer, 0, 1);

            bytesRead.Should().Be(2);
            endBytesRead.Should().Be(0);
        }

        [Fact]
        public async Task TextReaderReadAsyncReturnsInputLimitDiagnostic()
        {
            using var input = new StringReader(
                "openapi: 3.0.0\ninfo:\n  title: test\n  version: 1.0.0\npaths: {}");
            var reader = new OpenApiTextReaderReader(new()
            {
                MaxInputByteCount = 16
            });

            var result = await reader.ReadAsync(input);

            result.OpenApiDocument.Should().BeNull();
            result.OpenApiDiagnostic.Errors.Should().ContainSingle();
            result.OpenApiDiagnostic.Errors[0].Message.Should().Contain("maximum supported size");
        }

        [Fact]
        public void ValueNodeRejectsProgrammaticScalarAboveContextLimit()
        {
            var context = new ParsingContext(new())
            {
                MaxScalarLength = 3
            };

            var exception = Assert.Throws<OpenApiReaderException>(
                () => new ValueNode(context, new YamlScalarNode("four")));

            exception.Message.Should().Contain("maximum supported length");
        }

        [Fact]
        public void YamlHelperRejectsNonScalarNode()
        {
            var node = new YamlSequenceNode();

            var exception = Assert.Throws<OpenApiException>(() => node.GetScalarValue());

            exception.Message.Should().Contain("Expected scalar");
        }

        [Fact]
        public void YamlHelperRejectsScalarAboveExplicitLimit()
        {
            var node = new YamlScalarNode("four");

            var exception = Assert.Throws<OpenApiException>(() => node.GetScalarValue(3));

            exception.Message.Should().Contain("maximum supported length");
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
