// NetcdfSharp.java.text.SimpleDateFormat shim for netcdf-java translation.
using System;
using System.Globalization;

namespace NetcdfSharp.java.text
{
    /// <summary>
    /// Formats and parses dates using a pattern. Mirrors the subset of NetcdfSharp.java.text.SimpleDateFormat
    /// used by netcdf-java (yyyy-MM-dd, HH:mm:ss etc). Dates are represented as System.DateTime.
    /// </summary>
    public class SimpleDateFormat
    {
        private string _pattern;
        private NetcdfSharp.java.util.JTimeZone _zone;

        public SimpleDateFormat(string pattern)
        {
            _pattern = pattern;
            _zone = NetcdfSharp.java.util.JTimeZone.UTC;
        }

        public virtual void setTimeZone(NetcdfSharp.java.util.JTimeZone zone)
        {
            _zone = zone;
        }

        public virtual string format(DateTime date)
        {
            string p = ConvertPattern(_pattern);
            return date.ToString(p, CultureInfo.InvariantCulture);
        }

        public virtual DateTime parse(string text)
        {
            string p = ConvertPattern(_pattern);
            var styles = DateTimeStyles.None;
            if (DateTime.TryParseExact(text, p, CultureInfo.InvariantCulture, styles, out DateTime dt))
            {
                return dt;
            }
            // fall back: allow some leniency
            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dt))
            {
                return dt;
            }
            throw new FormatException("Unparseable date: \"" + text + "\" with pattern \"" + _pattern + "\"");
        }

        /// <summary>Translate java SimpleDateFormat pattern letters to .NET format tokens.</summary>
        internal static string ConvertPattern(string java)
        {
            string result = java;
            result = result.Replace("yyyy", "yyyy");
            result = result.Replace("MM", "MM");
            result = result.Replace("dd", "dd");
            result = result.Replace("HH", "HH");
            result = result.Replace("mm", "mm");
            result = result.Replace("ss", "ss");
            result = result.Replace("'T'", "'T'");
            result = result.Replace("Z", "K");
            return result;
        }
    }
}
