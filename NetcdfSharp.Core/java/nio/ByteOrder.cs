// NetcdfSharp.java.nio.ByteOrder compatible - minimal subset used by NetcdfSharp.
using System;

namespace NetcdfSharp.java.nio
{
    /// <summary>
    /// Byte order for buffers. Simplified replacement for NetcdfSharp.java.nio.ByteOrder.
    /// </summary>
    public sealed class ByteOrder
    {
        public static readonly ByteOrder BIG_ENDIAN = new ByteOrder("BIG_ENDIAN", true);
        public static readonly ByteOrder LITTLE_ENDIAN = new ByteOrder("LITTLE_ENDIAN", false);

        private readonly string _name;
        internal readonly bool BigEndian;

        private ByteOrder(string name, bool bigEndian)
        {
            _name = name;
            BigEndian = bigEndian;
        }

        public static ByteOrder nativeOrder()
        {
            return BitConverter.IsLittleEndian ? LITTLE_ENDIAN : BIG_ENDIAN;
        }

        public override string ToString() => _name;
    }
}
