// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// A class that contains static methods for converting multiple
	/// bytes into one float or integer.
	/// </summary>

	public sealed class BufrNumbers
	{

	  // used to check missing values when value is packed with all 1's
	  private static readonly long[] missing_value = new long[2049];

	  static BufrNumbers()
	  {
		long accum = 0;
		for (int i = 0; i < 65; i++)
		{
		  missing_value[i] = accum;
		  accum = accum * 2 + 1;
		}
	  }

	  public static bool isMissing(long raw, int bitWidth)
	  {
		return (raw == BufrNumbers.missing_value[bitWidth]);
	  }

	  internal static long missingValue(int bitWidth)
	  {
		return BufrNumbers.missing_value[bitWidth];
	  }

	  /// <summary>
	  /// if missing value is not defined use this value. </summary>
	  private const int UNDEFINED = -9999;

	  /// <summary>
	  /// Convert 2 bytes into a signed integer. </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static int int2(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  internal static int int2(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();

		return int2(a, b);
	  }

	  /// <summary>
	  /// Convert 2 bytes to a signed integer. </summary>
	  private static int int2(int a, int b)
	  {
		if ((a == 0xff && b == 0xff)) // all bits set to one
		{
		  return UNDEFINED;
		}

		return (1 - ((a & 128) >> 6)) * ((a & 127) << 8 | b);
	  }

	  /// <summary>
	  /// Read 3 bytes and turn into a signed integer. </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static int int3(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  internal static int int3(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();
		int c = raf.read();

		return int3(a, b, c);
	  }

	  /// <summary>
	  /// Convert 3 bytes to signed integer. </summary>
	  private static int int3(int a, int b, int c)
	  {
		return (1 - ((a & 128) >> 6)) * ((a & 127) << 16 | b << 8 | c);
	  }

	  /// <summary>
	  /// Convert 4 bytes into a signed integer. </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static int int4(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public static int int4(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();
		int c = raf.read();
		int d = raf.read();

		return int4(a, b, c, d);
	  }

	  /// <summary>
	  /// Convert 4 bytes into a signed integer. </summary>
	  private static int int4(int a, int b, int c, int d)
	  {
		// all bits set to ones
		if (a == 0xff && b == 0xff && c == 0xff && d == 0xff)
		{
		  return UNDEFINED;
		}

		return (1 - ((a & 128) >> 6)) * ((a & 127) << 24 | b << 16 | c << 8 | d);
	  } // end int4

	  /// <summary>
	  /// Convert 2 bytes into an unsigned integer. </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static int uint2(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  internal static int uint2(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();

		return uint2(a, b);
	  }

	  /// <summary>
	  /// Convert 2 bytes to an unsigned integer. </summary>
	  private static int uint2(int a, int b)
	  {
		return a << 8 | b;
	  }

	  /// <summary>
	  /// Read 3 bytes and convert into an unsigned integer. </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static int uint3(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public static int uint3(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();
		int c = raf.read();

		return uint3(a, b, c);
	  }

	  /// <summary>
	  /// Convert 3 bytes into an unsigned int. </summary>
	  private static int uint3(int a, int b, int c)
	  {
		return a << 16 | b << 8 | c;
	  }

	  /// <summary>
	  /// Read 4 bytes and convert into a float value. </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static float float4(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public static float float4(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();
		int c = raf.read();
		int d = raf.read();

		return float4(a, b, c, d);
	  }

	  /// <summary>
	  /// Convert 4 bytes to a float. </summary>
	  private static float float4(int a, int b, int c, int d)
	  {
		int sgn, mant, exp;

		mant = b << 16 | c << 8 | d;
		if (mant == 0)
		{
		  return 0.0f;
		}

		sgn = -(((a & 128) >> 6) - 1);
		exp = (a & 127) - 64;

		return (float)(sgn * Math.Pow(16.0, exp - 6) * mant);
	  }

	  /// <summary>
	  /// Read 8 bytes and convert into a signed long. </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long int8(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public static long int8(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();
		int c = raf.read();
		int d = raf.read();
		int e = raf.read();
		int f = raf.read();
		int g = raf.read();
		int h = raf.read();

		return (1 - ((a & 128) >> 6)) * ((long)(a & 127) << 56 | (long)b << 48 | (long)c << 40 | (long)d << 32 | (long)e << 24 | (long)f << 16 | (long)g << 8 | (long)h);
	  }
	}

}
