// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Readers.Exceptions;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Bounds bytes consumed from an input stream without buffering the complete input.
    /// </summary>
    internal sealed class InputLimitStream : Stream
    {
        private readonly Stream _inner;
        private readonly long _maxByteCount;
        private readonly bool _leaveOpen;
        private long _byteCount;

        public InputLimitStream(Stream inner, uint maxByteCount, bool leaveOpen)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _maxByteCount = maxByteCount;
            _leaveOpen = leaveOpen;

        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count == 0)
            {
                return 0;
            }

            var bytesRead = _inner.Read(buffer, offset, GetAllowedReadCount(count));
            Charge(bytesRead);
            return bytesRead;
        }

        public override async Task<int> ReadAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken)
        {
            if (count == 0)
            {
                return 0;
            }

            if (_byteCount >= _maxByteCount)
            {
                var probe = new byte[1];
                var probeBytesRead = await _inner.ReadAsync(
                    probe,
                    0,
                    1,
                    cancellationToken).ConfigureAwait(false);
                if (probeBytesRead == 0)
                {
                    return 0;
                }

                throw CreateLimitException((uint)_maxByteCount);
            }

            var bytesRead = await _inner.ReadAsync(
                buffer,
                offset,
                GetAllowedReadCount(count),
                cancellationToken).ConfigureAwait(false);
            Charge(bytesRead);
            return bytesRead;
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
            if (disposing && !_leaveOpen)
            {
                _inner.Dispose();
            }

            base.Dispose(disposing);
        }

        private int GetAllowedReadCount(int requestedCount)
        {
            var remaining = _maxByteCount - _byteCount;
            if (remaining > 0)
            {
                return (int)Math.Min(requestedCount, remaining);
            }

            if (_inner.CanSeek && _inner.Position >= _inner.Length)
            {
                return 0;
            }

            var probe = _inner.ReadByte();
            if (probe < 0)
            {
                return 0;
            }

            throw CreateLimitException((uint)_maxByteCount);
        }

        private void Charge(int bytesRead)
        {
            _byteCount += bytesRead;
        }

        internal static OpenApiReaderException CreateLimitException(uint maxByteCount)
        {
            return new OpenApiReaderException(
                $"The input exceeds the maximum supported size of {maxByteCount} bytes.");
        }
    }
}
