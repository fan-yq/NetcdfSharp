// java.util GregorianCalendar / Timer / TimerTask minimal shims.
using System;
using System.Threading;

namespace NetcdfSharp.java.util
{
    /// <summary>A Gregorian calendar. Minimal subset (setTime/toTimeZone/getTime) wrapping .NET DateTime.</summary>
    public class GregorianCalendar
    {
        public const int YEAR = 1;
        public const int MONTH = 2;
        public const int DAY_OF_MONTH = 5;
        public const int HOUR_OF_DAY = 11;
        public const int MINUTE = 12;
        public const int SECOND = 13;
        public const int MILLISECOND = 14;

        private DateTime _time = DateTime.MinValue;
        private readonly NetcdfSharp.java.util.JTimeZone _zone;

        public GregorianCalendar() : this(NetcdfSharp.java.util.JTimeZone.UTC) { }
        public GregorianCalendar(NetcdfSharp.java.util.JTimeZone zone) { _zone = zone ?? NetcdfSharp.java.util.JTimeZone.UTC; }

        public virtual void setTime(DateTime date) { _time = date; }
        public virtual DateTime getTime() => _time;
        public virtual NetcdfSharp.java.util.JTimeZone getTimeZone() => _zone;
        public virtual void setTimeZone(NetcdfSharp.java.util.JTimeZone zone) { }

        public virtual int get(int field)
        {
            DateTime t = _time == DateTime.MinValue ? DateTime.Now : _time;
            switch (field)
            {
                case YEAR: return t.Year;
                case MONTH: return t.Month - 1;
                case DAY_OF_MONTH: return t.Day;
                case HOUR_OF_DAY: return t.Hour;
                case MINUTE: return t.Minute;
                case SECOND: return t.Second;
                case MILLISECOND: return t.Millisecond;
                default: return 0;
            }
        }
    }

}
