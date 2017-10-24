using SharpYaml.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Readers.YamlReaders.ParseNodes
{
    internal class ListNode : ParseNode, IEnumerable<ParseNode>
    {
        YamlSequenceNode nodeList;

        public ListNode(ParsingContext context, OpenApiDiagnostic log, YamlSequenceNode sequenceNode) : base(context, log)
        {
            nodeList = sequenceNode;
        }

        public override List<T> CreateList<T>(Func<MapNode, T> map)
        {
            var yamlSequence = nodeList as YamlSequenceNode;
            if (yamlSequence == null) throw new OpenApiException($"Expected list at line {nodeList.Start.Line} while parsing {typeof(T).Name}");

            return yamlSequence.Select(n => map(new MapNode(Context, Log, n as YamlMappingNode))).Where(i => i != null).ToList();
        }

        public override List<T> CreateSimpleList<T>(Func<ValueNode, T> map)
        {
            var yamlSequence = this.nodeList as YamlSequenceNode;
            if (yamlSequence == null) throw new OpenApiException($"Expected list at line {nodeList.Start.Line} while parsing {typeof(T).Name}");

            return yamlSequence.Select(n => map(new ValueNode(Context, Log, (YamlScalarNode)n))).ToList();
        }

        public IEnumerator<ParseNode> GetEnumerator()
        {
            return nodeList.Select(n => Create(Context, Log, n)).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }


}
