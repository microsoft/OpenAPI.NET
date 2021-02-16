using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Interfaces
{
    // Generic interface to implement element specific serialization 
    public interface IOpenApiReferenceElementSerializer<TElement> : IOpenApiElementSerializer<TElement> where TElement : IOpenApiReferenceable
    {
        void SerializeWithoutReference(TElement element, IOpenApiWriter writer);
    }
}
