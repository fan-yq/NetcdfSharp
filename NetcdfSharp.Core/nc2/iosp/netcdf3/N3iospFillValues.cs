// Default netCDF fill values, extracted from the legacy NetcdfSharp.ucar.nc2.iosp.netcdf3.N3iosp class.
// Used by the HDF5/netCDF-3 read paths when a variable has no explicit _FillValue.
using NetcdfSharp.ucar.ma2;

namespace NetcdfSharp.ucar.nc2.iosp.netcdf3
{
    /// <summary>Default fill values per netCDF data type (from N3iosp).</summary>
    public static class N3iospFillValues
    {
        // Default fill values, used unless _FillValue variable attribute is set.
        public const byte NC_FILL_BYTE = unchecked((byte)129); // -127 as unsigned byte
        public static readonly char NC_FILL_CHAR = (char)0;
        public static readonly short NC_FILL_SHORT = (short)-32767;
        public const int NC_FILL_INT = -2147483647;
        public const float NC_FILL_FLOAT = 9.9692099683868690e+36f; // near 15 * 2^119
        public const double NC_FILL_DOUBLE = 9.9692099683868690e+36;
        public static readonly byte NC_FILL_UBYTE = unchecked((byte)255);
        public static readonly short NC_FILL_USHORT = unchecked((short)65535);
        public static readonly int NC_FILL_UINT = unchecked((int)4294967295L);
        public const long NC_FILL_INT64 = -9223372036854775806L; // 0x8000000000000002. Only bits 63 and 1 set.
        public const long NC_FILL_UINT64 = unchecked((long)0xfffffffffffffffeL);
        public const string NC_FILL_STRING = "";

        public static object getFillValueDefault(DataType dtype)
        {
            if ((dtype == DataType.BYTE) || (dtype == DataType.ENUM1))
            {
                return NC_FILL_BYTE;
            }
            if (dtype == DataType.UBYTE)
            {
                return NC_FILL_UBYTE;
            }
            if (dtype == DataType.CHAR)
            {
                return (byte)0;
            }
            if ((dtype == DataType.SHORT) || (dtype == DataType.ENUM2))
            {
                return NC_FILL_SHORT;
            }
            if (dtype == DataType.USHORT)
            {
                return NC_FILL_USHORT;
            }
            if ((dtype == DataType.INT) || (dtype == DataType.ENUM4))
            {
                return NC_FILL_INT;
            }
            if (dtype == DataType.UINT)
            {
                return NC_FILL_UINT;
            }
            if (dtype == DataType.LONG)
            {
                return NC_FILL_INT64;
            }
            if (dtype == DataType.ULONG)
            {
                return NC_FILL_UINT64;
            }
            if (dtype == DataType.FLOAT)
            {
                return NC_FILL_FLOAT;
            }
            if (dtype == DataType.DOUBLE)
            {
                return NC_FILL_DOUBLE;
            }
            if (dtype == DataType.STRING)
            {
                return NC_FILL_STRING;
            }
            return null;
        }
    }
}
