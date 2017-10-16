using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OpenApi
{
    public class GenericOpenApiExtension : IOpenApiExtension
    {
        string node;
        public GenericOpenApiExtension(string n)
        {
            this.node = n;
        }
    }
}
