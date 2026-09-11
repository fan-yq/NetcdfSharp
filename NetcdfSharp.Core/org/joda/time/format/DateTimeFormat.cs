// Joda-time compatible formatters - minimal subset used by NetcdfSharp.
// Translates a small set of joda-time patterns to .NET formatting/parsing.
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NetcdfSharp.org.joda.time.format
{
    /// <summary>
    /// Formats/parses DateTimes with a joda-time style pattern.
    /// Supports: yyyy, yy, MM, dd, HH, mm, ss, SSS, 'literal', Z, and '.'
    /// </summary>
    public sealed class DateTimeFormatter
    {
        private readonly string _pattern;
        private DateTimeZone _zone;
        private Chronology _chronology;

        internal DateTimeFormatter(string pattern)
        {
            _pattern = pattern;
            _zone = DateTimeZone.UTC;
            _chronology = null;
        }

        public DateTimeFormatter withZoneUTC()
        {
            _zone = DateTimeZone.UTC;
            return this;
        }

        public DateTimeFormatter withZone(DateTimeZone zone)
        {
            _zone = zone ?? DateTimeZone.UTC;
            return this;
        }

        public DateTimeFormatter withChronology(Chronology chrono)
        {
            _chronology = chrono;
            return this;
        }

        private static string ToDotNetFormat(string pattern)
        {
            // joda pattern -> .NET custom format string
            var sb = new System.Text.StringBuilder();
            int i = 0;
            while (i < pattern.Length)
            {
                char c = pattern[i];
                if (c == '\'')
                {
                    // quoted literal
                    i++;
                    int start = i;
                    while (i < pattern.Length && pattern[i] != '\'') i++;
                    sb.Append('\'');
                    sb.Append(pattern.Substring(start, i - start));
                    sb.Append('\'');
                    i++;
                }
                else if (c == 'y')
                {
                    int run = CountRun(pattern, i, 'y');
                    sb.Append(run >= 4 ? "yyyy" : "yy");
                    i += run;
                }
                else if (c == 'M')
                {
                    int run = CountRun(pattern, i, 'M');
                    sb.Append(run == 1 ? "M" : "MM");
                    i += run;
                }
                else if (c == 'd')
                {
                    int run = CountRun(pattern, i, 'd');
                    sb.Append(run == 1 ? "d" : "dd");
                    i += run;
                }
                else if (c == 'H')
                {
                    int run = CountRun(pattern, i, 'H');
                    sb.Append(run == 1 ? "H" : "HH");
                    i += run;
                }
                else if (c == 'm')
                {
                    int run = CountRun(pattern, i, 'm');
                    sb.Append(run == 1 ? "m" : "mm");
                    i += run;
                }
                else if (c == 's')
                {
                    int run = CountRun(pattern, i, 's');
                    sb.Append(run == 1 ? "s" : "ss");
                    i += run;
                }
                else if (c == 'S')
                {
                    int run = CountRun(pattern, i, 'S');
                    sb.Append(new string('f', Math.Min(run, 7)));
                    i += run;
                }
                else
                {
                    // literal char; escape .NET specials
                    sb.Append('\\').Append(c);
                    i++;
                }
            }
            return sb.ToString();
        }

        private static int CountRun(string s, int start, char c)
        {
            int n = 0;
            while (start + n < s.Length && s[start + n] == c) n++;
            return n;
        }

        public string print(JDateTime dateTime)
        {
            // Use the JDateTime's own chronology fields rather than converting to a .NET DateTime:
            // a .NET DateTime is always proleptic-Gregorian, so converting would produce wrong dates
            // for the fixed-year calendars (360_day, noleap, all_leap, julian) that netcdf-java supports.
            return PrintWithFields(_pattern, dateTime);
        }

        private static string PrintWithFields(string pattern, JDateTime dt)
        {
            var sb = new System.Text.StringBuilder();
            int i = 0;
            while (i < pattern.Length)
            {
                char c = pattern[i];
                if (c == '\'')
                {
                    // quoted literal
                    i++;
                    int start = i;
                    while (i < pattern.Length && pattern[i] != '\'') i++;
                    sb.Append(pattern.Substring(start, i - start));
                    i++;
                }
                else if (c == 'y')
                {
                    int run = CountRun(pattern, i, 'y');
                    int y = dt.getYear();
                    sb.Append(run >= 4 ? y.ToString("D4", CultureInfo.InvariantCulture) : (y % 100).ToString("D2", CultureInfo.InvariantCulture));
                    i += run;
                }
                else if (c == 'M')
                {
                    int run = CountRun(pattern, i, 'M');
                    int m = dt.getMonthOfYear();
                    sb.Append(run == 1 ? m.ToString(CultureInfo.InvariantCulture) : m.ToString("D2", CultureInfo.InvariantCulture));
                    i += run;
                }
                else if (c == 'd')
                {
                    int run = CountRun(pattern, i, 'd');
                    int d = dt.getDayOfMonth();
                    sb.Append(run == 1 ? d.ToString(CultureInfo.InvariantCulture) : d.ToString("D2", CultureInfo.InvariantCulture));
                    i += run;
                }
                else if (c == 'H')
                {
                    int run = CountRun(pattern, i, 'H');
                    int h = dt.getHourOfDay();
                    sb.Append(run == 1 ? h.ToString(CultureInfo.InvariantCulture) : h.ToString("D2", CultureInfo.InvariantCulture));
                    i += run;
                }
                else if (c == 'm')
                {
                    int run = CountRun(pattern, i, 'm');
                    int mm = dt.getMinuteOfHour();
                    sb.Append(run == 1 ? mm.ToString(CultureInfo.InvariantCulture) : mm.ToString("D2", CultureInfo.InvariantCulture));
                    i += run;
                }
                else if (c == 's')
                {
                    int run = CountRun(pattern, i, 's');
                    int s = dt.getSecondOfMinute();
                    sb.Append(run == 1 ? s.ToString(CultureInfo.InvariantCulture) : s.ToString("D2", CultureInfo.InvariantCulture));
                    i += run;
                }
                else if (c == 'S')
                {
                    int run = CountRun(pattern, i, 'S');
                    int ms = dt.getMillisOfSecond();
                    sb.Append(ms.ToString("D3", CultureInfo.InvariantCulture).PadRight(Math.Min(run, 3)).Substring(0, Math.Min(run, 3)));
                    i += run;
                }
                else
                {
                    // literal char
                    sb.Append(c);
                    i++;
                }
            }
            return sb.ToString();
        }

        public string print(long millis)
        {
            return print(new JDateTime(millis, DateTimeZone.UTC));
        }

        public JDateTime parseDateTime(string text)
        {
            string fmt = ToDotNetFormat(_pattern);
            // .NET strict parse; 'Z' in pattern is literal, trim trailing Z if needed
            string value = text;
            var styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal;
            DateTime result = DateTime.ParseExact(value, fmt, CultureInfo.InvariantCulture, styles);
            return new JDateTime(result.Ticks / TimeSpan.TicksPerMillisecond, DateTimeZone.UTC);
        }
    }

    /// <summary>
    /// Factory for joda-time style formatters.
    /// </summary>
    public static class DateTimeFormat
    {
        public static DateTimeFormatter forPattern(string pattern)
        {
            return new DateTimeFormatter(pattern);
        }
    }
}
