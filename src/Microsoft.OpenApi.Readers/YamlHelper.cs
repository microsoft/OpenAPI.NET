// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
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

            var scalarNode = node as JsonValue;
            if (node == null)
            {
                //throw new OpenApiException($"Expected scalar at line {node.Start.Line}");
            }

            return scalarNode.ToString();
        }
        
        public static JsonNode ParseJsonString(string yamlString)
        {
            var reader = new StringReader(yamlString);
            var yamlStream = new YamlStream();
            yamlStream.Load(reader);

            var yamlDocument = yamlStream.Documents.First();
            return yamlDocument.RootNode.ToJsonNode();
        }
    }
}
