// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// Body expression.
    /// </summary>
    public sealed class BodyExpression : SourceExpression
    {
        /// <summary>
        /// body string
        /// </summary>
        public const string Body = "body";

        /// <summary>
        /// Prefix for a pointer
        /// </summary>
        public const string PointerPrefix = "#";

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression
        {
            get
            {
                if (String.IsNullOrEmpty(Value))
                {
                    return Body;
                }

                return Body + PointerPrefix + Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyExpression"/> class.
        /// </summary>
        public BodyExpression()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyExpression"/> class.
        /// </summary>
        /// <param name="pointer">a JSON Pointer [RFC 6901](https://tools.ietf.org/html/rfc6901).</param>
        public BodyExpression(JsonPointer pointer)
            : base(pointer?.ToString())
        {
            if (pointer == null)
            {
                throw Error.ArgumentNull(nameof(pointer));
            }
        }
    }
}
