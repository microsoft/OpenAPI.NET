// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.YamlReader;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.Tests
{
    internal static class TestHelper
    {
        public static MapNode CreateYamlMapNode(Stream stream)
        {
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents[0].RootNode;

            var context = new ParsingContext(new OpenApiDiagnostic());
            var asJsonNode = yamlNode.ToJsonNode();

            return new MapNode(context, asJsonNode);
        }
    }
}
