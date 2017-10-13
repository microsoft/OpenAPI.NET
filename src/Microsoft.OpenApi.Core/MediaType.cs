
namespace Microsoft.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MediaType 
    {
        public Schema Schema { get; set; }
        public Dictionary<string,Example> Examples { get; set; }
        public string Example { get; set; }

        public Dictionary<string, string> Extensions { get; set; }


    }
}
