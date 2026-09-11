// Minimal stub for NetcdfSharp.com.google.common.collect.ImmutableListMultimap.
// Guava's read-only multimap; used by the grib2 table classes (WmoFlagTable, EccodesLocalTables,
// EccodesLocalConcepts).
using System.Collections.Generic;
using System.Linq;

namespace NetcdfSharp.com.google.common.collect
{
	public class ImmutableListMultimap<K, V>
	{
		private readonly Dictionary<K, List<V>> _map;

		internal ImmutableListMultimap(Dictionary<K, List<V>> map)
		{
			_map = map;
		}

		public ImmutableList<V> get(K key)
		{
			if (_map.TryGetValue(key, out List<V> list))
			{
				return ImmutableList<V>.copyOf(list);
			}
			return ImmutableList<V>.of();
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

		public ImmutableMap<K, ICollection<V>> asMap()
		{
			Dictionary<K, ICollection<V>> m = new Dictionary<K, ICollection<V>>();
			foreach (KeyValuePair<K, List<V>> entry in _map)
			{
				m[entry.Key] = entry.Value;
			}
			return ImmutableMap<K, ICollection<V>>.copyOf(m);
		}

		public static Builder builder()
		{
			return new Builder();
		}

		public sealed class Builder
		{
			private readonly Dictionary<K, List<V>> _map = new Dictionary<K, List<V>>();

			public Builder put(K key, V value)
			{
				if (!_map.TryGetValue(key, out List<V> list))
				{
					list = new List<V>();
					_map[key] = list;
				}
				list.Add(value);
				return this;
			}

			public ImmutableListMultimap<K, V> build()
			{
				return new ImmutableListMultimap<K, V>(_map);
			}
		}
	}
}
