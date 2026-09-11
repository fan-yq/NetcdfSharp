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


namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// The Indicator Section for GRIB-1 files
	/// 
	/// @author caron
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib1SectionIndicator
	public class Grib1SectionIndicator
	{
	  internal static readonly byte[] MAGIC = new byte[] {(byte)'G', (byte)'R', (byte)'I', (byte)'B'};

	  private readonly long messageLength;
	  private readonly long startPos;
	  private readonly int messageLengthNotFixed;
	  internal bool isMessageLengthFixed;

	  /// <summary>
	  /// Read Grib2SectionIndicator from raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile, with pointer at start (the "GRIB") </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on I/O error </exception>
	  /// <exception cref="IllegalArgumentException"> if not a GRIB-2 record </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Grib1SectionIndicator(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  internal Grib1SectionIndicator(RandomAccessFile raf)
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

		messageLengthNotFixed = GribNumbers.uint3(raf);
		int edition = raf.read();
		if (edition != 1)
		{
		  throw new ArgumentException("Not a GRIB-1 record");
		}
		messageLength = Grib1RecordScanner.getFixedTotalLengthEcmwfLargeGrib(raf, messageLengthNotFixed);
		if (messageLength != messageLengthNotFixed)
		{
		  isMessageLengthFixed = true;
		}
	  }

	  internal Grib1SectionIndicator(long startPos, long messageLength)
	  {
		this.startPos = startPos;
		this.messageLength = messageLength;
		this.messageLengthNotFixed = (int) messageLength;
		this.isMessageLengthFixed = false;
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

	  /// <summary>
	  /// Starting position of the entire GRIB message: the 'G' in GRIB
	  /// </summary>
	  /// <returns> the starting position of the entire GRIB message </returns>
	  public virtual long StartPos
	  {
		  get
		  {
			return startPos;
		  }
	  }

	  /// <summary>
	  /// Ending position of the entire GRIB message: startPos + messageLength
	  /// </summary>
	  /// <returns> the ending position of the entire GRIB message </returns>
	  public virtual long EndPos
	  {
		  get
		  {
			return startPos + messageLength;
		  }
	  }

	}


}
