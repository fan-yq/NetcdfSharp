// NetcdfSharp.java.util.zip.CRC32 shim for netcdf-java translation.
// Minimal subset used by the translated code: update / getValue / reset.
using System;

namespace NetcdfSharp.java.util.zip
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.util.zip.CRC32 (standard CRC-32 / IEEE 802.3).
    /// </summary>
    public class CRC32
    {
        private static readonly uint[] table = BuildTable();
        private uint crc;

        private static uint[] BuildTable()
        {
            var t = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                uint c = i;
                for (int k = 0; k < 8; k++)
                {
                    c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
                }
                t[i] = c;
            }
            return t;
        }

        public CRC32()
        {
        }

        public virtual void update(byte[] b)
        {
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            update(b, 0, b.Length);
        }

        public virtual void update(byte[] b, int off, int len)
        {
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            for (int i = off; i < off + len; i++)
            {
                crc = table[(crc ^ b[i]) & 0xFF] ^ (crc >> 8);
            }
        }

        public virtual void update(int b)
        {
            crc = table[(crc ^ (uint)b) & 0xFF] ^ (crc >> 8);
        }

        public virtual void reset()
        {
            crc = 0;
        }

        public virtual long getValue()
        {
            return (long)(crc ^ 0xFFFFFFFFu) & 0xFFFFFFFFL;
        }
    }
}
