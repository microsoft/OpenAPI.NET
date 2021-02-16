using System;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers
{
    public abstract class ReferenceOpenApiSerializer<TReferenceElement> : IOpenApiReferenceElementSerializer<TReferenceElement> where TReferenceElement : IOpenApiReferenceable
    {
        private readonly IOpenApiElementSerializer<OpenApiReference> _referenceSerializer;

        protected Func<TReferenceElement, IOpenApiWriter, bool> _predicate = (element, writer) =>
        element.Reference != null && writer.GetSettings().ReferenceInline != ReferenceInlineSetting.InlineLocalReferences;

        protected ReferenceOpenApiSerializer(IOpenApiElementSerializer<OpenApiReference> referenceSerializer)
        {
            _referenceSerializer = referenceSerializer;
        }

        public void Serialize(TReferenceElement element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (_predicate(element, writer))
            {
                _referenceSerializer.Serialize(element.Reference, writer);
                return;
            }

            SerializeWithoutReference(element, writer);
        }

        public abstract void SerializeWithoutReference(TReferenceElement element, IOpenApiWriter writer);
    }
}
