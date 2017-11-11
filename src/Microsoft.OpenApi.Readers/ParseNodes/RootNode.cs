// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    /// <summary>
    /// Wrapper class around YamlDocument to isolate semantic parsing from details of Yaml DOM.
    /// </summary>
    internal class RootNode : ParseNode
    {
        private readonly YamlDocument yamlDocument;

        public RootNode(ParsingContext context, OpenApiDiagnostic diagnostic, YamlDocument yamlDocument) : base(context, diagnostic)
        {
            this.yamlDocument = yamlDocument;
        }

        public ParseNode Find(JsonPointer refPointer)
        {
            var yamlNode = refPointer.Find(yamlDocument.RootNode);
            if (yamlNode == null)
            {
                return null;
            }

            return Create(Context, Diagnostic, yamlNode);
        }

        public MapNode GetMap()
        {
            return new MapNode(Context, Diagnostic, (YamlMappingNode)yamlDocument.RootNode);
        }
    }

    /// <summary>
    /// Extensions for JSON pointers.
    /// </summary>
    public static class JsonPointerExtensions
    {
        /// <summary>
        /// Finds the YAML node that corresponds to this JSON pointer based on the base YAML node.
        /// </summary>
        /// <param name="currentpointer"></param>
        /// <param name="baseYamlNode"></param>
        /// <returns></returns>
        public static YamlNode Find(this JsonPointer currentpointer, YamlNode baseYamlNode)
        {
            if (currentpointer.Tokens.Length == 0)
            {
                return baseYamlNode;
            }

            try
            {
                var pointer = baseYamlNode;
                foreach (var token in currentpointer.Tokens)
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
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to dereference pointer", ex);
            }
        }
    }
}