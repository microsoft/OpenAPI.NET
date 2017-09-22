using System;
using System.Collections.Generic;

namespace Tavis.OpenApi.Model
{

    public class Operation 
    {
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public string Summary { get; set; }
        public string Description { get; set; }
        public ExternalDocs ExternalDocs { get; set; } 
        public string OperationId { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
        public RequestBody RequestBody { get; set; }
        public Dictionary<string, Response> Responses { get; set; } = new Dictionary<string, Response>();
        public Dictionary<string, Callback> Callbacks { get; set; } = new Dictionary<string, Callback>();

        public const bool DeprecatedDefault = false;
        public bool Deprecated { get; set; } = DeprecatedDefault;
        public List<SecurityRequirement> Security { get; set; } = new List<SecurityRequirement>();
        public List<Server> Servers { get; set; } = new List<Server>();
        public Dictionary<string, AnyNode> Extensions { get; set; } = new Dictionary<string, AnyNode>();

    }
}