using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tavis.OpenApi.Model
{

    public class Header : IReference
    {
        public OpenApiReference Pointer { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Deprecated { get; set; }
        public bool AllowEmptyValue { get; set; }
        public string Style { get; set; }
        public bool Explode { get; set; }
        public bool AllowReserved { get; set; }
        public Schema Schema { get; set; }
        public AnyNode Example { get; set; }
        public List<AnyNode> Examples { get; set; }
        public Dictionary<string, MediaType> Content { get; set; }

        public Dictionary<string, AnyNode> Extensions { get; set; }
        
    }
}
