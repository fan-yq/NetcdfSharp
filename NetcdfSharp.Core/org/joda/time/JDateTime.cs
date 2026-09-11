// Joda-time compatible DateTime - minimal subset used by NetcdfSharp.
// Immutable: stores millis since 1970-01-01T00:00:00Z + a Chronology.
using System;

namespace NetcdfSharp.org.joda.time
{
    /// <summary>
    /// Immutable date-time. Simplified replacement for NetcdfSharp.org.joda.time.JDateTime.
    /// All instants are represented as milliseconds since 1970-01-01T00:00:00Z.
    /// </summary>
    public sealed class JDateTime : IComparable<JDateTime>, IEquatable<JDateTime>
    {
        private readonly long _millis;
        private readonly Chronology _chronology;

        public JDateTime(long millis, Chronology chronology)
        {
            _millis = millis;
            _chronology = chronology ?? ChronologyOf(DateTimeZone.UTC);
        }

        public JDateTime() : this(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), DateTimeZone.UTC)
        {
        }

        public JDateTime(long millis, DateTimeZone zone)
        {
            _millis = millis;
            _chronology = ChronologyOf(zone);
        }

        public JDateTime(JDateTime dateTime, DateTimeZone zone)
        {
            _millis = dateTime.getMillis();
            _chronology = ChronologyOf(zone);
        }

        public JDateTime(System.DateTime date, DateTimeZone zone)
        {
            _millis = date.Ticks / TimeSpan.TicksPerMillisecond;
            _chronology = ChronologyOf(zone);
        }

        public JDateTime(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute, Chronology chronology)
        {
            _chronology = chronology ?? ChronologyOf(DateTimeZone.UTC);
            _millis = _chronology.getDateTimeMillis(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, 0);
        }

        public JDateTime(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute, int millisOfSecond, Chronology chronology)
        {
            _chronology = chronology ?? ChronologyOf(DateTimeZone.UTC);
            _millis = _chronology.getDateTimeMillis(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, millisOfSecond);
        }

        private static Chronology ChronologyOf(DateTimeZone zone)
        {
            return NetcdfSharp.org.joda.time.chrono.ISOChronology.getInstanceUTC();
        }

        public long getMillis() => _millis;

        public static JDateTime now()
        {
            return new JDateTime(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), DateTimeZone.UTC);
        }

        public Chronology getChronology() => _chronology;

        public DateTimeZone getZone() => _chronology.getZone();

        // ---- fields ----

        public int getYear() => _chronology.getYear(_millis);

        public int getMonthOfYear() => _chronology.getMonthOfYear(_millis);

        public int getDayOfMonth() => _chronology.getDayOfMonth(_millis);

        public int getDayOfYear() => _chronology.getDayOfYear(_millis);

        public int getHourOfDay() => _chronology.getHourOfDay(_millis);

        public int getMinuteOfHour() => _chronology.getMinuteOfHour(_millis);

        public int getSecondOfMinute() => _chronology.getSecondOfMinute(_millis);

        public int getMillisOfSecond() => _chronology.getMillisOfSecond(_millis);

        public int get(DateTimeFieldType type)
        {
            switch (type.Name)
            {
                case "year": return getYear();
                case "monthOfYear": return getMonthOfYear();
                case "dayOfMonth": return getDayOfMonth();
                case "dayOfYear": return getDayOfYear();
                case "hourOfDay": return getHourOfDay();
                case "minuteOfHour": return getMinuteOfHour();
                case "secondOfMinute": return getSecondOfMinute();
                case "millisOfSecond": return getMillisOfSecond();
                default: throw new ArgumentException("unimplemented field " + type.Name);
            }
        }

        // ---- with* ----

        public JDateTime withZone(DateTimeZone zone)
        {
            if (zone.Equals(getZone())) return this;
            return new JDateTime(_millis, zone);
        }

        public JDateTime withChronology(Chronology chrono)
        {
            if (chrono.Equals(_chronology)) return this;
            return new JDateTime(_millis, chrono);
        }

        public JDateTime withZoneRetainFields(DateTimeZone zone)
        {
            return withZone(zone);
        }

        public JDateTime withYear(int year)
        {
            return withField(year, getMonthOfYear(), getDayOfMonth());
        }

        public JDateTime withMonthOfYear(int month)
        {
            return withField(getYear(), month, getDayOfMonth());
        }

        public JDateTime withDayOfMonth(int day)
        {
            return withField(getYear(), getMonthOfYear(), day);
        }

        public JDateTime withDayOfYear(int doy)
        {
            // compute month/day from day-of-year using the chronology's month lengths
            int year = getYear();
            int month = 1;
            int remaining = doy;
            int daysInMonth;
            while (month <= 12)
            {
                daysInMonth = DaysInMonth(year, month);
                if (remaining <= daysInMonth) break;
                remaining -= daysInMonth;
                month++;
            }
            return withField(year, month, remaining);
        }

        private int DaysInMonth(int year, int month)
        {
            // reconstruct via month-lengths of chronology by probing next month start
            long thisStart = _chronology.getDateTimeMillis(year, month, 1, 0, 0, 0, 0);
            long nextStart;
            if (month < 12)
                nextStart = _chronology.getDateTimeMillis(year, month + 1, 1, 0, 0, 0, 0);
            else
                nextStart = _chronology.getDateTimeMillis(year + 1, 1, 1, 0, 0, 0, 0);
            return (int)((nextStart - thisStart) / Chronology.MILLIS_PER_DAY);
        }

        private JDateTime withField(int year, int month, int day)
        {
            long millis = _chronology.getDateTimeMillis(year, month, day, getHourOfDay(), getMinuteOfHour(), getSecondOfMinute(), getMillisOfSecond());
            return new JDateTime(millis, _chronology);
        }

        // ---- arithmetic ----

        public JDateTime plus(long durationMillis) => new JDateTime(_millis + durationMillis, _chronology);

        public JDateTime plusMillis(int n) => new JDateTime(_millis + n, _chronology);

        public JDateTime plusSeconds(int n) => new JDateTime(_millis + n * Chronology.MILLIS_PER_SECOND, _chronology);

        public JDateTime plusMinutes(int n) => new JDateTime(_millis + n * Chronology.MILLIS_PER_MINUTE, _chronology);

        public JDateTime plusHours(int n) => new JDateTime(_millis + n * Chronology.MILLIS_PER_HOUR, _chronology);

        public JDateTime plusDays(int n) => new JDateTime(_millis + n * Chronology.MILLIS_PER_DAY, _chronology);

        public JDateTime plusMonths(int n)
        {
            int month = getMonthOfYear() + n;
            int year = getYear() + MonthToYearAdjust(month);
            month = ((month - 1) % 12 + 12) % 12 + 1;
            int day = getDayOfMonth();
            int maxDay = DaysInMonth(year, month);
            if (day > maxDay) day = maxDay;
            return withField(year, month, day);
        }

        public JDateTime plusYears(int n) => withField(getYear() + n, getMonthOfYear(), Math.Min(getDayOfMonth(), DaysInMonth(getYear() + n, getMonthOfYear())));

        public JDateTime minusMillis(int n) => plusMillis(-n);

        public JDateTime minusSeconds(int n) => plusSeconds(-n);

        public JDateTime minusMinutes(int n) => plusMinutes(-n);

        public JDateTime minusHours(int n) => plusHours(-n);

        public JDateTime minusDays(int n) => plusDays(-n);

        public JDateTime minusMonths(int n) => plusMonths(-n);

        public JDateTime minusYears(int n) => plusYears(-n);

        private static int MonthToYearAdjust(int month)
        {
            if (month > 0) return (month - 1) / 12;
            return (month - 12) / 12;
        }

        // ---- comparisons ----

        public int CompareTo(JDateTime other) => _millis.CompareTo(other._millis);

        public int compareTo(JDateTime other) => _millis.CompareTo(other._millis);

        public bool isAfter(JDateTime other) => _millis > other._millis;

        public bool isBefore(JDateTime other) => _millis < other._millis;

        public bool isEqual(JDateTime other) => _millis == other._millis;

        public bool Equals(JDateTime other) => other != null && _millis == other._millis && _chronology.Equals(other._chronology);

        public override bool Equals(object obj) => obj is JDateTime d && Equals(d);

        public override int GetHashCode() => unchecked((int)_millis * 31 + (_chronology?.GetHashCode() ?? 0));

        // ---- conversions ----

        public System.DateTime toDate()
        {
            // _millis is milliseconds since the Unix epoch (1970-01-01T00:00:00Z); a .NET DateTime
            // counts ticks since 0001-01-01, so offset by the Unix epoch's tick value.
            return new System.DateTime(DateTime.UnixEpoch.Ticks + _millis * TimeSpan.TicksPerMillisecond, DateTimeKind.Utc);
        }

        public long Ticks => DateTime.UnixEpoch.Ticks + _millis * TimeSpan.TicksPerMillisecond;

        public override string ToString()
        {
            var date = toDate();
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss.FFF'Z'");
        }
    }
}
