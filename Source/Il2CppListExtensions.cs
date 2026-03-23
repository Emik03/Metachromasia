// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

public static class Il2CppListExtensions
{
    public static IList<T> AsIList<T>(this Il2CppSystem.Collections.Generic.List<T> l) =>
        new List<T>(l);

    public static IReadOnlyList<T> AsIReadOnlyList<T>(this Il2CppSystem.Collections.Generic.List<T> l) =>
        new List<T>(l);

    sealed class List<T>(Il2CppSystem.Collections.Generic.List<T> list) : IList<T>,
        IReadOnlyList<T>
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

        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        // ReSharper disable NullableWarningSuppressionIsUsed
        sealed class Enumerator(List<T> list) : IEnumerator<T>
        {
            int _index;

            public T Current { get; private set; } = default!;

            object? IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext() =>
                (uint)_index >= (uint)list.Count ? OutOfBounds() : (Current = list[_index++]) is var _;

            void IEnumerator.Reset() => (_index, Current) = (0, default!);

            bool OutOfBounds() => !(((_index, Current) = (list.Count + 1, default!)) is var _);
        }
    }
}
