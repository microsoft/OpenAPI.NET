

namespace Microsoft.OpenApi.Services
{
    using System;
    using Microsoft.OpenApi;

    public class ReferenceService : IReferenceService
    {
        public Func<OpenApiReference, object, IReference> loadReference { get; set; }
        public Func<string, OpenApiReference> parseReference { get; set; }

        private object rootNode;

        public ReferenceService(object rootNode)
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

        public static SecurityScheme LoadSecuritySchemeByReference(ParsingContext context, string schemeName)
        {

            var schemeObject = (SecurityScheme)context.GetReferencedObject(new OpenApiReference()
            {
                ReferenceType = ReferenceType.SecurityScheme,
                TypeName = schemeName
            });

            return schemeObject;
        }


        public static Tag LoadTagByReference(ParsingContext context, string tagName)
        {

            var tagObject = (Tag)context.GetReferencedObject($"#/tags/{tagName}");

            if (tagObject == null)
            {
                tagObject = new Tag() { Name = tagName };
            }
            return tagObject;
        }
    }
}



