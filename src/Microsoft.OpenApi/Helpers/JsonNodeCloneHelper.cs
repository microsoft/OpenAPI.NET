// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Helpers
{
    internal static class JsonNodeCloneHelper
    {
        internal static JsonNode? Clone(JsonNode? value)
        {
            return value?.DeepClone();
        }
    }
}
