using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavis.OpenApi.Model
{
    public class Example : IReference
    {
        public string Summary { get; set; }
        public string Description { get; set; }

        public AnyNode Value { get; set; }
        public Dictionary<string, AnyNode> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get; set;
        }
     }

   

}
