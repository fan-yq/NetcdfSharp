// NetcdfSharp.java.util.Arrays - minimal subset used by NetcdfSharp.
using System;
using System.Text;

namespace NetcdfSharp.java.util
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.util.Arrays static helpers.
    /// </summary>
    public static class Arrays
    {
        public static int hashCode(int[] a)
        {
            if (a == null) return 0;
            int result = 1;
            foreach (int v in a) result = 31 * result + v;
            return result;
        }

        public static int hashCode(double[] a)
        {
            if (a == null) return 0;
            int result = 1;
            foreach (double v in a) result = 31 * result + (int)(BitConverter.DoubleToInt64Bits(v) ^ (BitConverter.DoubleToInt64Bits(v) >> 32));
            return result;
        }

        public static int hashCode(object[] a)
        {
            if (a == null) return 0;
            int result = 1;
            foreach (object v in a) result = 31 * result + (v != null ? v.GetHashCode() : 0);
            return result;
        }

        public static string toString(int[] a)
        {
            if (a == null) return "null";
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(a[i]);
            }
            return sb.Append(']').ToString();
        }

        public static string toString(double[] a)
        {
            if (a == null) return "null";
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(a[i]);
            }
            return sb.Append(']').ToString();
        }

        public static string toString(object[] a)
        {
            if (a == null) return "null";
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(a[i] == null ? "null" : a[i].ToString());
            }
            return sb.Append(']').ToString();
        }
    }
}
