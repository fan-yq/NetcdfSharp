// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.iosp.hdf5
{
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Temporary workaround to share code between H5header and internal/iosp/H5headerNew.
	/// Should go away in version 6.
	/// </summary>
	public interface H5headerIF
	{

	  RandomAccessFile RandomAccessFile {get;}

	  long getFileOffset(long address);

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: long readOffset() throws NetcdfSharp.java.io.IOException;
	  long readOffset();

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: long readLength() throws NetcdfSharp.java.io.IOException;
	  long readLength();

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: long readVariableSizeUnsigned(int i) throws NetcdfSharp.java.io.IOException;
	  long readVariableSizeUnsigned(int i);

	  byte SizeOffsets {get;}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: long readAddress() throws NetcdfSharp.java.io.IOException;
	  long readAddress();

	  byte SizeLengths {get;}

	  int getNumBytesFromMax(long l);

	  int makeIntFromBytes(byte[] heapId, int i, int n);

	  bool OffsetLong {get;}
	}

}
