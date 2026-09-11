// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2019 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.io

{

	using RandomAccessFileProvider = NetcdfSharp.ucar.unidata.io.spi.RandomAccessFileProvider;

	/// <summary>
	/// A RandomAccessFile stored entirely in memory as a byte array. </summary>
	public class InMemoryRandomAccessFile : NetcdfSharp.ucar.unidata.io.RandomAccessFile
	{

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="name"> used as the location </param>
	  /// <param name="data"> the complete data file </param>
	  public InMemoryRandomAccessFile(string name, byte[] data) : base(1)
	  {
		this.location = name;
		this.file = null;
		if (data == null)
		{
		  throw new ArgumentException("data array is null");
		}

		buffer = data;
		bufferStart = 0;
		dataSize = buffer.Length;
		dataEnd = buffer.Length;
		filePosition = 0;
		endOfFile = false;

		if (debugLeaks)
		{
		  openFiles.Add(location);
		}
	  }

	  public override long length()
	  {
		return dataEnd;
	  }

	  public override int BufferSize
	  {
		  set
		  {
			// do nothing
		  }
	  }

	  protected internal override int read_(long pos, byte[] b, int offset, int len)
	  {
		len = Math.Min(len, (int)(buffer.Length - pos));
		// copy out of buffer
		System.Array.Copy(buffer, (int) pos, b, offset, len);
		return len;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public long readToByteChannel(NetcdfSharp.java.nio.channels.WritableByteChannel dest, long offset, long nbytes) throws NetcdfSharp.java.io.IOException
	  public override long readToByteChannel(WritableByteChannel dest, long offset, long nbytes)
	  {
		return dest.write(ByteBuffer.wrap(buffer, (int) offset, (int) nbytes));
	  }

	  /// <summary>
	  /// Hook for service provider interface RandomAccessFileProvider
	  /// </summary>
	  public class Provider : RandomAccessFileProvider
	  {

		public virtual bool isOwnerOf(string location)
		{
		  return location.StartsWith("slurp:", StringComparison.Ordinal);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public RandomAccessFile open(String location) throws NetcdfSharp.java.io.IOException
		public virtual RandomAccessFile open(string location)
		{
		  throw new NotSupportedException("HTTP (slurp:) access is not supported in NetcdfSharp.");
		}

		public virtual RandomAccessFile open(string location, int bufferSize)
		{
		  return open(location);
		}
	  }
	}


}
