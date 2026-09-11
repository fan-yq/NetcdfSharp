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
	/// The Indicator Section for GRIB-2 files
	/// 
	/// @author caron
	/// @since 3/28/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2SectionIndicator
	public class Grib2SectionIndicator
	{
	  private static readonly byte[] MAGIC = new byte[] {(byte)'G', (byte)'R', (byte)'I', (byte)'B'};

	  private readonly long messageLength;
	  private readonly int discipline;
	  private readonly long startPos;

	  /// <summary>
	  /// Read Grib2SectionIndicator from raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile, with pointer at start (the "GRIB") </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on I/O error </exception>
	  /// <exception cref="IllegalArgumentException"> if not a GRIB-2 record </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2SectionIndicator(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib2SectionIndicator(RandomAccessFile raf)
	  {
		startPos = raf.FilePointer;
		byte[] b = new byte[4];
		raf.readFully(b);
		for (int i = 0; i < b.Length; i++)
		{
		  if (b[i] != MAGIC[i])
		  {
			throw new ArgumentException("Not a GRIB record");
		  }
		}

		raf.skipBytes(2);
		discipline = raf.read();
		int edition = raf.read();
		if (edition != 2)
		{
		  throw new ArgumentException("Not a GRIB-2 record");
		}

		messageLength = GribNumbers.int8(raf);
	  }

	  public Grib2SectionIndicator(long startPos, long messageLength, int discipline)
	  {
		this.startPos = startPos;
		this.messageLength = messageLength;
		this.discipline = discipline;
	  }

	  /// <summary>
	  /// Get the length of this GRIB record in bytes.
	  /// </summary>
	  /// <returns> length in bytes of GRIB record </returns>
	  public virtual long MessageLength
	  {
		  get
		  {
			return messageLength;
		  }
	  }

	  public virtual long StartPos
	  {
		  get
		  {
			return startPos;
		  }
	  }

	  public virtual long EndPos
	  {
		  get
		  {
			return startPos + messageLength;
		  }
	  }

	  /// <summary>
	  /// Discipline - GRIB Master Table Number.
	  /// </summary>
	  /// <returns> discipline number </returns>
	  public virtual int Discipline
	  {
		  get
		  {
			return discipline;
		  }
	  }
	}

}
