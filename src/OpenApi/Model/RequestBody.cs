using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavis.OpenApi.Model
{
    public class RequestBody : IReference
    {
        public OpenApiReference Pointer { get; set; }

        public string Description { get; set; }
        public Boolean Required { get; set; }
        public Dictionary<string, MediaType> Content { get; set; }
        public Dictionary<string,string> Extensions { get; set; }
    }
}
