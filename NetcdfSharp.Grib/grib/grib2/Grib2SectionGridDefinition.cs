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
	/// The Grid Definition section 3 for GRIB-2 files
	/// 
	/// <pre>
	///  Octet Contents
	///  1–4   Length of section in octets (nn)
	///  5     Number of section (3)
	///  6     Source of grid definition (see Code table 3.0 and Note 1)
	///  7–10  Number of data points
	///  11    Number of octets for optional list of numbers (see Note 2)
	///  12    Interpretation of list of numbers (see Code table 3.11)
	///  13–14 Grid definition template number (= N) (see Code table 3.1)
	///  15–xx Grid definition template (see Template 3.N, where N is the grid definition template number given in octets 13–14)
	///  [xx+1]–nn Optional list of numbers defining number of points (see Notes 2, 3 and 4)
	/// </pre>
	/// 
	/// Effectively immutable, but caching lazy gds
	/// 
	/// @author caron
	/// @since 3/28/11
	/// </summary>
	// @Immutable
	public class Grib2SectionGridDefinition
	{
	  private readonly byte[] rawData;
	  private readonly long startingPosition;
	  private readonly int templateNumber;

	  /// <summary>
	  /// Read Grib Definition section from raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile, with pointer at start of section </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on I/O error </exception>
	  /// <exception cref="IllegalArgumentException"> if not a GRIB-2 record </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2SectionGridDefinition(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib2SectionGridDefinition(RandomAccessFile raf)
	  {

		startingPosition = raf.FilePointer;

		// octets 1-4 (Length of GDS)
		int length = GribNumbers.int4(raf);

		// octet 5
		int section = raf.read(); // This is section 3
		if (section != 3)
		{
		  throw new ArgumentException("Not a GRIB-2 GDS section");
		}

		// octets 13-14
		raf.skipBytes(7);
		templateNumber = GribNumbers.uint2(raf);

		// read in whole GDS as byte[]
		rawData = new byte[length];
		raf.seek(startingPosition);
		raf.readFully(rawData);
	  }

	  /// <summary>
	  /// Set Grib Definition section from byte array.
	  /// </summary>
	  /// <param name="rawData"> the byte array </param>
	  public Grib2SectionGridDefinition(byte[] rawData)
	  {
		this.rawData = rawData;
		this.templateNumber = GribNumbers.uint2(getOctet(13), getOctet(14));
		this.startingPosition = -1;
	  }

	  /// <summary>
	  /// get the raw bytes of the GDS
	  /// </summary>
	  /// <returns> GDS as byte[] </returns>
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
	  /// <returns> CRC of the entire byte array </returns>
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

	  public virtual long Offset
	  {
		  get
		  {
			return startingPosition;
		  }
	  }

	  /// <summary>
	  /// octet 6
	  /// source of grid definition (Code Table 3.0)
	  /// "If octet 6 is not zero, octets 15–xx (15–nn if octet 11 is zero) may not be supplied. This should be documented
	  /// with all
	  /// bits set to 1 (missing value) in the grid definition template number."
	  /// </summary>
	  /// <returns> source </returns>
	  public virtual int Source
	  {
		  get
		  {
			return getOctet(6);
		  }
	  }

	  /// <summary>
	  /// octets 7-10
	  /// number of data points .
	  /// </summary>
	  /// <returns> numberPoints </returns>
	  public virtual int NumberPoints
	  {
		  get
		  {
			return GribNumbers.int4(getOctet(7), getOctet(8), getOctet(9), getOctet(10));
		  }
	  }

	  /// <summary>
	  /// Get GDS Template number (code table 3.1)
	  /// </summary>
	  /// <returns> GDS Template number. </returns>
	  public virtual int GDSTemplateNumber
	  {
		  get
		  {
			return templateNumber;
		  }
	  }

	  /// <summary>
	  /// Get the the byte in the raw Data
	  /// </summary>
	  /// <param name="index"> in the raw array </param>
	  /// <returns> byte[index-1] as int </returns>
	  private int getOctet(int index)
	  {
		return rawData[index - 1] & 0xff;
	  }

	  private Grib2Gds gds2;

	  public virtual Grib2Gds GDS
	  {
		  get
		  {
			  lock (this)
			  {
				if (gds2 == null)
				{
				  gds2 = Grib2Gds.factory(templateNumber, rawData);
				}
				return gds2;
			  }
		  }
	  }

	}

}
