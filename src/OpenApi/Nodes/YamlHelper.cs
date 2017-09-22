
namespace Tavis.OpenApi
{
    using SharpYaml.Serialization;
    using System.IO;
    using System.Linq;

    public static class YamlHelper
    {
        public static string GetScalarValue(this YamlNode node)
        {
            var scalarNode = node as YamlScalarNode;
            if (scalarNode == null) throw new DomainParseException($"Expected scalar at line {node.Start.Line}");

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
