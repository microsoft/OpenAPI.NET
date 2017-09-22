using SharpYaml.Serialization;

namespace Tavis.OpenApi
{
    public class AnyNode : ParseNode
    {
        ParseNode node;

        public static AnyNode Create(string yaml)
        {
            var context = new ParsingContext();
            return new AnyNode(ParseNode.Create(context, yaml));
        }

        public AnyNode(ParseNode node) : base(node.Context)
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
