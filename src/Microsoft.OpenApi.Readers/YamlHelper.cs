// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Threading;
using Microsoft.OpenApi.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers
{
    internal static class YamlHelper
    {
        public static string GetScalarValue(
            this YamlNode node,
            uint maxScalarLength = OpenApiReaderSettings.DefaultMaxScalarLength)
        {
            if (node is not YamlScalarNode scalarNode)
            {
                throw new OpenApiException(
                    $"Expected scalar at line {YamlNodeLocationRegistry.GetLine(node)}");
            }

            if (scalarNode.Value != null && scalarNode.Value.Length > maxScalarLength)
            {
                throw new OpenApiException(
                    $"The YAML scalar exceeds the maximum supported length of {maxScalarLength} characters.");
            }

            return scalarNode.Value;
        }

        public static YamlNode ParseYamlString(
            string yamlString,
            ParsingContext context = null)
        {
            var settings = new OpenApiReaderSettings();
            if (context != null)
            {
                settings.MaxDepth = context.MaxDepth;
                settings.MaxNodeCount = context.MaxNodeCount;
                settings.MaxAliasExpansionNodeCount = context.MaxAliasExpansionNodeCount;
                settings.MaxScalarLength = context.MaxScalarLength;
            }

            using var reader = new StringReader(yamlString);
            return new BoundedYamlDocumentParser(settings)
                .Parse(reader, settings.MaxInputByteCount, CancellationToken.None)
                .RootNode;
        }
    }
}
