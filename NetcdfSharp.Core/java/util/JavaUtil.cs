// NetcdfSharp.java.util.StringTokenizer / Collections / Formatter compatible - minimal subset used by NetcdfSharp.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetcdfSharp.java.util
{
    /// <summary>Thrown when an element is requested but none is present.</summary>
    public class NoSuchElementException : System.Exception
    {
        public NoSuchElementException() { }
        public NoSuchElementException(string message) : base(message) { }
    }
    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.util.StringTokenizer.
    /// </summary>
    public class StringTokenizer
    {
        private readonly string[] _tokens;
        private int _index;

        public StringTokenizer(string str, string delim = " \t\n\r\f", bool returnDelims = false)
        {
            if (string.IsNullOrEmpty(str))
            {
                _tokens = new string[0];
                return;
            }
            if (returnDelims)
            {
                var parts = new List<string>();
                var sb = new StringBuilder();
                foreach (char c in str)
                {
                    if (delim.IndexOf(c) >= 0)
                    {
                        if (sb.Length > 0) { parts.Add(sb.ToString()); sb.Clear(); }
                        parts.Add(c.ToString());
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                if (sb.Length > 0) parts.Add(sb.ToString());
                _tokens = parts.ToArray();
            }
            else
            {
                _tokens = str.Split(delim.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public bool hasMoreTokens() => _index < _tokens.Length;

        public bool hasMoreElements() => hasMoreTokens();

        public string nextToken()
        {
            if (_index >= _tokens.Length) throw new InvalidOperationException("No more tokens");
            return _tokens[_index++];
        }

        public string nextToken(string delim)
        {
            // re-split remaining
            var rest = string.Join(" ", _tokens.Skip(_index));
            return new StringTokenizer(rest, delim).nextToken();
        }

        public object nextElement() => nextToken();

        public int countTokens() => _tokens.Length - _index;
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.util.Collections static helpers.
    /// </summary>
    public static class Collections
    {
        public static void sort<T>(IList<T> list) where T : IComparable<T>
        {
            var arr = list.ToArray();
            System.Array.Sort(arr);
            for (int i = 0; i < arr.Length; i++) list[i] = arr[i];
        }

        public static void sort<T>(IList<T> list, IComparer<T> comparer)
        {
            var arr = list.ToArray();
            System.Array.Sort(arr, comparer);
            for (int i = 0; i < arr.Length; i++) list[i] = arr[i];
        }

        public static void addAll<T>(ICollection<T> target, params T[] items)
        {
            foreach (var item in items) target.Add(item);
        }

        public static void addAll<T>(ICollection<T> target, IEnumerable<T> items)
        {
            foreach (var item in items) target.Add(item);
        }

        public static List<T> singletonList<T>(T item)
        {
            return new List<T> { item };
        }

        public static IList<T> synchronizedList<T>(IList<T> list)
        {
            return list;
        }

        public static T max<T>(IEnumerable<T> collection) where T : IComparable<T>
        {
            return collection.Max();
        }

        public static T min<T>(IEnumerable<T> collection) where T : IComparable<T>
        {
            return collection.Min();
        }

        public static void reverse<T>(IList<T> list)
        {
            for (int i = 0, j = list.Count - 1; i < j; i++, j--)
            {
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static T[] emptyList<T>() => new T[0];

        /// <summary>
        /// Java Collections.binarySearch semantics on a sorted list;
        /// returns the index of the key, or -(insertion point) - 1 if not found.
        /// </summary>
        public static int binarySearch<T>(IList<T> list, object key) where T : IComparable
        {
            int low = 0;
            int high = list.Count - 1;
            while (low <= high)
            {
                int mid = (low + high) >> 1;
                int cmp = list[mid].CompareTo((T)key);
                if (cmp < 0)
                {
                    low = mid + 1;
                }
                else if (cmp > 0)
                {
                    high = mid - 1;
                }
                else
                {
                    return mid;
                }
            }
            return -(low + 1);
        }

        public static IDictionary<K, V> unmodifiableMap<K, V>(IDictionary<K, V> m)
        {
            return new Dictionary<K, V>(m);
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.util.Formatter.
    /// </summary>
    public class Formatter : IDisposable
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public Formatter()
        {
        }

        public Formatter(StringBuilder sb)
        {
            _sb.Append(sb.ToString());
        }

        public Formatter format(string format, params object[] args)
        {
            _sb.Append(NetcdfSharp.java.io.FormatHelper.Format(format, args));
            return this;
        }

        public Formatter printf(string format, params object[] args)
        {
            return this.format(format, args);
        }

        public void flush()
        {
        }

        public void close()
        {
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
