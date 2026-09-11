// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Represents Section 3 of a BUFR message.
	/// 
	/// @author caron
	/// @since May 10, 2008
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class BufrDataDescriptionSection
	public class BufrDataDescriptionSection
	{

	  /// <summary>
	  /// Offset to start of BufrDataDescriptionSection.
	  /// </summary>
	  private readonly long offset;

	  /// <summary>
	  /// Number of data sets.
	  /// </summary>
	  private readonly int ndatasets;

	  /// <summary>
	  /// data type (observed or compressed).
	  /// </summary>
	  private readonly int datatype;

	  /// <summary>
	  /// List of data set descriptors.
	  /// </summary>
	  private readonly IList<short> descriptors = new List<short>();

	  /// <summary>
	  /// Constructs a BufrDataDescriptionSection object by reading section 3 from a BUFR file.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile, position must be on a BUFR section 3 </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public BufrDataDescriptionSection(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public BufrDataDescriptionSection(RandomAccessFile raf)
	  {
		offset = raf.FilePointer;
		int length = BufrNumbers.uint3(raf);
		long EOS = offset + length;

		// reserved byte
		raf.read();

		// octets 5-6 number of datasets
		ndatasets = BufrNumbers.uint2(raf);

		// octet 7 data type bit 2 is for compressed data 192 or 64,
		// non-compressed data is 0 or 128
		datatype = raf.read();

		// get descriptors
		int ndesc = (length - 7) / 2;
		for (int i = 0; i < ndesc; i++)
		{
		  int ch1 = raf.read();
		  int ch2 = raf.read();
		  short fxy = (short)((ch1 << 8) + (ch2));
		  descriptors.Add(fxy);
		}

		// reset for any offset discrepancies
		raf.seek(EOS);
	  }

	  /// <summary>
	  /// Offset to the beginning of BufrDataDescriptionSection.
	  /// </summary>
	  /// <returns> offset in bytes of BUFR record </returns>
	  public long Offset
	  {
		  get
		  {
			return offset;
		  }
	  }

	  /// <summary>
	  /// Number of data sets in this record.
	  /// </summary>
	  /// <returns> datasets </returns>
	  public int NumberDatasets
	  {
		  get
		  {
			return ndatasets;
		  }
	  }

	  /// <summary>
	  /// Data type (compressed or non-compressed).
	  /// </summary>
	  /// <returns> datatype </returns>
	  public int DataType
	  {
		  get
		  {
			return datatype;
		  }
	  }

	  /// <summary>
	  /// Observation data
	  /// </summary>
	  /// <returns> true if observation data </returns>
	  public virtual bool Observed
	  {
		  get
		  {
			return (datatype & 0x80) != 0;
		  }
	  }

	  /// <summary>
	  /// Is data compressed?
	  /// </summary>
	  /// <returns> true if data is compressed </returns>
	  public virtual bool Compressed
	  {
		  get
		  {
			return (datatype & 0x40) != 0;
		  }
	  }

	  /// <summary>
	  /// get list of data descriptors as Shorts
	  /// </summary>
	  /// <returns> descriptors as List<Short> </returns>
	  public IList<short> DataDescriptors
	  {
		  get
		  {
			return descriptors;
		  }
	  }

	  /// <summary>
	  /// get list of data descriptors as Strings
	  /// </summary>
	  /// <returns> descriptors as List<String> </returns>
	  public IList<string> Descriptors
	  {
		  get
		  {
			IList<string> desc = new List<string>();
			foreach (short fxy in descriptors)
			{
			  desc.Add(Descriptor.makeString(fxy));
			}
			return desc;
		  }
	  }
	}

}
