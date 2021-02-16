using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers
{
    public class LazyOpenApiElementSerializer<TElementSerializer, TElement> : IOpenApiElementSerializer<TElement> 
        where TElementSerializer : IOpenApiElementSerializer<TElement>
        where TElement : IOpenApiElement
    {
        private readonly IServiceProvider _serviceProvider;
        public LazyOpenApiElementSerializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Serialize(TElement element, IOpenApiWriter writer)
        {
            var serializer = _serviceProvider.GetService<TElementSerializer>();
            serializer.Serialize(element, writer);
        }
    }
}
