using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavis.OpenApi.Model
{
    public class MediaType 
    {
        public Schema Schema { get; set; }
        public Dictionary<string,Example> Examples { get; set; }
        public AnyNode Example { get; set; }

        public Dictionary<string, string> Extensions { get; set; }


    }
}
