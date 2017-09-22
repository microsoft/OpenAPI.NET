using System;
using SharpYaml.Serialization;
using System.Collections.Generic;

namespace Tavis.OpenApi.Model
{

    public class Response : IReference
    {

        public string Description { get; set; }
        public Dictionary<string, MediaType> Content { get; set; }
        public Dictionary<string, Header> Headers { get; set; }
        public Dictionary<string, Link> Links { get; set; }
        public Dictionary<string, AnyNode> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get; set;
        }


    }
}