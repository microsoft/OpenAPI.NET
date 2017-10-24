// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.YamlReaders.ParseNodes
{
    /// <summary>
    /// Wrapper class around YamlDocument to isolate semantic parsing from details of Yaml DOM.
    /// </summary>
    internal class RootNode : ParseNode
    {
        private readonly YamlDocument yamlDocument;

        public RootNode(ParsingContext context, OpenApiDiagnostic log, YamlDocument yamlDocument) : base(context, log)
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

    public static class JsonPointerExtensions
    {
        public static YamlNode Find(this JsonPointer currentpointer, YamlNode sample)
        {
            if (currentpointer.Tokens.Length == 0)
            {
                return sample;
            }

            try
            {
                var pointer = sample;
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