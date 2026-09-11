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
namespace NetcdfSharp.ucar.unidata.io
{

	/// <summary>
	/// Use NIO MappedByteBuffer to implement a RandomAccessFile.
	/// Limited to 2G size. Not currently used - NOT UP TO DATE DO NOT USE
	/// </summary>
	/// @deprecated not part of public API. 
	[Obsolete("not part of public API.")]
	public class MMapRandomAccessFile : RandomAccessFile
	{

	  private MappedByteBuffer source;

	  /// <summary>
	  /// Constructor for in-memory "files"
	  /// </summary>
	  /// <param name="location"> used as a name </param>
	  /// <param name="mode"> the open mode </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public MMapRandomAccessFile(String location, String mode) throws IOException
	  public MMapRandomAccessFile(string location, string mode) : base(location, mode, 1)
	  {
		FileChannel channel = file.getChannel();
		source = channel.map(@readonly ? FileChannel.MapMode.READ_ONLY : FileChannel.MapMode.READ_WRITE, (long) 0, channel.size());
		// channel.close();

		bufferStart = 0;
		dataSize = (int) channel.size();
		dataEnd = channel.size();
		filePosition = 0;
		buffer = null;
		endOfFile = false;
	  }


	  public override void flush()
	  {
		if (bufferModified)
		{
		  source.force();
		  bufferModified = false;
		}
	  }

	  public override long length()
	  {
		return dataEnd;
	  }

	  public override void seek(long pos)
	  {
		filePosition = pos;
		if (filePosition < dataEnd)
		{
		  source.position((int) filePosition);
		}
		else
		{
		  endOfFile = true;
		}
	  }

	  public override void unread()
	  {
		seek(filePosition - 1);
	  }

	  public override int read()
	  {
		// If the file position is within the data, return the byte...
		if (filePosition < dataEnd)
		{
		  filePosition++;
		  return source.get() & 0xff;

		}
		else
		{
		  return -1;
		}
	  }

	  public override int readBytes(byte[] dst, int offset, int length)
	  {
		if (endOfFile)
		{
		  return -1;
		}

		length = (int) Math.Min(length, dataEnd - filePosition);
		if (length > 0)
		{
		  source.get(dst, offset, length);
		  filePosition += length;
		}
		return length;
	  }

	  public override void write(int b)
	  {
		source.put((byte) b);
		filePosition++;
		bufferModified = true;
	  }

	  public override void writeBytes(byte[] dst, int offset, int length)
	  {
		source.put(dst, offset, length);
		filePosition += length;
	  }

	}

}
