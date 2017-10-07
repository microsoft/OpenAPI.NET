

namespace Microsoft.OpenApi
{
    using System;
    using System.Collections.Generic;

    public class License 
    {
        public string Name { get; set; }
        public Uri Url { get; set; }
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();
        

        
    }
}