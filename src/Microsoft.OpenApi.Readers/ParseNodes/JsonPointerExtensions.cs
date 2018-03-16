// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
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

            try
            {
                var pointer = baseYamlNode;
                foreach (var token in currentPointer.Tokens)
                {
                    var sequence = pointer as YamlSequenceNode;

                    if (sequence != null)
                    {
                        pointer = sequence.Children[Convert.ToInt32(token)];
                    }
                    else
                    {
                        var map = pointer as YamlMappingNode;
                        if (map != null)
                        {
                            if (!map.Children.TryGetValue(new YamlScalarNode(token), out pointer))
                            {
                                return null;
                            }
                        }
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