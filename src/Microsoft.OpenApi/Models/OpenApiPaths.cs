// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Paths object.
    /// </summary>
    public class OpenApiPaths : IDictionary<string, OpenApiPathItem>, IOpenApiExtension
    {
        public OpenApiPathItem this[string key] { get => this.PathItems[key]; set => this.PathItems[key] = value; }

        private IDictionary<string, OpenApiPathItem> PathItems { get; set; } = new Dictionary<string, OpenApiPathItem>();
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public ICollection<string> Keys => this.PathItems.Keys;

        public ICollection<OpenApiPathItem> Values => this.PathItems.Values;

        public int Count => this.PathItems.Count;

        public bool IsReadOnly => this.PathItems.IsReadOnly;

        public void Add(string key, OpenApiPathItem value)
        {
            PathItems.Add(key, value); 
        }

        public void Add(KeyValuePair<string, OpenApiPathItem> item)
        {
            PathItems.Add(item);
        }
        
        public void Clear()
        {
            this.PathItems.Clear();
        }

        public bool Contains(KeyValuePair<string, OpenApiPathItem> item)
        {
            return this.PathItems.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return this.PathItems.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, OpenApiPathItem>[] array, int arrayIndex)
        {
            this.PathItems.CopyTo(array, arrayIndex);
        }

        
        public bool Remove(string key)
        {
            return this.PathItems.Remove(key);
        }

        public bool Remove(KeyValuePair<string, OpenApiPathItem> item)
        {
            return this.PathItems.Remove(item);
        }

        public bool TryGetValue(string key, out OpenApiPathItem value)
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

        IEnumerator<KeyValuePair<string, OpenApiPathItem>> IEnumerable<KeyValuePair<string, OpenApiPathItem>>.GetEnumerator()
        {
            return this.PathItems.GetEnumerator();
        }
    }
}
