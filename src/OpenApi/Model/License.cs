using System;
using System.Collections.Generic;

namespace Tavis.OpenApi.Model
{
    public class License 
    {
        public string Name { get; set; }
        public Uri Url { get; set; }
        public Dictionary<string, AnyNode> Extensions { get; set; } = new Dictionary<string, AnyNode>();
        

        
    }
}