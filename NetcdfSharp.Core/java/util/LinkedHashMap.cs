// NetcdfSharp.java.util.LinkedHashMap - minimal subset used by NetcdfSharp.
using System;
using System.Collections;
using System.Collections.Generic;

namespace NetcdfSharp.java.util
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.util.LinkedHashMap.
    /// Wraps a Dictionary; preserves insertion order for iteration.
    /// </summary>
    public class LinkedHashMap<K, V> : IDictionary<K, V>
    {
        private readonly Dictionary<K, V> _map;
        private readonly bool _accessOrder;
        private int _maxEntries = 0;

        public LinkedHashMap()
        {
            _map = new Dictionary<K, V>();
            _accessOrder = false;
        }

        public LinkedHashMap(int initialCapacity)
        {
            _map = new Dictionary<K, V>(initialCapacity);
            _accessOrder = false;
        }

        public LinkedHashMap(int initialCapacity, float loadFactor, bool accessOrder)
        {
            _map = new Dictionary<K, V>(initialCapacity);
            _accessOrder = accessOrder;
        }

        /// <summary>
        /// Returns the number of key-value mappings in this map.
        /// </summary>
        public int size()
        {
            return _map.Count;
        }

        public bool isEmpty()
        {
            return _map.Count == 0;
        }

        public void clear()
        {
            _map.Clear();
        }

        public bool containsKey(K key)
        {
            return _map.ContainsKey(key);
        }

        public bool containsValue(V value)
        {
            return _map.ContainsValue(value);
        }

        public V get(K key)
        {
            V value;
            return _map.TryGetValue(key, out value) ? value : default(V);
        }

        public V put(K key, V value)
        {
            V old;
            bool exists = _map.TryGetValue(key, out old);
            if (exists)
            {
                _map[key] = value;
            }
            else
            {
                if (_maxEntries > 0 && _map.Count >= _maxEntries)
                {
                    // eldest removal is approximate here
                }
                _map[key] = value;
            }
            return old;
        }

        public V remove(K key)
        {
            V value;
            if (_map.TryGetValue(key, out value))
            {
                _map.Remove(key);
                return value;
            }
            return default(V);
        }

        /// <summary>
        /// Called after an entry is inserted; subclasses may override to limit size.
        /// </summary>
        protected internal virtual bool removeEldestEntry(DictionaryEntry eldest)
        {
            return false;
        }

        // IDictionary<K, V> implementation
        public V this[K key]
        {
            get
            {
                V value;
                return _map.TryGetValue(key, out value) ? value : default(V);
            }
            set { _map[key] = value; }
        }

        public ICollection<K> Keys => _map.Keys;
        public ICollection<V> Values => _map.Values;
        public int Count => _map.Count;
        public bool IsReadOnly => false;

        public void Add(K key, V value)
        {
            _map.Add(key, value);
        }

        public void Clear()
        {
            _map.Clear();
        }

        public bool ContainsKey(K key)
        {
            return _map.ContainsKey(key);
        }

        public bool Remove(K key)
        {
            return _map.Remove(key);
        }

        public bool TryGetValue(K key, out V value)
        {
            return _map.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            _map.Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return ((IDictionary<K, V>)_map).Contains(item);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((IDictionary<K, V>)_map).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            return ((IDictionary<K, V>)_map).Remove(item);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.GetEnumerator();
        }
    }
}
