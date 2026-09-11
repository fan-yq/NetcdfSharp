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
	/// The Product Definition section 4 for GRIB-2 files
	/// 
	/// @author caron
	/// @since 3/28/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2SectionProductDefinition
	public class Grib2SectionProductDefinition
	{
	  private readonly byte[] rawData;
	  private readonly int templateNumber;

	  /// <summary>
	  /// Read Product Definition section from raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile, with pointer at start of section </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on I/O error </exception>
	  /// <exception cref="IllegalArgumentException"> if not a GRIB-2 record </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2SectionProductDefinition(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib2SectionProductDefinition(RandomAccessFile raf)
	  {

		long startingPosition = raf.FilePointer;

		// octets 1-4 (Length of GDS)
		int length = GribNumbers.int4(raf);

		// octet 5
		int section = raf.read();
		if (section != 4)
		{
		  throw new ArgumentException("Not a GRIB-2 PDS section");
		}

		// octets 8-9
		raf.skipBytes(2);
		templateNumber = GribNumbers.int2(raf);

		// read in whole GDS as byte[]
		rawData = new byte[length];
		raf.seek(startingPosition);
		raf.readFully(rawData);
	  }

	  /// <summary>
	  /// Set PDS section from byte array.
	  /// </summary>
	  /// <param name="rawData"> the byte array </param>
	  public Grib2SectionProductDefinition(byte[] rawData)
	  {
		this.rawData = rawData;
		this.templateNumber = GribNumbers.int2(getInt(8), getInt(9));
	  }

	  /// <summary>
	  /// get the raw bytes of the PDS
	  /// </summary>
	  /// <returns> PDS as byte[] </returns>
	  public virtual byte[] RawBytes
	  {
		  get
		  {
			return rawData;
		  }
	  }

	  /// <summary>
	  /// Calculate the CRC of the entire byte array
	  /// </summary>
	  /// <returns> CRC </returns>
	  public virtual long calcCRC()
	  {
		CRC32 crc32 = new CRC32();
		crc32.update(rawData);
		return crc32.getValue();
	  }

	  public virtual int Length
	  {
		  get
		  {
			return rawData.Length;
		  }
	  }

	  /// <summary>
	  /// Get PDS Template Number (code table 4.0)
	  /// </summary>
	  /// <returns> PDS Template Number </returns>
	  public virtual int PDSTemplateNumber
	  {
		  get
		  {
			return templateNumber;
		  }
	  }

	  private int getInt(int index)
	  {
		return rawData[index - 1] & 0xff;
	  }

	  /// <summary>
	  /// Parse the raw bytes into a Grib2Pds
	  /// </summary>
	  /// <returns> Grib2Pds </returns>
	  public virtual Grib2Pds PDS
	  {
		  get
		  {
			return Grib2Pds.factory(templateNumber, rawData);
		  }
	  }
	}

}
