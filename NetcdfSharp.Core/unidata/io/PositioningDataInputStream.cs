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

	/// <summary>
	/// Similar to a DataInputStream that keeps track of position.
	/// position must always increase, no going backwards.
	/// cant handle byte order yet - assume big endian(?).
	/// 
	/// @author caron
	/// @since Jan 3, 2008 </summary>
	/// @deprecated not part of public API. 
	[Obsolete("not part of public API.")]
	public class PositioningDataInputStream
	{
	  private DataInputStream @delegate;
	  private long cpos;

	  public PositioningDataInputStream(Stream @is)
	  {
		@delegate = new DataInputStream(@is);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void seek(long pos) throws IOException
	  private void seek(long pos)
	  {
		if (pos < cpos)
		{
		  throw new ArgumentException("Cannot go backwards; current=" + cpos + " request=" + pos);
		}
		long want = pos - cpos;
		while (want > 0)
		{
		  want -= @delegate.skip(want);
		}
		cpos = pos;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void read(long pos, byte[] dest, int off, int len) throws IOException
	  public virtual void read(long pos, byte[] dest, int off, int len)
	  {
		seek(pos);
		@delegate.readFully(dest, off, len);
		cpos += len;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void readShort(long pos, short[] dest, int off, int len) throws IOException
	  public virtual void readShort(long pos, short[] dest, int off, int len)
	  {
		seek(pos);
		for (int i = 0; i < len; i++)
		{
		  dest[off + i] = @delegate.readShort();
		}
		cpos += len * 2;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void readInt(long pos, int[] dest, int off, int len) throws IOException
	  public virtual void readInt(long pos, int[] dest, int off, int len)
	  {
		seek(pos);
		for (int i = 0; i < len; i++)
		{
		  dest[off + i] = @delegate.readInt();
		}
		cpos += len * 4;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void readLong(long pos, long[] dest, int off, int len) throws IOException
	  public virtual void readLong(long pos, long[] dest, int off, int len)
	  {
		seek(pos);
		for (int i = 0; i < len; i++)
		{
		  dest[off + i] = @delegate.readLong();
		}
		cpos += len * 8;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void readFloat(long pos, float[] dest, int off, int len) throws IOException
	  public virtual void readFloat(long pos, float[] dest, int off, int len)
	  {
		seek(pos);
		for (int i = 0; i < len; i++)
		{
		  dest[off + i] = @delegate.readFloat();
		}
		cpos += len * 4;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void readDouble(long pos, double[] dest, int off, int len) throws IOException
	  public virtual void readDouble(long pos, double[] dest, int off, int len)
	  {
		seek(pos);
		for (int i = 0; i < len; i++)
		{
		  dest[off + i] = @delegate.readDouble();
		}
		cpos += len * 8;
	  }
	}

}
