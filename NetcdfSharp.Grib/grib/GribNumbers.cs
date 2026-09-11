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

namespace NetcdfSharp.ucar.nc2.grib
{
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Utilities for reading and interpreting GRIB bytes.
	/// </summary>
	public sealed class GribNumbers
	{
	  /// <summary>
	  /// If missing value is not defined use this value.
	  /// </summary>
	  public const int UNDEFINED = -9999;
	  public const double UNDEFINEDD = -9999.0;

	  private static readonly int[] bitmask = new int[] {128, 64, 32, 16, 8, 4, 2, 1};

	  /// <summary>
	  /// Test if the given gribBitNumber is set in the test value.
	  /// </summary>
	  /// <param name="value"> test the 8 bits in this value . </param>
	  /// <param name="gribBitNumber"> one based, starting from highest bit. Must be between 1-8. </param>
	  /// <returns> true if the given gribBitNumber is set. </returns>
	  public static bool testGribBitIsSet(int value, int gribBitNumber)
	  {
		return (value & bitmask[gribBitNumber - 1]) != 0;
	  }

	  /// <summary>
	  /// Test if the given bit is set in the test value.
	  /// </summary>
	  /// <param name="value"> test the 8 bits in this value . </param>
	  /// <param name="bit"> zero based, starting from highest bit. Must be between 0-7. </param>
	  /// <returns> true if the given bit is set. </returns>
	  public static bool testBitIsSet(int value, int bit)
	  {
		return (value & bitmask[bit]) != 0;
	  }

	  /// <summary>
	  /// Grib uses this internally to mean missing
	  /// </summary>
	  public const int MISSING = 255;

	  public static bool isUndefined(double d)
	  {
		return d.CompareTo(UNDEFINEDD) == 0;
	  }

	  /// <summary>
	  /// Convert 2 bytes into a signed integer.
	  /// </summary>
	  /// <param name="raf"> read from here </param>
	  /// <returns> integer value </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static int int2(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public static int int2(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();

		return int2(a, b);
	  }

	  /// <summary>
	  /// Convert unsigned bytes into an integer.
	  /// </summary>
	  /// <param name="raf"> read one byte from here </param>
	  /// <returns> integer value </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static int uint(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public static int @uint(RandomAccessFile raf)
	  {
		int a = raf.read();
		return (int) DataTypes.unsignedByteToShort((byte) a);
	  }

	  /// <summary>
	  /// convert 2 bytes to a signed integer.
	  /// </summary>
	  /// <param name="a"> first byte </param>
	  /// <param name="b"> second byte </param>
	  /// <returns> int </returns>
	  public static int int2(int a, int b)
	  {
		if (((a == 0xff) && (b == 0xff)))
		{ // all bits set to one
		  return UNDEFINED;
		}

		return (1 - ((a & 128) >> 6)) * ((a & 127) << 8 | b);
	  }

	  /// <summary>
	  /// Convert 3 bytes into a signed integer.
	  /// </summary>
	  /// <param name="raf"> read from here </param>
	  /// <returns> integer value </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static int int3(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public static int int3(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();
		int c = raf.read();

		return int3(a, b, c);
	  }

	  /// <summary>
	  /// Convert 3 bytes to signed integer.
	  /// </summary>
	  /// <param name="a"> first byte </param>
	  /// <param name="b"> second byte </param>
	  /// <param name="c"> third byte </param>
	  /// <returns> int </returns>
	  public static int int3(int a, int b, int c)
	  {
		return (1 - ((a & 128) >> 6)) * ((a & 127) << 16 | b << 8 | c);
	  }

	  /// <summary>
	  /// Convert 4 bytes into a signed integer.
	  /// </summary>
	  /// <param name="raf"> read from here </param>
	  /// <returns> integer value </returns>
	  /// <exception cref="IOException"> on read error </exception>
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
	  /// Convert 4 bytes into a signed integer.
	  /// </summary>
	  /// <param name="a"> first byte </param>
	  /// <param name="b"> second byte </param>
	  /// <param name="c"> third byte </param>
	  /// <param name="d"> fourth byte </param>
	  /// <returns> int </returns>
	  public static int int4(int a, int b, int c, int d)
	  {
		// all bits set to ones
		if ((a == 0xff) && (b == 0xff) && (c == 0xff) && (d == 0xff))
		{
		  return UNDEFINED;
		}

		return (1 - ((a & 128) >> 6)) * ((a & 127) << 24 | b << 16 | c << 8 | d);
	  } // end int4

	  /// <summary>
	  /// Convert 2 bytes into an unsigned integer.
	  /// </summary>
	  /// <param name="raf"> read from here </param>
	  /// <returns> integer value </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static int uint2(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public static int uint2(RandomAccessFile raf)
	  {
		int a = raf.read();
		int b = raf.read();

		return uint2(a, b);
	  }

	  /// <summary>
	  /// convert 2 bytes to an unsigned integer.
	  /// </summary>
	  /// <param name="a"> first byte </param>
	  /// <param name="b"> second byte </param>
	  /// <returns> unsigned int </returns>
	  public static int uint2(int a, int b)
	  {
		return a << 8 | b;
	  }

	  /// <summary>
	  /// Convert 3 bytes into an unsigned integer.
	  /// </summary>
	  /// <param name="raf"> read from here </param>
	  /// <returns> integer </returns>
	  /// <exception cref="IOException"> on read error </exception>
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
	  /// Convert 3 bytes into an unsigned int.
	  /// </summary>
	  /// <param name="a"> first byte </param>
	  /// <param name="b"> second byte </param>
	  /// <param name="c"> third byte </param>
	  /// <returns> unsigned integer </returns>
	  public static int uint3(int a, int b, int c)
	  {
		return a << 16 | b << 8 | c;
	  }

	  /// <summary>
	  /// Convert 4 bytes into a float value.
	  /// </summary>
	  /// <param name="raf"> read from here </param>
	  /// <returns> float value </returns>
	  /// <exception cref="IOException"> on read error </exception>
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
	  /// Convert 4 bytes to a float.
	  /// </summary>
	  /// <param name="a"> first byte </param>
	  /// <param name="b"> second byte </param>
	  /// <param name="c"> third byte </param>
	  /// <param name="d"> fourth byte </param>
	  /// <returns> float </returns>
	  public static float float4(int a, int b, int c, int d)
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
	  /// Convert 8 bytes into a signed long.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile </param>
	  /// <returns> long value </returns>
	  /// <exception cref="IOException"> on read error </exception>
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

		return (1 - ((a & 128) >> 6)) * ((long)(a & 127) << 56 | (long)b << 48 | (long)c << 40 | (long)d << 32 | (long)(e << 24) | (long)(f << 16) | (long)(g << 8) | (long)h);

	  }

	  /// <summary>
	  /// A signed byte has a sign bit then 1 15-bit value.
	  /// This is not twos complement (!)
	  /// </summary>
	  /// <param name="v"> convert byte to signed int </param>
	  /// <returns> signed int </returns>
	  public static int convertSignedByte(byte v)
	  {
		int sign = ((v & 0x80) != 0) ? -1 : 1;
		int value = v & 0x7f;
		return sign * value;
	  }

	  public static int convertSignedByte2(byte v)
	  {
		return ((v & 0x80) != 0) ? -(v & 0x7f) : v;
	  }

	  // count number of bits on in bitmap
	  public static int countBits(byte[] bitmap)
	  {
		int bits = 0;
		foreach (byte b in bitmap)
		{
		  short s = DataTypes.unsignedByteToShort(b);
		  bits += JavaLangCompat.BitCount(s);
		}
		return bits;
	  }

	}

}
