// Joda-time compatible chronologies - minimal subset used by NetcdfSharp.
// Concrete CF calendars: ISO (proleptic gregorian), Julian, GJ (mixed), and
// fixed-year calendars (noleap, all_leap, 360_day).
using System;

namespace NetcdfSharp.org.joda.time.chrono
{
    /// <summary>
    /// Standard proleptic Gregorian calendar (ISO 8601). Matches DateTime semantics.
    /// </summary>
    public sealed class ISOChronology : Chronology
    {
        private static readonly ISOChronology INSTANCE_UTC = new ISOChronology();

        private ISOChronology() { }

        public static ISOChronology getInstanceUTC() => INSTANCE_UTC;

        protected internal override int GetDaysInYear(int year)
        {
            return IsLeapYear(year) ? 366 : 365;
        }

        protected internal override int GetDaysInMonth(int year, int month)
        {
            switch (month)
            {
                case 1: case 3: case 5: case 7: case 8: case 10: case 12: return 31;
                case 4: case 6: case 9: case 11: return 30;
                case 2: return IsLeapYear(year) ? 29 : 28;
                default: throw new ArgumentOutOfRangeException("month");
            }
        }

        internal static bool IsLeapYear(int year)
        {
            return (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;
        }

        protected internal override long DaysFromYearStart(int year)
        {
            // days from 1970-01-01 to Jan 1 of the given year
            long y = year;
            // number of leap days between 1970 and year
            // = floor((year-1)/4) - floor((year-1)/100) + floor((year-1)/400)
            //   - [1970-based offsets]
            long leapYearsBefore = LeapDaysBeforeYear(y) - LeapDaysBeforeYear(1970);
            long commonYears = (y - 1970) - leapYearsBefore;
            return commonYears * 365 + leapYearsBefore * 366;
        }

        private static long LeapDaysBeforeYear(long y)
        {
            long y1 = y - 1;
            return (y1 / 4) - (y1 / 100) + (y1 / 400);
        }

        public override string ToString() => "ISOChronology[UTC]";
    }

    /// <summary>
    /// Proleptic Julian calendar: leap year every 4 years (no century rule), no year zero.
    /// </summary>
    public sealed class JulianChronology : Chronology
    {
        private static readonly JulianChronology INSTANCE_UTC = new JulianChronology();

        private JulianChronology() { }

        public static JulianChronology getInstanceUTC() => INSTANCE_UTC;

        protected internal override int GetDaysInYear(int year)
        {
            return IsLeapYear(year) ? 366 : 365;
        }

        protected internal override int GetDaysInMonth(int year, int month)
        {
            switch (month)
            {
                case 1: case 3: case 5: case 7: case 8: case 10: case 12: return 31;
                case 4: case 6: case 9: case 11: return 30;
                case 2: return IsLeapYear(year) ? 29 : 28;
                default: throw new ArgumentOutOfRangeException("month");
            }
        }

        private static bool IsLeapYear(int year)
        {
            // Julian: leap every 4 years. Year 0 does not exist; year 1 is followed by year -1.
            // For proleptic purposes treat divisibility by 4 as leap.
            return ((year % 4) == 0);
        }

        protected internal override long DaysFromYearStart(int year)
        {
            long y = year;
            long leapYearsBefore = LeapYearsBeforeYear(y) - LeapYearsBeforeYear(1970);
            long commonYears = (y - 1970) - leapYearsBefore;
            return commonYears * 365 + leapYearsBefore * 366;
        }

        private static long LeapYearsBeforeYear(long y)
        {
            long y1 = y - 1;
            if (y1 < 0)
            {
                // Julian year 0 does not exist; years go ... -2, -1, 1, 2, 3 ...
                // Leap years are years divisible by 4 in this sequence.
                // Number of leap years before negative year y:
                // years considered: y-1, y-2, ... (negative); year 0 skipped.
                return FloorDivNeg(y1);
            }
            return y1 / 4;
        }

        private static long FloorDivNeg(long y1)
        {
            // count of multiples of 4 in [y1, -1], i.e. floor(-y1/4)... handled carefully
            if (y1 >= -1) return 0;
            return (-y1) / 4; // number of negative years divisible by 4
        }

        public override string ToString() => "JulianChronology[UTC]";
    }

    /// <summary>
    /// Gregorian/Julian hybrid: Julian before 1582-10-15, Gregorian from then on.
    /// Matches the CF "gregorian"/"standard" calendar.
    /// </summary>
    public sealed class GJChronology : Chronology
    {
        private static readonly GJChronology INSTANCE_UTC = new GJChronology();
        // cutover: 1582-10-15T00:00:00Z in proleptic Gregorian millis
        private static readonly long CUTOVER_MILLIS = (new DateTime(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc)).Ticks / TimeSpan.TicksPerMillisecond;

        private GJChronology() { }

        public static GJChronology getInstanceUTC() => INSTANCE_UTC;

        public override int getYear(long instant)
        {
            if (instant < CUTOVER_MILLIS) return JulianYear(instant);
            return ISOYear(instant);
        }

        public override int getMonthOfYear(long instant)
        {
            if (instant < CUTOVER_MILLIS) return JulianMonth(instant);
            return ISOChronology.getInstanceUTC().getMonthOfYear(instant);
        }

        public override int getDayOfMonth(long instant)
        {
            if (instant < CUTOVER_MILLIS) return JulianDay(instant);
            return ISOChronology.getInstanceUTC().getDayOfMonth(instant);
        }

        public override int getDayOfYear(long instant)
        {
            if (instant < CUTOVER_MILLIS) return JulianDayOfYear(instant);
            return ISOChronology.getInstanceUTC().getDayOfYear(instant);
        }

        protected internal override int GetDaysInYear(int year) => ISOChronology.IsLeapYear(year) ? 366 : 365;
        protected internal override int GetDaysInMonth(int year, int month)
        {
            switch (month)
            {
                case 1: case 3: case 5: case 7: case 8: case 10: case 12: return 31;
                case 4: case 6: case 9: case 11: return 30;
                case 2: return ISOChronology.IsLeapYear(year) ? 29 : 28;
                default: throw new ArgumentOutOfRangeException("month");
            }
        }
        protected internal override long DaysFromYearStart(int year) => 0; // not used directly

        public override long getDateTimeMillis(int year, int month, int day, int h, int m, int s, int ms)
        {
            // For dates before cutover, interpret as Julian.
            long isoMillis = ISOChronology.getInstanceUTC().getDateTimeMillis(year, month, day, h, m, s, ms);
            if (isoMillis >= CUTOVER_MILLIS) return isoMillis;
            // Julian: reconstruct from Julian calendar fields
            return JulianToMillis(year, month, day, h, m, s, ms);
        }

        // ---- Julian helpers ----
        private static int JulianYear(long instant)
        {
            long days = Chronology.FloorDiv(instant, Chronology.MILLIS_PER_DAY);
            int year = (int)Chronology.FloorDiv(days, 366);
            while (JulianDaysFromYearStart(year) > days) year--;
            while (JulianDaysFromYearStart(year + 1) <= days) year++;
            return year;
        }

        private static long JulianDaysFromYearStart(int year)
        {
            long y = year;
            long leaps = (y < 1 ? (-y) / 4 : (y - 1) / 4) - (1970 < 1 ? (-1970) / 4 : (1970 - 1) / 4);
            long commons = (y - 1970) - leaps;
            return commons * 365 + leaps * 366;
        }

        private static int JulianMonth(long instant)
        {
            long days = Chronology.FloorDiv(instant, Chronology.MILLIS_PER_DAY);
            int year = JulianYear(instant);
            long doy = days - JulianDaysFromYearStart(year);
            int month = 1;
            int[] ml = { 31, IsLeapYear(year) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            while (doy >= ml[month - 1] && month < 12) { doy -= ml[month - 1]; month++; }
            return month;
        }

        private static int JulianDay(long instant)
        {
            long days = Chronology.FloorDiv(instant, Chronology.MILLIS_PER_DAY);
            int year = JulianYear(instant);
            long doy = days - JulianDaysFromYearStart(year);
            int month = 1;
            int[] ml = { 31, IsLeapYear(year) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            while (doy >= ml[month - 1] && month < 12) { doy -= ml[month - 1]; month++; }
            return (int)doy + 1;
        }

        private static int JulianDayOfYear(long instant)
        {
            long days = Chronology.FloorDiv(instant, Chronology.MILLIS_PER_DAY);
            int year = JulianYear(instant);
            return (int)(days - JulianDaysFromYearStart(year)) + 1;
        }

        private static long JulianToMillis(int year, int month, int day, int h, int m, int s, int ms)
        {
            long days = JulianDaysFromYearStart(year);
            int[] ml = { 31, IsLeapYear(year) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            for (int i = 1; i < month; i++) days += ml[i - 1];
            days += (day - 1);
            long millis = days * Chronology.MILLIS_PER_DAY;
            millis += (long)h * Chronology.MILLIS_PER_HOUR + (long)m * Chronology.MILLIS_PER_MINUTE + (long)s * Chronology.MILLIS_PER_SECOND + ms;
            return millis;
        }

        private static int ISOYear(long instant) => ISOChronology.getInstanceUTC().getYear(instant);

        private static bool IsLeapYear(int year) => ISOChronology.IsLeapYear(year);

        public override string ToString() => "GJChronology[UTC]";
    }

    /// <summary>
    /// Base for fixed-year-length calendars (noleap, all_leap, 360_day).
    /// </summary>
    public abstract class FixedYearChronology : Chronology
    {
        private readonly int[] _monthLengths;
        private readonly int _daysInYear;
        private readonly long _millisInYear;

        protected FixedYearChronology(int[] monthLengths)
        {
            _monthLengths = (int[])monthLengths.Clone();
            _daysInYear = 0;
            foreach (int len in _monthLengths) _daysInYear += len;
            _millisInYear = (long)_daysInYear * MILLIS_PER_DAY;
        }

        protected internal override int GetDaysInYear(int year) => _daysInYear;

        protected internal override int GetDaysInMonth(int year, int month) => _monthLengths[month - 1];

        protected internal override long DaysFromYearStart(int year)
        {
            return (long)(year - 1970) * _daysInYear;
        }

        public override int getYear(long instant)
        {
            return (int)FloorDiv(instant, _millisInYear) + 1970;
        }

        public int[] GetMonthLengths() => (int[])_monthLengths.Clone();

        public int GetDaysInYear() => _daysInYear;
    }

    /// <summary>
    /// No-leap calendar: 365 days/year, February always 28 days.
    /// </summary>
    public sealed class NoLeapChronology : FixedYearChronology
    {
        private static readonly NoLeapChronology INSTANCE_UTC =
            new NoLeapChronology(new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 });

        private NoLeapChronology(int[] monthLengths) : base(monthLengths) { }

        public static NoLeapChronology getInstanceUTC() => INSTANCE_UTC;

        public override string ToString() => "No-leap Chronology in UTC";
    }

    /// <summary>
    /// All-leap calendar: 366 days/year, February always 29 days.
    /// </summary>
    public sealed class AllLeapChronology : FixedYearChronology
    {
        private static readonly AllLeapChronology INSTANCE_UTC =
            new AllLeapChronology(new int[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 });

        private AllLeapChronology(int[] monthLengths) : base(monthLengths) { }

        public static AllLeapChronology getInstanceUTC() => INSTANCE_UTC;

        public override string ToString() => "All-leap Chronology in UTC";
    }

    /// <summary>
    /// 360-day calendar: 12 months of 30 days each.
    /// </summary>
    public sealed class ThreeSixtyDayChronology : FixedYearChronology
    {
        private static readonly ThreeSixtyDayChronology INSTANCE_UTC =
            new ThreeSixtyDayChronology(new int[] { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 });

        private ThreeSixtyDayChronology(int[] monthLengths) : base(monthLengths) { }

        public static ThreeSixtyDayChronology getInstanceUTC() => INSTANCE_UTC;

        public override string ToString() => "360-day Chronology in UTC";
    }
}
