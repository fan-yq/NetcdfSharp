// Guava (NetcdfSharp.com.google.common.base) compatible helpers - minimal subset used by NetcdfSharp.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetcdfSharp.com.google.common.@base
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.base.Preconditions.
    /// </summary>
    public static class Preconditions
    {
        public static T checkNotNull<T>(T reference)
        {
            if (reference == null)
            {
                throw new NullReferenceException("null reference");
            }
            return reference;
        }

        public static void checkArgument(bool expression)
        {
            if (!expression)
            {
                throw new ArgumentException("Illegal Argument");
            }
        }

        public static void checkArgument(bool expression, string errorMessage, params object[] errorMessageArgs)
        {
            if (!expression)
            {
                throw new ArgumentException(FormatMessage(errorMessage, errorMessageArgs));
            }
        }

        public static void checkState(bool expression)
        {
            if (!expression)
            {
                throw new InvalidOperationException("Illegal State");
            }
        }

        public static void checkState(bool expression, string errorMessage, params object[] errorMessageArgs)
        {
            if (!expression)
            {
                throw new InvalidOperationException(FormatMessage(errorMessage, errorMessageArgs));
            }
        }

        private static string FormatMessage(string template, object[] args)
        {
            if (string.IsNullOrEmpty(template) || args == null || args.Length == 0)
            {
                return template;
            }
            return string.Format(template, args);
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.base.Objects.
    /// </summary>
    public static class Objects
    {
        public static bool equal(object a, object b)
        {
            return Equals(a, b);
        }

        public static bool equals(object a, object b)
        {
            return Equals(a, b);
        }

        public static int hash(params object[] objs)
        {
            return hashCode(objs);
        }

        public static T requireNonNull<T>(T obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }
            return obj;
        }

        public static int hashCode(params object[] objs)
        {
            if (objs == null)
            {
                return 0;
            }
            unchecked
            {
                int hash = 17;
                foreach (object o in objs)
                {
                    hash = hash * 31 + (o == null ? 0 : o.GetHashCode());
                }
                return hash;
            }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.base.MoreObjects.
    /// </summary>
    public static class MoreObjects
    {
        public static ToStringHelper toStringHelper(object self)
        {
            return new ToStringHelper(self != null ? self.GetType().Name : "Object");
        }

        public static ToStringHelper toStringHelper(string className)
        {
            return new ToStringHelper(className);
        }

        public sealed class ToStringHelper
        {
            private readonly string _className;
            private readonly List<string> _pairs = new List<string>();

            internal ToStringHelper(string className)
            {
                _className = className;
            }

            public ToStringHelper add(string name, object value)
            {
                _pairs.Add(name + "=" + StringValue(value));
                return this;
            }

            public ToStringHelper add(string name, object value, object defaultValue)
            {
                if (Equals(value, defaultValue))
                {
                    return this;
                }
                return add(name, value);
            }

            public ToStringHelper omitNullValues()
            {
                return this;
            }

            public override string ToString()
            {
                var sb = new StringBuilder(_className);
                sb.Append('{');
                for (int i = 0; i < _pairs.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(_pairs[i]);
                }
                sb.Append('}');
                return sb.ToString();
            }

            private static string StringValue(object value)
            {
                if (value == null)
                {
                    return "null";
                }
                if (value is System.Collections.IEnumerable seq && !(value is string))
                {
                    return "[" + string.Join(", ", seq.Cast<object>().Select(StringValue)) + "]";
                }
                if (value is string s)
                {
                    return s;
                }
                return value.ToString();
            }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.base.CharMatcher.
    /// </summary>
    public class CharMatcher
    {
        private readonly Func<char, bool> _predicate;

        private CharMatcher(Func<char, bool> predicate)
        {
            _predicate = predicate;
        }

        public static CharMatcher whitespace()
        {
            return new CharMatcher(c => char.IsWhiteSpace(c));
        }

        public static CharMatcher anyOf(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                return none();
            }
            return new CharMatcher(c => sequence.IndexOf(c) >= 0);
        }

        public static CharMatcher none()
        {
            return new CharMatcher(c => false);
        }

        public static CharMatcher any()
        {
            return new CharMatcher(c => true);
        }

        public static CharMatcher @is(char match)
        {
            return new CharMatcher(c => c == match);
        }

        public bool matches(char c)
        {
            return _predicate(c);
        }

        public string trimFrom(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                return sequence;
            }
            int start = 0;
            int end = sequence.Length;
            while (start < end && _predicate(sequence[start]))
            {
                start++;
            }
            while (end > start && _predicate(sequence[end - 1]))
            {
                end--;
            }
            return sequence.Substring(start, end - start);
        }

        public string trimLeadingFrom(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                return sequence;
            }
            int start = 0;
            while (start < sequence.Length && _predicate(sequence[start]))
            {
                start++;
            }
            return sequence.Substring(start);
        }

        public string trimTrailingFrom(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                return sequence;
            }
            int end = sequence.Length;
            while (end > 0 && _predicate(sequence[end - 1]))
            {
                end--;
            }
            return sequence.Substring(0, end);
        }

        public string retainFrom(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                return sequence;
            }
            var sb = new StringBuilder(sequence.Length);
            foreach (char c in sequence)
            {
                if (_predicate(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string removeFrom(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                return sequence;
            }
            var sb = new StringBuilder(sequence.Length);
            foreach (char c in sequence)
            {
                if (!_predicate(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string collapseFrom(string sequence, char replacement)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                return sequence;
            }
            var sb = new StringBuilder(sequence.Length);
            bool inMatch = false;
            foreach (char c in sequence)
            {
                if (_predicate(c))
                {
                    if (!inMatch)
                    {
                        sb.Append(replacement);
                        inMatch = true;
                    }
                }
                else
                {
                    sb.Append(c);
                    inMatch = false;
                }
            }
            return sb.ToString();
        }

        public string replaceFrom(string sequence, char replacement)
        {
            return collapseFrom(sequence, replacement);
        }

        public int countIn(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                return 0;
            }
            return sequence.Count(c => _predicate(c));
        }

        public CharMatcher or(CharMatcher other)
        {
            Func<char, bool> self = _predicate;
            return new CharMatcher(c => self(c) || other.matches(c));
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.base.Splitter.
    /// </summary>
    public class Splitter
    {
        private readonly Func<string, IEnumerable<string>> _splitFunc;
        private readonly bool _omitEmpty;
        private readonly bool _trim;

        private Splitter(Func<string, IEnumerable<string>> splitFunc, bool omitEmpty, bool trim)
        {
            _splitFunc = splitFunc;
            _omitEmpty = omitEmpty;
            _trim = trim;
        }

        public static Splitter on(char separator)
        {
            return new Splitter(s => s.Split(separator), false, false);
        }

        public static Splitter on(string separator)
        {
            return new Splitter(s => s.Split(new[] { separator }, StringSplitOptions.None), false, false);
        }

        public static Splitter on(CharMatcher separatorMatcher)
        {
            return new Splitter(s => SplitOnMatcher(s, separatorMatcher), false, false);
        }

        public Splitter omitEmptyStrings()
        {
            return new Splitter(_splitFunc, true, _trim);
        }

        public Splitter trimResults()
        {
            return new Splitter(_splitFunc, _omitEmpty, true);
        }

        public Splitter limit(int limit)
        {
            return this;
        }

        public IEnumerable<string> split(string sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }
            IEnumerable<string> parts = _splitFunc(sequence);
            foreach (string part in parts)
            {
                string p = part;
                if (_trim)
                {
                    p = p.Trim();
                }
                if (_omitEmpty && p.Length == 0)
                {
                    continue;
                }
                yield return p;
            }
        }

        public IList<string> splitToList(string sequence)
        {
            return split(sequence).ToList();
        }

        private static IEnumerable<string> SplitOnMatcher(string s, CharMatcher matcher)
        {
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (matcher.matches(c))
                {
                    if (sb.Length > 0)
                    {
                        yield return sb.ToString();
                        sb.Clear();
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.base.Predicate (functional interface).
    /// </summary>
    public delegate bool Predicate<T>(T input);
}
