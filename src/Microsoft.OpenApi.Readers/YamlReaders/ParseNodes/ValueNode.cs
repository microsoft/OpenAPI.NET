
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.YamlReaders.ParseNodes
{
    internal class ValueNode : ParseNode
    {
        YamlScalarNode node;

        public ValueNode(ParsingContext context, OpenApiDiagnostic log, YamlScalarNode scalarNode) : base(context, log)
        {
            this.node = scalarNode;
        }

        public override string GetScalarValue()
        {
            var scalarNode = this.node;

            if (scalarNode == null) throw new OpenApiException($"Expected scalar at line {node.Start.Line}");

            return scalarNode.Value;
        }
    }
}
