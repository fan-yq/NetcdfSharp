// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// The Local Use section 2 for GRIB-2 files
	/// 
	/// @author caron
	/// @since 3/28/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2SectionLocalUse
	public class Grib2SectionLocalUse
	{

	  private readonly byte[] rawData;

	  /// <summary>
	  /// Read Grib2SectionLocalUse from raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile, with pointer at start od section </param>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2SectionLocalUse(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib2SectionLocalUse(RandomAccessFile raf)
	  {

		// octets 1-4 (Length of GDS)
		int length = GribNumbers.int4(raf);
		int section = raf.read(); // This is section 2

		if (section != 2)
		{ // no local use section
		  raf.skipBytes(-5);
		  rawData = null;
		}
		else
		{
		  rawData = new byte[length - 5];
		  raf.readFully(rawData);
		}
	  }

	  internal Grib2SectionLocalUse(byte[] rawData)
	  {
		this.rawData = rawData;
	  }

	  public virtual byte[] RawBytes
	  {
		  get
		  {
			return rawData;
		  }
	  }
	}

}
