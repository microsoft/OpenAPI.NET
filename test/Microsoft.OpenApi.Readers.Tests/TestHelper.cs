// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
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

            var context = new ParsingContext(new OpenApiDiagnostic());
            var asJsonNode = yamlNode.ToJsonNode();

            return new MapNode(context, asJsonNode);
        }
    }
}
