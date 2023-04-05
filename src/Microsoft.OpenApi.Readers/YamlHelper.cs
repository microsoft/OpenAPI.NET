// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers
{
    internal static class YamlHelper
    {
        public static string GetScalarValue(this JsonNode node)
        {
            if (node == null)
            {
                //throw new OpenApiException($"Expected scalar at line {node.Start.Line}");
            }

            return node.GetValue<string>();
        }

        public static JsonObject ParseJsonString(string jsonString)
        {
            var jsonNode = JsonDocument.Parse(jsonString);
            return (JsonObject)jsonNode.Root;
        }
    }
}
