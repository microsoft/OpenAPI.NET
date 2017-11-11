// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// JSON pointer.
    /// </summary>
    public class JsonPointer
    {
        private readonly string[] _Tokens;

        /// <summary>
        /// Tokens.
        /// </summary>
        public string[] Tokens { get { return _Tokens;  } }

        /// <summary>
        /// Initializes the <see cref="JsonPointer"/> class.
        /// </summary>
        /// <param name="pointer">Pointer as string.</param>
        public JsonPointer(string pointer)
        {
            _Tokens = pointer.Split('/').Skip(1).Select(Decode).ToArray();
        }

        /// <summary>
        /// Initializes the <see cref="JsonPointer"/> class.
        /// </summary>
        /// <param name="tokens">Pointer as tokenized string.</param>
        private JsonPointer(string[] tokens)
        {
            _Tokens = tokens;
        }

        /// <summary>
        /// Decode the string.
        /// </summary>
        private string Decode(string token)
        {
            return Uri.UnescapeDataString(token).Replace("~1", "/").Replace("~0", "~");
        }

        /// <summary>
        /// Gets the parent pointer.
        /// </summary>
        public JsonPointer ParentPointer
        {
            get
            {
                if (_Tokens.Length == 0) return null;
                return new JsonPointer(_Tokens.Take(_Tokens.Length - 1).ToArray());
            }
        }

        /// <summary>
        /// Gets the string representation of this JSON pointer.
        /// </summary>
        public override string ToString()
        {
            return "/" + String.Join("/", _Tokens);
        }
    }
}
