using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Represents a generic dictionary that preserves the order in which keys and values are added.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary. Must be non-nullable.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : notnull
    {
        private readonly List<KeyValuePair<TKey, TValue>> _items = new();
        private readonly Dictionary<TKey, int> _indexMap;

        /// <summary>
        /// Gets the number of key-value pairs contained in the dictionary.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Gets a value indicating whether the dictionary is read-only. Always returns false.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a collection containing the keys in the dictionary, in insertion order.
        /// </summary>
        public ICollection<TKey> Keys => _items.Select(kvp => kvp.Key).ToList();

        /// <summary>
        /// Gets a collection containing the values in the dictionary, in insertion order.
        /// </summary>
        public ICollection<TValue> Values => _items.Select(kvp => kvp.Value).ToList();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class
        /// using the default equality comparer for the key type.
        /// </summary>
        public OrderedDictionary()
            : this((IEqualityComparer<TKey>?)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class
        /// using the specified equality comparer for the keys.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys,
        /// or <c>null</c> to use the default equality comparer for the type of the key.
        /// </param>
        public OrderedDictionary(IEqualityComparer<TKey>? comparer)
        {
            _indexMap = new Dictionary<TKey, int>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class
        /// by copying elements from an existing <see cref="OrderedDictionary{TKey, TValue}"/>,
        /// preserving the key comparer used by the source.
        /// </summary>
        /// <param name="source">The source <see cref="OrderedDictionary{TKey, TValue}"/> to copy from.</param>
        /// <exception cref="ArgumentNullException">Thrown when the source dictionary is <c>null</c>.</exception>
        public OrderedDictionary(OrderedDictionary<TKey, TValue> source)
            : this(source == null ? throw new ArgumentNullException(nameof(source)) : source._indexMap.Comparer)
        {
            foreach (var kvp in source)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public TValue this[TKey key]
        {
            get
            {
                if (!_indexMap.TryGetValue(key, out int index))
                    throw new KeyNotFoundException($"Key '{key}' not found.");
                return _items[index].Value;
            }
            set
            {
                if (_indexMap.TryGetValue(key, out int index))
                {
                    _items[index] = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    _items.Add(new KeyValuePair<TKey, TValue>(key, value));
                    _indexMap[key] = _items.Count - 1;
                }
            }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <exception cref="ArgumentException">Thrown when the key already exists in the dictionary.</exception>
        public void Add(TKey key, TValue value)
        {
            if (_indexMap.ContainsKey(key))
                throw new ArgumentException($"An item with the same key '{key}' already exists.");

            _items.Add(new KeyValuePair<TKey, TValue>(key, value));
            _indexMap[key] = _items.Count - 1;
        }

        /// <summary>
        /// Adds the specified key-value pair to the dictionary.
        /// </summary>
        /// <param name="item">The key-value pair to add.</param>
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        /// <summary>
        /// Determines whether the dictionary contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>true if the dictionary contains an element with the key; otherwise, false.</returns>
        public bool ContainsKey(TKey key) => _indexMap.ContainsKey(key);

        /// <summary>
        /// Determines whether the dictionary contains the specified key-value pair.
        /// </summary>
        /// <param name="item">The key-value pair to locate in the dictionary.</param>
        /// <returns>true if the key-value pair is found; otherwise, false.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) =>
            _indexMap.TryGetValue(item.Key, out int index) &&
            EqualityComparer<TValue>.Default.Equals(_items[index].Value, item.Value);

        /// <summary>
        /// Attempts to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">The value associated with the specified key, if found.</param>
        /// <returns>true if the key was found; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_indexMap.TryGetValue(key, out int index))
            {
                value = _items[index].Value;
                return true;
            }
            value = default!;
            return false;
        }

        /// <summary>
        /// Removes the value with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false.</returns>
        public bool Remove(TKey key)
        {
            if (!_indexMap.TryGetValue(key, out int index))
                return false;

            _items.RemoveAt(index);
            _indexMap.Remove(key);
            ReindexFrom(index);
            return true;
        }

        /// <summary>
        /// Removes the specified key-value pair from the dictionary.
        /// </summary>
        /// <param name="item">The key-value pair to remove.</param>
        /// <returns>true if the item was successfully removed; otherwise, false.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!Contains(item))
                return false;

            return Remove(item.Key);
        }

        /// <summary>
        /// Removes all keys and values from the dictionary.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _indexMap.Clear();
        }

        /// <summary>
        /// Copies the elements of the dictionary to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns>An enumerator for the dictionary.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Inserts a key-value pair at the specified index. If index equals Count, behaves like Add.
        /// </summary>
        /// <param name="index">The zero-based index at which the key-value pair should be inserted.</param>
        /// <param name="key">The key of the element to insert.</param>
        /// <param name="value">The value of the element to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is not in a valid range.</exception>
        /// <exception cref="ArgumentException">Thrown when the key already exists.</exception>
        public void Insert(int index, TKey key, TValue value)
        {
            if (_indexMap.ContainsKey(key))
                throw new ArgumentException($"An item with the same key '{key}' already exists.");

            if (index < 0 || index > _items.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _items.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
            ReindexFrom(index);
        }

        /// <summary>
        /// Attempts to add the specified key and value to the dictionary if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns>true if the key-value pair was added; false if the key already exists.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (_indexMap.ContainsKey(key))
                return false;
            Add(key, value);
            return true;
        }

        /// <summary>
        /// Updates the index mapping starting from the specified index.
        /// </summary>
        /// <param name="startIndex">The starting index to begin reindexing.</param>
        private void ReindexFrom(int startIndex)
        {
            for (int i = startIndex; i < _items.Count; i++)
            {
                _indexMap[_items[i].Key] = i;
            }
        }
    }

    /// <summary>
    /// Provides extension methods for converting collections to <see cref="OrderedDictionary{TKey, TValue}"/>.
    /// </summary>
    public static class OrderedDictionaryExtensions
    {
        /// <summary>
        /// Creates an <see cref="OrderedDictionary{TKey, TValue}"/> from an <see cref="IEnumerable{T}"/>
        /// using the specified key and value selector functions.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
        /// <typeparam name="TKey">The type of keys in the resulting dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the resulting dictionary.</typeparam>
        /// <param name="source">The source sequence to convert.</param>
        /// <param name="keySelector">A function to extract the key from each element.</param>
        /// <param name="valueSelector">A function to extract the value from each element.</param>
        /// <returns>An <see cref="OrderedDictionary{TKey, TValue}"/> containing keys and values selected from the source sequence.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector)
            where TKey : notnull
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));

            var dict = new OrderedDictionary<TKey, TValue>();
            foreach (var item in source)
            {
                dict.Add(keySelector(item), valueSelector(item));
            }
            return dict;
        }
    }
}
