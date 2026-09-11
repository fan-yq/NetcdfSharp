// Guava (NetcdfSharp.com.google.common.primitives) compatible helpers - minimal subset used by NetcdfSharp.
using System;

namespace NetcdfSharp.com.google.common.primitives
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.primitives.Ints.
    /// </summary>
    public static class Ints
    {
        public static byte[] toByteArray(int value)
        {
            byte[] result = new byte[4];
            result[0] = (byte)(value >> 24);
            result[1] = (byte)(value >> 16);
            result[2] = (byte)(value >> 8);
            result[3] = (byte)value;
            return result;
        }

        public static int fromByteArray(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
            {
                throw new ArgumentException("array too small: " + (bytes == null ? 0 : bytes.Length) + " < 4");
            }
            return (bytes[0] & 0xFF) << 24
                 | (bytes[1] & 0xFF) << 16
                 | (bytes[2] & 0xFF) << 8
                 | (bytes[3] & 0xFF);
        }

        public static int compare(int a, int b)
        {
            return a < b ? -1 : (a > b ? 1 : 0);
        }

        public static int[] concat(params int[][] arrays)
        {
            if (arrays == null || arrays.Length == 0)
            {
                return new int[0];
            }
            int total = 0;
            foreach (int[] a in arrays)
            {
                if (a != null)
                {
                    total += a.Length;
                }
            }
            int[] result = new int[total];
            int pos = 0;
            foreach (int[] a in arrays)
            {
                if (a != null)
                {
                    System.Array.Copy(a, 0, result, pos, a.Length);
                    pos += a.Length;
                }
            }
            return result;
        }

        public static bool contains(int[] array, int target)
        {
            return array != null && System.Array.IndexOf(array, target) >= 0;
        }

        public static int indexOf(int[] array, int target)
        {
            return array == null ? -1 : System.Array.IndexOf(array, target);
        }

        public static int min(params int[] array)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException("empty array");
            }
            int m = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] < m)
                {
                    m = array[i];
                }
            }
            return m;
        }

        public static int max(params int[] array)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException("empty array");
            }
            int m = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > m)
                {
                    m = array[i];
                }
            }
            return m;
        }

        public static int constrainToRange(int value, int min, int max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        public static int saturatedCast(long value)
        {
            if (value > int.MaxValue)
            {
                return int.MaxValue;
            }
            if (value < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)value;
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.com.google.common.primitives.Longs.
    /// </summary>
    public static class Longs
    {
        public static byte[] toByteArray(long value)
        {
            byte[] result = new byte[8];
            for (int i = 7; i >= 0; i--)
            {
                result[i] = (byte)(value & 0xFF);
                value >>= 8;
            }
            return result;
        }

        public static long fromByteArray(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 8)
            {
                throw new ArgumentException("array too small: " + (bytes == null ? 0 : bytes.Length) + " < 8");
            }
            long result = 0;
            for (int i = 0; i < 8; i++)
            {
                result = (result << 8) | (long)(bytes[i] & 0xFF);
            }
            return result;
        }

        public static int compare(long a, long b)
        {
            return a < b ? -1 : (a > b ? 1 : 0);
        }
    }
}
