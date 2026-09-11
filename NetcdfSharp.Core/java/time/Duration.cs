// NetcdfSharp.java.time.Duration compatible shim - minimal subset used by NetcdfSharp.
using System;

namespace NetcdfSharp.java.time
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.time.Duration.
    /// </summary>
    public class Duration
    {
        private readonly long _millis;

        private Duration(long millis)
        {
            _millis = millis;
        }

        public static Duration ofMillis(long millis)
        {
            return new Duration(millis);
        }

        public static Duration ofSeconds(long seconds)
        {
            return new Duration(checked(seconds * 1000));
        }

        public static Duration ofMinutes(long minutes)
        {
            return new Duration(checked(minutes * 60 * 1000));
        }

        public static Duration ofHours(long hours)
        {
            return new Duration(checked(hours * 60 * 60 * 1000));
        }

        public static Duration ofDays(long days)
        {
            return new Duration(checked(days * 24 * 60 * 60 * 1000));
        }

        public long toMillis()
        {
            return _millis;
        }

        public long getTimeInMillis() => _millis;

        public long getSeconds()
        {
            return _millis / 1000;
        }

        public int getNano()
        {
            return (int)((_millis % 1000) * 1000000);
        }

        public override string ToString()
        {
            return _millis + " ms";
        }
    }
}
