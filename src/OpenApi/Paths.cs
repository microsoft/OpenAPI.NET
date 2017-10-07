using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    public class Paths 
    {
        public IDictionary<string, PathItem> PathItems { get; set; } = new Dictionary<string, PathItem>();
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();
        
        public void AddPathItem(string key, PathItem pathItem)
        {
            PathItems.Add(key, pathItem);
        }
        public PathItem GetPath(string key)
        {
            return PathItems[key];
        }
        internal void Validate(List<string> errors)
        {
            //TODO:
        }

    }
}
