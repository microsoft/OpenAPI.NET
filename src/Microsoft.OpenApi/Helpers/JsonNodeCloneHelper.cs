// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Helpers
{
    internal static class JsonNodeCloneHelper
    {
        private static readonly JsonSerializerOptions options = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        internal static OpenApiAny Clone(OpenApiAny value)
        {
            var jsonString = Serialize(value);
            var result = JsonSerializer.Deserialize<OpenApiAny>(jsonString, options);

            return result;
        }

        internal static JsonSchema CloneJsonSchema(JsonSchema schema)
        {
            var jsonString = Serialize(schema);
            var result = JsonSerializer.Deserialize<JsonSchema>(jsonString, options);
            return result;
        }

        private static string Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var result = JsonSerializer.Serialize(obj, options);
            return result;
        }
    }
}
