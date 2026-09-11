using System;
using System.Globalization;

namespace NetcdfSharp.java.lang
{
    /// <summary>
    /// Java primitive wrapper class compat shims used by the translated code.
    /// Central place for the semantics that differ from plain .NET.
    /// </summary>
    public static class JavaLangCompat
    {
        public const double DoubleMinNormal = 2.2250738585072014E-308;
        public const float FloatMinNormal = 1.1754943508222875E-38F;

        // ---- parsing (Java parse* is locale-independent) ----
        public static double ParseDouble(string s) => double.Parse(s, CultureInfo.InvariantCulture);
        public static int ParseInt(string s) => int.Parse(s, CultureInfo.InvariantCulture);
        public static long ParseLong(string s) => long.Parse(s, CultureInfo.InvariantCulture);

        // ---- toString ----
        // Integer.toString / Long.toString are identical to invariant .NET format.
        public static string IntToString(int i) => i.ToString(CultureInfo.InvariantCulture);
        public static string LongToString(long l) => l.ToString(CultureInfo.InvariantCulture);

        // Java Double.toString: "1.0" for integral values, "1.0E10" style exponents.
        public static string DoubleToString(double d)
        {
            if (double.IsNaN(d)) return "NaN";
            if (double.IsPositiveInfinity(d)) return "Infinity";
            if (double.IsNegativeInfinity(d)) return "-Infinity";
            if (d == Math.Floor(d) && !double.IsInfinity(d) && Math.Abs(d) < 1.0E21)
            {
                return d.ToString("0.0", CultureInfo.InvariantCulture);
            }
            return d.ToString("0.#############################E+0", CultureInfo.InvariantCulture);
        }

        // ---- compare (Java semantics: NaN is greater than all others, incl +Inf) ----
        public static int DoubleCompare(double a, double b)
        {
            if (double.IsNaN(a)) return double.IsNaN(b) ? 0 : 1;
            if (double.IsNaN(b)) return -1;
            if (a == b) return 0;
            if (a == 0.0 && b == 0.0) return 0; // +0.0 and -0.0
            return a < b ? -1 : 1;
        }

        public static int FloatCompare(float a, float b)
        {
            if (float.IsNaN(a)) return float.IsNaN(b) ? 0 : 1;
            if (float.IsNaN(b)) return -1;
            if (a == b) return 0;
            if (a == 0.0f && b == 0.0f) return 0;
            return a < b ? -1 : 1;
        }

        // Integer.compare / Long.compare are exact .NET CompareTo.
        public static int IntCompare(int a, int b) => a.CompareTo(b);
        public static int LongCompare(long a, long b) => a.CompareTo(b);

        // ---- bit fiddling ----
        public static int ToUnsignedInt(byte b) => b & 0xFF;

        // Java JavaLangCompat.ToUnsignedLong(int) = i & 0xFFFFFFFFL
        public static long ToUnsignedLong(int i) => (long)(uint)i;

        public static int ReverseBytes(int i) =>
            System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(i);

        public static int BitCount(long l) =>
            System.Numerics.BitOperations.PopCount((ulong)l);

        // ---- decode (Java Integer.decode / Long.decode) ----
        // Supports: optional sign, then 0x/# (hex), 0 (octal), else decimal.
        public static int IntDecode(string s) => checked((int)Decode(s, 32));
        public static long LongDecode(string s) => Decode(s, 64);

        private static long Decode(string nm, int bits)
        {
            if (string.IsNullOrEmpty(nm)) throw new FormatException("For input string: \"" + nm + "\"");
            int sign = 1;
            int idx = 0;
            if (nm[idx] == '-') { sign = -1; idx++; }
            else if (nm[idx] == '+') { idx++; }

            int radix = 10;
            string digits;
            if (idx < nm.Length - 1 && nm[idx] == '0' && (nm[idx + 1] == 'x' || nm[idx + 1] == 'X'))
            {
                radix = 16; idx += 2; digits = nm.Substring(idx);
            }
            else if (idx < nm.Length - 1 && nm[idx] == '#')
            {
                radix = 16; idx++; digits = nm.Substring(idx);
            }
            else if (idx < nm.Length - 1 && nm[idx] == '0')
            {
                radix = 8; idx++; digits = nm.Substring(idx);
            }
            else
            {
                digits = nm.Substring(idx);
            }
            if (digits.Length == 0) throw new FormatException("For input string: \"" + nm + "\"");
            return sign * Convert.ToInt64(digits, radix);
        }

        // Java Character.isJavaIdentifierPart (approximation: letters, digits, '_', '$')
        public static bool IsJavaIdentifierPart(char c) =>
            char.IsLetterOrDigit(c) || c == '_' || c == '$';
    }
}
