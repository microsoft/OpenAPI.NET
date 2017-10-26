using System;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.YamlReaders
{
    internal class OpenApiReferenceService : IOpenApiReferenceService
    {
        public Func<OpenApiReference, object, IOpenApiReference> loadReference { get; set; }
        public Func<string, OpenApiReference> parseReference { get; set; }

        private readonly object rootNode;

        public OpenApiReferenceService(object rootNode)
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

        public static OpenApiSecurityScheme LoadSecuritySchemeByReference(
            ParsingContext context, 
            OpenApiDiagnostic diagnostic,
            string schemeName)
        {
            var securitySchemeObject = (OpenApiSecurityScheme)context.GetReferencedObject(
                diagnostic, 
                new OpenApiReference()
                {
                    ReferenceType = ReferenceType.SecurityScheme,
                    TypeName = schemeName
                });

            return securitySchemeObject;
        }
        
        public static OpenApiTag LoadTagByReference(
            ParsingContext context, 
            OpenApiDiagnostic diagnostic,
            string tagName)
        {
            var tagObject = (OpenApiTag)context.GetReferencedObject(
                diagnostic,
                $"#/tags/{tagName}");

            if (tagObject == null)
            {
                tagObject = new OpenApiTag() { Name = tagName };
            }

            return tagObject;
        }
    }
}



