// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Bounds a text reader by the UTF-8 encoded size of consumed characters.
    /// </summary>
    internal sealed class InputLimitTextReader : TextReader
    {
        private readonly TextReader _inner;
        private readonly uint _maxByteCount;
        private ulong _byteCount;
        private bool _hasPendingHighSurrogate;
        private bool _endCharged;

        public InputLimitTextReader(TextReader inner, uint maxByteCount)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _maxByteCount = maxByteCount;
        }

        public override int Peek()
        {
            return _inner.Peek();
        }

        public override int Read()
        {
            var value = _inner.Read();
            if (value < 0)
            {
                ChargeEndOfInput();
                return value;
            }

            Charge((char)value);
            return value;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            var charsRead = _inner.Read(buffer, index, count);
            if (charsRead == 0)
            {
                ChargeEndOfInput();
                return 0;
            }

            for (var offset = 0; offset < charsRead; offset++)
            {
                Charge(buffer[index + offset]);
            }

            return charsRead;
        }

        private void Charge(char value)
        {
            if (_hasPendingHighSurrogate)
            {
                if (char.IsLowSurrogate(value))
                {
                    AddBytes(4);
                    _hasPendingHighSurrogate = false;
                    return;
                }

                AddBytes(3);
                _hasPendingHighSurrogate = false;
            }

            if (char.IsHighSurrogate(value))
            {
                _hasPendingHighSurrogate = true;
            }
            else if (char.IsLowSurrogate(value))
            {
                AddBytes(3);
            }
            else if (value <= 0x7F)
            {
                AddBytes(1);
            }
            else if (value <= 0x7FF)
            {
                AddBytes(2);
            }
            else
            {
                AddBytes(3);
            }
        }

        private void ChargeEndOfInput()
        {
            if (_endCharged)
            {
                return;
            }

            _endCharged = true;
            if (_hasPendingHighSurrogate)
            {
                AddBytes(3);
                _hasPendingHighSurrogate = false;
            }
        }

        private void AddBytes(uint count)
        {
            if (count > _maxByteCount - _byteCount)
            {
                throw InputLimitStream.CreateLimitException(_maxByteCount);
            }

            _byteCount += count;
        }
    }
}
