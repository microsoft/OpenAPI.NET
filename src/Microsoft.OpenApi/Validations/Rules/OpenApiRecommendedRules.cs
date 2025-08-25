// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Additional recommended validation rules for OpenAPI.
    /// </summary>
    public static class OpenApiRecommendedRules
    {
        /// <summary>
        /// A relative path to an individual endpoint. The field name MUST begin with a slash.
        /// </summary>
        public static ValidationRule<OpenApiPaths> GetOperationShouldNotHaveRequestBody =>
            new(nameof(GetOperationShouldNotHaveRequestBody),
                (context, item) =>
                {
                    foreach (var path in item)
                    {
                        if (path.Value.Operations is not { Count: > 0 } operations)
                        {
                            continue;
                        }

                        context.Enter(path.Key);

                        foreach (var operation in operations)
                        {
                            if (!operation.Key.Equals(HttpMethod.Get))
                            {
                                continue;
                            }

                            if (operation.Value.RequestBody != null)
                            {
                                context.Enter(operation.Key.Method.ToLowerInvariant());
                                context.Enter("requestBody");

                                context.CreateWarning(
                                    nameof(GetOperationShouldNotHaveRequestBody),
                                    "GET operations should not have a request body.");

                                context.Exit();
                                context.Exit();
                            }
                        }

                        context.Exit();
                    }
                });
    }
}
