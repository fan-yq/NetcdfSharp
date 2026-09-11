// Guava (NetcdfSharp.com.google.common.net) compatible helpers - minimal subset used by NetcdfSharp.
using System;
using System.Text;
using NetcdfSharp.com.google.common.escape;

namespace NetcdfSharp.com.google.common.net
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.net.UrlEscapers.
    /// </summary>
    public static class UrlEscapers
    {
        private static readonly PercentEscaper PathSegment = new PercentEscaper("-._~!$&'()*+,;=:@", false);
        private static readonly PercentEscaper Fragment = new PercentEscaper("!$&'()*+,;=:@/?", false);
        private static readonly PercentEscaper Form = new PercentEscaper("-._*", true);

        public static Escaper urlPathSegmentEscaper()
        {
            return PathSegment;
        }

        public static Escaper urlFragmentEscaper()
        {
            return Fragment;
        }

        public static Escaper urlFormParameterEscaper()
        {
            return Form;
        }

        private sealed class PercentEscaper : Escaper
        {
            private readonly string _safeChars;
            private readonly bool _plusForSpace;

            public PercentEscaper(string safeChars, bool plusForSpace)
            {
                _safeChars = safeChars;
                _plusForSpace = plusForSpace;
            }

            public override string escape(string s)
            {
                if (string.IsNullOrEmpty(s))
                {
                    return s;
                }
                var sb = new StringBuilder(s.Length);
                foreach (char c in s)
                {
                    if (IsSafe(c))
                    {
                        sb.Append(c);
                    }
                    else if (_plusForSpace && c == ' ')
                    {
                        sb.Append('+');
                    }
                    else if (c <= 0xFF)
                    {
                        sb.Append('%').Append(((int)c).ToString("X2"));
                    }
                    else
                    {
                        // UTF-8 encode multi-byte characters
                        byte[] bytes = Encoding.UTF8.GetBytes(new[] { c });
                        foreach (byte b in bytes)
                        {
                            sb.Append('%').Append(b.ToString("X2"));
                        }
                    }
                }
                return sb.ToString();
            }

            private bool IsSafe(char c)
            {
                if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9')
                {
                    return true;
                }
                return _safeChars.IndexOf(c) >= 0;
            }
        }
    }
}
