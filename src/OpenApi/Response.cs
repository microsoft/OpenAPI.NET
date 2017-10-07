
namespace Microsoft.OpenApi
{
    using System;
    using System.Collections.Generic;

    public class Response : IReference
    {

        public string Description { get; set; }
        public Dictionary<string, MediaType> Content { get; set; }
        public Dictionary<string, Header> Headers { get; set; }
        public Dictionary<string, Link> Links { get; set; }
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get; set;
        }


    }
}