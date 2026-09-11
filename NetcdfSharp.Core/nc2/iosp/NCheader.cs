// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp
{

	/// <summary>
	/// Common header for netcdf3, netcdf4, hdf5, hdf4.
	/// 
	/// @author dmh </summary>
	/// @deprecated Will move to internal module in version 6. 

	public class NCheader
	{
	  //////////////////////////////////////////////////
	  // Constants

	  // Constants for check_file_type
	  private const int MAGIC_NUMBER_LEN = 8;
	  private const long MAXHEADERPOS = 50000; // header's gotta be within this range

	  public static readonly byte[] H5HEAD = new byte[] {unchecked((byte) 0x89), (byte)'H', (byte)'D', (byte)'F', (byte)'\r', (byte)'\n', (byte) 0x1a, (byte)'\n'};
	  public static readonly byte[] H4HEAD = new byte[] {(byte) 0x0e, (byte) 0x03, (byte) 0x13, (byte) 0x01};
	  public static readonly byte[] CDF1HEAD = new byte[] {(byte) 'C', (byte) 'D', (byte) 'F', (byte) 0x01};
	  public static readonly byte[] CDF2HEAD = new byte[] {(byte) 'C', (byte) 'D', (byte) 'F', (byte) 0x02};
	  public static readonly byte[] CDF5HEAD = new byte[] {(byte) 'C', (byte) 'D', (byte) 'F', (byte) 0x05};

	  // These should match the constants in netcdf-c/include/netcdf.h
	  public const int NC_FORMAT_NETCDF3 = (1);
	  public const int NC_FORMAT_64BIT_OFFSET = (2);
	  public const int NC_FORMAT_NETCDF4 = (3);
	  public const int NC_FORMAT_64BIT_DATA = (5);

	  // Extensions
	  public const int NC_FORMAT_HDF4 = (0x7005);

	  // Aliases
	  public static readonly int NC_FORMAT_CLASSIC = (NC_FORMAT_NETCDF3);
	  public static readonly int NC_FORMAT_64BIT = (NC_FORMAT_64BIT_OFFSET);
	  public static readonly int NC_FORMAT_CDF5 = (NC_FORMAT_64BIT_DATA);
	  public static readonly int NC_FORMAT_HDF5 = (NC_FORMAT_NETCDF4);

	  //////////////////////////////////////////////////
	  // Static Methods

	  /// <summary>
	  /// Figure out what kind of netcdf-related file we have.
	  /// Constraint: leave raf read pointer to point just after the magic number.
	  /// </summary>
	  /// <param name="raf"> to test type </param>
	  /// <returns> integer that matches constants in netcdf-c/include/netcdf.h </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static int checkFileType(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public static int checkFileType(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf)
	  {
		int format;

		byte[] magic = new byte[MAGIC_NUMBER_LEN];

		// If this is not an HDF5 file, then the magic number is at
		// position 0; If it is an HDF5 file, then we need to search
		// forward for it.

		// Look for the relevant leading tag
		raf.seek(0);
		if (raf.readBytes(magic, 0, MAGIC_NUMBER_LEN) < MAGIC_NUMBER_LEN)
		{
		  return 0; // unknown
		}
		// Some version of CDF
		int hdrlen;
		hdrlen = CDF1HEAD.Length; // all CDF headers are assumed to be same length
		format = 0;
		if (memequal(CDF1HEAD, magic, CDF1HEAD.Length))
		{
		  format = NC_FORMAT_CLASSIC;
		}
		else if (memequal(CDF2HEAD, magic, CDF2HEAD.Length))
		{
		  format = NC_FORMAT_64BIT_OFFSET;
		}
		else if (memequal(CDF5HEAD, magic, CDF5HEAD.Length))
		{
		  format = NC_FORMAT_CDF5;
		}
		else if (memequal(H4HEAD, magic, H4HEAD.Length))
		{
		  format = NC_FORMAT_HDF4;
		}
		if (format != 0)
		{
		  raf.seek(hdrlen);
		  return format;
		}
		// For HDF5, we need to search forward
		format = 0;
		long filePos = 0;
		long size = raf.length();
		while ((filePos < size - 8) && (filePos < MAXHEADERPOS))
		{
		  raf.seek(filePos);
		  if (raf.readBytes(magic, 0, MAGIC_NUMBER_LEN) < MAGIC_NUMBER_LEN)
		  {
			return 0; // unknown
		  }
		  // Test for HDF5
		  if (memequal(H5HEAD, magic, H5HEAD.Length))
		  {
			format = NC_FORMAT_HDF5;
			break;
		  }
		  filePos = (filePos == 0) ? 512 : 2 * filePos;
		}
		if (format != 0)
		{
		  raf.seek(filePos + H5HEAD.Length);
		}
		return format;
	  }

	  /// <summary>
	  /// Not quite memcmp
	  /// </summary>
	  private static bool memequal(byte[] b1, byte[] b2, int len)
	  {
		if (b1 == b2)
		{
		  return true;
		}
		if (b1 == null || b2 == null)
		{
		  return false;
		}
		if (b1.Length < len || b2.Length < len)
		{
		  return false;
		}
		for (int i = 0; i < len; i++)
		{
		  if (b1[i] != b2[i])
		  {
			return false;
		  }
		}
		return true;
	  }

	  public static string formatName(int format)
	  {
		switch (format)
		{
		  case NCheader.NC_FORMAT_NETCDF3:
			return "netcdf-3";
		  case NCheader.NC_FORMAT_64BIT_OFFSET:
			return "netcdf-3 64bit-offset";
		  case NCheader.NC_FORMAT_NETCDF4:
			return "netcdf-4";
		  case NCheader.NC_FORMAT_64BIT_DATA:
			return "netcdf-5";
		  case NCheader.NC_FORMAT_HDF4:
			return "HDF-4";
		  default:
			break;
		}
		return null;
	  }
	}

}
