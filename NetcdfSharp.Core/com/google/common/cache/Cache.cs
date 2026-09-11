// Guava (NetcdfSharp.com.google.common.cache) compatible helpers - minimal subset used by NetcdfSharp.
using System;
using System.Collections.Generic;

namespace NetcdfSharp.com.google.common.cache
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.cache.CacheLoader.
    /// Minimal in-memory loading cache; no expiry is performed.
    /// </summary>
    public abstract class CacheLoader<K, V>
    {
        public virtual V load(K key)
        {
            return default(V);
        }

        /// <summary>
        /// Allows a CacheLoader&lt;K,V&gt; to be passed where CacheLoader&lt;Object,Object&gt; is expected.
        /// </summary>
        public static implicit operator CacheLoader<object, object>(CacheLoader<K, V> source)
        {
            if (source == null)
            {
                return null;
            }
            return new WrapperLoader(source);
        }

        private sealed class WrapperLoader : CacheLoader<object, object>
        {
            private readonly CacheLoader<K, V> _inner;

            public WrapperLoader(CacheLoader<K, V> inner)
            {
                _inner = inner;
            }

            public override object load(object key)
            {
                if (key is K k)
                {
                    V value = _inner.load(k);
                    return value;
                }
                return default(V);
            }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.cache.Cache.
    /// </summary>
    public class Cache<K, V>
    {
        private readonly Dictionary<K, V> _map;
        private readonly object _lock = new object();

        public Cache()
        {
            _map = new Dictionary<K, V>();
        }

        internal Cache(Dictionary<K, V> map)
        {
            _map = map;
        }

        /// <summary>
        /// Bridges Cache&lt;Object,Object&gt; (from a generic builder chain) to the typed cache requested by the caller.
        /// </summary>
        public static implicit operator Cache<K, V>(Cache<object, object> source)
        {
            if (source == null)
            {
                return null;
            }
            var map = new Dictionary<K, V>();
            lock (source._lock)
            {
                foreach (KeyValuePair<object, object> entry in source._map)
                {
                    if (entry.Key is K && entry.Value is V)
                    {
                        map[(K)entry.Key] = (V)entry.Value;
                    }
                }
            }
            return new Cache<K, V>(map);
        }

        public V getIfPresent(K key)
        {
            lock (_lock)
            {
                return _map.TryGetValue(key, out V value) ? value : default(V);
            }
        }

        public virtual V get(K key)
        {
            lock (_lock)
            {
                return _map.TryGetValue(key, out V value) ? value : default(V);
            }
        }

        public void put(K key, V value)
        {
            lock (_lock)
            {
                _map[key] = value;
            }
        }

        public void putAll(IDictionary<K, V> m)
        {
            lock (_lock)
            {
                foreach (KeyValuePair<K, V> entry in m)
                {
                    _map[entry.Key] = entry.Value;
                }
            }
        }

        public bool containsKey(K key)
        {
            lock (_lock)
            {
                return _map.ContainsKey(key);
            }
        }

        public void invalidate(K key)
        {
            lock (_lock)
            {
                _map.Remove(key);
            }
        }

        public void invalidateAll()
        {
            lock (_lock)
            {
                _map.Clear();
            }
        }

        public long size()
        {
            lock (_lock)
            {
                return _map.Count;
            }
        }

        public CacheStats stats()
        {
            return new CacheStats();
        }

        public ISet<K> keySet()
        {
            lock (_lock)
            {
                return new HashSet<K>(_map.Keys);
            }
        }

        internal Dictionary<K, V> BackingMap
        {
            get { return _map; }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.cache.LoadingCache.
    /// </summary>
    public class LoadingCache<K, V> : Cache<K, V>
    {
        private readonly CacheLoader<K, V> _loader;

        public LoadingCache(CacheLoader<K, V> loader)
            : base(new Dictionary<K, V>())
        {
            _loader = loader;
        }

        internal LoadingCache(Dictionary<K, V> map, CacheLoader<K, V> loader)
            : base(map)
        {
            _loader = loader;
        }

        /// <summary>
        /// Bridges LoadingCache&lt;Object,Object&gt; (from a generic builder chain) to the typed cache requested by the caller.
        /// </summary>
        public static implicit operator LoadingCache<K, V>(LoadingCache<object, object> source)
        {
            if (source == null)
            {
                return null;
            }
            var map = new Dictionary<K, V>();
            lock (source.BackingMap)
            {
                foreach (KeyValuePair<object, object> entry in source.BackingMap)
                {
                    if (entry.Key is K && entry.Value is V)
                    {
                        map[(K)entry.Key] = (V)entry.Value;
                    }
                }
            }
            return new LoadingCache<K, V>(map, null);
        }

        public override V get(K key)
        {
            V value = getIfPresent(key);
            if (Equals(value, default(V)))
            {
                if (_loader != null)
                {
                    value = _loader.load(key);
                }
                put(key, value);
            }
            return value;
        }

        public V getUnchecked(K key)
        {
            return get(key);
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.cache.CacheBuilder.
    /// </summary>
    public class CacheBuilder<K, V>
    {
        private long _maximumSize = long.MaxValue;

        public static CacheBuilder<object, object> newBuilder()
        {
            return new CacheBuilder<object, object>();
        }

        public CacheBuilder<K, V> maximumSize(long size)
        {
            _maximumSize = size;
            return this;
        }

        public CacheBuilder<K, V> maximumWeight(long weight)
        {
            return this;
        }

        public CacheBuilder<K, V> expireAfterAccess(long duration, object unit)
        {
            return this;
        }

        public CacheBuilder<K, V> expireAfterWrite(long duration, object unit)
        {
            return this;
        }

        public CacheBuilder<K, V> expireAfterWrite(NetcdfSharp.java.time.Duration duration)
        {
            return this;
        }

        public CacheBuilder<K, V> recordStats()
        {
            return this;
        }

        public Cache<K, V> build()
        {
            return new Cache<K, V>();
        }

        public LoadingCache<K, V> build(CacheLoader<K, V> loader)
        {
            return new LoadingCache<K, V>(loader);
        }
    }

    /// <summary>
    /// Minimal cache statistics placeholder.
    /// </summary>
    public class CacheStats
    {
        public long hitCount()
        {
            return 0;
        }

        public long missCount()
        {
            return 0;
        }

        public long requestCount()
        {
            return 0;
        }

        public override string ToString()
        {
            return "CacheStats{hits=0, misses=0}";
        }
    }
}
