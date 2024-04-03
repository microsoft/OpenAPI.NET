// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Exceptions;

namespace Microsoft.OpenApi.Reader
{
    internal static class JsonNodeHelper
    {
        public static string GetScalarValue(this JsonNode node)
        {

            var scalarNode = node is JsonValue value ? value : throw new OpenApiException($"Expected scalar value.");

            return Convert.ToString(scalarNode?.GetValue<object>(), CultureInfo.InvariantCulture);
        }
    }
}
