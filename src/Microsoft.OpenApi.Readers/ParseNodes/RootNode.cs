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
            OpenApiDiagnostic diagnostic,
            YamlDocument yamlDocument) : base(context, diagnostic)
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

            return Create(Context, Diagnostic, yamlNode);
        }

        public MapNode GetMap()
        {
            return new MapNode(Context, Diagnostic, (YamlMappingNode)_yamlDocument.RootNode);
        }
    }
}