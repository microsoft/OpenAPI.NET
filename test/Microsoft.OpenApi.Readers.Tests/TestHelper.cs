// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using Microsoft.OpenApi.Readers.ParseNodes;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.Tests
{
    internal class TestHelper
    {
        public static MapNode CreateYamlMapNode(Stream stream)
        {
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            return new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);
        }
    }
}