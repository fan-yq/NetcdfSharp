// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// The BitMap section 6 for GRIB-2 files
	/// 
	/// @author caron
	/// @since 3/29/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2SectionBitMap
	public class Grib2SectionBitMap
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2SectionBitMap));

	  private readonly long startingPosition;
	  private readonly int bitMapIndicator;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static Grib2SectionBitMap factory(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long startingPos) throws NetcdfSharp.java.io.IOException
	  public static Grib2SectionBitMap factory(RandomAccessFile raf, long startingPos)
	  {
		raf.seek(startingPos);
		return new Grib2SectionBitMap(raf);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2SectionBitMap(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib2SectionBitMap(RandomAccessFile raf)
	  {
		startingPosition = raf.FilePointer;

		// octets 1-4 (Length of section)
		int length = GribNumbers.int4(raf);

		// octet 5
		int section = raf.read();
		if (section != 6)
		{
		  throw new ArgumentException("Not a GRIB-2 Bitmap section");
		}

		// octet 6
		bitMapIndicator = raf.read();
		raf.seek(startingPosition + length);
	  }

	  internal Grib2SectionBitMap(long startingPosition, int bitMapIndicator)
	  {
		this.startingPosition = startingPosition;
		this.bitMapIndicator = bitMapIndicator;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: int getLength(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  internal virtual int getLength(RandomAccessFile raf)
	  {
		raf.seek(startingPosition);
		return GribNumbers.int4(raf);
	  }

	  /*
	   * Code Table Code table 6.0 - Bit map indicator (6.0)
	   * 0: A bit map applies to this product and is specified in this Section
	   * -1: A bit map predetermined by the originating/generating centre applies to this product and is not specified in
	   * this Section
	   * 254: A bit map defined previously in the same "GRIB" message applies to this product
	   * 255: A bit map does not apply to this product
	   */
	  public virtual int BitMapIndicator
	  {
		  get
		  {
			return bitMapIndicator;
		  }
	  }

	  public virtual long StartingPosition
	  {
		  get
		  {
			return startingPosition;
		  }
	  }

	  /// <summary>
	  /// Read the bit map array.
	  /// </summary>
	  /// <param name="raf"> read from here </param>
	  /// <returns> bit map as array of byte values, or null if none. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Nullable public byte[] getBitmap(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual byte[] getBitmap(RandomAccessFile raf)
	  {
		// no bitMap
		if (bitMapIndicator == 255)
		{
		  return null;
		}

		// LOOK: bitMapIndicator=254 == previously defined bitmap
		if (bitMapIndicator == 254)
		{
		  logger.debug("bitMapIndicator=254 previously defined bitmap");
		}

		if (bitMapIndicator != 0)
		{
		  throw new System.NotSupportedException("Grib2 Bit map section pre-defined (provided by center) = " + bitMapIndicator);
		}

		raf.seek(startingPosition);
		int length = GribNumbers.int4(raf);
		raf.skipBytes(2);

		byte[] data = new byte[length - 6];
		raf.readFully(data);

		return data;
	  }

	  public override string ToString()
	  {
		return MoreObjects.toStringHelper(this).add("startingPosition", startingPosition).add("bitMapIndicator", bitMapIndicator).ToString();
	  }
	}

}
