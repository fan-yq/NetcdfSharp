// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// A class representing the IndicatorSection (section 0) of a BUFR record.
	/// Handles editions 2,3,4.
	/// 
	/// @author Robb Kambic
	/// @author caron
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class BufrIndicatorSection
	public class BufrIndicatorSection
	{
	  private readonly long startPos;
	  private readonly int bufrLength; // Length in bytes of BUFR record.
	  private readonly int edition;

	  // *** constructors *******************************************************

	  /// <summary>
	  /// Constructs a <tt>BufrIndicatorSection</tt> object from a raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile with IndicatorSection content </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public BufrIndicatorSection(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public BufrIndicatorSection(RandomAccessFile raf)
	  {
		this.startPos = raf.FilePointer - 4; // start of BUFR message, including "BUFR"
		bufrLength = BufrNumbers.uint3(raf);
		edition = raf.read();
	  }

	  /// <summary>
	  /// Get the byte length of this BUFR record.
	  /// </summary>
	  /// <returns> length in bytes of BUFR record </returns>
	  public int BufrLength
	  {
		  get
		  {
			return bufrLength;
		  }
	  }

	  /// <summary>
	  /// Get the edition of the BUFR specification used.
	  /// </summary>
	  /// <returns> edition number of BUFR specification </returns>
	  public int BufrEdition
	  {
		  get
		  {
			return edition;
		  }
	  }

	  /// <summary>
	  /// Get starting position in the file. This should point to the "BUFR" chars .
	  /// </summary>
	  /// <returns> byte offset in file of start of BUFR meessage. </returns>
	  public long StartPos
	  {
		  get
		  {
			return startPos;
		  }
	  }
	}

}
