// Guava (NetcdfSharp.com.google.common.math) compatible helpers - minimal subset used by NetcdfSharp.
using System;

namespace NetcdfSharp.com.google.common.math
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.math.DoubleMath.
    /// </summary>
    public static class DoubleMath
    {
        /// <summary>
        /// Compares two doubles allowing a tolerance: returns -1, 0 or 1 like Double.compare
        /// once the values differ by more than tolerance.
        /// </summary>
        public static int fuzzyCompare(double a, double b, double tolerance)
        {
            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return double.IsNaN(a) && double.IsNaN(b) ? 0 : (double.IsNaN(a) ? 1 : -1);
            }
            if (a == b)
            {
                return 0;
            }
            return Math.Abs(a - b) < tolerance ? 0 : (a < b ? -1 : 1);
        }

        public static bool fuzzyEquals(double a, double b, double tolerance)
        {
            return fuzzyCompare(a, b, tolerance) == 0;
        }

        public static bool isMathematicalInteger(double x)
        {
            return !double.IsNaN(x) && !double.IsInfinity(x) && x == Math.Floor(x);
        }

        public static int roundToInt(double x)
        {
            return checked((int)Math.Round(x, MidpointRounding.AwayFromZero));
        }

        public static double log2(double x)
        {
            return Math.Log(x, 2.0);
        }

        public static double log2(double x, object roundingMode)
        {
            return Math.Log(x, 2.0);
        }
    }
}
