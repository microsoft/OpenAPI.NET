using SharpYaml.Serialization;
using System;
using System.Collections.Generic;

namespace Tavis.OpenApi.Model
{
    public class SecurityScheme : IReference
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string In { get; set; }
        public string Scheme { get; set; }
        public string BearerFormat { get; set; }
        public Uri OpenIdConnectUrl { get; set; }
        public string Flow { get; set; }
        public Uri AuthorizationUrl { get; set; }
        public Uri TokenUrl { get; set; }
        public Dictionary<string,string> Scopes { get; set; }

        public Dictionary<string, AnyNode> Extensions { get; set; } = new Dictionary<string, AnyNode>();

        public OpenApiReference Pointer
        {
            get; set;
        }

        internal static SecurityScheme LoadByReference(ParseNode node)
        {
            var schemeName = node.GetScalarValue();
            var context = node.Context;
            var schemeObject = (SecurityScheme)context.GetReferencedObject(new OpenApiReference()
            {
                ReferenceType = ReferenceType.SecurityScheme,
                TypeName = schemeName
            });

            return schemeObject;
        }

    }
}