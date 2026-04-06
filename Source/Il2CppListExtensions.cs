// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

/// <summary>Extension methods for <see cref="Il2CppSystem.Collections.Generic.List{T}"/>.</summary>
public static class Il2CppListExtensions
{
    /// <param name="list">The list to wrap.</param>
    /// <typeparam name="T">The type of list.</typeparam>
    extension<T>(Il2CppSystem.Collections.Generic.List<T> list)
    {
        /// <summary>Wraps <see cref="Il2CppSystem.Collections.Generic.List{T}"/> as <see cref="IList{T}"/>.</summary>
        /// <returns><see cref="Il2CppSystem.Collections.Generic.List{T}"/> as <see cref="IReadOnlyList{T}"/>.</returns>
        public IList<T> AsI() => new List<T>(list);

        /// <summary>
        /// Wraps <see cref="Il2CppSystem.Collections.Generic.List{T}"/> as <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <returns><see cref="Il2CppSystem.Collections.Generic.List{T}"/> as <see cref="IReadOnlyList{T}"/>.</returns>
        public IReadOnlyList<T> AsIReadOnly() => new List<T>(list);
    }

    /// <summary>The list wrapper for <see cref="Il2CppSystem.Collections.Generic.List{T}"/>.</summary>
    /// <typeparam name="T">The type of list.</typeparam>
    /// <param name="list">The list to wrap.</param>
    sealed class List<T>(Il2CppSystem.Collections.Generic.List<T> list) : IList<T>, IReadOnlyList<T>
    {
        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc cref="global::System.Collections.Generic.ICollection{T}.Count"/>
        public int Count => list.Count;

        /// <inheritdoc cref="global::System.Collections.Generic.IList{T}.this"/>
        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        /// <inheritdoc />
        public void Add(T item) => list.Add(item);

        /// <inheritdoc />
        public void Clear() => list.Clear();

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var a in array)
                list[arrayIndex++] = a;
        }

        /// <inheritdoc />
        public void Insert(int index, T item) => list.Insert(index, item);

        /// <inheritdoc />
        public void RemoveAt(int index) => list.RemoveAt(index);

        /// <inheritdoc />
        public bool Contains(T item) => list.Contains(item);

        /// <inheritdoc />
        public bool Remove(T item) => list.Remove(item);

        /// <inheritdoc />
        public int IndexOf(T item) => list.IndexOf(item);

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>The enumerator for <see cref="List{T}"/>.</summary>
        /// <param name="list">The <see cref="List{T}"/> to enumerate.</param>
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        sealed class Enumerator(List<T> list) : IEnumerator<T>
        {
            /// <summary>The current index.</summary>
            int _index;

            /// <inheritdoc />
            public T Current { get; private set; } = default!;

            /// <inheritdoc />
            object? IEnumerator.Current => Current;

            /// <inheritdoc />
            public void Dispose() { }

            /// <inheritdoc />
            public bool MoveNext() =>
                (uint)_index >= (uint)list.Count ? OutOfBounds() : (Current = list[_index++]) is var _;

            /// <inheritdoc />
            void IEnumerator.Reset() => (_index, Current) = (0, default!);

            /// <summary>Sets itself to be out of bounds.</summary>
            /// <returns>The value <see langword="false"/>.</returns>
            bool OutOfBounds() => !(((_index, Current) = (list.Count + 1, default!)) is var _);
        }
    }
}
