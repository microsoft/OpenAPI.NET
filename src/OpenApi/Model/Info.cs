using System.Collections.Generic;
using SharpYaml.Serialization;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tavis.OpenApi.Model
{

    public class Info 
    {
        public string Title { get; set; } = "[Title Required]";
        public string Description { get; set; }
        public string TermsOfService
        {
            get { return this.termsOfService; }
            set
            {
                if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
                {
                    throw new DomainParseException("`info.termsOfService` MUST be a URL");
                };
                this.termsOfService = value;
            }
        }
        string termsOfService;
        public Contact Contact { get; set; }
        public License License { get; set; }
        public string Version { get; set; } = "1.0";
        public Dictionary<string, AnyNode> Extensions { get; set; } = new Dictionary<string, AnyNode>();

        private static Regex versionRegex = new Regex(@"\d+\.\d+\.\d+");
        
        
    }

}
