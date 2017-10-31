// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Generic dictionary type for Open API dictionary element.
    /// </summary>
    /// <typeparam name="T">The Open API element, <see cref="IOpenApiElement"/></typeparam>
    public abstract class OpenApiDictionaryElement<T> : OpenApiElement, IDictionary<string, T>
        where T : IOpenApiElement
    {
        public IDictionary<string, T> Items { get; set; } = new Dictionary<string, T>();

        public T this[string key]
        {
            get => this.Items[key];
            set => this.Items[key] = value;
        }

        public ICollection<string> Keys => this.Items.Keys;

        public ICollection<T> Values => this.Items.Values;

        public int Count => this.Items.Count;

        public bool IsReadOnly => this.Items.IsReadOnly;

        public void Add(string key, T value)
        {
            Items.Add(key, value);
        }

        public void Add(KeyValuePair<string, T> item)
        {
            Items.Add(item);
        }

        public void Clear()
        {
            this.Items.Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return this.Items.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return this.Items.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(string key)
        {
            return this.Items.Remove(key);
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            return this.Items.Remove(item);
        }

        public bool TryGetValue(string key, out T value)
        {
            return this.Items.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }
    }
}
