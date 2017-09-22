using System;
using System.Collections.Generic;

namespace Tavis.OpenApi.Model
{
    public class Paths 
    {
        public IDictionary<string, PathItem> PathItems { get; set; } = new Dictionary<string, PathItem>();
        public Dictionary<string, AnyNode> Extensions { get; set; } = new Dictionary<string, AnyNode>();
        
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
