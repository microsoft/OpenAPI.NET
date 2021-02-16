using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers
{
    public abstract class OpenApiExtensibleDictionarySerializer<TDictionary, TElement> : IOpenApiElementSerializer<TDictionary>
        where TDictionary : OpenApiExtensibleDictionary<TElement>
        where TElement : IOpenApiElement
    {
        private readonly IOpenApiSpecProvider _specProvider;

        private readonly IOpenApiElementSerializer<TElement> _elementSerializer;
        protected OpenApiExtensibleDictionarySerializer(
            IOpenApiElementSerializer<TElement> elementSerializer,
            IOpenApiSpecProvider specProvider)
        {
            _specProvider = specProvider;
            _elementSerializer = elementSerializer;
        }
        public void Serialize(TDictionary element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            foreach (var item in element)
            {
                writer.WriteRequiredObject(item.Key, item.Value, (w, p) => _elementSerializer.Serialize(p, w));
            }

            writer.WriteExtensions(element.Extensions, _specProvider.OpenApiSpec);

            writer.WriteEndObject();
        }
    }
}
