
namespace Tavis.OpenApi
{
    using SharpYaml.Serialization;

    /// <summary>
    /// Wrapper class around YamlDocument to isolate semantic parsing from details of Yaml DOM.
    /// </summary>
    public class RootNode : ParseNode
    {
        YamlDocument yamlDocument;
        public RootNode(ParsingContext ctx, YamlDocument yamlDocument) : base(ctx)
        {
            this.yamlDocument = yamlDocument;
        }

        public MapNode GetMap()
        {
            return new MapNode(Context, (YamlMappingNode)yamlDocument.RootNode);
        }

        internal ParseNode Find(JsonPointer refPointer)
        {
            var yamlNode = refPointer.Find(this.yamlDocument.RootNode);
            if (yamlNode == null) return null;
            return ParseNode.Create(this.Context, yamlNode);
        }
    }


}
