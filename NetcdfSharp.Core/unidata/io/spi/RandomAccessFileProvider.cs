// NetcdfSharp.ucar.unidata.io.spi.RandomAccessFileProvider shim.
// Service provider interface for RandomAccessFile implementations.
using System;

namespace NetcdfSharp.ucar.unidata.io.spi
{
    /// <summary>A service provider that can open a RandomAccessFile for a location it owns.</summary>
    public interface RandomAccessFileProvider
    {
        bool isOwnerOf(string location);
        NetcdfSharp.ucar.unidata.io.RandomAccessFile open(string location);
        NetcdfSharp.ucar.unidata.io.RandomAccessFile open(string location, int bufferSize);
    }
}
