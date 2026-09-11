// Guava (NetcdfSharp.com.google.common.collect) compatible helpers - minimal subset used by NetcdfSharp.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NetcdfSharp.com.google.common.collect
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.collect.ImmutableList.
    /// Read-only list wrapper; fully ordered.
    /// </summary>
    public class ImmutableList<T> : IList<T>, IReadOnlyList<T>, IList
    {
        internal readonly T[] _items;

        internal ImmutableList(T[] items)
        {
            _items = items;
        }

        public int Count
        {
            get { return _items.Length; }
        }

        public int size()
        {
            return _items.Length;
        }

        public T this[int index]
        {
            get { return _items[index]; }
            set { throw new NotSupportedException("ImmutableList is read-only"); }
        }

        public T get(int index)
        {
            return _items[index];
        }

        public bool isEmpty()
        {
            return _items.Length == 0;
        }

        public bool contains(T value)
        {
            return System.Array.IndexOf(_items, value) >= 0;
        }

        public T[] ToArray()
        {
            return (T[])_items.Clone();
        }

        public IList<T> ToList()
        {
            return new List<T>(_items);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        // ---- IList<T> (read-only) ----
        public bool IsReadOnly => true;
        public int IndexOf(T item) => System.Array.IndexOf(_items, item);
        public void Insert(int index, T item) => throw new NotSupportedException("ImmutableList is read-only");
        public void RemoveAt(int index) => throw new NotSupportedException("ImmutableList is read-only");
        public void Add(T item) => throw new NotSupportedException("ImmutableList is read-only");
        public void Clear() => throw new NotSupportedException("ImmutableList is read-only");
        public bool Contains(T item) => System.Array.IndexOf(_items, item) >= 0;
        public void CopyTo(T[] array, int arrayIndex) => System.Array.Copy(_items, 0, array, arrayIndex, _items.Length);
        public bool Remove(T item) => throw new NotSupportedException("ImmutableList is read-only");

        // ---- IList (non-generic, read-only) ----
        public bool IsFixedSize => true;
        public bool IsSynchronized => false;
        public object SyncRoot => _items;
        int IList.Add(object value) => throw new NotSupportedException("ImmutableList is read-only");
        bool IList.Contains(object value) => value is T t && Contains(t);
        int IList.IndexOf(object value) => value is T t ? IndexOf(t) : -1;
        void IList.Insert(int index, object value) => throw new NotSupportedException("ImmutableList is read-only");
        bool IList.IsReadOnly => true;
        void IList.Remove(object value) => throw new NotSupportedException("ImmutableList is read-only");
        void IList.RemoveAt(int index) => throw new NotSupportedException("ImmutableList is read-only");
        void IList.Clear() => throw new NotSupportedException("ImmutableList is read-only");
        object IList.this[int index]
        {
            get { return _items[index]; }
            set { throw new NotSupportedException("ImmutableList is read-only"); }
        }
        void ICollection.CopyTo(Array array, int index) => System.Array.Copy(_items, 0, array, index, _items.Length);

        public static ImmutableList<T> of()
        {
            return new ImmutableList<T>(new T[0]);
        }

        public static ImmutableList<T> of(params T[] elements)
        {
            if (elements == null)
            {
                return of();
            }
            var copy = new T[elements.Length];
            System.Array.Copy(elements, copy, elements.Length);
            return new ImmutableList<T>(copy);
        }

        public static ImmutableList<T> copyOf(IEnumerable<T> elements)
        {
            if (elements == null)
            {
                return of();
            }
            return new ImmutableList<T>(elements.ToArray());
        }

        public static ImmutableList<T> sortedCopyOf(IEnumerable<T> elements)
        {
            if (elements == null)
            {
                return of();
            }
            var list = elements.ToList();
            list.Sort(Comparer<T>.Default);
            return new ImmutableList<T>(list.ToArray());
        }

        public static Builder builder()
        {
            return new Builder();
        }

        /// <summary>
        /// Returns a marker that can be passed to a (missing) stream .collect() call.
        /// </summary>
        public static ImmutableList<T> toImmutableList()
        {
            return of();
        }

        public sealed class Builder
        {
            private readonly List<T> _list = new List<T>();

            public Builder add(T element)
            {
                _list.Add(element);
                return this;
            }

            public Builder addAll(IEnumerable<T> elements)
            {
                if (elements != null)
                {
                    _list.AddRange(elements);
                }
                return this;
            }

            public ImmutableList<T> build()
            {
                return new ImmutableList<T>(_list.ToArray());
            }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.collect.ImmutableSet.
    /// Read-only set wrapper (no duplicate elements).
    /// </summary>
    public class ImmutableSet<T> : IEnumerable<T>
    {
        private readonly HashSet<T> _set;

        internal ImmutableSet(HashSet<T> set)
        {
            _set = set;
        }

        public int Count
        {
            get { return _set.Count; }
        }

        public int size()
        {
            return _set.Count;
        }

        public bool contains(T value)
        {
            return _set.Contains(value);
        }

        public T[] ToArray()
        {
            return _set.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        public static ImmutableSet<T> of()
        {
            return new ImmutableSet<T>(new HashSet<T>());
        }

        public static ImmutableSet<T> of(params T[] elements)
        {
            var set = new HashSet<T>();
            if (elements != null)
            {
                foreach (T e in elements)
                {
                    set.Add(e);
                }
            }
            return new ImmutableSet<T>(set);
        }

        public static ImmutableSet<T> copyOf(IEnumerable<T> elements)
        {
            var set = new HashSet<T>();
            if (elements != null)
            {
                foreach (T e in elements)
                {
                    set.Add(e);
                }
            }
            return new ImmutableSet<T>(set);
        }

        public static Builder builder()
        {
            return new Builder();
        }

        public sealed class Builder
        {
            private readonly HashSet<T> _set = new HashSet<T>();

            public Builder add(T element)
            {
                _set.Add(element);
                return this;
            }

            public Builder addAll(IEnumerable<T> elements)
            {
                if (elements != null)
                {
                    foreach (T e in elements)
                    {
                        _set.Add(e);
                    }
                }
                return this;
            }

            public ImmutableSet<T> build()
            {
                return new ImmutableSet<T>(new HashSet<T>(_set));
            }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.collect.ImmutableMap.
    /// Read-only map wrapper.
    /// </summary>
    public class ImmutableMap<K, V>
    {
        private readonly Dictionary<K, V> _map;

        internal ImmutableMap(Dictionary<K, V> map)
        {
            _map = map;
        }

        public int Count
        {
            get { return _map.Count; }
        }

        public int size()
        {
            return _map.Count;
        }

        public V this[K key]
        {
            get { return get(key); }
        }

        /// <summary>
        /// Java ImmutableMap.get(Object) semantics: returns default value (null) when absent.
        /// </summary>
        public V get(object key)
        {
            if (key == null)
            {
                return default(V);
            }
            if (key is K k && _map.TryGetValue(k, out V value))
            {
                return value;
            }
            return default(V);
        }

        public bool containsKey(object key)
        {
            return key is K k && _map.ContainsKey(k);
        }

        public bool containsValue(V value)
        {
            return _map.ContainsValue(value);
        }

        public IEnumerable<V> Values
        {
            get { return _map.Values; }
        }

        public IEnumerable<K> keySet()
        {
            return _map.Keys;
        }

        public IEnumerable<KeyValuePair<K, V>> entrySet()
        {
            return _map;
        }

        public static ImmutableMap<K, V> of()
        {
            return new ImmutableMap<K, V>(new Dictionary<K, V>());
        }

        public static ImmutableMap<K, V> of(K k1, V v1)
        {
            return new ImmutableMap<K, V>(new Dictionary<K, V> { { k1, v1 } });
        }

        public static ImmutableMap<K, V> copyOf(IDictionary<K, V> map)
        {
            if (map == null)
            {
                return of();
            }
            return new ImmutableMap<K, V>(new Dictionary<K, V>(map));
        }

        public static Builder builder()
        {
            return new Builder();
        }

        public sealed class Builder
        {
            private readonly Dictionary<K, V> _map = new Dictionary<K, V>();

            public Builder put(K key, V value)
            {
                _map[key] = value;
                return this;
            }

            public ImmutableMap<K, V> build()
            {
                return new ImmutableMap<K, V>(new Dictionary<K, V>(_map));
            }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.collect.Lists static helpers.
    /// </summary>
    public static class Lists
    {
        public static IList<T> newArrayList<T>(params T[] elements)
        {
            return elements == null ? new List<T>() : new List<T>(elements);
        }

        public static IList<T> newArrayList<T>(IEnumerable<T> elements)
        {
            return elements == null ? new List<T>() : new List<T>(elements);
        }

        public static IList<T> newArrayList<T>(System.Collections.IEnumerable elements)
        {
            var list = new List<T>();
            if (elements != null)
            {
                foreach (object o in elements)
                {
                    list.Add((T)o);
                }
            }
            return list;
        }

        public static IList<T> newArrayList<T>(int size)
        {
            return new List<T>(size);
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.collect.Iterables static helpers.
    /// </summary>
    public static class Iterables
    {
        public static bool isEmpty(IEnumerable items)
        {
            if (items == null)
            {
                return true;
            }
            IEnumerator e = items.GetEnumerator();
            try
            {
                return !e.MoveNext();
            }
            finally
            {
                (e as IDisposable)?.Dispose();
            }
        }

        public static int size(IEnumerable items)
        {
            if (items == null)
            {
                return 0;
            }
            if (items is ICollection c)
            {
                return c.Count;
            }
            int n = 0;
            foreach (object o in items)
            {
                n++;
            }
            return n;
        }

        public static T get<T>(IEnumerable<T> iterable, int position)
        {
            int index = 0;
            foreach (T item in iterable)
            {
                if (index++ == position)
                {
                    return item;
                }
            }
            throw new ArgumentOutOfRangeException("position " + position + " beyond size");
        }

        public static bool addAll<T>(ICollection<T> addTo, IEnumerable<T> elements)
        {
            bool changed = false;
            foreach (T item in elements)
            {
                addTo.Add(item);
                changed = true;
            }
            return changed;
        }

        public static T[] toArray<T>(IEnumerable<T> iterable, Type type)
        {
            return iterable.ToArray();
        }

        public static IEnumerable<T> filter<T>(IEnumerable<T> unfiltered, System.Predicate<T> predicate)
        {
            return unfiltered.Where(item => predicate(item));
        }
    }
}
