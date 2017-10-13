
namespace Microsoft.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Example : IReference
    {
        public string Summary { get; set; }
        public string Description { get; set; }

        public string Value { get; set; }
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        public OpenApiReference Pointer
        {
            get; set;
        }
     }

   

}
