// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Microsoft.OpenApi.Helpers
{
    internal class JsonNodeCloneHelper
    {
        internal static JsonNode Clone(JsonNode value)
        {
            if(value == null)
            {
                return null;
            }
            
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var jsonString = JsonSerializer.Serialize(value, options);
            var result = JsonSerializer.Deserialize<JsonNode>(jsonString, options);

            return result;
        }
    }
}
