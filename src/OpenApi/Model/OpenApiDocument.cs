using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace Tavis.OpenApi.Model
{
    public class OpenApiDocument 
    {
        string version;
        public string Version { get { return version; }
            set {
                if (versionRegex.IsMatch(value))
                {
                    version = value;
                } else
                {
                    throw new DomainParseException("`openapi` property does not match the required format major.minor.patch");
                }
            } } // Swagger


        public Info Info { get; set; } = new Info();
        public List<Server> Servers { get; set; } = new List<Server>();
        public List<SecurityRequirement> SecurityRequirements { get; set; }
        public Paths Paths { get; set; } = new Paths();
        public Components Components { get; set; } = new Components();
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public ExternalDocs ExternalDocs { get; set; } = new ExternalDocs();
        public Dictionary<string, AnyNode> Extensions { get; set; } = new Dictionary<string, AnyNode>();

        private static Regex versionRegex = new Regex(@"\d+\.\d+\.\d+");


        
        internal void Diff(List<OpenApiDifference> diffs, OpenApiDocument source)
        {
            // need some kind of context object for tracking where the diffs are found

            if (this.Version != source.Version)
            {

            }
        }

        public void Save(Stream stream, IOpenApiWriter openApiWriter = null)
        {
            if (openApiWriter == null)
            {
                openApiWriter = new OpenApiV3Writer();
            }
            openApiWriter.Write(stream, this);
        }
    }


}
