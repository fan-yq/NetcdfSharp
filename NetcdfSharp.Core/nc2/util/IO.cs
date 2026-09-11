// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.util
{
#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;

	/// <summary>
	/// Input/Output static utilities.
	/// 
	/// @author John Caron </summary>
	/// <seealso cref=""http://stackoverflow.com/questions/12552863/correct-idiom-for-managing-multiple-chained-resources-in-try-with-resources-bloc""/>
	public class IO
	{

	  public const int default_file_buffersize = 9200;
	  public const int default_socket_buffersize = 64000;

	  private const bool showStackTrace = false;
	  private const bool debug = false, showCopy = false;
	  private const bool showHeaders = false;

	  private static Type cl;

	  /// <summary>
	  /// Open a resource as a Stream. First try ClassLoader.getResourceAsStream().
	  /// If that fails, try a plain old FileInputStream().
	  /// </summary>
	  /// <param name="resourcePath"> name of file path (use forward slashes!) </param>
	  /// <returns> InputStream or null on failure </returns>
	  public static Stream getFileResource(string resourcePath)
	  {
		if (cl == null)
		{
		  cl = typeof(IO); // (new IO()).getClass();
		}

		Stream @is = cl.Assembly.GetManifestResourceStream(resourcePath);
		if (@is != null)
		{
		  if (debug)
		  {
			Console.WriteLine("Resource.getResourceAsStream ok on " + resourcePath);
		  }
		  return @is;
		}
		else if (debug)
		{
		  Console.WriteLine("Resource.getResourceAsStream failed on (" + resourcePath + ")");
		}

		try
		{
		  @is = new FileStream(resourcePath, FileMode.Open, FileAccess.Read);
		  if (debug)
		  {
			Console.WriteLine("Resource.FileInputStream ok on " + resourcePath);
		  }
		}
		catch (FileNotFoundException)
		{
		  if (debug)
		  {
			Console.WriteLine("  FileNotFoundException: Resource.getFile failed on " + resourcePath);
		  }
		}
		catch (System.Security.SecurityException)
		{
		  if (debug)
		  {
			Console.WriteLine("  AccessControlException: Resource.getFile failed on " + resourcePath);
		  }
		}

		return @is;
	  }

	  /// <summary>
	  /// copy all bytes from in to out.
	  /// </summary>
	  /// <param name="in"> InputStream </param>
	  /// <param name="out"> OutputStream </param>
	  /// <returns> number of bytes copied </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long copy(InputStream in, OutputStream out) throws IOException
	  public static long copy(Stream @in, Stream @out)
	  {
		long totalBytesRead = 0;
		byte[] buffer = new byte[default_file_buffersize];
		while (true)
		{
		  int bytesRead = @in.Read(buffer, 0, buffer.Length);
		  if (bytesRead == -1)
		  {
			break;
		  }
		  @out.Write(buffer, 0, bytesRead);
		  totalBytesRead += bytesRead;
		}
		@out.Flush();
		return totalBytesRead;
	  }

	  /// <summary>
	  /// copy all bytes from in and throw them away.
	  /// </summary>
	  /// <param name="in"> InputStream </param>
	  /// <param name="buffersize"> size of buffer to use, if -1 uses default value (9200) </param>
	  /// <returns> number of bytes copied </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long copy2null(InputStream in, int buffersize) throws IOException
	  public static long copy2null(Stream @in, int buffersize)
	  {
		long totalBytesRead = 0;
		if (buffersize <= 0)
		{
		  buffersize = default_file_buffersize;
		}
		byte[] buffer = new byte[buffersize];
		while (true)
		{
		  int n = @in.Read(buffer, 0, buffer.Length);
			  if (n <= 0) // .NET Stream.Read returns 0 at EOF
		  {
			break;
		  }
		  totalBytesRead += n;
		}
		// if (fout != null) fout.format("done=%d %n",totalBytesRead);
		return totalBytesRead;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long touch(InputStream in, int buffersize) throws IOException
	  public static long touch(Stream @in, int buffersize)
	  {
		long touch = 0;
		if (buffersize <= 0)
		{
		  buffersize = default_file_buffersize;
		}
		byte[] buffer = new byte[buffersize];
		while (true)
		{
		  int n = @in.Read(buffer, 0, buffer.Length);
			  if (n <= 0) // .NET Stream.Read returns 0 at EOF
		  {
			break;
		  }
		  for (int i = 0; i < buffersize; i++)
		  {
			touch += buffer[i];
		  }
		}
		// if (fout != null) fout.format("done=%d %n",totalBytesRead);
		return touch;
	  }

	  /// <summary>
	  /// copy all bytes from in and throw them away.
	  /// </summary>
	  /// <param name="in"> FileChannel </param>
	  /// <param name="buffersize"> size of buffer to use, if -1 uses default value (9200) </param>
	  /// <returns> number of bytes copied </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long copy2null(NetcdfSharp.java.nio.channels.FileChannel in, int buffersize) throws IOException
	  public static long copy2null(FileChannel @in, int buffersize)
	  {
		long totalBytesRead = 0;
		if (buffersize <= 0)
		{
		  buffersize = default_file_buffersize;
		}
		ByteBuffer buffer = ByteBuffer.allocate(buffersize);
		while (true)
		{
		  int n = @in.read(buffer);
			  if (n <= 0) // FileChannel.read returns 0 at EOF in the shim
		  {
			break;
		  }
		  totalBytesRead += n;
		  buffer.flip();
		}
		return totalBytesRead;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long touch(NetcdfSharp.java.nio.channels.FileChannel in, int buffersize) throws IOException
	  public static long touch(FileChannel @in, int buffersize)
	  {
		long touch = 0;
		if (buffersize <= 0)
		{
		  buffersize = default_file_buffersize;
		}
		ByteBuffer buffer = ByteBuffer.allocate(buffersize);
		while (true)
		{
		  int n = @in.read(buffer);
			  if (n <= 0) // FileChannel.read returns 0 at EOF in the shim
		  {
			break;
		  }

		  // touch all the bytes
		  // buffer.rewind();
		  // for (int i=0; i<buffersize; i++)
		  // touch += buffer.get();

		  byte[] result = buffer.array();
		  for (int i = 0; i < buffersize; i++)
		  {
			touch += result[i];
		  }

		  buffer.flip();
		}
		return touch;
	  }

	  /// <summary>
	  /// copy all bytes from in to out, specify buffer size
	  /// </summary>
	  /// <param name="in"> InputStream </param>
	  /// <param name="out"> OutputStream </param>
	  /// <param name="bufferSize"> : internal buffer size. </param>
	  /// <returns> number of bytes copied </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long copyB(InputStream in, OutputStream out, int bufferSize) throws IOException
	  public static long copyB(Stream @in, Stream @out, int bufferSize)
	  {
		long totalBytesRead = 0;
		int done = 0, next = 1;

		byte[] buffer = new byte[bufferSize];
		while (true)
		{
		  int n = @in.Read(buffer, 0, buffer.Length);
			  if (n <= 0) // .NET Stream.Read returns 0 at EOF
		  {
			break;
		  }
		  @out.Write(buffer, 0, n);
		  totalBytesRead += n;

		  if (showCopy)
		  {
			done += n;
			if (done > 1000 * 1000 * next)
			{
			  Console.WriteLine(next + " Mb");
			  next++;
			}
		  }
		}
		@out.Flush();
		return totalBytesRead;
	  }

	  /// @deprecated use copyMaxBytes() 
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Deprecated("use copyMaxBytes()") public static void copy(InputStream in, OutputStream out, int n) throws IOException
	  [Obsolete("use copyMaxBytes()")]
	  public static void copy(Stream @in, Stream @out, int n)
	  {
		copyMaxBytes(@in, @out, n);
	  }

	  /// <summary>
	  /// Copy up to maxBytes bytes from in to out.
	  /// </summary>
	  /// <param name="in"> InputStream </param>
	  /// <param name="out"> OutputStream </param>
	  /// <param name="maxBytes"> number of bytes to copy </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void copyMaxBytes(InputStream in, OutputStream out, int maxBytes) throws IOException
	  public static void copyMaxBytes(Stream @in, Stream @out, int maxBytes)
	  {
		byte[] buffer = new byte[default_file_buffersize];
		int count = 0;
		while (true)
		{
		  int bytesRead = @in.Read(buffer, 0, buffer.Length);
		  if (bytesRead == -1)
		  {
			break;
		  }
		  @out.Write(buffer, 0, bytesRead);
		  count += bytesRead;
		  if (count > maxBytes)
		  {
			return;
		  }
		}
		@out.Flush();
	  }

	  /// <summary>
	  /// Read the contents from the inputStream and place into a String,
	  /// with any error messages put in the return String.
	  /// Assume UTF-8 encoding.
	  /// </summary>
	  /// <param name="is"> the inputStream to read from. </param>
	  /// <returns> String holding the contents, or an error message. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static String readContents(InputStream is) throws IOException
	  public static string readContents(Stream @is)
	  {
		return readContents(@is, CDM.UTF8);
	  }

	  /// <summary>
	  /// Read the contents from the inputStream and place into a String,
	  /// with any error messages put in the return String.
	  /// </summary>
	  /// <param name="is"> the inputStream to read from. </param>
	  /// <returns> String holding the contents, or an error message. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static String readContents(InputStream is, String charset) throws IOException
	  public static string readContents(Stream @is, string charset)
	  {
		MemoryStream bout = new MemoryStream(10 * default_file_buffersize);
		IO.copy(@is, bout);
		return Encoding.GetEncoding(charset).GetString(bout.ToArray());	  }

	  /// <summary>
	  /// Read the contents from the inputStream and place into a byte array,
	  /// with any error messages put in the return String.
	  /// </summary>
	  /// <param name="is"> the inputStream to read from. </param>
	  /// <returns> byte[] holding the contents, or an error message. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static byte[] readContentsToByteArray(InputStream is) throws IOException
	  public static byte[] readContentsToByteArray(Stream @is)
	  {
		MemoryStream bout = new MemoryStream(10 * default_file_buffersize);
		IO.copy(@is, bout);
		return bout.ToArray();
	  }

	  /// <summary>
	  /// Wite the contents from the String to a Stream,
	  /// </summary>
	  /// <param name="contents"> String holding the contents. </param>
	  /// <param name="os"> write to this OutputStream </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void writeContents(String contents, OutputStream os) throws IOException
	  public static void writeContents(string contents, Stream os)
	  {
		MemoryStream bin = new MemoryStream(Encoding.UTF8.GetBytes(contents));
		IO.copy(bin, os);
	  }

	  ////////////////////////////////////////////////////////////////////////////////////////////////////////
	  // Files

	  /// <summary>
	  /// copy one file to another.
	  /// </summary>
	  /// <param name="fileInName"> copy from this file, which must exist. </param>
	  /// <param name="fileOutName"> copy to this file, which is overrwritten if already exists. </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void copyFile(String fileInName, String fileOutName) throws IOException
	  public static void copyFile(string fileInName, string fileOutName)
	  {
		using (FileStream fin = new FileStream(fileInName, FileMode.Open, FileAccess.Read), fout = new FileStream(fileOutName, FileMode.Create, FileAccess.Write))
		{

		  Stream @in = new BufferedInputStream(fin);
		  Stream @out = new BufferedOutputStream(fout);
		  IO.copy(@in, @out);
		}
	  }

	  /// <summary>
	  /// copy one file to another.
	  /// </summary>
	  /// <param name="fileIn"> copy from this file, which must exist. </param>
	  /// <param name="fileOut"> copy to this file, which is overrwritten if already exists. </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void copyFile(File fileIn, File fileOut) throws IOException
	  public static void copyFile(File fileIn, File fileOut)
	  {
		using (FileStream fin = new FileStream(fileIn.GetPath(), FileMode.Open, FileAccess.Read), fout = new FileStream(fileOut.GetPath(), FileMode.Create, FileAccess.Write))
		{
		  Stream @in = new BufferedInputStream(fin);
		  Stream @out = new BufferedOutputStream(fout);
		  IO.copy(@in, @out);
		}
	  }

	  /// <summary>
	  /// copy file to output stream
	  /// </summary>
	  /// <param name="src"> source </param>
	  /// <param name="fileOut"> copy to this file </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void copy2File(byte[] src, String fileOut) throws IOException
	  public static void copy2File(byte[] src, string fileOut)
	  {
		using (FileStream fout = new FileStream(fileOut, FileMode.Create, FileAccess.Write))
		{
		  Stream @in = new BufferedInputStream(new MemoryStream(src));
		  Stream @out = new BufferedOutputStream(fout);
		  IO.copy(@in, @out);
		}
	  }

	  /// <summary>
	  /// copy file to output stream
	  /// </summary>
	  /// <param name="fileInName"> open this file </param>
	  /// <param name="out"> copy here </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void copyFile(String fileInName, OutputStream out) throws IOException
	  public static void copyFile(string fileInName, Stream @out)
	  {
		copyFileB(new File(fileInName), @out, default_file_buffersize);
	  }

	  /// <summary>
	  /// copy file to output stream
	  /// </summary>
	  /// <param name="fileIn"> copy this file </param>
	  /// <param name="out"> copy here </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void copyFile(File fileIn, OutputStream out) throws IOException
	  public static void copyFile(File fileIn, Stream @out)
	  {
		copyFileB(fileIn, @out, default_file_buffersize);
	  }

	  /// <summary>
	  /// copy file to output stream, specify internal buffer size
	  /// </summary>
	  /// <param name="fileIn"> copy this file </param>
	  /// <param name="out"> copy to this stream </param>
	  /// <param name="bufferSize"> internal buffer size. </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void copyFileB(File fileIn, OutputStream out, int bufferSize) throws IOException
	  public static void copyFileB(File fileIn, Stream @out, int bufferSize)
	  {
		using (FileStream fin = new FileStream(fileIn.GetPath(), FileMode.Open, FileAccess.Read))
		{
		  Stream @in = new BufferedInputStream(fin);
		  IO.copyB(@in, @out, bufferSize);
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void copyFileWithChannels(File fileIn, NetcdfSharp.java.nio.channels.WritableByteChannel out) throws IOException
	  public static void copyFileWithChannels(File fileIn, WritableByteChannel @out)
	  {
		FileChannel @in = (new FileStream(fileIn.GetPath(), FileMode.Open, FileAccess.Read)).getChannel();
		try
		{
		  long want = fileIn.length();
		  long pos = 0;
		  while (pos < want)
		  {
			long did = @in.transferTo(pos, want - pos, @out);
			pos += did;
			want -= did;
		  }
		}
		finally
		{
		  @in.close();
		}
	  }

	  /*
	   * static public void copyFileWithChannels2(File fileIn, WritableByteChannel out, int bufferSize) throws IOException {
	   * ReadableByteChannel in = new FileInputStream(fileIn).getChannel();
	   * copy(in, out, 32 * 1024);
	   * }
	   * 
	   * // Read all available bytes from one channel and copy them to the other.
	   * static public void copy(ReadableByteChannel in, WritableByteChannel out, int bufferSize) throws IOException {
	   * // First, we need a buffer to hold blocks of copied bytes.
	   * ByteBuffer buffer = ByteBuffer.allocateDirect(bufferSize);
	   * 
	   * // Now loop until no more bytes to read and the buffer is empty
	   * while (in.read(buffer) != -1 || buffer.position() > 0) {
	   * // The read() call leaves the buffer in "fill mode". To prepare
	   * // to write bytes from the bufferwe have to put it in "drain mode"
	   * // by flipping it: setting limit to position and position to zero
	   * ((Buffer) buffer).flip();
	   * 
	   * // Now write some or all of the bytes out to the output channel
	   * out.write(buffer);
	   * 
	   * // Compact the buffer by discarding bytes that were written,
	   * // and shifting any remaining bytes. This method also
	   * // prepares the buffer for the next call to read() by setting the
	   * // position to the limit and the limit to the buffer capacity.
	   * buffer.compact();
	   * }
	   * }
	   */

	  /// <summary>
	  /// Copy part of a RandomAccessFile to output stream
	  /// </summary>
	  /// <param name="raf"> copy this file </param>
	  /// <param name="offset"> start here (byte offset) </param>
	  /// <param name="length"> number of bytes to copy </param>
	  /// <param name="out"> copy to this stream </param>
	  /// <returns> number of bytes copied </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long copyRafB(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long offset, long length, OutputStream out) throws IOException
	  public static long copyRafB(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long offset, long length, Stream @out)
	  {
		return copyRafB(raf, offset, length, @out, new byte[default_file_buffersize]);
	  }

	  /// <summary>
	  /// Copy part of a RandomAccessFile to output stream, specify internal buffer size
	  /// </summary>
	  /// <param name="raf"> copy this file </param>
	  /// <param name="offset"> start here (byte offset) </param>
	  /// <param name="length"> number of bytes to copy </param>
	  /// <param name="out"> copy to this stream </param>
	  /// <param name="buffer"> use this buffer. </param>
	  /// <returns> number of bytes copied </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long copyRafB(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long offset, long length, OutputStream out, byte[] buffer) throws IOException
	  public static long copyRafB(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long offset, long length, Stream @out, byte[] buffer)
	  {
		int bufferSize = buffer.Length;
		long want = length;
		raf.seek(offset);
		while (want > 0)
		{
		  int len = (int) Math.Min(want, bufferSize);
		  int bytesRead = raf.read(buffer, 0, len);
		  if (bytesRead <= 0)
		  {
			break;
		  }
		  @out.Write(buffer, 0, bytesRead);
		  want -= bytesRead;
		}
		@out.Flush();
		return length - want;
	  }

	  /// <summary>
	  /// Copy an entire directory tree.
	  /// </summary>
	  /// <param name="fromDirName"> from this directory (do nothing if not exist) </param>
	  /// <param name="toDirName"> to this directory (will create if not exist) </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void copyDirTree(String fromDirName, String toDirName) throws IOException
	  public static void copyDirTree(string fromDirName, string toDirName)
	  {
		File fromDir = new File(fromDirName);
		File toDir = new File(toDirName);

		if (!fromDir.exists())
		{
		  return;
		}

		if (!toDir.exists())
		{
		  if (!toDir.mkdirs())
		  {
			throw new IOException("Could not create directory: " + toDir);
		  }
		}

		File[] files = fromDir.listFiles();
		if (files != null)
		{
		  foreach (File f in files)
		  {
			if (f.isDirectory())
			{
			  copyDirTree(f.getAbsolutePath(), toDir.getAbsolutePath() + "/" + f.getName());
			}
			else
			{
			  copyFile(f.getAbsolutePath(), toDir.getAbsolutePath() + "/" + f.getName());
			}
		  }
		}
	  }

	  /// <summary>
	  /// Read the file and place contents into a byte array,
	  /// with any error messages put in the return String.
	  /// </summary>
	  /// <param name="filename"> the file to read from. </param>
	  /// <returns> byte[] holding the contents, or an error message. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static byte[] readFileToByteArray(String filename) throws IOException
	  public static byte[] readFileToByteArray(string filename)
	  {
		using (FileStream fin = new FileStream(filename, FileMode.Open, FileAccess.Read))
		{
		  Stream @in = new BufferedInputStream(fin);
		  return readContentsToByteArray(@in);
		}
	  }

	  /// <summary>
	  /// Read the contents from the named file and place into a String, assuming UTF-8 encoding.
	  /// </summary>
	  /// <param name="filename"> the URL to read from. </param>
	  /// <returns> String holding the file contents </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static String readFile(String filename) throws IOException
	  public static string readFile(string filename)
	  {
		using (FileStream fin = new FileStream(filename, FileMode.Open, FileAccess.Read))
		{
		  StreamReader reader = new StreamReader(fin, Encoding.UTF8);
		  return reader.ReadToEnd();
		}
	  }

	  /// <summary>
	  /// Write String contents to a file, using UTF-8 encoding.
	  /// </summary>
	  /// <param name="contents"> String holding the contents </param>
	  /// <param name="file"> write to this file (overwrite if exists) </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void writeToFile(String contents, File file) throws IOException
	  public static void writeToFile(string contents, File file)
	  {
		using (FileStream fout = new FileStream(file.GetPath(), FileMode.Create, FileAccess.Write))
		{
		  StreamWriter fw = new StreamWriter(fout, Encoding.UTF8);
		  fw.Write(contents);
		  fw.Flush();
		}
	  }

	  /// <summary>
	  /// Write byte[] contents to a file.
	  /// </summary>
	  /// <param name="contents"> String holding the contents </param>
	  /// <param name="file"> write to this file (overwrite if exists) </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void writeToFile(byte[] contents, File file) throws IOException
	  public static void writeToFile(byte[] contents, File file)
	  {
		using (FileStream fw = new FileStream(file.GetPath(), FileMode.Create, FileAccess.Write))
		{
		  fw.Write(contents, 0, contents.Length);
		  fw.Flush();
		}
	  }

	  /// <summary>
	  /// Write contents to a file, using UTF-8 encoding.
	  /// </summary>
	  /// <param name="contents"> String holding the contents </param>
	  /// <param name="fileOutName"> write to this file (overwrite if exists) </param>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void writeToFile(String contents, String fileOutName) throws IOException
	  public static void writeToFile(string contents, string fileOutName)
	  {
		writeToFile(contents, new File(fileOutName));
	  }

	  /// <summary>
	  /// copy input stream to file. close input stream when done.
	  /// </summary>
	  /// <param name="in"> copy from here </param>
	  /// <param name="fileOutName"> open this file (overwrite) and copy to it. </param>
	  /// <returns> number of bytes copied </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long writeToFile(InputStream in, String fileOutName) throws IOException
	  public static long writeToFile(Stream @in, string fileOutName)
	  {
		try
		{
				using (FileStream fout = new FileStream(fileOutName, FileMode.Create, FileAccess.Write))
				{
			  Stream @out = new BufferedOutputStream(fout);
			  return IO.copy(@in, @out);
				}
		}
		finally
		{
		  if (null != @in)
		  {
			@in.Close();
		  }
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long appendToFile(InputStream in, String fileOutName) throws IOException
	  public static long appendToFile(Stream @in, string fileOutName)
	  {
		using (FileStream fout = new FileStream(fileOutName, FileMode.Create, FileAccess.Write))
		{
		  Stream @out = new BufferedOutputStream(fout);
		  return IO.copy(@in, @out);
		}
	  }

	  ////////////////////////////////////////////////////////////////////////////////////////////////////////
	  // URLs

	  /// <summary>
	  /// copy contents of URL to output stream, specify internal buffer size. request gzip encoding
	  /// </summary>
	  /// <param name="urlString"> copy the contents of this URL </param>
	  /// <param name="out"> copy to this stream. If null, throw bytes away </param>
	  /// <param name="bufferSize"> internal buffer size. </param>
	  /// <returns> number of bytes copied </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static long copyUrlB(String urlString, OutputStream out, int bufferSize) throws IOException
	  public static long copyUrlB(string urlString, Stream @out, int bufferSize)
	  {
		long count = 0;
		URL url;

		try
		{
		  url = new URL(urlString);
		}
		catch (MalformedURLException e)
		{
		  throw new IOException("** MalformedURLException on URL <" + urlString + ">\n" + e.Message + "\n");
		}

		try
		{
		  NetcdfSharp.java.net.URLConnection connection = url.openConnection();
		  HttpURLConnection httpConnection = null;
		  if (connection is HttpURLConnection)
		  {
			httpConnection = (HttpURLConnection) connection;
			httpConnection.addRequestProperty("Accept-Encoding", "gzip");
		  }


		  if (showHeaders)
		  {
			showRequestHeaders(urlString, connection);
		  }

		  // get response
		  if (httpConnection != null)
		  {
			int responseCode = httpConnection.getResponseCode();
			if (responseCode / 100 != 2)
			{
			  throw new IOException("** Cant open URL <" + urlString + ">\n Response code = " + responseCode + "\n" + httpConnection.getResponseMessage() + "\n");
			}
		  }

		  if (showHeaders && (httpConnection != null))
		  {
			int code = httpConnection.getResponseCode();
			string response = httpConnection.getResponseMessage();

			// response headers
			Console.WriteLine("\nRESPONSE for " + urlString + ": ");
			Console.WriteLine(" HTTP/1.x " + code + " " + response);
			Console.WriteLine("Headers: ");

			for (int j = 1;; j++)
			{
			  string header = connection.getHeaderField(j);
			  string key = connection.getHeaderFieldKey(j);
			  if (header == null || key == null)
			  {
				break;
			  }
			  Console.WriteLine(" " + key + ": " + header);
			}
		  }

		  // read it
		  using (Stream @is = connection.getInputStream())
		  {
			BufferedInputStream bis;

			// check if its gzipped
			if ("gzip".Equals(connection.getContentEncoding(), StringComparison.OrdinalIgnoreCase))
			{
			  bis = new BufferedInputStream(new GZipStream(@is, CompressionMode.Decompress), 8000);
			}
			else
			{
			  bis = new BufferedInputStream(@is, 8000);
			}

			if (@out == null)
			{
			  count = IO.copy2null(bis, bufferSize);
			}
			else
			{
			  count = IO.copyB(bis, @out, bufferSize);
			}
		  }
		}
		catch (NetcdfSharp.java.net.ConnectException e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  throw new IOException("** ConnectException on URL: <" + urlString + ">\n" + e.Message + "\nServer probably not running");
		}
		catch (NetcdfSharp.java.net.UnknownHostException e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  throw new IOException("** UnknownHostException on URL: <" + urlString + ">\n");
		}
		catch (Exception e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  throw new IOException("** Exception on URL: <" + urlString + ">\n" + e);
		}

		return count;
	  }

	  private static void showRequestHeaders(string urlString, NetcdfSharp.java.net.URLConnection connection)
	  {
		Console.WriteLine("\nREQUEST Properties for " + urlString + ": ");
		IDictionary<string, List<string>> reqs = connection.getRequestProperties();
		foreach (KeyValuePair<string, List<string>> entry in reqs)
		{
		  Console.Write(" {0}:", entry.Key);
		  foreach (string v in entry.Value)
		  {
			Console.Write("{0},", v);
		  }
		  Console.Write("\n");
		}
	  }

	  /// <summary>
	  /// get input stream from URL
	  /// </summary>
	  /// <param name="urlString"> URL </param>
	  /// <returns> input stream, unzipped if needed </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
	  public static Stream getInputStreamFromUrl(string urlString)
	  {
		URL url;

		try
		{
		  url = new URL(urlString);
		}
		catch (MalformedURLException e)
		{
		  throw new IOException("** MalformedURLException on URL <" + urlString + ">\n" + e.Message + "\n");
		}

		try
		{
		  NetcdfSharp.java.net.URLConnection connection = url.openConnection();
		  HttpURLConnection httpConnection = null;
		  if (connection is HttpURLConnection)
		  {
			httpConnection = (HttpURLConnection) connection;
			httpConnection.addRequestProperty("Accept-Encoding", "gzip");
		  }

		  if (showHeaders)
		  {
			showRequestHeaders(urlString, connection);
		  }

		  // get response
		  if (httpConnection != null)
		  {
			int responseCode = httpConnection.getResponseCode();
			if (responseCode / 100 != 2)
			{
			  throw new IOException("** Cant open URL <" + urlString + ">\n Response code = " + responseCode + "\n" + httpConnection.getResponseMessage() + "\n");
			}
		  }

		  if (showHeaders && (httpConnection != null))
		  {
			int code = httpConnection.getResponseCode();
			string response = httpConnection.getResponseMessage();

			// response headers
			Console.WriteLine("\nRESPONSE for " + urlString + ": ");
			Console.WriteLine(" HTTP/1.x " + code + " " + response);
			Console.WriteLine("Headers: ");

			for (int j = 1;; j++)
			{
			  string header = connection.getHeaderField(j);
			  string key = connection.getHeaderFieldKey(j);
			  if (header == null || key == null)
			  {
				break;
			  }
			  Console.WriteLine(" " + key + ": " + header);
			}
		  }

		  Stream @is = null;
		  try
		  {
			// read it
			@is = connection.getInputStream();

			// check if its gzipped
			if ("gzip".Equals(connection.getContentEncoding(), StringComparison.OrdinalIgnoreCase))
			{
			  @is = new BufferedInputStream(new GZipStream(@is, CompressionMode.Decompress), 1000);
			}
		  }
		  catch (Exception)
		  {
			if (@is != null)
			{
			  @is.Close();
			}
		  }
		  return @is;
		}
		catch (NetcdfSharp.java.net.ConnectException e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  throw new IOException("** ConnectException on URL: <" + urlString + ">\n" + e.Message + "\nServer probably not running");
		}
		catch (NetcdfSharp.java.net.UnknownHostException e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  throw new IOException("** UnknownHostException on URL: <" + urlString + ">\n");
		}
		catch (Exception e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  throw new IOException("** Exception on URL: <" + urlString + ">\n" + e);
		}
	  }

	  /// <summary>
	  /// read the contents from the named URL, write to a file.
	  /// </summary>
	  /// <param name="urlString"> the URL to read from. </param>
	  /// <param name="file"> write to this file </param>
	  /// <returns> status or error message. </returns>
	  public static string readURLtoFile(string urlString, File file)
	  {
		try
		{
		  using (FileStream fout = new FileStream(file.GetPath(), FileMode.Create, FileAccess.Write))
		  {
			Stream @out = new BufferedOutputStream(fout);
			copyUrlB(urlString, @out, 20000);
			return "ok";
		  }
		}
		catch (FileNotFoundException e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  return "** IOException opening file: <" + file.getPath() + ">\n" + e.Message + "\n";
		}
		catch (IOException e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  return "** IOException reading URL: <" + urlString + ">\n" + e.Message + "\n";
		}
	  }

	  /// <summary>
	  /// Read the contents from the given URL and place into a byte array,
	  /// with any error messages put in the return String.
	  /// </summary>
	  /// <param name="urlString"> read from this URL. </param>
	  /// <returns> byte[] holding the contents, or an error message. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on io error </exception>
	  public static byte[] readURLContentsToByteArray(string urlString)
	  {
		MemoryStream bout = new MemoryStream(200000);
		copyUrlB(urlString, bout, 200000);
		return bout.ToArray();
	  }

	  /// <summary>
	  /// read the contents from the named URL, write to a file.
	  /// </summary>
	  /// <param name="urlString"> the URL to read from. </param>
	  /// <param name="file"> write to this file </param>
	  /// <returns> status or error message. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> if failure </exception>
	  public static string readURLtoFileWithExceptions(string urlString, File file)
	  {
		return readURLtoFileWithExceptions(urlString, file, default_socket_buffersize);
	  }

	  /// <summary>
	  /// read the contents from the named URL, write to a file.
	  /// </summary>
	  /// <param name="urlString"> the URL to read from. </param>
	  /// <param name="file"> write to this file </param>
	  /// <param name="buffer_size"> read/write in this size chunks </param>
	  /// <returns> status or error message. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> if failure </exception>
	  public static string readURLtoFileWithExceptions(string urlString, File file, int buffer_size)
	  {
		using (FileStream fout = new FileStream(file.GetPath(), FileMode.Create, FileAccess.Write))
		{
		  Stream @out = new BufferedOutputStream(fout);
		  copyUrlB(urlString, @out, buffer_size);
		  return "ok";
		}
	  }

	  /// <summary>
	  /// Read the contents from the named URL and place into a String.
	  /// </summary>
	  /// <param name="urlString"> the URL to read from. </param>
	  /// <returns> String holding the contents. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> if fails </exception>
	  public static string readURLcontentsWithException(string urlString)
	  {
		MemoryStream bout = new MemoryStream(20000);
		copyUrlB(urlString, bout, 20000);
		return Encoding.GetEncoding(CDM.UTF8).GetString(bout.ToArray());	  }

	  /// <summary>
	  /// Read the contents from the named URL and place into a String,
	  /// with any error messages put in the return String.
	  /// </summary>
	  /// <param name="urlString"> the URL to read from. </param>
	  /// <returns> String holding the contents, or an error message. </returns>
	  public static string readURLcontents(string urlString)
	  {
		try
		{
		  return readURLcontentsWithException(urlString);
		}
		catch (IOException e)
		{
		  return e.Message;
		}
	  }

	  /// <summary>
	  /// use HTTP PUT to send the contents to the named URL.
	  /// </summary>
	  /// <param name="urlString"> the URL to read from. must be http: </param>
	  /// <param name="contents"> String holding the contents </param>
	  /// <returns> a Result object; generally 0 &lt;= code &lt;=400 is ok </returns>
	  public static HttpResult putToURL(string urlString, string contents)
	  {
		URL url;
		try
		{
		  url = new URL(urlString);
		}
		catch (MalformedURLException e)
		{
		  return new HttpResult(-1, "** MalformedURLException on URL (" + urlString + ")\n" + e.Message);
		}

		try
		{
		  HttpURLConnection c = (HttpURLConnection) url.openConnection();
		  c.setDoOutput(true);
		  c.setRequestMethod("PUT");

		  // write it
		  using (Stream @out = c.getOutputStream())
		  {
			BufferedOutputStream bout = new BufferedOutputStream(@out);
			IO.copy(new MemoryStream(Encoding.UTF8.GetBytes(contents)), bout);
		  }

		  int code = c.getResponseCode();
		  string mess = c.getResponseMessage();
		  return new HttpResult(code, mess);
		}
		catch (NetcdfSharp.java.net.ConnectException e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  return new HttpResult(-2, "** ConnectException on URL: <" + urlString + ">\n" + e.Message + "\nServer probably not running");
		}
		catch (IOException e)
		{
		  if (showStackTrace)
		  {
			Console.WriteLine(e);
		  }
		  return new HttpResult(-3, "** IOException on URL: (" + urlString + ")\n" + e.Message);
		}
	  }

	  /// <summary>
	  /// Holds the result of an HTTP action.
	  /// </summary>
	  public class HttpResult
	  {
		public int statusCode;
		public string message;

		internal HttpResult(int code, string message)
		{
		  this.statusCode = code;
		  this.message = message;
		}
	  }

	}
#pragma warning restore CS0162

}
