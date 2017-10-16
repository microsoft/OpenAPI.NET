using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace Microsoft.OpenApi
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
                    throw new OpenApiException("`openapi` property does not match the required format major.minor.patch");
                }
            } } // Swagger


        public Info Info { get; set; } = new Info();
        public List<Server> Servers { get; set; } = new List<Server>();
        public List<SecurityRequirement> SecurityRequirements { get; set; }
        public Paths Paths { get; set; } = new Paths();
        public Components Components { get; set; } = new Components();
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public ExternalDocs ExternalDocs { get; set; } = new ExternalDocs();
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        private static Regex versionRegex = new Regex(@"\d+\.\d+\.\d+");

        public void CreatePath(string key, Action<PathItem> configure)
        {
            var pathItem = new PathItem();
            configure(pathItem);
            Paths.Add(key, pathItem);
        }

    }
}
