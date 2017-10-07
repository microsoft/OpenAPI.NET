using Microsoft.OpenApi.Services;
using SharpYaml.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Readers
{
    public class ListNode : ParseNode, IEnumerable<ParseNode>, IListNode
    {
        YamlSequenceNode nodeList;
        ParsingContext context;
        public ListNode(ParsingContext ctx, YamlSequenceNode sequenceNode) : base(ctx)
        {
            this.context = ctx;
            nodeList = sequenceNode;
        }

        public override List<T> CreateList<T>(Func<IMapNode, T> map)
        {
            var yamlSequence = nodeList as YamlSequenceNode;
            if (yamlSequence == null) throw new DomainParseException($"Expected list at line {nodeList.Start.Line} while parsing {typeof(T).Name}");

            return yamlSequence.Select(n => map(new MapNode(this.context,n as YamlMappingNode))).Where(i => i != null).ToList();
        }

        public override List<T> CreateSimpleList<T>(Func<IValueNode, T> map)
        {
            var yamlSequence = this.nodeList as YamlSequenceNode;
            if (yamlSequence == null) throw new DomainParseException($"Expected list at line {nodeList.Start.Line} while parsing {typeof(T).Name}");

            return yamlSequence.Select(n => map(new ValueNode(this.Context,(YamlScalarNode)n))).ToList();
        }

        public IEnumerator<ParseNode> GetEnumerator()
        {
            return nodeList.Select(n => YamlHelper.Create(this.Context,n)).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }


}
