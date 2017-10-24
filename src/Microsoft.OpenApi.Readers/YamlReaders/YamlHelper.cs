// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using System.Linq;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.YamlReaders
{
    internal static class YamlHelper
    {
        public static string GetScalarValue(this YamlNode node)
        {
            var scalarNode = node as YamlScalarNode;
            if (scalarNode == null)
            {
                throw new OpenApiException($"Expected scalar at line {node.Start.Line}");
            }

            return scalarNode.Value;
        }

        public static YamlNode ParseYamlString(string yamlString)
        {
            var reader = new StringReader(yamlString);
            var yamlStream = new YamlStream();
            yamlStream.Load(reader);
            var yamlDocument = yamlStream.Documents.First();
            return yamlDocument.RootNode;
        }
    }
}