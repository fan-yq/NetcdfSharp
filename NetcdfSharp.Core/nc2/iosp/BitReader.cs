// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp
{
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Helper for reading data that has been bit packed.
	/// 
	/// @author caron
	/// @since Apr 7, 2008
	/// </summary>
	public class BitReader
	{

	  private static readonly int BIT_LENGTH = (sizeof(byte) * 8);
	  private const int BYTE_BITMASK = 0xFF;
	  private static readonly long LONG_BITMASK = long.MaxValue;

	  private RandomAccessFile raf;
	  private long startPos;

	  private byte[] data;
	  private int dataPos;

	  private byte bitBuf;
	  private int bitPos; // Current bit position in bitBuf.

	  // for testing
	  public BitReader(byte[] test)
	  {
		this.data = test;
		this.dataPos = 0;
	  }

	  /// <summary>
	  /// Constructor
	  /// </summary>
	  /// <param name="raf"> the RandomAccessFile </param>
	  /// <param name="startPos"> points to start of data in data section, in bytes </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public BitReader(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long startPos) throws NetcdfSharp.java.io.IOException
	  public BitReader(RandomAccessFile raf, long startPos)
	  {
		this.raf = raf;
		this.startPos = startPos;
		raf.seek(startPos);
	  }

	  /// <summary>
	  /// Go to the next byte in the stream
	  /// </summary>
	  public virtual void incrByte()
	  {
		this.bitPos = 0;
	  }

	  /// <summary>
	  /// Position file at bitOffset from startPos
	  /// </summary>
	  /// <param name="bitOffset"> bit offset from starting position </param>
	  /// <exception cref="IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void setBitOffset(int bitOffset) throws NetcdfSharp.java.io.IOException
	  public virtual int BitOffset
	  {
		  set
		  {
			if (value % 8 == 0)
			{
			  raf.seek(startPos + value / 8);
			  bitPos = 0;
			  bitBuf = 0;
			}
			else
			{
			  raf.seek(startPos + value / 8);
			  bitPos = 8 - (value % 8);
			  bitBuf = (byte) raf.read();
			  bitBuf &= (byte)(0xff >> (8 - bitPos)); // mask off consumed bits
			}
		  }
	  }

	  public virtual long Pos
	  {
		  get
		  {
			if (raf != null)
			{
			  return raf.FilePointer;
			}
			else
			{
			  return dataPos;
			}
		  }
	  }

	  /// <summary>
	  /// Read the next nb bits and return an Unsigned Long .
	  /// </summary>
	  /// <param name="nb"> the number of bits to convert to int, must be 0 <= nb <= 64. </param>
	  /// <returns> result </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public long bits2UInt(int nb) throws NetcdfSharp.java.io.IOException
	  public virtual long bits2UInt(int nb)
	  {
		Debug.Assert(nb <= 64);
		Debug.Assert(nb >= 0);

		long result = 0;
		int bitsLeft = nb;

		while (bitsLeft > 0)
		{

		  // we ran out of bits - fetch the next byte...
		  if (bitPos == 0)
		  {
			bitBuf = nextByte();
			bitPos = BIT_LENGTH;
		  }

		  // -- retrieve bit from current byte ----------
		  // how many bits to read from the current byte
		  int size = Math.Min(bitsLeft, bitPos);
		  // move my part to start
		  int myBits = bitBuf >> (bitPos - size);
		  // mask-off sign-extending
		  myBits &= BYTE_BITMASK;
		  // mask-off bits of next value
		  myBits &= ~(BYTE_BITMASK << size);

		  // -- put bit to result ----------------------
		  // where to place myBits inside of result
		  int shift = bitsLeft - size;
		  Debug.Assert(shift >= 0);

		  // put it there
		  result |= (long)(myBits << shift);

		  // -- put bit to result ----------------------
		  // update information on what we consumed
		  bitsLeft -= size;
		  bitPos -= size;
		}

		return result;
	  }

	  /// <summary>
	  /// Read the next nb bits and return an Signed Long .
	  /// </summary>
	  /// <param name="nb"> the number of bits to convert to int, must be <= 64. </param>
	  /// <returns> result </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public long bits2SInt(int nb) throws NetcdfSharp.java.io.IOException
	  public virtual long bits2SInt(int nb)
	  {

		long result = bits2UInt(nb);

		// check if we're negative
		if (getBit(result, nb))
		{
		  // it's negative! reset leading bit
		  result = setBit(result, nb, false);
		  // build 2's-complement
		  result = ~result & LONG_BITMASK;
		  result = result + 1;
		}

		return result;

	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private byte nextByte() throws NetcdfSharp.java.io.IOException
	  private byte nextByte()
	  {
		if (raf != null)
		{
		  int result = raf.read();
		  if (result == -1)
		  {
			throw new EOFException();
		  }
		  return (byte) result;
		}
		else
		{
		  return data[dataPos++];
		}
	  }

	  public static long setBit(long @decimal, int N, bool value)
	  {
		return value ? @decimal | (long)(1 << (N - 1)) : @decimal & ~(long)(1 << (N - 1));
	  }

	  public static bool getBit(long @decimal, int N)
	  {
		long constant = (long)(1 << (N - 1));
		return (@decimal & constant) > 0;
	  }

	}

}
