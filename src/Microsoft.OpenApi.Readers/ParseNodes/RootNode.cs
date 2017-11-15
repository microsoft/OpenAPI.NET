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

        public RootNode(
            ParsingContext context,
            OpenApiDiagnostic diagnostic, 
            YamlDocument yamlDocument) : base(context, diagnostic)
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
}