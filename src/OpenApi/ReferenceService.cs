

namespace Tavis.OpenApi
{
    using System;
    using Tavis.OpenApi.Model;

    public class ReferenceService : IReferenceService
    {
        internal Func<OpenApiReference, RootNode, IReference> loadReference { get; set; }
        internal Func<string, OpenApiReference> parseReference { get; set; }

        private RootNode rootNode;

        public ReferenceService(RootNode rootNode)
        {
            this.rootNode = rootNode;
        }
        public IReference LoadReference(OpenApiReference reference)
        {
            var referenceObject = this.loadReference(reference,this.rootNode);
            if (referenceObject == null)
            {
                throw new DomainParseException($"Cannot locate $ref {reference.ToString()}");
            }
            return referenceObject;
        }

        public OpenApiReference ParseReference(string pointer)
        {
            return this.parseReference(pointer);
        }
    }
}



