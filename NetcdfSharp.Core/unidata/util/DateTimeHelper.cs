// DateTimeHelper shim: System.currentTimeMillis() compat.
using System;

namespace NetcdfSharp.unidata.util
{
    /// <summary>
    /// Static helpers for current time in Unix milliseconds.
    /// </summary>
    public static class DateTimeHelper
    {
        public static long CurrentUnixTimeMillis()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
