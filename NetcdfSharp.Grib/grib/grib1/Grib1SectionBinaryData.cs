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
	using GribData = NetcdfSharp.ucar.nc2.grib.GribData;
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// The Binary Data Section for GRIB-1 files
	/// 
	/// @author caron
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib1SectionBinaryData
	public class Grib1SectionBinaryData
	{
	  private readonly int length;
	  private readonly long startingPosition;

	  /*
	   * FM 92-XI EXT. GRIB EDITION 1
	   * Section 4 – Binary data section
	   * Octet Contents
	   * 1–3 Length of section
	   * 4 Flag (see Code table 11) (first 4 bits). Number of unused bits at end of Section 4 (last 4 bits)
	   * 5–6 Scale factor (E)
	   * 7–10 Reference value (minimum of packed values)
	   * 11 Number of bits containing each packed value
	   * 12– depending on the flag value in octet 4
	   */

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Grib1SectionBinaryData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  internal Grib1SectionBinaryData(RandomAccessFile raf)
	  {
		startingPosition = raf.FilePointer;

		// octets 1-3 (Length of section)
		length = GribNumbers.uint3(raf);
		// if (length < 0)
		// throw new IllegalStateException("GRIB record has bad length, pos = " + startingPosition);
		raf.seek(startingPosition + length);
	  }

	  internal Grib1SectionBinaryData(long startingPosition, int length)
	  {
		this.startingPosition = startingPosition;
		this.length = length;
	  }

	  public virtual long StartingPosition
	  {
		  get
		  {
			return startingPosition;
		  }
	  }

	  public virtual int Length
	  {
		  get
		  {
			return length;
		  }
	  }

	  /////////////////////

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public byte[] getBytes(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual byte[] getBytes(RandomAccessFile raf)
	  {
		raf.seek(startingPosition); // go to the data section
		byte[] data = new byte[length];
		raf.readFully(data);
		return data;
	  }

	  // for debugging
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: NetcdfSharp.ucar.nc2.grib.GribData.Info getBinaryDataInfo(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  internal virtual GribData.Info getBinaryDataInfo(RandomAccessFile raf)
	  {
		return getBinaryDataInfo(raf, startingPosition);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static NetcdfSharp.ucar.nc2.grib.GribData.Info getBinaryDataInfo(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long start) throws NetcdfSharp.java.io.IOException
	  public static GribData.Info getBinaryDataInfo(RandomAccessFile raf, long start)
	  {
		raf.seek(start); // go to the data section

		GribData.Info info = new GribData.Info();

		info.dataLength = GribNumbers.uint3(raf); // // octets 1-3 (section length)

		// octet 4, 1st half (packing flag)
		info.flag = raf.read();

		// Y × 10^D = R + X × 2^E
		// octets 5-6 (E = binary scale factor)
		info.binaryScaleFactor = GribNumbers.int2(raf);

		// octets 7-10 (R = reference point = minimum value)
		info.referenceValue = GribNumbers.float4(raf);

		// octet 11 (number of bits per value)
		info.numberOfBits = raf.read();

		return info;
	  }



	}

}
