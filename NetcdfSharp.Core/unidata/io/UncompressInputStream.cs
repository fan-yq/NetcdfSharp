// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.IO;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.unidata.io
{
#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks


	/// <summary>
	/// This class decompresses an input stream containing data compressed with
	/// the unix "compress" utility (LZC, a LZW variant). This code is based
	/// heavily on the <var>unlzw.c</var> code in <var>gzip-1.2.4</var> (written
	/// by Peter Jannesen) and the original compress code.
	/// 
	/// 
	/// <para>
	/// This version has been modified from the original 0.3-3 version by the
	/// Unidata Program Center (support@unidata.ucar.edu) to make the constructor
	/// public and to fix a couple of bugs.
	/// Also:
	/// - markSupported() returns false
	/// - add uncompress() static method
	/// 
	/// @version 0.3-3 06/05/2001
	/// @author Ronald Tschalar
	/// @author Unidata Program Center
	/// </para>
	/// </summary>
	/// @deprecated not part of public API. 
	[Obsolete("not part of public API.")]
	public class UncompressInputStream : FilterInputStream
	{

	  // string table stuff
	  private const int TBL_CLEAR = 0x100;
	  private static readonly int TBL_FIRST = TBL_CLEAR + 1;

	  private int[] tab_prefix;
	  private byte[] tab_suffix;
	  private int[] zeros = new int[256];
	  private byte[] stack;

	  // various state
	  private bool block_mode;
	  private int n_bits;
	  private int maxbits;
	  private int maxmaxcode;
	  private int maxcode;
	  private int bitmask;
	  private int oldcode;
	  private byte finchar;
	  private int stackp;
	  private int free_ent;

	  // input buffer
	  private byte[] data = new byte[10000];
	  private int bit_pos, end, got;
	  private bool eof;
	  private const int EXTRA = 64;


	  /// <param name="is"> the input stream to decompress </param>
	  /// <exception cref="IOException"> if the header is malformed </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public UncompressInputStream(InputStream is) throws IOException
	  public UncompressInputStream(Stream @is) : base(new NetcdfSharp.java.io.StreamInputStream(@is))
	  {
		parse_header();
	  }


	  private byte[] one = new byte[1];

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public int read() throws IOException
	  public override int read()
	  {
		int b = read(one, 0, 1);
		if (b == 1)
		{
		  return (one[0] & 0xff);
		}
		else
		{
		  return -1;
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public int read(byte[] buf, int off, int len) throws IOException
	  public override int read(byte[] buf, int off, int len)
	  {
		if (eof)
		{
		  return -1;
		}
		int start = off;

		/*
		 * Using local copies of various variables speeds things up by as
		 * much as 30% !
		 */
		int[] l_tab_prefix = tab_prefix;
		byte[] l_tab_suffix = tab_suffix;
		byte[] l_stack = stack;
		int l_n_bits = n_bits;
		int l_maxcode = maxcode;
		int l_maxmaxcode = maxmaxcode;
		int l_bitmask = bitmask;
		int l_oldcode = oldcode;
		byte l_finchar = finchar;
		int l_stackp = stackp;
		int l_free_ent = free_ent;
		byte[] l_data = data;
		int l_bit_pos = bit_pos;


		// empty stack if stuff still left

		int s_size = l_stack.Length - l_stackp;
		if (s_size > 0)
		{
		  int num = (s_size >= len) ? len : s_size;
		  System.Array.Copy(l_stack, l_stackp, buf, off, num);
		  off += num;
		  len -= num;
		  l_stackp += num;
		}

		if (len == 0)
		{
		  stackp = l_stackp;
		  return off - start;
		}


		// loop, filling local buffer until enough data has been decompressed

		do
		{
		  if (end < EXTRA)
		  {
			fill();
		  }

		  int bit_in = (got > 0) ? (end - end % l_n_bits) << 3 : (end << 3) - (l_n_bits - 1);

		  while (l_bit_pos < bit_in)
		  {
			// handle 1-byte reads correctly
			if (len == 0)
			{
			  n_bits = l_n_bits;
			  maxcode = l_maxcode;
			  maxmaxcode = l_maxmaxcode;
			  bitmask = l_bitmask;
			  oldcode = l_oldcode;
			  finchar = l_finchar;
			  stackp = l_stackp;
			  free_ent = l_free_ent;
			  bit_pos = l_bit_pos;

			  return off - start;
			}

			// check for code-width expansion

			if (l_free_ent > l_maxcode)
			{
			  int n_bytes = l_n_bits << 3;
			  l_bit_pos = (l_bit_pos - 1) + n_bytes - (l_bit_pos - 1 + n_bytes) % n_bytes;

			  l_n_bits++;
			  l_maxcode = (l_n_bits == maxbits) ? l_maxmaxcode : (1 << l_n_bits) - 1;

			  if (debug)
			  {
				Console.Error.WriteLine("Code-width expanded to " + l_n_bits);
			  }

			  l_bitmask = (1 << l_n_bits) - 1;
			  l_bit_pos = resetbuf(l_bit_pos);
			  continue;
			}


			// read next code

			int pos = l_bit_pos >> 3;
			int code = (((l_data[pos] & 0xFF) | ((l_data[pos + 1] & 0xFF) << 8) | ((l_data[pos + 2] & 0xFF) << 16)) >> (l_bit_pos & 0x7)) & l_bitmask;
			l_bit_pos += l_n_bits;


			// handle first iteration

			if (l_oldcode == -1)
			{
			  if (code >= 256)
			  {
				throw new IOException("corrupt input: " + code + " > 255");
			  }
			  l_finchar = (byte)(l_oldcode = code);
			  buf[off++] = l_finchar;
			  len--;
			  continue;
			}


			// handle CLEAR code

			if (code == TBL_CLEAR && block_mode)
			{
			  System.Array.Copy(zeros, 0, l_tab_prefix, 0, zeros.Length);
			  l_free_ent = TBL_FIRST - 1;

			  int n_bytes = l_n_bits << 3;
			  l_bit_pos = (l_bit_pos - 1) + n_bytes - (l_bit_pos - 1 + n_bytes) % n_bytes;
			  l_n_bits = INIT_BITS;
			  l_maxcode = (1 << l_n_bits) - 1;
			  l_bitmask = l_maxcode;

			  if (debug)
			  {
				Console.Error.WriteLine("Code tables reset");
			  }

			  l_bit_pos = resetbuf(l_bit_pos);
			  continue;
			}


			// setup

			int incode = code;
			l_stackp = l_stack.Length;


			// Handle KwK case

			if (code >= l_free_ent)
			{
			  if (code > l_free_ent)
			  {
				throw new IOException("corrupt input: code=" + code + ", free_ent=" + l_free_ent);
			  }

			  l_stack[--l_stackp] = l_finchar;
			  code = l_oldcode;
			}


			// Generate output characters in reverse order

			while (code >= 256)
			{
			  l_stack[--l_stackp] = l_tab_suffix[code];
			  code = l_tab_prefix[code];
			}
			l_finchar = l_tab_suffix[code];
			buf[off++] = l_finchar;
			len--;


			// And put them out in forward order

			s_size = l_stack.Length - l_stackp;
			int num = (s_size >= len) ? len : s_size;
			System.Array.Copy(l_stack, l_stackp, buf, off, num);
			off += num;
			len -= num;
			l_stackp += num;


			// generate new entry in table

			if (l_free_ent < l_maxmaxcode)
			{
			  l_tab_prefix[l_free_ent] = l_oldcode;
			  l_tab_suffix[l_free_ent] = l_finchar;
			  l_free_ent++;
			}


			// Remember previous code

			l_oldcode = incode;


			// if output buffer full, then return

			if (len == 0)
			{
			  n_bits = l_n_bits;
			  maxcode = l_maxcode;
			  bitmask = l_bitmask;
			  oldcode = l_oldcode;
			  finchar = l_finchar;
			  stackp = l_stackp;
			  free_ent = l_free_ent;
			  bit_pos = l_bit_pos;

			  return off - start;
			}
		  }

		  l_bit_pos = resetbuf(l_bit_pos);
		} while (got > 0);

		n_bits = l_n_bits;
		maxcode = l_maxcode;
		bitmask = l_bitmask;
		oldcode = l_oldcode;
		finchar = l_finchar;
		stackp = l_stackp;
		free_ent = l_free_ent;
		bit_pos = l_bit_pos;

		eof = true;
		return off - start;
	  }


	  // Moves the unread data in the buffer to the beginning and resets the pointers.
	  // @return 0
	  private int resetbuf(int bit_pos)
	  {
		int pos = bit_pos >> 3;
		System.Array.Copy(data, pos, data, 0, end - pos);
		end -= pos;
		return 0;
	  }


// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void fill() throws IOException
	  private void fill()
	  {
		got = @in.read(data, end, data.Length - 1 - end);
		if (got > 0)
		{
		  end += got;
		}
	  }


// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public long skip(long num) throws IOException
	  public override long skip(long num)
	  {
		byte[] tmp = new byte[(int) num];
		int got = read(tmp, 0, (int) num);

		if (got > 0)
		{
		  return (long) got;
		}
		else
		{
		  return 0L;
		}
	  }


// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public int available() throws IOException
	  public override int available()
	  {
		if (eof)
		{
		  return 0;
		}

		// Fred Hansen, 2008
		// the old code follows. it fails because read() can return bytes even after exhausting in.read()
		// return in.available();

		int avail = @in.available();
		return (avail == 0) ? 1 : avail;
	  }


	  private const int LZW_MAGIC = 0x1f9d;
	  private const int MAX_BITS = 16;
	  private const int INIT_BITS = 9;
	  private const int HDR_MAXBITS = 0x1f;
	  private const int HDR_EXTENDED = 0x20;
	  private const int HDR_FREE = 0x40;
	  private const int HDR_BLOCK_MODE = 0x80;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void parse_header() throws IOException
	  private void parse_header()
	  {
		// read in and check magic number

		int t = @in.read();
		if (t < 0)
		{
		  throw new EOFException("Failed to read magic number");
		}
		int magic = (t & 0xff) << 8;
		t = @in.read();
		if (t < 0)
		{
		  throw new EOFException("Failed to read magic number");
		}
		magic += t & 0xff;
		if (magic != LZW_MAGIC)
		{
		  throw new IOException("Input not in compress format (read " + "magic number 0x" + Convert.ToString(magic, 16) + ")");
		}


		// read in header byte

		int header = @in.read();
		if (header < 0)
		{
		  throw new EOFException("Failed to read header");
		}

		block_mode = (header & HDR_BLOCK_MODE) > 0;
		maxbits = header & HDR_MAXBITS;

		if (maxbits > MAX_BITS)
		{
		  throw new IOException("Stream compressed with " + maxbits + " bits, but can only handle " + MAX_BITS + " bits");
		}

		if ((header & HDR_EXTENDED) > 0)
		{
		  throw new IOException("Header extension bit set");
		}

		if ((header & HDR_FREE) > 0)
		{
		  throw new IOException("Header bit 6 set");
		}

		if (debug)
		{
		  Console.Error.WriteLine("block mode: " + block_mode);
		  Console.Error.WriteLine("max bits:   " + maxbits);
		}


		// initialize stuff

		maxmaxcode = 1 << maxbits;
		n_bits = INIT_BITS;
		maxcode = (1 << n_bits) - 1;
		bitmask = maxcode;
		oldcode = -1;
		finchar = 0;
		free_ent = block_mode ? TBL_FIRST : 256;

		tab_prefix = new int[1 << maxbits];
		tab_suffix = new byte[1 << maxbits];
		stack = new byte[1 << maxbits];
		stackp = stack.Length;

		for (int idx = 255; idx >= 0; idx--)
		{
		  tab_suffix[idx] = (byte) idx;
		}
	  }

	  /// <summary>
	  /// This stream does not support mark/reset on the stream.
	  /// </summary>
	  /// <returns> false </returns>
	  public override bool markSupported()
	  {
		return false;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void uncompress(String fileInName, FileOutputStream out) throws IOException
	  public static void uncompress(string fileInName, FileStream @out)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();
		int total = 0;

		using (Stream @in = new UncompressInputStream(new FileStream(fileInName, FileMode.Open, FileAccess.Read)))
		{
		  byte[] buffer = new byte[100000];
		  while (true)
		  {
			int bytesRead = @in.Read(buffer, 0, buffer.Length);
			if (bytesRead == -1)
			{
			  break;
			}
			@out.Write(buffer, 0, bytesRead);
			total += bytesRead;
		  }
		}

		if (debugTiming)
		{
		  long end = DateTimeHelper.CurrentUnixTimeMillis();
		  Console.Error.WriteLine("Decompressed " + total + " bytes");
		  Console.Error.WriteLine("Time: " + (end - start) / 1000.0 + " seconds");
		}
	  }


	  private const bool debug = false, debugTiming = false;

#pragma warning restore CS0162

	}


}
