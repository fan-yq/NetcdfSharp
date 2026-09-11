// Joda-time compatible Chronology - minimal subset used by NetcdfSharp.
// Implements millisecond<->calendar-field conversion for the CF calendars:
//   ISO (proleptic gregorian), Julian, GJ (mixed), noleap, all_leap, 360_day.
// A millisecond instant of zero corresponds to 1970-01-01T00:00:00.000Z.
using System;

namespace NetcdfSharp.org.joda.time
{
    /// <summary>
    /// Base class for calendar systems. Provides millisecond<->field conversion.
    /// Simplified replacement for NetcdfSharp.org.joda.time.Chronology.
    /// </summary>
    public abstract class Chronology
    {
        internal const long MILLIS_PER_SECOND = 1000;
        internal const long MILLIS_PER_MINUTE = 60000;
        internal const long MILLIS_PER_HOUR = 3600000;
        internal const long MILLIS_PER_DAY = 86400000;

        internal static long FloorDiv(long a, long b)
        {
            long q = a / b;
            if ((a % b != 0) && ((a ^ b) < 0)) q--;
            return q;
        }

        internal static long FloorMod(long a, long b)
        {
            long r = a % b;
            if (r != 0 && ((r ^ b) < 0)) r += b;
            return r;
        }

        // ---- abstract calendar-specific parts ----

        /// <summary>Number of days in the given year.</summary>
        protected internal abstract int GetDaysInYear(int year);

        /// <summary>Number of days in the given month (1-12).</summary>
        protected internal abstract int GetDaysInMonth(int year, int month);

        /// <summary>Era offset: year 0 corresponds to this proleptic year offset (used for ISO).</summary>
        protected internal virtual int YearOffset => 1970;

        // ---- time-of-day fields (calendar independent) ----

        public virtual int getHourOfDay(long instant)
        {
            long dayMillis = FloorMod(instant, MILLIS_PER_DAY);
            return (int)(dayMillis / MILLIS_PER_HOUR);
        }

        public virtual int getMinuteOfHour(long instant)
        {
            long dayMillis = FloorMod(instant, MILLIS_PER_DAY);
            return (int)(FloorMod(dayMillis, MILLIS_PER_HOUR) / MILLIS_PER_MINUTE);
        }

        public virtual int getSecondOfMinute(long instant)
        {
            long dayMillis = FloorMod(instant, MILLIS_PER_DAY);
            return (int)(FloorMod(dayMillis, MILLIS_PER_MINUTE) / MILLIS_PER_SECOND);
        }

        public virtual int getMillisOfSecond(long instant)
        {
            return (int)FloorMod(instant, MILLIS_PER_SECOND);
        }

        // ---- date fields (calendar specific) ----

        public virtual int getYear(long instant)
        {
            long days = FloorDiv(instant, MILLIS_PER_DAY);
            int year = (int)FloorDiv(days, 366);
            // refine by iterating (calendars have bounded year lengths)
            while (DaysFromYearStart(year) > days) year--;
            while (DaysFromYearStart(year + 1) <= days) year++;
            return year;
        }

        /// <summary>Days from 1970-01-01 to the first day of the given year (i.e. Jan 1 of year).</summary>
        protected internal abstract long DaysFromYearStart(int year);

        public virtual int getMonthOfYear(long instant)
        {
            long days = FloorDiv(instant, MILLIS_PER_DAY);
            int year = getYear(instant);
            long dayOfYear = days - DaysFromYearStart(year); // 0-based
            int month = 1;
            int daysInMonth = GetDaysInMonth(year, month);
            while (dayOfYear >= daysInMonth && month < 12)
            {
                dayOfYear -= daysInMonth;
                month++;
                daysInMonth = GetDaysInMonth(year, month);
            }
            return month;
        }

        public virtual int getDayOfMonth(long instant)
        {
            long days = FloorDiv(instant, MILLIS_PER_DAY);
            int year = getYear(instant);
            long dayOfYear = days - DaysFromYearStart(year); // 0-based
            int month = 1;
            int daysInMonth = GetDaysInMonth(year, month);
            while (dayOfYear >= daysInMonth && month < 12)
            {
                dayOfYear -= daysInMonth;
                month++;
                daysInMonth = GetDaysInMonth(year, month);
            }
            return (int)dayOfYear + 1;
        }

        public virtual int getDayOfYear(long instant)
        {
            long days = FloorDiv(instant, MILLIS_PER_DAY);
            int year = getYear(instant);
            return (int)(days - DaysFromYearStart(year)) + 1; // 1-based
        }

        /// <summary>
        /// Convert calendar fields to a millisecond instant.
        /// </summary>
        public virtual long getDateTimeMillis(int year, int month, int dayOfMonth, int hourOfDay, int minuteOfHour,
            int secondOfMinute, int millisOfSecond)
        {
            long days = DaysFromYearStart(year);
            for (int m = 1; m < month; m++)
            {
                days += GetDaysInMonth(year, m);
            }
            days += (dayOfMonth - 1);
            long millis = days * MILLIS_PER_DAY;
            millis += (long)hourOfDay * MILLIS_PER_HOUR;
            millis += (long)minuteOfHour * MILLIS_PER_MINUTE;
            millis += (long)secondOfMinute * MILLIS_PER_SECOND;
            millis += millisOfSecond;
            return millis;
        }

        // ---- zone handling ----

        public virtual DateTimeZone getZone() => DateTimeZone.UTC;

        public virtual Chronology withZone(DateTimeZone zone)
        {
            if (zone == DateTimeZone.UTC || zone.Equals(DateTimeZone.UTC)) return withUTC();
            throw new NotSupportedException("Only UTC is supported in custom chronologies.");
        }

        public virtual Chronology withUTC() => this;
    }
}
