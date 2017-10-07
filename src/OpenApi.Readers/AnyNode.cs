

namespace Microsoft.OpenApi.Readers
{
    using Microsoft.OpenApi.Services;
    using Microsoft.OpenApi.Writers;

    public class AnyNode : ParseNode, IAnyNode
    {
        IParseNode node;

        public static AnyNode Create(string yaml)
        {
            var context = new ParsingContext();
            return new AnyNode(YamlHelper.Create(context, yaml));
        }

        public AnyNode(IParseNode node) : base(node.Context)
        {
            this.node = node;
        }

        public void Write(IParseNodeWriter writer)
        {
            writer.WriteValue(node.ToString());
        }

        public static void Write(IParseNodeWriter writer, AnyNode node)
        {
            node.Write(writer);
        }

        public ValueNode GetValueNode()
        {
            return node as ValueNode; 
        }
        public MapNode GetMapNode()
        {
            return node as MapNode;
        }
    }


}
