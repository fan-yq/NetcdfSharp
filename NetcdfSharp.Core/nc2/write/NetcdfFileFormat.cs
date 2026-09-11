// NetcdfSharp.ucar.nc2.write.NetcdfFileFormat shim: NetCDF file format detection.
// Replicates the subset of NetcdfSharp.ucar.nc2.write.NetcdfFileFormat used by the read path.
using System;

namespace NetcdfSharp.ucar.nc2.write
{
    /// <summary>NetCDF file formats and detection. Subset of NetcdfSharp.ucar.nc2.write.NetcdfFileFormat.</summary>
    public class NetcdfFileFormat
    {
        /// <summary>Enum values identifying a format.</summary>
        public enum InnerEnum
        {
            INVALID,
            NETCDF3,
            NETCDF3_64BIT_OFFSET,
            NETCDF4,
            NETCDF4_CLASSIC,
            NETCDF3_64BIT_DATA,
            NCSTREAM,
        }

        public readonly int versionValue;
        public readonly string formatName;
        public readonly InnerEnum innerEnumValue;

        private NetcdfFileFormat(InnerEnum value, int version, string formatName)
        {
            this.innerEnumValue = value;
            this.versionValue = version;
            this.formatName = formatName;
        }

        public static readonly NetcdfFileFormat INVALID = new NetcdfFileFormat(InnerEnum.INVALID, 0, "Invalid");
        public static readonly NetcdfFileFormat NETCDF3 = new NetcdfFileFormat(InnerEnum.NETCDF3, 1, "netcdf-3");
        public static readonly NetcdfFileFormat NETCDF3_64BIT_OFFSET = new NetcdfFileFormat(InnerEnum.NETCDF3_64BIT_OFFSET, 2, "netcdf-3 64bit-offset");
        public static readonly NetcdfFileFormat NETCDF4 = new NetcdfFileFormat(InnerEnum.NETCDF4, 3, "netcdf-4");
        public static readonly NetcdfFileFormat NETCDF4_CLASSIC = new NetcdfFileFormat(InnerEnum.NETCDF4_CLASSIC, 4, "netcdf-4 classic");
        public static readonly NetcdfFileFormat NETCDF3_64BIT_DATA = new NetcdfFileFormat(InnerEnum.NETCDF3_64BIT_DATA, 5, "netcdf-5");
        public static readonly NetcdfFileFormat NCSTREAM = new NetcdfFileFormat(InnerEnum.NCSTREAM, 42, "ncstream");

        public int version() => versionValue;
        public bool isNetcdf3Format() => innerEnumValue == InnerEnum.NETCDF3 || innerEnumValue == InnerEnum.NETCDF3_64BIT_OFFSET || innerEnumValue == InnerEnum.NETCDF3_64BIT_DATA;
        public bool isNetcdf4Format() => innerEnumValue == InnerEnum.NETCDF4 || innerEnumValue == InnerEnum.NETCDF4_CLASSIC;
        public override string ToString() => formatName;

        public static bool memequal(byte[] b1, byte[] b2, int len)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length < len || b2.Length < len) return false;
            for (int i = 0; i < len; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }

        public static bool memequal(byte[] b1, int start, byte[] b2, int len)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length - start < len || b2.Length < len) return false;
            for (int i = 0; i < len; i++)
            {
                if (b1[start + i] != b2[i]) return false;
            }
            return true;
        }

        /// <summary>Detect the NetCDF file format from the magic numbers at the start of a file.</summary>
        public static NetcdfFileFormat findNetcdfFormatType(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf)
        {
            byte[] buf = new byte[8];
            try
            {
                raf.seek(0);
                raf.readFully(buf);
                raf.seek(0);
            }
            catch (Exception)
            {
                return INVALID;
            }
            // netCDF magic: C D F \x01 / \x02 / \x05
            if (buf[0] == (byte)'C' && buf[1] == (byte)'D' && buf[2] == (byte)'F')
            {
                switch (buf[3])
                {
                    case 1: return NETCDF3;
                    case 2: return NETCDF3_64BIT_OFFSET;
                    case 5: return NETCDF3_64BIT_DATA;
                }
                return INVALID;
            }
            // HDF5 magic: 0x89 H D F \r \n \x1a \n
            byte[] hdf5Magic = { 0x89, (byte)'H', (byte)'D', (byte)'F', 0x0d, 0x0a, 0x1a, 0x0a };
            if (memequal(buf, hdf5Magic, 8))
            {
                return NETCDF4; // could be HDF5 that is not netCDF-4; treated as netcdf-4 for detection
            }
            return INVALID;
        }
    }
}
