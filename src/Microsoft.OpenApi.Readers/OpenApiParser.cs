

namespace Microsoft.OpenApi.Readers
{
    using SharpYaml.Serialization;
    using System.IO;
    using System.Linq;
    

    /// <summary>
    /// Service class for converting strings or streams into OpenApiDocument instances
    /// </summary>
    public class OpenApiParser
    {
        public static ParsingContext Parse(string document)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(document);
            writer.Flush();
            ms.Position = 0;
            return Parse(ms);
        }

        public static ParsingContext Parse(Stream stream)
        {
            RootNode rootNode;
            var context = new ParsingContext();
            try
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));

                var yamlDocument = yamlStream.Documents.First();
                rootNode = new RootNode(context, yamlDocument);

            }
            catch (SharpYaml.SyntaxErrorException ex)
            {
                context.ParseErrors.Add(new OpenApiError("", ex.Message));
                context.OpenApiDocument = new OpenApiDocument();  // Could leave this null?
                return context;
            }

            var inputVersion = GetVersion(rootNode);

            switch (inputVersion)
            {
                case "2.0":
                    context.SetReferenceService(new ReferenceService(rootNode)
                    {
                        loadReference = OpenApiV2Reader.LoadReference,
                        parseReference = p => OpenApiV2Reader.ParseReference(p)
                    });
                    context.OpenApiDocument = OpenApiV2Reader.LoadOpenApi(rootNode);
                    break;
               default:
                    context.SetReferenceService(new ReferenceService(rootNode)
                    {
                        loadReference = OpenApiV3Reader.LoadReference,
                        parseReference = p => new OpenApiReference(p)
                    });
                    context.OpenApiDocument = OpenApiV3Reader.LoadOpenApi(rootNode);
                    break;
            }
            return context;
        }

        private static string GetVersion(RootNode rootNode)
        {
            var versionNode = rootNode.Find(new JsonPointer("/openapi"));
            if (versionNode != null)
            {
                return versionNode.GetScalarValue();
            }

            versionNode = rootNode.Find(new JsonPointer("/swagger"));
            if (versionNode != null)
            {
                return versionNode.GetScalarValue();
            }
            return null;
        }

    }
}



