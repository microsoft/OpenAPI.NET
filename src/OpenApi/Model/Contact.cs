using System;
using System.Collections.Generic;
using SharpYaml.Serialization;

namespace Tavis.OpenApi.Model
{
    public class Contact 
    {
        public string Name { get; set; }
        public Uri Url { get; set; }
        public string Email { get; set; }
        public Dictionary<string, AnyNode> Extensions { get; set; } = new Dictionary<string, AnyNode>();

        
    }
}