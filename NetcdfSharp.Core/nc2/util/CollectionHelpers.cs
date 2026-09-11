// Extension methods bridging Java Map/Collection idioms that C# interfaces lack.
using System;
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.util
{
    /// <summary>Extension helpers for translated Java collections code.</summary>
    public static class CollectionHelpers
    {
        /// <summary>Java Map.getOrDefault semantics.</summary>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue defaultValue)
        {
            return map != null && map.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>IList<T> does not expose AddRange; NetcdfSharp.java.util.List.addAll maps here.</summary>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items) list.Add(item);
        }

        /// <summary>Java Map.entrySet() semantics.</summary>
        public static IEnumerable<KeyValuePair<TKey, TValue>> SetOfKeyValuePairs<TKey, TValue>(this IDictionary<TKey, TValue> map)
        {
            return map;
        }

        /// <summary>Java Collections.sort semantics on IList.</summary>
        public static void Sort<T>(this IList<T> list) where T : IComparable
        {
            var arr = new T[list.Count];
            list.CopyTo(arr, 0);
            System.Array.Sort(arr);
            for (int i = 0; i < arr.Length; i++) list[i] = arr[i];
        }

        /// <summary>Java Collections.sort semantics on IList with comparer.</summary>
        public static void Sort<T>(this IList<T> list, IComparer<T> comparer)
        {
            var arr = new T[list.Count];
            list.CopyTo(arr, 0);
            System.Array.Sort(arr, comparer);
            for (int i = 0; i < arr.Length; i++) list[i] = arr[i];
        }
    }
}
