// NetcdfSharp.java.lang.JSystem compatible - minimal subset used by NetcdfSharp.
// Renamed from System to JSystem to avoid shadowing the .NET System namespace.
using System;
using System.IO;

namespace NetcdfSharp.java.lang
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.lang.System.
    /// </summary>
    public static class JSystem
    {
        public static NetcdfSharp.java.io.PrintStream @out => NetcdfSharp.java.io.PrintStream.Out;

        public static TextReader @in => Console.In;

        public static NetcdfSharp.java.io.PrintStream err => NetcdfSharp.java.io.PrintStream.Err;

        public static void arraycopy(Array src, int srcPos, Array dest, int destPos, int length)
        {
            global::System.Array.Copy(src, srcPos, dest, destPos, length);
        }

        public static void arraycopy(Array src, int srcPos, Array dest, int destPos, long length)
        {
            global::System.Array.Copy(src, srcPos, dest, destPos, (int)length);
        }

        public static long currentTimeMillis()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static long nanoTime()
        {
            return DateTime.UtcNow.Ticks;
        }

        public static string lineSeparator()
        {
            return Environment.NewLine;
        }

        public static string getProperty(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        public static string getProperty(string key, string defaultValue)
        {
            string v = Environment.GetEnvironmentVariable(key);
            return v ?? defaultValue;
        }

        public static void setProperty(string key, string value)
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
