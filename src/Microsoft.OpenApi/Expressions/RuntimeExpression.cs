// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// Base class for the Open API runtime expression.
    /// </summary>
    public abstract class RuntimeExpression
    {
        /// <summary>
        /// The dollar sign prefix for a runtime expression.
        /// </summary>
        public const string Prefix = "$";

        /// <summary>
        /// The expression string.
        /// </summary>
        public abstract string Expression { get; }

        /// <summary>
        /// Build the runtime expression from input string.
        /// </summary>
        /// <param name="expression">The runtime expression.</param>
        /// <returns>The built runtime expression object.</returns>
        public static RuntimeExpression Build(string expression)
        {
            if (String.IsNullOrWhiteSpace(expression))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(expression));
            }

            if (!expression.StartsWith(Prefix))
            {
                throw new OpenApiException(String.Format(SRResource.RuntimeExpressionMustBeginWithDollar, expression));
            }

            // $url
            if (expression == UrlExpression.Url)
            {
                return new UrlExpression();
            }

            // $method
            if (expression == MethodExpression.Method)
            {
                return new MethodExpression();
            }

            // $statusCode
            if (expression == StatusCodeExpression.StatusCode)
            {
                return new StatusCodeExpression();
            }

            // $request.
            if (expression.StartsWith(RequestExpression.Request))
            {
                string subString = expression.Substring(RequestExpression.Request.Length);
                SourceExpression source = SourceExpression.Build(subString);
                return new RequestExpression(source);
            }

            // $response.
            if (expression.StartsWith(ResponseExpression.Response))
            {
                string subString = expression.Substring(ResponseExpression.Response.Length);
                SourceExpression source = SourceExpression.Build(subString);
                return new ResponseExpression(source);
            }

            throw new OpenApiException(String.Format(SRResource.RuntimeExpressionHasInvalidFormat, expression));
        }
    }
}
