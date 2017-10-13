
namespace Microsoft.OpenApi.Readers
{
    using SharpYaml.Serialization;
    using System.IO;
    using System.Linq;

    public static class YamlHelper
    {

        
        public static ParseNode Create(ParsingContext context, YamlNode node)
        {
            var listNode = node as YamlSequenceNode;
            if (listNode != null)
            {
                return new ListNode(context, listNode);
            }
            else
            {
                var mapNode = node as YamlMappingNode;
                if (mapNode != null)
                {
                    return new MapNode(context, mapNode);
                }
                else
                {
                    return new ValueNode(context, node as YamlScalarNode);
                }
            }
        }


        public static string GetScalarValue(this YamlNode node)
        {
            var scalarNode = node as YamlScalarNode;
            if (scalarNode == null) throw new OpenApiException($"Expected scalar at line {node.Start.Line}");

            return scalarNode.Value;
        }

        public static YamlNode ParseYaml(string yaml)
        {
            var reader = new StringReader(yaml);
            var yamlStream = new YamlStream();
            yamlStream.Load(reader);
            var yamlDocument = yamlStream.Documents.First();
            return yamlDocument.RootNode;
        }

    }



}
