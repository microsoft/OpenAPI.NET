// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.ParseNodes
{
    /// <summary>
    /// Extensions for JSON pointers.
    /// </summary>
    public static class JsonPointerExtensions
    {
        /// <summary>
        /// Finds the JSON node that corresponds to this JSON pointer based on the base Json node.
        /// </summary>
        public static JsonNode Find(this JsonPointer currentPointer, JsonNode baseJsonNode)
        {
            if (currentPointer.Tokens.Length == 0)
            {
                return baseJsonNode;
            }

            try
            {
                var pointer = baseJsonNode;
                foreach (var token in currentPointer.Tokens)
                {
                    var array = pointer as JsonArray;

                    if (array != null && int.TryParse(token, out var tokenValue))
                    {
                        pointer = array[tokenValue];
                    }
                    else if (pointer is JsonObject map && !map.TryGetPropertyValue(token, out pointer))
                    {
                        return null;
                    }
                }

                return pointer;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
