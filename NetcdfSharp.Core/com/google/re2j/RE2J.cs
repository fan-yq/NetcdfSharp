// RE2J (NetcdfSharp.com.google.re2j) compatible shim - minimal subset used by NetcdfSharp.
// Thin wrapper over System.Text.RegularExpressions.
using System.Text;
using System.Text.RegularExpressions;

namespace NetcdfSharp.com.google.re2j
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.re2j.Pattern.
    /// </summary>
    public class Pattern
    {
        private readonly Regex _regex;

        private Pattern(Regex regex)
        {
            _regex = regex;
        }

        public static Pattern compile(string pattern)
        {
            return new Pattern(new Regex(pattern, RegexOptions.CultureInvariant));
        }

        public static Pattern compile(string pattern, int flags)
        {
            return new Pattern(new Regex(pattern, RegexOptions.CultureInvariant));
        }

        public static bool matches(string regex, string input)
        {
            return new Regex(regex, RegexOptions.CultureInvariant).IsMatch(input);
        }

        public Matcher matcher(string input)
        {
            return new Matcher(_regex, input);
        }

        public string pattern()
        {
            return _regex.ToString();
        }

        public string[] split(string input)
        {
            return _regex.Split(input);
        }

        public bool isMatch(string input)
        {
            return _regex.IsMatch(input);
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.re2j.Matcher.
    /// </summary>
    public class Matcher
    {
        private readonly Regex _regex;
        private string _input;
        private Match _match;
        private bool _matched;

        internal Matcher(Regex regex, string input)
        {
            _regex = regex;
            _input = input;
        }

        public bool matches()
        {
            _match = _regex.Match(_input);
            _matched = _match.Success && _match.Index == 0 && _match.Length == _input.Length;
            return _matched;
        }

        public bool find()
        {
            if (_match != null && _match.Success && _match.Index + _match.Length < _input.Length)
            {
                _match = _match.NextMatch();
            }
            else
            {
                _match = _regex.Match(_input);
            }
            _matched = _match.Success;
            return _matched;
        }

        public bool lookingAt()
        {
            _match = _regex.Match(_input);
            _matched = _match.Success && _match.Index == 0;
            return _matched;
        }

        public bool find(int start)
        {
            _match = _regex.Match(_input, start);
            _matched = _match.Success;
            return _matched;
        }

        public void reset()
        {
            _match = null;
            _matched = false;
        }

        public void reset(string input)
        {
            _input = input;
            reset();
        }

        public string group()
        {
            return _match != null && _match.Success ? _match.Value : null;
        }

        public string group(int group)
        {
            if (_match == null || !_match.Success)
            {
                return null;
            }
            if (group < 0 || group >= _match.Groups.Count)
            {
                return null;
            }
            Group g = _match.Groups[group];
            return g.Success ? g.Value : null;
        }

        public int groupCount()
        {
            return _regex.GetGroupNumbers().Length - 1;
        }

        public int start()
        {
            return _match != null ? _match.Index : -1;
        }

        public int start(int group)
        {
            if (_match != null && group >= 0 && group < _match.Groups.Count)
            {
                return _match.Groups[group].Index;
            }
            return -1;
        }

        public int end()
        {
            return _match != null ? _match.Index + _match.Length : -1;
        }

        public int end(int group)
        {
            if (_match != null && group >= 0 && group < _match.Groups.Count)
            {
                Group g = _match.Groups[group];
                return g.Index + g.Length;
            }
            return -1;
        }

        public string replaceAll(string replacement)
        {
            return _regex.Replace(_input, replacement);
        }

        public string replaceFirst(string replacement)
        {
            return _regex.Replace(_input, replacement, 1);
        }

        public Matcher appendReplacement(StringBuilder sb, string replacement)
        {
            if (_match != null && _match.Success)
            {
                _match.Result(replacement);
                sb.Append(_input, _lastAppendPosition, _match.Index - _lastAppendPosition);
                sb.Append(_match.Result(replacement));
                _lastAppendPosition = _match.Index + _match.Length;
            }
            return this;
        }

        public StringBuilder appendTail(StringBuilder sb)
        {
            if (_input != null)
            {
                sb.Append(_input, _lastAppendPosition, _input.Length - _lastAppendPosition);
            }
            return sb;
        }

        private int _lastAppendPosition;
    }
}
