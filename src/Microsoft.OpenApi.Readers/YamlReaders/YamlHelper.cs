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
        public static ParseNode Create(ParsingContext context, YamlNode node)
        {
            var listNode = node as YamlSequenceNode;
            if (listNode != null)
            {
                return new ListNode(context, listNode);
            }

            var mapNode = node as YamlMappingNode;
            if (mapNode != null)
            {
                return new MapNode(context, mapNode);
            }

            return new ValueNode(context, node as YamlScalarNode);
        }

        public static string GetScalarValue(this YamlNode node)
        {
            var scalarNode = node as YamlScalarNode;
            if (scalarNode == null)
            {
                throw new OpenApiException($"Expected scalar at line {node.Start.Line}");
            }

            return scalarNode.Value;
        }

        public static YamlNode ParseYaml(string yaml)
        {
            var reader = new StringReader(yaml);
            var yamlStream = new YamlStream();
            yamlStream.Load(reader);
            var yamlDocument = yamlStream.Documents.First();
            return yamlDocument.RootNode;
        }
    }
}