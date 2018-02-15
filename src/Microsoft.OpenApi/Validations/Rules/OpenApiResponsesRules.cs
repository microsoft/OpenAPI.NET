// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiResponses"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiResponsesRules
    {
        /// <summary>
        /// An OpenAPI operation must contain at least one response
        /// </summary>
        public static ValidationRule<OpenApiResponses> ResponsesMustContainAtLeastOneResponse =>
            new ValidationRule<OpenApiResponses>(
                (context, responses) =>
                {
                    if (!responses.Keys.Any())
                    {
                        context.AddError(
                            new ValidationError(
                                ErrorReason.Required,
                                context.PathString,
                                "Responses must contain at least one response"));
                    }
                });

        /// <summary>
        /// The response key must either be "default" or an HTTP status code (1xx, 2xx, 3xx, 4xx, 5xx)
        /// </summary>
        public static ValidationRule<OpenApiResponses> ResponsesMustBeIdentifiedByDefaultOrStatusCode =>
            new ValidationRule<OpenApiResponses>(
                (context, responses) =>
                {
                    foreach (var key in responses.Keys)
                    {
                        context.Enter(key);

                        if (key != "default" && !Regex.IsMatch(key, "^[1-5|X][0-9|X][0-9|X]$"))
                        {
                            context.AddError(
                                new ValidationError(
                                    ErrorReason.Format,
                                    context.PathString,
                                    "Responses key must be 'default' or an HTTP status code"));
                        }

                        context.Exit();
                    }
                });
    }
}