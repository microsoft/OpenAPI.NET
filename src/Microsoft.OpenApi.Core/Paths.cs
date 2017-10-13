using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    public class Paths : IDictionary<string,PathItem>
    {
        public PathItem this[string key] { get => this.PathItems[key]; set => this.PathItems[key] = value; }

        private IDictionary<string, PathItem> PathItems { get; set; } = new Dictionary<string, PathItem>();
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        public ICollection<string> Keys => this.PathItems.Keys;

        public ICollection<PathItem> Values => this.PathItems.Values;

        public int Count => this.PathItems.Count;

        public bool IsReadOnly => this.PathItems.IsReadOnly;

        public void Add(string key, PathItem value)
        {
            PathItems.Add(key, value); 
        }

        public void Add(KeyValuePair<string, PathItem> item)
        {
            PathItems.Add(item);
        }
        
        public void Clear()
        {
            this.PathItems.Clear();
        }

        public bool Contains(KeyValuePair<string, PathItem> item)
        {
            return this.PathItems.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return this.PathItems.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, PathItem>[] array, int arrayIndex)
        {
            this.PathItems.CopyTo(array, arrayIndex);
        }

        
        public bool Remove(string key)
        {
            return this.PathItems.Remove(key);
        }

        public bool Remove(KeyValuePair<string, PathItem> item)
        {
            return this.PathItems.Remove(item);
        }

        public bool TryGetValue(string key, out PathItem value)
        {
            return this.PathItems.TryGetValue(key, out value);
        }

        internal void Validate(List<OpenApiException> errors)
        {
            //TODO:
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.PathItems.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, PathItem>> IEnumerable<KeyValuePair<string, PathItem>>.GetEnumerator()
        {
            return this.PathItems.GetEnumerator();
        }
    }
}
