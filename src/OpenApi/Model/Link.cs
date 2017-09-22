using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavis.OpenApi.Model
{
    public class Link :  IReference
    {
        public string Href { get; set; }
        public string OperationId { get; set; }
        public Dictionary<string, RuntimeExpression> Parameters { get; set; }
        public RuntimeExpression RequestBody { get; set; }

        public string Description { get; set; }
        public Dictionary<string, string> Extensions { get; set; }

        public OpenApiReference Pointer { get; set; }


        
    }
}
