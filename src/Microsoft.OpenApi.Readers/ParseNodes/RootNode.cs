// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    /// <summary>
    /// Wrapper class around YamlDocument to isolate semantic parsing from details of Yaml DOM.
    /// </summary>
    internal class RootNode : ParseNode
    {
        private readonly YamlDocument _yamlDocument;

        public RootNode(
            ParsingContext context,
            YamlDocument yamlDocument) : base(context)
        {
            _yamlDocument = yamlDocument;
        }

        public ParseNode Find(JsonPointer referencePointer)
        {
            var yamlNode = referencePointer.Find(_yamlDocument.RootNode);
            if (yamlNode == null)
            {
                return null;
            }

            return Create(Context, yamlNode);
        }

        public MapNode GetMap()
        {
            return new(Context, (YamlMappingNode)_yamlDocument.RootNode);
        }
    }
}
