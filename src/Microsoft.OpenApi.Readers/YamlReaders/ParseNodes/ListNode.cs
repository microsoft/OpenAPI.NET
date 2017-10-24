// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.YamlReaders.ParseNodes
{
    internal class ListNode : ParseNode, IEnumerable<ParseNode>
    {
        private readonly YamlSequenceNode nodeList;

        public ListNode(ParsingContext context, OpenApiDiagnostic diagnostic, YamlSequenceNode sequenceNode) : base(
            context,
            diagnostic)
        {
            nodeList = sequenceNode;
        }

        public override List<T> CreateList<T>(Func<MapNode, T> map)
        {
            var yamlSequence = nodeList;
            if (yamlSequence == null)
            {
                throw new OpenApiException(
                    $"Expected list at line {nodeList.Start.Line} while parsing {typeof(T).Name}");
            }

            return yamlSequence.Select(n => map(new MapNode(Context, Diagnostic, n as YamlMappingNode)))
                .Where(i => i != null)
                .ToList();
        }

        public override List<T> CreateSimpleList<T>(Func<ValueNode, T> map)
        {
            var yamlSequence = nodeList;
            if (yamlSequence == null)
            {
                throw new OpenApiException(
                    $"Expected list at line {nodeList.Start.Line} while parsing {typeof(T).Name}");
            }

            return yamlSequence.Select(n => map(new ValueNode(Context, Diagnostic, (YamlScalarNode)n))).ToList();
        }

        public IEnumerator<ParseNode> GetEnumerator()
        {
            return nodeList.Select(n => Create(Context, Diagnostic, n)).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}