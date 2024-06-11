// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    /// <summary>
    /// Extensions for JSON pointers.
    /// </summary>
    public static class JsonPointerExtensions
    {
        /// <summary>
        /// Finds the YAML node that corresponds to this JSON pointer based on the base YAML node.
        /// </summary>
        public static YamlNode Find(this JsonPointer currentPointer, YamlNode baseYamlNode)
        {
            if (currentPointer.Tokens.Length == 0)
            {
                return baseYamlNode;
            }

            var pointer = baseYamlNode;
            foreach (var token in currentPointer.Tokens)
            {
                if (pointer is YamlSequenceNode sequence && sequence.Children is not null && int.TryParse(token, out var index) && index < sequence.Children.Count)
                {
                    pointer = sequence.Children[index];
                }
                else if (pointer is YamlMappingNode map && map.Children is not null && !map.Children.TryGetValue(new YamlScalarNode(token), out pointer))
                {
                    return null;
                }
            }

            return pointer;
        }
    }
}
