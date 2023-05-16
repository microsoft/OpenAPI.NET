// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Helpers
{
    internal class JsonNodeCloneHelper
    {
        internal static OpenApiAny Clone(OpenApiAny value)
        {
            if(value == null)
            {
                return null;
            }
            
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var jsonString = JsonSerializer.Serialize(value.Node, options);
            var result = JsonSerializer.Deserialize<OpenApiAny>(jsonString, options);

            return result;
        }
    }
}
