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
	/// The Data Representation section (5) for GRIB-2 files
	/// 
	/// @author caron
	/// @since 3/29/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2SectionDataRepresentation
	public class Grib2SectionDataRepresentation
	{
	  private readonly long startingPosition;
	  private readonly int dataPoints;
	  private readonly int dataTemplate;
	  private readonly int length; // dont have length in index

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2SectionDataRepresentation(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib2SectionDataRepresentation(RandomAccessFile raf)
	  {
		startingPosition = raf.FilePointer;

		// octets 1-4 (Length of DRS)
		length = GribNumbers.int4(raf);
		if (length == 0)
		{
		  throw new ArgumentException("Not a GRIB-2 Data representation section");
		}

		// octet 5
		int section = raf.read();
		if (section != 5)
		{
		  throw new ArgumentException("Not a GRIB-2 Data representation section");
		}

		// octets 6-9 number of datapoints
		dataPoints = GribNumbers.int4(raf);

		// octet 10
		int dt = GribNumbers.uint2(raf);
		dataTemplate = (dt == 40000) ? 40 : dt; // ?? NCEP bug ??

		raf.seek(startingPosition + length);
	  }

	  public Grib2SectionDataRepresentation(long startingPosition, int dataPoints, int dataTemplate)
	  {
		this.startingPosition = startingPosition;
		this.dataPoints = dataPoints;
		this.dataTemplate = dataTemplate;
		this.length = 0;
	  }

	  /*
	   * Number of data points where one or more values are specified in Section 7 when a bit map
	   * is present, total number of data points when a bit map is absent.
	   */
	  public virtual int DataPoints
	  {
		  get
		  {
			return dataPoints;
		  }
	  }

	  public virtual int DataTemplate
	  {
		  get
		  {
			return dataTemplate;
		  }
	  }

	  public virtual long StartingPosition
	  {
		  get
		  {
			return startingPosition;
		  }
	  }

	  // debug
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public long getLength(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual long getLength(RandomAccessFile raf)
	  {
		if (length == 0)
		{
		  raf.seek(startingPosition);
		  return GribNumbers.int4(raf);
		}
		return length;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2Drs getDrs(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual Grib2Drs getDrs(RandomAccessFile raf)
	  {
		raf.seek(startingPosition + 11);
		return Grib2Drs.factory(dataTemplate, raf);
	  }

	  public override string ToString()
	  {
		return "Grib2SectionDataRepresentation{" + "startingPosition=" + startingPosition + ", dataPoints=" + dataPoints + ", dataTemplate=" + dataTemplate + ", length=" + length + '}';
	  }
	}

}
