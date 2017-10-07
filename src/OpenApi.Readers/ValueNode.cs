
namespace Microsoft.OpenApi.Readers
{
    using Microsoft.OpenApi.Services;
    using SharpYaml.Serialization;

    public class ValueNode : ParseNode, IValueNode
    {
        YamlScalarNode node;
        public ValueNode(ParsingContext ctx, YamlScalarNode scalarNode) : base(ctx)
        {
            this.node = scalarNode;
        }

        public override string GetScalarValue()
        {

            var scalarNode = this.node as YamlScalarNode;
            if (scalarNode == null) throw new DomainParseException($"Expected scalar at line {node.Start.Line}");

            return scalarNode.Value;
        }

    }


}
