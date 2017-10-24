using System;
using Microsoft.OpenApi;

namespace Microsoft.OpenApi.Readers.YamlReaders
{
    internal class ReferenceService : IOpenApiReferenceService
    {
        public Func<OpenApiReference, object, IOpenApiReference> loadReference { get; set; }
        public Func<string, OpenApiReference> parseReference { get; set; }

        private object rootNode;

        public ReferenceService(object rootNode)
        {
            this.rootNode = rootNode;
        }
        public IOpenApiReference LoadReference(OpenApiReference reference)
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

        public static OpenApiSecurityScheme LoadSecuritySchemeByReference(ParsingContext context, OpenApiDiagnostic log, string schemeName)
        {
            var securitySchemeObject = (OpenApiSecurityScheme)context.GetReferencedObject(
                log, 
                new OpenApiReference()
                {
                    ReferenceType = ReferenceType.SecurityScheme,
                    TypeName = schemeName
                });

            return securitySchemeObject;
        }


        public static OpenApiTag LoadTagByReference(ParsingContext context, OpenApiDiagnostic log, string tagName)
        {

            var tagObject = (OpenApiTag)context.GetReferencedObject(log, $"#/tags/{tagName}");

            if (tagObject == null)
            {
                tagObject = new OpenApiTag() { Name = tagName };
            }
            return tagObject;
        }
    }
}



