﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// $response. expression.
    /// </summary>
    public sealed class ResponseExpression : RuntimeExpression
    {
        public const string Response = "$response.";

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression
        {
            get
            {
                return Response + Source.Expression;
            }
        }

        /// <summary>
        /// The <see cref="SourceExpression"/> expression.
        /// </summary>
        public SourceExpression Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseExpression"/> class.
        /// </summary>
        /// <param name="source">The source of the response.</param>
        public ResponseExpression(SourceExpression source)
        {
            Source = source ?? throw Error.ArgumentNull(nameof(source));
        }
    }
}
