using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Represents a collection of unique elements that preserves insertion order.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    public class HashSet<T> : ICollection<T>
        where T : notnull
    {
        private readonly OrderedDictionary<T, bool> _dict;

        /// <summary>
        /// Gets the equality comparer used to determine element equality.
        /// </summary>
        public IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty 
        /// and uses the default equality comparer for the type.
        /// </summary>
        public HashSet() : this(EqualityComparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty 
        /// and uses the specified equality comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use when determining equality of elements.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is null.</exception>
        public HashSet(IEqualityComparer<T> comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _dict = new OrderedDictionary<T, bool>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that contains elements copied 
        /// from the specified collection and uses the specified equality comparer.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">The comparer to use when determining equality of elements.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="collection"/> or <paramref name="comparer"/> is null.</exception>
        public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : this(comparer)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that contains elements copied 
        /// from the specified collection and uses the default equality comparer.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="collection"/> is null.</exception>
        public HashSet(IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default)
        {
        }

        /// <inheritdoc />
        public int Count => _dict.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <summary>
        /// Attempts to add the specified element to the set.
        /// </summary>
        /// <param name="item">The element to add.</param>
        /// <returns><c>true</c> if the element was added; <c>false</c> if it already exists in the set.</returns>
        public bool Add(T item) => _dict.TryAdd(item, true);

        void ICollection<T>.Add(T item) => Add(item);

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        public void Clear() => _dict.Clear();

        /// <summary>
        /// Determines whether the set contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate.</param>
        /// <returns><c>true</c> if the set contains the element; otherwise, <c>false</c>.</returns>
        public bool Contains(T item) => _dict.ContainsKey(item);

        /// <summary>
        /// Copies the elements of the set to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">Thrown if the number of elements in the source set is greater than the available space in the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("The array is too small.");

            foreach (var key in _dict.Keys)
            {
                array[arrayIndex++] = key;
            }
        }

        /// <summary>
        /// Removes the specified element from the set.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns><c>true</c> if the element was successfully removed; <c>false</c> if it was not found.</returns>
        public bool Remove(T item) => _dict.Remove(item);

        /// <summary>
        /// Returns an enumerator that iterates through the set in insertion order.
        /// </summary>
        /// <returns>An enumerator for the set.</returns>
        public IEnumerator<T> GetEnumerator() => _dict.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
