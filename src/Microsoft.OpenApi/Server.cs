
namespace Microsoft.OpenApi
{
    using System.Collections.Generic;

    public class Server 
    {
        public string Description { get; set; }
        public string Url { get; set; }
        public Dictionary<string, ServerVariable> Variables { get; set; } = new Dictionary<string, ServerVariable>();

        public Dictionary<string, string> Extensions { get; set; } = new Dictionary<string, string>();

        
    }

}