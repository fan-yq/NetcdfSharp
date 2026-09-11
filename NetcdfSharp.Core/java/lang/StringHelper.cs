// StringHelper helper: Java new String(byte[], Charset) compat, now backed by System.Text.Encoding.
using System;
using System.Text;

namespace NetcdfSharp.java.lang
{
    /// <summary>
    /// Static helpers mirroring Java's new String(byte[], Charset) constructors.
    /// </summary>
    public static class StringHelper
    {
        public static string NewString(byte[] bytes, Encoding charset)
        {
            return (charset ?? Encoding.UTF8).GetString(bytes);
        }

        public static string NewString(byte[] bytes, int offset, int length, Encoding charset)
        {
            return (charset ?? Encoding.UTF8).GetString(bytes, offset, length);
        }

        /// <summary>Java String.replaceAll semantics (regex).</summary>
        public static string replaceAll(this string s, string regex, string replacement)
        {
            if (s == null || regex == null) return s;
            try
            {
                return System.Text.RegularExpressions.Regex.Replace(s, regex, replacement);
            }
            catch (ArgumentException)
            {
                return s.Replace(regex, replacement);
            }
        }
    }
}
