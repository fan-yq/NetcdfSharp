// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Grib1 Section 3 (BitMap)
	/// 
	/// @author John
	/// @since 9/3/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib1SectionBitMap
	public class Grib1SectionBitMap
	{
	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(Grib1SectionBitMap));

	  private readonly long startingPosition;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Grib1SectionBitMap(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  internal Grib1SectionBitMap(RandomAccessFile raf)
	  {
		startingPosition = raf.FilePointer;

		// octets 1-3 (Length of section)
		int length = GribNumbers.int3(raf);

		raf.seek(startingPosition + length);
	  }

	  internal Grib1SectionBitMap(long startingPosition)
	  {
		this.startingPosition = startingPosition;
	  }

	  public virtual long StartingPosition
	  {
		  get
		  {
			return startingPosition;
		  }
	  }

	  /// <summary>
	  /// Read the bitmap array when needed, return null if none.
	  /// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Nullable public byte[] getBitmap(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual byte[] getBitmap(RandomAccessFile raf)
	  {
		if (startingPosition <= 0)
		{
		  throw new System.InvalidOperationException("Grib1 Bit map has bad starting position");
		}

		raf.seek(startingPosition);

		// octet 1-3 (length of section)
		int length = GribNumbers.uint3(raf);

		// octet 4 unused bits
		raf.read(); // unused

		// octets 5-6
		int bm = raf.readShort();
		if (bm != 0)
		{
		  logger.warn("Grib1 Bit map section pre-defined (provided by center) bitmap number = {}", bm);
		  return null;
		}

		// not sure if length is set correctly when pre-define bitmap is used, so wait until that to test
		// seeing a -1, bail out
		if (length <= 6 || length > 10e6)
		{ // look max ??
		  return null;
		}

		// read the bits as integers
		int n = length - 6;
		byte[] data = new byte[n];
		raf.readFully(data);
		return data;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: int getLength(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  internal virtual int getLength(RandomAccessFile raf)
	  {
		raf.seek(startingPosition);
		return GribNumbers.uint3(raf);
	  }

	}

}
