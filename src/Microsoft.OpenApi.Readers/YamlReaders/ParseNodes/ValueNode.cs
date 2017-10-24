// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.YamlReaders.ParseNodes
{
    internal class ValueNode : ParseNode
    {
        private readonly YamlScalarNode node;

        public ValueNode(ParsingContext context, OpenApiDiagnostic diagnostic, YamlScalarNode scalarNode) : base(context, diagnostic)
        {
            node = scalarNode;
        }

        public override string GetScalarValue()
        {
            var scalarNode = node;

            if (scalarNode == null)
            {
                throw new OpenApiException($"Expected scalar at line {node.Start.Line}");
            }

            return scalarNode.Value;
        }
    }
}