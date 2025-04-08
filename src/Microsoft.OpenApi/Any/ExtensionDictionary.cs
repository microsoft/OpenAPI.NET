using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// 
    /// </summary>
    public class ExtensionDictionary : IDictionary<string, IOpenApiExtension>
    {
        private readonly Dictionary<string, IOpenApiExtension> _inner = new();

        /// <summary>
        /// Allow simplified usage via object indexer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IOpenApiExtension this[string key]
        {
            get => _inner[key];
            set => _inner[key] = ConvertToExtension(value);
        }

        /// <summary>
        /// Allow setting JsonNode directly
        /// </summary>
        /// <param name="key"></param>
        /// <param name="node"></param>
        public void Set(string key, JsonNode node)
        {
            _inner[key] = new OpenApiAny(node);
        }

        /// <summary>
        /// Allow simplified usage via object indexer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public object this[string key, JsonNode node]
        {
            set => Set(key, node);
        }

        private static IOpenApiExtension ConvertToExtension(IOpenApiExtension extension)
        {
            // Just in case you want to extend in future to convert primitives
            return extension;
        }

        /// <summary>
        /// Core IDictionary implementation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, IOpenApiExtension value) => _inner.Add(key, ConvertToExtension(value));

        /// <summary>
        /// Checks whether a key exists in the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key) => _inner.ContainsKey(key);

        /// <summary>
        /// Remove the extension value for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key) => _inner.Remove(key);
        /// <summary>
        /// Get the extension value for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out IOpenApiExtension value) => _inner.TryGetValue(key, out value);

        /// <summary>
        /// Get the keys in the dictionary.
        /// </summary>
        public ICollection<string> Keys => _inner.Keys;

        /// <summary>
        /// Get the values in the dictionary.
        /// </summary>
        public ICollection<IOpenApiExtension> Values => _inner.Values;

        /// <summary>
        /// Get the number of items in the dictionary.
        /// </summary>
        public int Count => _inner.Count;

        /// <summary>
        /// Checks if the dictionary is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Add an item to the dictionary.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<string, IOpenApiExtension> item) => Add(item.Key, item.Value);

        /// <summary>
        /// Clears the dictionary.
        /// </summary>
        public void Clear() => _inner.Clear();

        /// <summary>
        /// Checks if the dictionary contains a key-value pair.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, IOpenApiExtension> item) => _inner.Contains(item);
        public void CopyTo(KeyValuePair<string, IOpenApiExtension>[] array, int arrayIndex) =>
            ((IDictionary<string, IOpenApiExtension>)_inner).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<string, IOpenApiExtension> item) => _inner.Remove(item.Key);
        public IEnumerator<KeyValuePair<string, IOpenApiExtension>> GetEnumerator() => _inner.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _inner.GetEnumerator();
    }

}
