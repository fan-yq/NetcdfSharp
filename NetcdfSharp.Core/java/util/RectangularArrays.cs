// NetcdfSharp.java.util.RectangularArrays helper - reproduces Java's rectangular array initialization
// (new T[n][m] creates all sub-arrays) which C# jagged arrays do not.
using System;

namespace NetcdfSharp.java.util
{
    /// <summary>Static helpers creating fully-initialized rectangular (jagged) arrays.</summary>
    public static class RectangularArrays
    {
        public static int[][] RectangularIntArray(int rows, int cols)
        {
            int[][] result = new int[rows][];
            for (int i = 0; i < rows; i++) result[i] = new int[cols];
            return result;
        }

        public static char[][] RectangularCharArray(int rows, int cols)
        {
            char[][] result = new char[rows][];
            for (int i = 0; i < rows; i++) result[i] = new char[cols];
            return result;
        }

        public static byte[][] RectangularByteArray(int rows, int cols)
        {
            byte[][] result = new byte[rows][];
            for (int i = 0; i < rows; i++) result[i] = new byte[cols];
            return result;
        }

        public static short[][] RectangularShortArray(int rows, int cols)
        {
            short[][] result = new short[rows][];
            for (int i = 0; i < rows; i++) result[i] = new short[cols];
            return result;
        }

        public static long[][] RectangularLongArray(int rows, int cols)
        {
            long[][] result = new long[rows][];
            for (int i = 0; i < rows; i++) result[i] = new long[cols];
            return result;
        }

        public static float[][] RectangularFloatArray(int rows, int cols)
        {
            float[][] result = new float[rows][];
            for (int i = 0; i < rows; i++) result[i] = new float[cols];
            return result;
        }

        public static double[][] RectangularDoubleArray(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; i++) result[i] = new double[cols];
            return result;
        }
    }
}
