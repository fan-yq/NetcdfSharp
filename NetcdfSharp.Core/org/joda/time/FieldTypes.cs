// Joda-time compatible field type markers - minimal subset used by NetcdfSharp.
namespace NetcdfSharp.org.joda.time
{
    /// <summary>
    /// Identifies a field of a date-time. Simplified replacement for NetcdfSharp.org.joda.time.DateTimeFieldType.
    /// </summary>
    public sealed class DateTimeFieldType
    {
        public string Name { get; }

        private DateTimeFieldType(string name)
        {
            Name = name;
        }

        public static DateTimeFieldType year() => new DateTimeFieldType("year");
        public static DateTimeFieldType monthOfYear() => new DateTimeFieldType("monthOfYear");
        public static DateTimeFieldType dayOfMonth() => new DateTimeFieldType("dayOfMonth");
        public static DateTimeFieldType dayOfYear() => new DateTimeFieldType("dayOfYear");
        public static DateTimeFieldType hourOfDay() => new DateTimeFieldType("hourOfDay");
        public static DateTimeFieldType minuteOfHour() => new DateTimeFieldType("minuteOfHour");
        public static DateTimeFieldType secondOfMinute() => new DateTimeFieldType("secondOfMinute");
        public static DateTimeFieldType millisOfSecond() => new DateTimeFieldType("millisOfSecond");

        public override string ToString() => Name;
    }

    /// <summary>
    /// Identifies a period type. Simplified replacement for NetcdfSharp.org.joda.time.PeriodType.
    /// </summary>
    public sealed class PeriodType
    {
        public string Name { get; }

        private PeriodType(string name)
        {
            Name = name;
        }

        public static PeriodType millis() => new PeriodType("millis");
        public static PeriodType seconds() => new PeriodType("seconds");
        public static PeriodType minutes() => new PeriodType("minutes");
        public static PeriodType hours() => new PeriodType("hours");
        public static PeriodType days() => new PeriodType("days");
        public static PeriodType weeks() => new PeriodType("weeks");
        public static PeriodType months() => new PeriodType("months");
        public static PeriodType years() => new PeriodType("years");

        public override string ToString() => Name;
    }

    /// <summary>
    /// A duration. Simplified replacement for NetcdfSharp.org.joda.time.Period.
    /// </summary>
    public sealed class Period
    {
        public long Value { get; }
        public PeriodType Type { get; }

        private Period(long value, PeriodType type)
        {
            Value = value;
            Type = type;
        }

        public static Period millis(long n) => new Period(n, PeriodType.millis());
        public static Period seconds(long n) => new Period(n, PeriodType.seconds());
        public static Period minutes(long n) => new Period(n, PeriodType.minutes());
        public static Period hours(long n) => new Period(n, PeriodType.hours());
        public static Period days(long n) => new Period(n, PeriodType.days());
        public static Period weeks(long n) => new Period(n, PeriodType.weeks());
        public static Period months(long n) => new Period(n, PeriodType.months());
        public static Period years(long n) => new Period(n, PeriodType.years());
    }
}
