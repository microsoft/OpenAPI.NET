

namespace Microsoft.OpenApi.Readers
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
                throw new OpenApiException($"Cannot locate $ref {reference.ToString()}");
            }
            return referenceObject;
        }

        public OpenApiReference ParseReference(string pointer)
        {
            return this.parseReference(pointer);
        }

        public static OpenApiSecurityScheme LoadSecuritySchemeByReference(ParsingContext context, string schemeName)
        {

            var schemeObject = (OpenApiSecurityScheme)context.GetReferencedObject(new OpenApiReference()
            {
                ReferenceType = ReferenceType.SecurityScheme,
                TypeName = schemeName
            });

            return schemeObject;
        }


        public static OpenApiTag LoadTagByReference(ParsingContext context, string tagName)
        {

            var tagObject = (OpenApiTag)context.GetReferencedObject($"#/tags/{tagName}");

            if (tagObject == null)
            {
                tagObject = new OpenApiTag() { Name = tagName };
            }
            return tagObject;
        }
    }
}



