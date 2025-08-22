// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiResponses"/>.
    /// </summary>
    [OpenApiRule]
    public static partial class OpenApiResponsesRules
    {
        /// <summary>
        /// The response key regex pattern for status codes and ranges.
        /// </summary>
#if NET8_0_OR_GREATER
        [GeneratedRegex(@"^[1-5](?>[0-9]{2}|[xX]{2})$", RegexOptions.None, matchTimeoutMilliseconds: 100)]
        internal static partial Regex StatusCodeRegex();
#else
        internal static readonly Regex StatusCodeRegex = new(@"^[1-5](?>[0-9]{2}|[xX]{2})$", RegexOptions.None, TimeSpan.FromMilliseconds(100));
#endif
        /// <summary>
        /// An OpenAPI operation must contain at least one response
        /// </summary>
        public static ValidationRule<OpenApiResponses> ResponsesMustContainAtLeastOneResponse =>
            new(nameof(ResponsesMustContainAtLeastOneResponse),
                (context, responses) =>
                {
                    if (responses.Keys.Count == 0)
                    {
                        context.CreateError(nameof(ResponsesMustContainAtLeastOneResponse),
                                "Responses must contain at least one response");
                    }
                });

        /// <summary>
        /// The response key must either be "default" or an HTTP status code (1xx, 2xx, 3xx, 4xx, 5xx).
        /// </summary>
        public static ValidationRule<OpenApiResponses> ResponsesMustBeIdentifiedByDefaultOrStatusCode =>
            new(nameof(ResponsesMustBeIdentifiedByDefaultOrStatusCode),
                (context, responses) =>
                {
                    foreach (var key in responses.Keys)
                    {
                        if (!"default".Equals(key, StringComparison.OrdinalIgnoreCase) && !StatusCodeRegex
#if NET8_0_OR_GREATER
                            ().IsMatch(key)
#else
                            .IsMatch(key)
#endif
                            )
                        {
                            context.Enter(key);
                            context.CreateError(nameof(ResponsesMustBeIdentifiedByDefaultOrStatusCode),
                                    "Responses key must be 'default', an HTTP status code, " +
                                    "or one of the following strings representing a range of HTTP status codes: " +
                                    "'1XX', '2XX', '3XX', '4XX', '5XX' (case insensitive)");
                            context.Exit();
                        }
                    }
                });
    }
}
