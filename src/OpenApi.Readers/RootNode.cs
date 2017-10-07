
namespace Microsoft.OpenApi.Readers
{
    using SharpYaml.Serialization;
    using System;

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

        public ParseNode Find(JsonPointer refPointer)
        {
            var yamlNode = refPointer.Find(this.yamlDocument.RootNode);
            if (yamlNode == null) return null;
            return YamlHelper.Create(this.Context, yamlNode);
        }
    }

    public static class JsonPointerExtensions
    {

        public static YamlNode Find(this JsonPointer currentpointer, YamlNode sample)
        {
            if (currentpointer.Tokens.Length == 0)
            {
                return sample;
            }
            try
            {
                var pointer = sample;
                foreach (var token in currentpointer.Tokens)
                {
                    var sequence = pointer as YamlSequenceNode;
                    if (sequence != null)
                    {
                        pointer = sequence.Children[Convert.ToInt32(token)];
                    }
                    else
                    {
                        var map = pointer as YamlMappingNode;
                        if (map != null)
                        {
                            if (!map.Children.TryGetValue(new YamlScalarNode(token), out pointer))
                            {
                                return null;
                            }
                        }
                    }
                }
                return pointer;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to dereference pointer", ex);
            }
        }

    }
}
