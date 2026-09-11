// Guava (NetcdfSharp.com.google.common.collect) compatible Multimap helpers - minimal subset used by NetcdfSharp.
using System;
using System.Collections;
using System.Collections.Generic;

namespace NetcdfSharp.com.google.common.collect
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.collect.Multimap.
    /// A map that maps each key to a collection of values.
    /// </summary>
    public class Multimap<K, V>
    {
        private readonly Dictionary<K, List<V>> _map;

        public Multimap()
        {
            _map = new Dictionary<K, List<V>>();
        }

        protected Multimap(Dictionary<K, List<V>> map)
        {
            _map = map;
        }

        public int size()
        {
            int n = 0;
            foreach (List<V> list in _map.Values)
            {
                n += list.Count;
            }
            return n;
        }

        public bool isEmpty()
        {
            return _map.Count == 0;
        }

        public bool containsKey(object key)
        {
            return key is K k && _map.ContainsKey(k);
        }

        public bool containsEntry(K key, V value)
        {
            return _map.TryGetValue(key, out List<V> list) && list.Contains(value);
        }

        public bool put(K key, V value)
        {
            if (!_map.TryGetValue(key, out List<V> list))
            {
                list = new List<V>();
                _map[key] = list;
            }
            list.Add(value);
            return true;
        }

        public bool putAll(K key, IEnumerable<V> values)
        {
            if (values == null)
            {
                return false;
            }
            if (!_map.TryGetValue(key, out List<V> list))
            {
                list = new List<V>();
                _map[key] = list;
            }
            list.AddRange(values);
            return true;
        }

        public ICollection<V> get(K key)
        {
            if (_map.TryGetValue(key, out List<V> list))
            {
                return list;
            }
            return new List<V>();
        }

        public ISet<K> keySet()
        {
            return new HashSet<K>(_map.Keys);
        }

        public IEnumerable<K> keys()
        {
            foreach (KeyValuePair<K, List<V>> entry in _map)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    yield return entry.Key;
                }
            }
        }

        public IEnumerable<V> values()
        {
            foreach (List<V> list in _map.Values)
            {
                foreach (V v in list)
                {
                    yield return v;
                }
            }
        }

        public IEnumerable<KeyValuePair<K, V>> entries()
        {
            foreach (KeyValuePair<K, List<V>> entry in _map)
            {
                foreach (V v in entry.Value)
                {
                    yield return new KeyValuePair<K, V>(entry.Key, v);
                }
            }
        }

        public bool remove(K key, V value)
        {
            if (_map.TryGetValue(key, out List<V> list))
            {
                return list.Remove(value);
            }
            return false;
        }

        public ICollection<V> removeAll(K key)
        {
            if (_map.TryGetValue(key, out List<V> list))
            {
                _map.Remove(key);
                return list;
            }
            return new List<V>();
        }

        public void clear()
        {
            _map.Clear();
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.collect.ArrayListMultimap.
    /// </summary>
    public class ArrayListMultimap<K, V> : Multimap<K, V>
    {
        private ArrayListMultimap()
            : base(new Dictionary<K, List<V>>())
        {
        }

        public static ArrayListMultimap<K, V> create()
        {
            return new ArrayListMultimap<K, V>();
        }
    }
}
