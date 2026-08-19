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
        private const char MaxOneByteUtf8Value = '\u007F';
        private const char MaxTwoByteUtf8Value = '\u07FF';
        private const uint OneByteUtf8Length = 1;
        private const uint TwoByteUtf8Length = 2;
        private const uint ThreeByteUtf8Length = 3;
        private const uint SurrogatePairUtf8Length = 4;

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

        /// <summary>
        /// Charges the UTF-8 encoded length of one UTF-16 code unit. Valid surrogate pairs are
        /// charged as one four-byte code point; unpaired surrogates use the three-byte UTF-8
        /// replacement-character length used by the default .NET encoder fallback.
        /// </summary>
        private void Charge(char value)
        {
            if (_hasPendingHighSurrogate)
            {
                if (char.IsLowSurrogate(value))
                {
                    AddBytes(SurrogatePairUtf8Length);
                    _hasPendingHighSurrogate = false;
                    return;
                }

                AddBytes(ThreeByteUtf8Length);
                _hasPendingHighSurrogate = false;
            }

            if (char.IsHighSurrogate(value))
            {
                _hasPendingHighSurrogate = true;
            }
            else if (char.IsLowSurrogate(value))
            {
                AddBytes(ThreeByteUtf8Length);
            }
            else if (value <= MaxOneByteUtf8Value)
            {
                AddBytes(OneByteUtf8Length);
            }
            else if (value <= MaxTwoByteUtf8Value)
            {
                AddBytes(TwoByteUtf8Length);
            }
            else
            {
                AddBytes(ThreeByteUtf8Length);
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
                AddBytes(ThreeByteUtf8Length);
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
