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
	/// The Data section 7 for GRIB-2 files
	/// 
	/// @author caron
	/// @since 3/29/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2SectionData
	public class Grib2SectionData
	{

	  private readonly long startingPosition;
	  private readonly int msgLength;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2SectionData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib2SectionData(RandomAccessFile raf)
	  {
		startingPosition = raf.FilePointer;

		// octets 1-4 (Length of section)
		msgLength = GribNumbers.int4(raf);

		// octet 5
		int section = raf.read();
		if (section != 7)
		{
		  throw new System.InvalidOperationException("Not a Grib2SectionData (section 7)");
		}

		// skip to end of the data section
		raf.seek(startingPosition + msgLength);
	  }

	  internal Grib2SectionData(long startingPosition, int msgLength)
	  {
		this.startingPosition = startingPosition;
		this.msgLength = msgLength;
	  }

	  public virtual long StartingPosition
	  {
		  get
		  {
			return startingPosition;
		  }
	  }

	  public virtual long EndingPosition
	  {
		  get
		  {
			return startingPosition + msgLength;
		  }
	  }

	  public virtual int MsgLength
	  {
		  get
		  {
			return msgLength;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public byte[] getBytes(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public virtual byte[] getBytes(RandomAccessFile raf)
	  {
		raf.seek(startingPosition); // go to the data section
		byte[] data = new byte[msgLength];
		raf.readFully(data);
		return data;
	  }

	  public override string ToString()
	  {
		return "Grib2SectionData{" + "startingPosition=" + startingPosition + ", msgLength=" + msgLength + '}';
	  }
	}

}
