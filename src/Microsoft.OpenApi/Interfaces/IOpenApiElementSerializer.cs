using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Interfaces
{
    // Generic interface to implement element specific serialization 
    public interface IOpenApiElementSerializer<TElement> where TElement : IOpenApiElement
    {
        void Serialize(TElement element, IOpenApiWriter writer);
    }
}
