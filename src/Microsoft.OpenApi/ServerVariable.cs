

namespace Microsoft.OpenApi
{
    using System.Collections.Generic;

    public class ServerVariable 
    {
        public string Description { get; set; }
        public string Default { get; set; }
        public List<string> Enum { get; set; } = new List<string>();

        public Dictionary<string, string> Extensions { get; set; } = new Dictionary<string, string>();

        

    }
}
