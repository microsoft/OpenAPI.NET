// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Generic dictionary type for Open API dictionary element.
    /// </summary>
    /// <typeparam name="T">The Open API element, <see cref="IOpenApiElement"/></typeparam>
    public abstract class OpenApiDictionary<T> : OpenApiElement, IDictionary<string, T>
        where T : IOpenApiElement
    {
        /// <summary>
        /// Items in this class stored as Dictionary.
        /// </summary>
        protected IDictionary<string, T> Items { get; set; } = new Dictionary<string, T>();

        /// <summary>
        /// Gets or sets the element with the specified string key.
        /// </summary>
        /// <param name="key">the specified string key.</param>
        /// <returns></returns>
        public T this[string key]
        {
            get => this.Items[key];
            set => this.Items[key] = value;
        }

        /// <summary>
        /// Gets a collection of the keys.
        /// </summary>
        public ICollection<string> Keys => this.Items.Keys;

        /// <summary>
        /// Gets a collection of the values.
        /// </summary>
        public ICollection<T> Values => this.Items.Values;

        /// <summary>
        /// Gets the number of elements contained.
        /// </summary>
        public int Count => this.Items.Count;

        /// <summary>
        ///  Gets a value indicating whether the collection is read-only or not.
        /// </summary>
        public bool IsReadOnly => this.Items.IsReadOnly;

        /// <summary>
        /// Adds an item into dictionary.
        /// </summary>
        /// <param name="key">The specified string key.</param>
        /// <param name="value">The specified value.</param>
        public void Add(string key, T value)
        {
            Items.Add(key, value);
        }

        /// <summary>
        /// Adds an item into dictionary.
        /// </summary>
        /// <param name="item">The specified key/value.</param>
        public void Add(KeyValuePair<string, T> item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Clear the dictionary.
        /// </summary>
        public void Clear()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific key/value.
        /// </summary>
        /// <param name="item">The specified key/value.</param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, T> item)
        {
            return this.Items.Contains(item);
        }

        /// <summary>
        /// Determines whether the dictionary contains a specific key.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return this.Items.ContainsKey(key);
        }

        /// <summary>
        /// Copies the elements to an <see cref="Array" /> starting at a particular index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied to.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return this.Items.Remove(key);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The specified key/value pair.</param>
        public bool Remove(KeyValuePair<string, T> item)
        {
            return this.Items.Remove(item);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <param name="value">The output value.</param>
        /// <returns>true if contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(string key, out T value)
        {
            return this.Items.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }
    }
}
