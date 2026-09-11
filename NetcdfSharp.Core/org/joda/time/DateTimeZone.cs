// Joda-time compatible DateTimeZone - minimal subset used by NetcdfSharp.
using System;
using System.Collections.Generic;

namespace NetcdfSharp.org.joda.time
{
    /// <summary>
    /// A time zone abstraction. This is a simplified replacement for NetcdfSharp.org.joda.time.DateTimeZone
    /// supporting UTC and fixed-offset zones only.
    /// </summary>
    public sealed class DateTimeZone
    {
        public static readonly DateTimeZone UTC = new DateTimeZone("UTC", TimeSpan.Zero);

        private readonly string _id;
        private readonly TimeSpan _offset;

        private DateTimeZone(string id, TimeSpan offset)
        {
            _id = id;
            _offset = offset;
        }

        public string ID => _id;

        public int getOffset(long instant) => (int)_offset.TotalMilliseconds;

        public int getOffset(JDateTime instant) => (int)_offset.TotalMilliseconds;

        public int getOffsetFromLocal(long instant) => (int)_offset.TotalMilliseconds;

        public bool isFixed() => true;

        public TimeZoneInfo ToTimeZone()
        {
            if (_offset == TimeSpan.Zero) return TimeZoneInfo.Utc;
            return TimeZoneInfo.CreateCustomTimeZone(_id, _offset, _id, _id);
        }

        /// <summary>joda DateTimeZone.toTimeZone() returns NetcdfSharp.java.util.TimeZone.</summary>
        public NetcdfSharp.java.util.JTimeZone toTimeZone() => new NetcdfSharp.java.util.JTimeZone(_offset, _id);

        public override bool Equals(object obj)
        {
            if (obj is DateTimeZone other) return _id == other._id;
            return false;
        }

        public override int GetHashCode() => _id.GetHashCode();

        public override string ToString() => _id;

        public static DateTimeZone forID(string id)
        {
            if (id == "UTC" || id == "Etc/UTC" || id == "Z") return UTC;
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(id);
                return forTimeZone(tz);
            }
            catch (TimeZoneNotFoundException)
            {
                return UTC;
            }
        }

        public static DateTimeZone forOffsetHoursMinutes(int hours, int minutes)
        {
            return new DateTimeZone("+" + hours + ":" + minutes, new TimeSpan(hours, minutes, 0));
        }

        public static DateTimeZone forTimeZone(TimeZoneInfo tz)
        {
            return new DateTimeZone(tz.Id, tz.BaseUtcOffset);
        }

        public static DateTimeZone forTimeZone(NetcdfSharp.java.util.JTimeZone tz)
        {
            return new DateTimeZone(tz.getID(), tz.rawOffset());
        }

        public static IList<string> getAvailableIDs()
        {
            var result = new List<string> { "UTC" };
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                result.Add(tz.Id);
            }
            return result;
        }
    }
}
