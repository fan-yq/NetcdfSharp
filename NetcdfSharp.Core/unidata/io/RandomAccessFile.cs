// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.unidata.io
{

	using DatasetUrl = NetcdfSharp.ucar.nc2.dataset.DatasetUrl;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using FileCache = NetcdfSharp.ucar.nc2.util.cache.FileCache;
	using FileCacheIF = NetcdfSharp.ucar.nc2.util.cache.FileCacheIF;
	using FileCacheable = NetcdfSharp.ucar.nc2.util.cache.FileCacheable;
	using FileFactory = NetcdfSharp.ucar.nc2.util.cache.FileFactory;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;


	/// <summary>
	/// A class intended as drop-in replacement for NetcdfSharp.java.io.RandomAccessFile, with some notable extensions:
	/// <ul>
	/// <li>Buffered I/O: instances of this class realise substantial speed increases over
	/// * NetcdfSharp.java.io.RandomAccessFile through the use of buffering.</li>
	/// <li>Read String methods support user-specified Charsets (default UTF-8).</li>
	/// <li>Support for both big and little endiannness on reads and write: users may specify the byte order for I/O
	/// operations.</li>
	/// <li>Support for distributed file access through the RandomAccessDirectory subclass.</li>
	/// </ul>
	/// This is a subclass of Object, as it was not possible to subclass
	/// NetcdfSharp.java.io.RandomAccessFile because many of the methods are
	/// final. However, if it is necessary to use RandomAccessFile and
	/// NetcdfSharp.java.io.RandomAccessFile interchangeably, both classes implement the
	/// DataInput and DataOutput interfaces.
	/// <p/>
	/// <para>
	/// By Russ Rew, based on
	/// BufferedRandomAccessFile by Alex McManus, based on Sun's source code
	/// for NetcdfSharp.java.io.RandomAccessFile. For Alex McManus version from which
	/// this derives, see his <a href="http://www.aber.ac.uk/~agm/Java.html">
	/// Freeware Java Classes</a>.
	/// <p/>
	/// Must be thread confined - that is, can only be used by a single thread at a time..
	/// 
	/// @author Alex McManus
	/// @author Russ Rew
	/// @author john caron
	/// </para>
	/// </summary>
	/// <seealso cref="NetcdfSharp.java.io.DataInput"/>
	/// <seealso cref="NetcdfSharp.java.io.DataOutput"/>
	/// <seealso cref="NetcdfSharp.java.io.RandomAccessFile"/>

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @NotThreadSafe public class RandomAccessFile implements DataInput, DataOutput, NetcdfSharp.ucar.nc2.util.cache.FileCacheable, Closeable
	public class RandomAccessFile : DataInput, DataOutput, FileCacheable, IDisposable
	{

	  public const int BIG_ENDIAN = 0;
	  public const int LITTLE_ENDIAN = 1;

	  protected internal const int defaultBufferSize = 8092; // The default buffer size, in bytes.

	  ///////////////////////////////////////////////////////////////////////
	  // debug leaks - keep track of open files
	  protected internal static bool debugLeaks;
	  protected internal static bool debugAccess;
	  protected internal static ISet<string> allFiles;
	  protected internal static IList<string> openFiles = Collections.synchronizedList(new List<string>()); // could keep map on file
																								 // hashcode
	  private static AtomicLong count_openFiles = new AtomicLong();
	  private static AtomicInteger maxOpenFiles = new AtomicInteger();
	  private static AtomicInteger debug_nseeks = new AtomicInteger();
	  private static AtomicLong debug_nbytes = new AtomicLong();

	  protected internal static bool showOpen;
	  protected internal static bool showRead;


	  /// @deprecated do not use. 
	  [Obsolete("do not use.")]
	  public static IList<string> OpenFiles
	  {
		  get
		  {
			return openFiles.AsReadOnly();
		  }
	  }

	  /// @deprecated do not use. 
	  [Obsolete("do not use.")]
	  public static IList<string> AllFiles
	  {
		  get
		  {
			if (null == allFiles)
			{
			  return null;
			}
			return allFiles.OrderBy(c => c).ToList();
		  }
	  }

	  /////////////////////////////////////////////////////////////////////////////////////////////
	  // internal File Caching. this allows a global pool of OS files.
	  // note read only

	  private static readonly FileFactory factory = new FileFactoryAnonymousInnerClass();

	  private class FileFactoryAnonymousInnerClass : FileFactory
	  {

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.nc2.util.cache.FileCacheable open(NetcdfSharp.ucar.nc2.dataset.DatasetUrl durl, int buffer_size, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask, Object iospMessage) throws IOException
		  public FileCacheable open(DatasetUrl durl, int buffer_size, CancelTask cancelTask, object iospMessage)
		  {
			string location = StringUtil2.replace(durl.location, "\\", "/"); // canonicalize the name
			RandomAccessFile result = new RandomAccessFile(location, "r", buffer_size);
			result.cacheState = CacheState.IN_USE;
			return result;
		  }
	  }

	  private static FileCacheIF cache;

	  public static void enableDefaultGlobalFileCache()
	  {
		  lock (typeof(RandomAccessFile))
		  {
			// file caching is disabled in NetcdfSharp; keep cache null
			cache = null;
		  }
	  }

	  public static FileCacheIF GlobalFileCache
	  {
		  set
		  {
			  lock (typeof(RandomAccessFile))
			  {
				cache = value;
			  }
		  }
		  get
		  {
			  lock (typeof(RandomAccessFile))
			  {
				return cache;
			  }
		  }
	  }


// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static RandomAccessFile acquire(String location) throws IOException
	  public static RandomAccessFile acquire(string location)
	  {
		return new RandomAccessFile(location, "r");
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static RandomAccessFile acquire(String location, int buffer_size) throws IOException
	  public static RandomAccessFile acquire(string location, int buffer_size)
	  {
		return new RandomAccessFile(location, "r", buffer_size);
	  }

	  public static void eject(string location)
	  {
		// file caching is disabled in NetcdfSharp
	  }

	  public static void shutdown()
	  {
		// file caching is disabled in NetcdfSharp
	  }

	  /////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// File location
	  /// </summary>
	  protected internal string location;

	  internal enum CacheState
	  {
		NOT_IN_CACHE,
		IN_USE,
		NOT_IN_USE
	  }

	  // Java reads cacheState via getCacheState() and a switch in release(); C# lost those readers. Kept for cache logic parity.
#pragma warning disable CS0414 // assigned but never read in the C# translation
	  private CacheState cacheState = CacheState.NOT_IN_CACHE;
#pragma warning restore CS0414

	  /// <summary>
	  /// The underlying NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  protected internal NetcdfSharp.java.io.RandomAccessFile file;
	  protected internal NetcdfSharp.java.nio.channels.FileChannel fileChannel;

	  /// <summary>
	  /// The offset in bytes from the file start, of the next read or
	  /// write operation.
	  /// </summary>
	  protected internal long filePosition;

	  /// <summary>
	  /// The buffer used for reading the data.
	  /// </summary>
	  protected internal byte[] buffer;

	  /// <summary>
	  /// The offset in bytes of the start of the buffer, from the start of the file.
	  /// </summary>
	  protected internal long bufferStart;

	  /// <summary>
	  /// The offset in bytes of the end of the data in the buffer, from
	  /// the start of the file. This can be calculated from
	  /// <code>bufferStart + dataSize</code>, but it is cached to speed
	  /// up the read( ) method.
	  /// </summary>
	  protected internal long dataEnd;

	  /// <summary>
	  /// The size of the data stored in the buffer, in bytes. This may be
	  /// less than the size of the buffer.
	  /// </summary>
	  protected internal int dataSize;

	  /// <summary>
	  /// True if we are at the end of the file.
	  /// </summary>
	  protected internal bool endOfFile;

	  /// <summary>
	  /// The access mode of the file.
	  /// </summary>
	  protected internal bool @readonly;

	  /// <summary>
	  /// The current endian (big or little) mode of the file.
	  /// </summary>
	  protected internal bool bigEndian;

	  /// <summary>
	  /// True if the data in the buffer has been modified.
	  /// </summary>
	  protected internal bool bufferModified;

	  /// <summary>
	  /// make sure file is at least this long when closed
	  /// </summary>
	  private long minLength;

	  /// <summary>
	  /// STUPID extendMode for truncated, yet valid files. old netcdf C library code allowed NOFILL to do this
	  /// </summary>
	  private bool extendMode;

	  /// <summary>
	  /// Constructor, for subclasses
	  /// </summary>
	  /// <param name="bufferSize"> size of read buffer </param>
	  protected internal RandomAccessFile(int bufferSize)
	  {
		file = null;
		@readonly = true;
		init(bufferSize);
	  }

	  /// <summary>
	  /// Constructor, default buffer size.
	  /// </summary>
	  /// <param name="location"> location of the file </param>
	  /// <param name="mode"> same as for NetcdfSharp.java.io.RandomAccessFile, usually "r" or "rw" </param>
	  /// <exception cref="IOException"> on open error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public RandomAccessFile(String location, String mode) throws IOException
	  public RandomAccessFile(string location, string mode) : this(location, mode, defaultBufferSize)
	  {
		this.location = location;
	  }

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="location"> location of the file </param>
	  /// <param name="mode"> same as for NetcdfSharp.java.io.RandomAccessFile </param>
	  /// <param name="bufferSize"> size of buffer to use. </param>
	  /// <exception cref="IOException"> on open error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public RandomAccessFile(String location, String mode, int bufferSize) throws IOException
	  public RandomAccessFile(string location, string mode, int bufferSize)
	  {
		if (bufferSize < 0)
		{
		  bufferSize = defaultBufferSize;
		}
		this.location = location;
		if (debugLeaks)
		{
		  allFiles.Add(location);
		}

		try
		{
		  this.file = new NetcdfSharp.java.io.RandomAccessFile(location, mode);
		}
		catch (IOException ioe)
		{
		  if (ioe.Message.Equals("Too many open files"))
		  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			Console.Write("RandomAccessFile {0}%n", ioe);
			try
			{
			  Thread.Sleep(100);
			}
			catch (InterruptedException e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			}
			this.file = new NetcdfSharp.java.io.RandomAccessFile(location, mode); // Windows having trouble keeping up ??
		  }
		  else
		  {
			throw;
		  }
		}

		this.@readonly = mode.Equals("r");
		init(bufferSize);

		if (debugLeaks)
		{
		  openFiles.Add(location);
		  int max = Math.Max(openFiles.Count, maxOpenFiles.get());
		  maxOpenFiles.set(max);
		  count_openFiles.getAndIncrement();
		  if (showOpen)
		  {
			Console.WriteLine(" DebugRAF open " + location);
		  }
		}
	  }

	  /// <summary>
	  /// Allow access to the underlying NetcdfSharp.java.io.RandomAccessFile.
	  /// WARNING! BROKEN ENCAPSOLATION, DO NOT USE. May change implementation in the future.
	  /// </summary>
	  /// <returns> the underlying NetcdfSharp.java.io.RandomAccessFile. </returns>
	  public virtual NetcdfSharp.java.io.RandomAccessFile getRandomAccessFile()
	  {
		  return this.file;
	  }

	  private void init(int bufferSize)
	  {
		// Initialise the buffer
		bufferStart = 0;
		dataEnd = 0;
		dataSize = 0;
		filePosition = 0;
		buffer = new byte[bufferSize];
		endOfFile = false;
		bigEndian = true;
	  }

	  /// <summary>
	  /// Set the buffer size.
	  /// If writing, call flush() first.
	  /// </summary>
	  /// <param name="bufferSize"> length in bytes </param>
	  public virtual int BufferSize
	  {
		  set
		  {
			init(value);
		  }
		  get
		  {
			return buffer.Length;
		  }
	  }


	  /// <summary>
	  /// Close the file, and release any associated system resources.
	  /// </summary>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public synchronized void close() throws IOException
	  public virtual void Dispose()
	  {
		  lock (this)
		  {
			if (debugLeaks)
			{
			  openFiles.Remove(location);
			  if (showOpen)
			  {
				Console.WriteLine("  close " + location);
			  }
			}

			if (file == null)
			{
			  return;
			}

			// If we are writing and the buffer has been modified, flush the contents of the buffer.
			flush();

			// may need to extend file, in case no fill is being used
			// may need to truncate file in case overwriting a longer file
			// use only if minLength is set (by N3iosp)
			long fileSize = file.length();
			if (!@readonly && (minLength != 0) && (minLength != fileSize))
			{
			  file.setLength(minLength);
			}

			// Close the underlying file object.
			file.close();
			file = null; // help the gc
		  }
	  }

	  public virtual void close()
	  {
		  Dispose();
	  }

	  /// @deprecated do not use 
	  [Obsolete("do not use")]
	  public virtual void release()
	  { // one to one with NetcdfSharp.java.io.RandomAccessFile
		cacheState = NetcdfSharp.ucar.unidata.io.RandomAccessFile.CacheState.NOT_IN_USE;
	  }

	  /// @deprecated do not use 
	  [Obsolete("do not use")]
	  public virtual void reacquire()
	  {
		cacheState = NetcdfSharp.ucar.unidata.io.RandomAccessFile.CacheState.IN_USE;
	  }

	  /// @deprecated do not use 
	  [Obsolete("do not use")]
	  public virtual FileCacheIF FileCache
	  {
		  set
		  {
			  lock (this)
			  {
				if (value == null)
				{
				  cacheState = NetcdfSharp.ucar.unidata.io.RandomAccessFile.CacheState.NOT_IN_CACHE;
				}
			  }
		  }
	  }

	  public virtual long LastModified
	  {
		  get
		  {
			File file = new File(Location);
			return file.lastModified();
		  }
	  }

	  /// <summary>
	  /// Returns true if file pointer is at end of file. </summary>
	  public virtual bool AtEndOfFile
	  {
		  get
		  {
			return endOfFile;
		  }
	  }

	  /// <summary>
	  /// Returns true if RandomAccessFile represents a directory structure </summary>
	  public virtual bool Directory
	  {
		  get
		  {
			return false;
		  }
	  }

	  /// <summary>
	  /// Set the position in the file for the next read or write.
	  /// </summary>
	  /// <param name="pos"> the offset (in bytes) from the start of the file. </param>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void seek(long pos) throws IOException
	  public virtual void seek(long pos)
	  {
		if (pos < 0)
		{
		  throw new System.IO.IOException("Negative seek offset");
		}

		// If the seek is into the buffer, just update the file pointer.
		if ((pos >= bufferStart) && (pos < dataEnd))
		{
		  filePosition = pos;
		  endOfFile = false;
		  return;
		}

		// need new buffer, starting at pos
		readBuffer(pos);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected void readBuffer(long pos) throws IOException
	  protected internal virtual void readBuffer(long pos)
	  {
		// If the current buffer is modified, write it to disk.
		if (bufferModified)
		{
		  flush();
		}

		bufferStart = pos;
		filePosition = pos;

		dataSize = read_(pos, buffer, 0, buffer.Length);

		if (dataSize <= 0)
		{
		  dataSize = 0;
		  endOfFile = true;
		}
		else
		{
		  endOfFile = false;
		}

		// Cache the position of the buffer end.
		dataEnd = bufferStart + dataSize;
	  }

	  /// <summary>
	  /// Returns the current position in the file, where the next read or
	  /// write will occur.
	  /// </summary>
	  /// <returns> the offset from the start of the file in bytes. </returns>
	  public virtual long FilePointer
	  {
		  get
		  {
			return filePosition;
		  }
	  }

	  /// <summary>
	  /// Get the file location, or name.
	  /// </summary>
	  /// <returns> file location </returns>
	  public virtual string Location
	  {
		  get
		  {
			return location;
		  }
	  }

	  /// <summary>
	  /// Get the file location, or name.
	  /// </summary>
	  /// <returns> file location </returns>
	  public virtual string getLocation()
	  {
		return location;
	  }

	  /// <summary>
	  /// Get the length of the file. The data in the buffer (which may not
	  /// have been written the disk yet) is taken into account.
	  /// </summary>
	  /// <returns> the length of the file in bytes. </returns>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public long length() throws IOException
	  public virtual long length()
	  {
		long fileLength = (file == null) ? -1L : file.length(); // GRIB has closed the data raf
		if (fileLength < dataEnd)
		{
		  return dataEnd;
		}
		else
		{
		  return fileLength;
		}
	  }

	  /// <summary>
	  /// Change the current endian mode. Subsequent reads of short, int, float, double, long, char will
	  /// use this. Does not currently affect writes - ByteOrder must be explicitly specified on writes.
	  /// Default values is BIG_ENDIAN.
	  /// <para/>
	  /// This method is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="endian"> RandomAccessFile.BIG_ENDIAN or RandomAccessFile.LITTLE_ENDIAN </param>
	  public virtual void order(int endian)
	  {
		if (endian < 0)
		{
		  return;
		}
		this.bigEndian = (endian == BIG_ENDIAN);
	  }

	  public virtual void order(NetcdfSharp.java.nio.ByteOrder bo)
	  {
		if (bo == null)
		{
		  return;
		}
		this.bigEndian = bo == NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
	  }

	  /// <summary>
	  /// Copy the contents of the buffer to the disk.
	  /// </summary>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void flush() throws IOException
	  public virtual void flush()
	  {
		if (bufferModified)
		{
		  file.seek(bufferStart);
		  file.write(buffer, 0, dataSize);
		  bufferModified = false;
		}
	  }

	  /// <summary>
	  /// Make sure file is at least this long when its closed.
	  /// needed when not using fill mode, and not all data is written.
	  /// </summary>
	  /// <param name="minLength"> minimum length of the file. </param>
	  public virtual long MinLength
	  {
		  set
		  {
			this.minLength = value;
		  }
	  }

	  /// <summary>
	  /// Make sure file is at least this long when its closed.
	  /// needed when not using fill mode, and not all data is written.
	  /// </summary>
	  /// <param name="minLength"> minimum length of the file. </param>
	  public virtual void setMinLength(long minLength)
	  {
		this.minLength = minLength;
	  }

	  /// <summary>
	  /// Set extendMode for truncated, yet valid files - old NetCDF code allowed this
	  /// when NOFILL on, and user doesnt write all variables.
	  /// </summary>
	  public virtual void setExtendMode()
	  {
		this.extendMode = true;
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////
	  // Read primitives.
	  //

	  /// <summary>
	  /// Read a byte of data from the file, blocking until data is
	  /// available.
	  /// </summary>
	  /// <returns> the next byte of data, or -1 if the end of the file is
	  ///         reached. </returns>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public int read() throws IOException
	  public virtual int read()
	  {

		// If the file position is within the data, return the byte...
		if (filePosition < dataEnd)
		{
		  int pos = (int) (filePosition - bufferStart);
		  filePosition++;
		  return (buffer[pos] & 0xff);

		// ...or should we indicate EOF...
		}
		else if (endOfFile)
		{
		  return -1;

		// ...or seek to fill the buffer, and try again.
		}
		else
		{
		  seek(filePosition);
		  return read();
		}
	  }

	  /// <summary>
	  /// Read up to <code>len</code> bytes into an array, at a specified
	  /// offset. This will block until at least one byte has been read.
	  /// </summary>
	  /// <param name="b"> the byte array to receive the bytes. </param>
	  /// <param name="off"> the offset in the array where copying will start. </param>
	  /// <param name="len"> the number of bytes to copy. </param>
	  /// <returns> the actual number of bytes read, or -1 if there is not
	  ///         more data due to the end of the file being reached. </returns>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public int readBytes(byte[] b, int off, int len) throws IOException
	  public virtual int readBytes(byte[] b, int off, int len)
	  {

		// Check for end of file.
		if (endOfFile)
		{
		  return -1;
		}

		// See how many bytes are available in the buffer - if none,
		// seek to the file position to update the buffer and try again.
		int bytesAvailable = (int) (dataEnd - filePosition);
		if (bytesAvailable < 1)
		{
		  seek(filePosition);
		  return readBytes(b, off, len);
		}

		// Copy as much as we can.
		int copyLength = (bytesAvailable >= len) ? len : bytesAvailable;
		System.Array.Copy(buffer, (int) (filePosition - bufferStart), b, off, copyLength);
		filePosition += copyLength;

		// If there is more to copy...
		if (copyLength < len)
		{
		  int extraCopy = len - copyLength;

		  // If the amount remaining is more than a buffer's length, read it
		  // directly from the file.
		  if (extraCopy > buffer.Length)
		  {
			extraCopy = read_(filePosition, b, off + copyLength, len - copyLength);

			// ...or read a new buffer full, and copy as much as possible...
		  }
		  else
		  {
			seek(filePosition);
			if (!endOfFile)
			{
			  extraCopy = (extraCopy > dataSize) ? dataSize : extraCopy;
			  System.Array.Copy(buffer, 0, b, off + copyLength, extraCopy);
			}
			else
			{
			  extraCopy = -1;
			}
		  }

		  // If we did manage to copy any more, update the file position and
		  // return the amount copied.
		  if (extraCopy > 0)
		  {
			filePosition += extraCopy;
			return copyLength + extraCopy;
		  }
		}

		// Return the amount copied.
		return copyLength;
	  }

	  /// <summary>
	  /// Read <code>nbytes</code> bytes, at the specified file offset, send to a WritableByteChannel.
	  /// This will block until all bytes are read.
	  /// This uses the underlying file channel directly, bypassing all user buffers.
	  /// </summary>
	  /// <param name="dest"> write to this WritableByteChannel. </param>
	  /// <param name="offset"> the offset in the file where copying will start. </param>
	  /// <param name="nbytes"> the number of bytes to read. </param>
	  /// <returns> the actual number of bytes read and transfered </returns>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public long readToByteChannel(WritableByteChannel dest, long offset, long nbytes) throws IOException
	  public virtual long readToByteChannel(WritableByteChannel dest, long offset, long nbytes)
	  {

		if (fileChannel == null)
		{
		  fileChannel = file.getChannel();
		}

		long need = nbytes;
		while (need > 0)
		{
		  long count = fileChannel.transferTo(offset, need, dest);
		  if (count == 0)
		  {
			break; // EOF condition
		  }
		  need -= count;
		  offset += count;
		}
		return nbytes - need;
	  }


	  /// <summary>
	  /// Read directly from file, without going through the buffer.
	  /// All reading goes through here or readToByteChannel;
	  /// </summary>
	  /// <param name="pos"> start here in the file </param>
	  /// <param name="b"> put data into this buffer </param>
	  /// <param name="offset"> buffer offset </param>
	  /// <param name="len"> this number of bytes </param>
	  /// <returns> actual number of bytes read, -1 if underlying random access file was closed </returns>
	  /// <exception cref="IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected int read_(long pos, byte[] b, int offset, int len) throws IOException
	  protected internal virtual int read_(long pos, byte[] b, int offset, int len)
	  {
		if (file == null)
		{
		  return -1;
		}

		file.seek(pos);
		int n = file.read(b, offset, len);
		if (debugAccess)
		{
		  if (showRead)
		  {
			Console.WriteLine(" **read_ {0} = {1} bytes at {2}; block = {3}%n", location, len, pos, (pos / buffer.Length));
		  }
		  debug_nseeks.incrementAndGet();
		  debug_nbytes.addAndGet(len);
		}

		if (extendMode && (n < len))
		{
		  n = len;
		}
		return n;
	  }

	  /// <summary>
	  /// Read up to <code>len</code> bytes into an array, at a specified
	  /// offset. This will block until at least one byte has been read.
	  /// </summary>
	  /// <param name="b"> the byte array to receive the bytes. </param>
	  /// <param name="off"> the offset in the array where copying will start. </param>
	  /// <param name="len"> the number of bytes to copy. </param>
	  /// <returns> the actual number of bytes read, or -1 if there is not
	  ///         more data due to the end of the file being reached. </returns>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public int read(byte[] b, int off, int len) throws IOException
	  public virtual int read(byte[] b, int off, int len)
	  {
		return readBytes(b, off, len);
	  }

	  /// <summary>
	  /// Read up to <code>b.length( )</code> bytes into an array. This
	  /// will block until at least one byte has been read.
	  /// </summary>
	  /// <param name="b"> the byte array to receive the bytes. </param>
	  /// <returns> the actual number of bytes read, or -1 if there is not
	  ///         more data due to the end of the file being reached. </returns>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public int read(byte[] b) throws IOException
	  public virtual int read(byte[] b)
	  {
		return readBytes(b, 0, b.Length);
	  }

	  /// <summary>
	  /// Read fully count number of bytes
	  /// </summary>
	  /// <param name="count"> how many bytes tp read </param>
	  /// <returns> a byte array of length count, fully read in </returns>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public byte[] readBytes(int count) throws IOException
	  public virtual byte[] readBytes(int count)
	  {
		byte[] b = new byte[count];
		readFully(b);
		return b;
	  }

	  /// <summary>
	  /// Reads <code>b.length</code> bytes from this file into the byte
	  /// array. This method reads repeatedly from the file until all the
	  /// bytes are read. This method blocks until all the bytes are read,
	  /// the end of the stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <param name="b"> the buffer into which the data is read. </param>
	  /// <exception cref="EOFException"> if this file reaches the end before reading
	  ///         all the bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void readFully(byte[] b) throws IOException
	  public virtual void readFully(byte[] b)
	  {
		readFully(b, 0, b.Length);
	  }

	  /// <summary>
	  /// Reads exactly <code>len</code> bytes from this file into the byte
	  /// array. This method reads repeatedly from the file until all the
	  /// bytes are read. This method blocks until all the bytes are read,
	  /// the end of the stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <param name="b"> the buffer into which the data is read. </param>
	  /// <param name="off"> the start offset of the data. </param>
	  /// <param name="len"> the number of bytes to read. </param>
	  /// <exception cref="EOFException"> if this file reaches the end before reading
	  ///         all the bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void readFully(byte[] b, int off, int len) throws IOException
	  public virtual void readFully(byte[] b, int off, int len)
	  {
		int n = 0;
		while (n < len)
		{
		  int count = this.read(b, off + n, len - n);
		  if (count < 0)
		  {
			throw new System.IO.EndOfStreamException("Reading " + location + " at " + filePosition + " file length = " + length());
		  }
		  n += count;
		}
	  }

	  /// <summary>
	  /// Skips exactly <code>n</code> bytes of input.
	  /// This method blocks until all the bytes are skipped, the end of
	  /// the stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <param name="n"> the number of bytes to be skipped. </param>
	  /// <returns> the number of bytes skipped, which is always <code>n</code>. </returns>
	  /// <exception cref="EOFException"> if this file reaches the end before skipping
	  ///         all the bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public int skipBytes(int n) throws IOException
	  public virtual int skipBytes(int n)
	  {
		seek(FilePointer + n);
		return n;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public long skipBytes(long n) throws IOException
	  public virtual long skipBytes(long n)
	  {
		seek(FilePointer + n);
		return n;
	  }

	  /// <summary>
	  /// Unread the last byte read.
	  /// This method should not be used more than once
	  /// between reading operations, or strange things might happen.
	  /// </summary>
	  public virtual void unread()
	  {
		filePosition--;
	  }

	  //
	  // Write primitives.
	  //

	  /// <summary>
	  /// Write a byte to the file. If the file has not been opened for
	  /// writing, an IOException will be raised only when an attempt is
	  /// made to write the buffer to the file.
	  /// <para/>
	  /// Caveat: the effects of seek( )ing beyond the end of the file are
	  /// undefined.
	  /// </summary>
	  /// <param name="b"> write this byte </param>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void write(int b) throws IOException
	  public virtual void write(int b)
	  {

		// If the file position is within the block of data...
		if (filePosition < dataEnd)
		{
		  int pos = (int) (filePosition - bufferStart);
		  buffer[pos] = (byte) b;
		  bufferModified = true;
		  filePosition++;

		// ...or (assuming that seek will not allow the file pointer
		// to move beyond the end of the file) get the correct block of
		// data...
		}
		else
		{

		  // If there is room in the buffer, expand it...
		  if (dataSize != buffer.Length)
		  {
			int pos = (int) (filePosition - bufferStart);
			buffer[pos] = (byte) b;
			bufferModified = true;
			filePosition++;
			dataSize++;
			dataEnd++;

			// ...or do another seek to get a new buffer, and start again...
		  }
		  else
		  {
			seek(filePosition);
			write(b);
		  }
		}
	  }

	  /// <summary>
	  /// Write <code>len</code> bytes from an array to the file.
	  /// </summary>
	  /// <param name="b"> the array containing the data. </param>
	  /// <param name="off"> the offset in the array to the data. </param>
	  /// <param name="len"> the length of the data. </param>
	  /// <exception cref="IOException"> if an I/O error occurrs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void writeBytes(byte[] b, int off, int len) throws IOException
	  public virtual void writeBytes(byte[] b, int off, int len)
	  {
		// If the amount of data is small (less than a full buffer)...
		if (len < buffer.Length)
		{

		  // If any of the data fits within the buffer...
		  int spaceInBuffer = 0;
		  int copyLength = 0;
		  if (filePosition >= bufferStart)
		  {
			spaceInBuffer = (int) ((bufferStart + buffer.Length) - filePosition);
		  }

		  if (spaceInBuffer > 0)
		  {
			// Copy as much as possible to the buffer.
			copyLength = (spaceInBuffer > len) ? len : spaceInBuffer;
			System.Array.Copy(b, off, buffer, (int) (filePosition - bufferStart), copyLength);
			bufferModified = true;
			long myDataEnd = filePosition + copyLength;
			dataEnd = (myDataEnd > dataEnd) ? myDataEnd : dataEnd;
			dataSize = (int) (dataEnd - bufferStart);
			filePosition += copyLength;
		  }

		  // If there is any data remaining, move to the new position and copy to
		  // the new buffer.
		  if (copyLength < len)
		  {
			seek(filePosition); // triggers a flush
			System.Array.Copy(b, off + copyLength, buffer, (int) (filePosition - bufferStart), len - copyLength);
			bufferModified = true;
			long myDataEnd = filePosition + (len - copyLength);
			dataEnd = (myDataEnd > dataEnd) ? myDataEnd : dataEnd;
			dataSize = (int) (dataEnd - bufferStart);
			filePosition += (len - copyLength);
		  }

		  // ...or write a lot of data...
		}
		else
		{

		  // Flush the current buffer, and write this data to the file.
		  if (bufferModified)
		  {
			flush();
		  }
		  file.seek(filePosition); // moved per Steve Cerruti; Jan 14, 2005
		  file.write(b, off, len);

		  filePosition += len;
		  bufferStart = filePosition; // an empty buffer
		  dataSize = 0;
		  dataEnd = bufferStart + dataSize;
		}
	  }

	  /// <summary>
	  /// Writes <code>b.length</code> bytes from the specified byte array
	  /// starting at offset <code>off</code> to this file.
	  /// </summary>
	  /// <param name="b"> the data. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void write(byte[] b) throws IOException
	  public virtual void write(byte[] b)
	  {
		writeBytes(b, 0, b.Length);
	  }

	  /// <summary>
	  /// Writes <code>len</code> bytes from the specified byte array
	  /// starting at offset <code>off</code> to this file.
	  /// </summary>
	  /// <param name="b"> the data. </param>
	  /// <param name="off"> the start offset in the data. </param>
	  /// <param name="len"> the number of bytes to write. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void write(byte[] b, int off, int len) throws IOException
	  public virtual void write(byte[] b, int off, int len)
	  {
		writeBytes(b, off, len);
	  }

	  //
	  // DataInput methods.
	  //

	  /// <summary>
	  /// Reads a <code>boolean</code> from this file. This method reads a
	  /// single byte from the file. A value of <code>0</code> represents
	  /// <code>false</code>. Any other value represents <code>true</code>.
	  /// This method blocks until the byte is read, the end of the stream
	  /// is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> the <code>boolean</code> value read. </returns>
	  /// <exception cref="EOFException"> if this file has reached the end. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final boolean readBoolean() throws IOException
	  public virtual bool readBoolean()
	  {
		int ch = this.read();
		if (ch < 0)
		{
		  throw new System.IO.EndOfStreamException();
		}
		return (ch != 0);
	  }

	  /// <summary>
	  /// Reads a signed 8-bit value from this file. This method reads a
	  /// byte from the file. If the byte read is <code>b</code>, where
	  /// <code>0&nbsp;&lt;=&nbsp;b&nbsp;&lt;=&nbsp;255</code>,
	  /// then the result is:
	  /// <ul>
	  /// <code>
	  /// (byte)(b)
	  /// </code>
	  /// </ul>
	  /// <para/>
	  /// This method blocks until the byte is read, the end of the stream
	  /// is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> the next byte of this file as a signed 8-bit
	  ///         <code>byte</code>. </returns>
	  /// <exception cref="EOFException"> if this file has reached the end. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final byte readByte() throws IOException
	  public virtual byte readByte()
	  {
		int ch = this.read();
		if (ch < 0)
		{
		  throw new System.IO.EndOfStreamException();
		}
		return (byte) (ch);
	  }

	  /// <summary>
	  /// Reads an unsigned 8-bit number from this file. This method reads
	  /// a byte from this file and returns that byte.
	  /// <para/>
	  /// This method blocks until the byte is read, the end of the stream
	  /// is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> the next byte of this file, interpreted as an unsigned
	  ///         8-bit number. </returns>
	  /// <exception cref="EOFException"> if this file has reached the end. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final int readUnsignedByte() throws IOException
	  public virtual int readUnsignedByte()
	  {
		int ch = this.read();
		if (ch < 0)
		{
		  throw new System.IO.EndOfStreamException();
		}
		return ch;
	  }

	  /// <summary>
	  /// Reads a signed 16-bit number from this file. The method reads 2
	  /// bytes from this file. If the two bytes read, in order, are
	  /// <code>b1</code> and <code>b2</code>, where each of the two values is
	  /// between <code>0</code> and <code>255</code>, inclusive, then the
	  /// result is equal to:
	  /// <ul>
	  /// <code>
	  /// (short)((b1 &lt;&lt; 8) | b2)
	  /// </code>
	  /// </ul>
	  /// <para/>
	  /// This method blocks until the two bytes are read, the end of the
	  /// stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> the next two bytes of this file, interpreted as a signed
	  ///         16-bit number. </returns>
	  /// <exception cref="EOFException"> if this file reaches the end before reading
	  ///         two bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final short readShort() throws IOException
	  public virtual short readShort()
	  {
		int ch1 = this.read();
		int ch2 = this.read();
		if ((ch1 | ch2) < 0)
		{
		  throw new System.IO.EndOfStreamException();
		}
		if (bigEndian)
		{
		  return (short) ((ch1 << 8) + (ch2));
		}
		else
		{
		  return (short) ((ch2 << 8) + (ch1));
		}
	  }

	  /// <summary>
	  /// Read an array of shorts
	  /// </summary>
	  /// <param name="pa"> read into this array </param>
	  /// <param name="start"> starting at pa[start] </param>
	  /// <param name="n"> read this many elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void readShort(short[] pa, int start, int n) throws IOException
	  public virtual void readShort(short[] pa, int start, int n)
	  {
		for (int i = 0; i < n; i++)
		{
		  pa[start + i] = readShort();
		}
	  }

	  /// <summary>
	  /// Reads an unsigned 16-bit number from this file. This method reads
	  /// two bytes from the file. If the bytes read, in order, are
	  /// <code>b1</code> and <code>b2</code>, where
	  /// <code>0&nbsp;&lt;=&nbsp;b1, b2&nbsp;&lt;=&nbsp;255</code>,
	  /// then the result is equal to:
	  /// <ul>
	  /// <code>
	  /// (b1 &lt;&lt; 8) | b2
	  /// </code>
	  /// </ul>
	  /// <para/>
	  /// This method blocks until the two bytes are read, the end of the
	  /// stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> the next two bytes of this file, interpreted as an unsigned
	  ///         16-bit integer. </returns>
	  /// <exception cref="EOFException"> if this file reaches the end before reading
	  ///         two bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final int readUnsignedShort() throws IOException
	  public virtual int readUnsignedShort()
	  {
		int ch1 = this.read();
		int ch2 = this.read();
		if ((ch1 | ch2) < 0)
		{
		  throw new System.IO.EndOfStreamException();
		}
		if (bigEndian)
		{
		  return ((ch1 << 8) + (ch2));
		}
		else
		{
		  return ((ch2 << 8) + (ch1));
		}
	  }

	  /// <summary>
	  /// Reads a Unicode character from this file. This method reads two
	  /// bytes from the file. If the bytes read, in order, are
	  /// <code>b1</code> and <code>b2</code>, where
	  /// <code>0&nbsp;&lt;=&nbsp;b1,&nbsp;b2&nbsp;&lt;=&nbsp;255</code>,
	  /// then the result is equal to:
	  /// <ul>
	  /// <code>
	  /// (char)((b1 &lt;&lt; 8) | b2)
	  /// </code>
	  /// </ul>
	  /// <para/>
	  /// This method blocks until the two bytes are read, the end of the
	  /// stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> the next two bytes of this file as a Unicode character. </returns>
	  /// <exception cref="EOFException"> if this file reaches the end before reading
	  ///         two bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final char readChar() throws IOException
	  public virtual char readChar()
	  {
		int ch1 = this.read();
		int ch2 = this.read();
		if ((ch1 | ch2) < 0)
		{
		  throw new System.IO.EndOfStreamException();
		}
		if (bigEndian)
		{
		  return (char) ((ch1 << 8) + (ch2));
		}
		else
		{
		  return (char) ((ch2 << 8) + (ch1));
		}
	  }

	  /// <summary>
	  /// Reads a signed 32-bit integer from this file. This method reads 4
	  /// bytes from the file. If the bytes read, in order, are <code>b1</code>,
	  /// <code>b2</code>, <code>b3</code>, and <code>b4</code>, where
	  /// <code>0&nbsp;&lt;=&nbsp;b1, b2, b3, b4&nbsp;&lt;=&nbsp;255</code>,
	  /// then the result is equal to:
	  /// <ul>
	  /// <code>
	  /// (b1 &lt;&lt; 24) | (b2 &lt;&lt; 16) + (b3 &lt;&lt; 8) + b4
	  /// </code>
	  /// </ul>
	  /// <para/>
	  /// This method blocks until the four bytes are read, the end of the
	  /// stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> the next four bytes of this file, interpreted as an
	  ///         <code>int</code>. </returns>
	  /// <exception cref="EOFException"> if this file reaches the end before reading
	  ///         four bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final int readInt() throws IOException
	  public virtual int readInt()
	  {
		int ch1 = this.read();
		int ch2 = this.read();
		int ch3 = this.read();
		int ch4 = this.read();
		if ((ch1 | ch2 | ch3 | ch4) < 0)
		{
		  throw new System.IO.EndOfStreamException();
		}

		if (bigEndian)
		{
		  return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4));
		}
		else
		{
		  return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + (ch1));
		}
	  }

	  /// <summary>
	  /// Read an integer at the given position, bypassing all buffering.
	  /// </summary>
	  /// <param name="pos"> read a byte at this position </param>
	  /// <returns> The int that was read </returns>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final int readIntUnbuffered(long pos) throws IOException
	  public virtual int readIntUnbuffered(long pos)
	  {
		byte[] bb = new byte[4];
		read_(pos, bb, 0, 4);
		int ch1 = bb[0] & 0xff;
		int ch2 = bb[1] & 0xff;
		int ch3 = bb[2] & 0xff;
		int ch4 = bb[3] & 0xff;
		if ((ch1 | ch2 | ch3 | ch4) < 0)
		{
		  throw new System.IO.EndOfStreamException();
		}

		if (bigEndian)
		{
		  return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4));
		}
		else
		{
		  return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + (ch1));
		}
	  }


	  /// <summary>
	  /// Read an array of ints
	  /// </summary>
	  /// <param name="pa"> read into this array </param>
	  /// <param name="start"> starting at pa[start] </param>
	  /// <param name="n"> read this many elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void readInt(int[] pa, int start, int n) throws IOException
	  public virtual void readInt(int[] pa, int start, int n)
	  {
		for (int i = 0; i < n; i++)
		{
		  pa[start + i] = readInt();
		}
	  }

	  /// <summary>
	  /// Reads a signed 64-bit integer from this file. This method reads eight
	  /// bytes from the file. If the bytes read, in order, are
	  /// <code>b1</code>, <code>b2</code>, <code>b3</code>,
	  /// <code>b4</code>, <code>b5</code>, <code>b6</code>,
	  /// <code>b7</code>, and <code>b8,</code> where:
	  /// <ul>
	  /// <code>
	  /// 0 &lt;= b1, b2, b3, b4, b5, b6, b7, b8 &lt;=255,
	  /// </code>
	  /// </ul>
	  /// <para/>
	  /// then the result is equal to:
	  /// <p>
	  /// <blockquote>
	  /// </para>
	  /// <pre>
	  /// ((long) b1 &lt;&lt; 56) + ((long) b2 &lt;&lt; 48) + ((long) b3 &lt;&lt; 40) + ((long) b4 &lt;&lt; 32) + ((long) b5 &lt;&lt; 24)
	  ///     + ((long) b6 &lt;&lt; 16) + ((long) b7 &lt;&lt; 8) + b8
	  /// </pre>
	  /// 
	  /// <para>
	  /// </blockquote>
	  /// This method blocks until the eight bytes are read, the end of the
	  /// stream is detected, or an exception is thrown.
	  /// </para>
	  /// <returns> the next eight bytes of this file, interpreted as a
	  ///         <code>long</code>. </returns>
	  /// <exception cref="EOFException"> if this file reaches the end before reading
	  ///         eight bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final long readLong() throws IOException
	  public virtual long readLong()
	  {
		if (bigEndian)
		{
		  return ((long) (readInt()) << 32) + (readInt() & 0xFFFFFFFFL); // tested ok
		}
		else
		{
		  return ((readInt() & 0xFFFFFFFFL) + ((long) readInt() << 32)); // not tested yet ??
		}
	  }

	  /// <summary>
	  /// Read an array of longs
	  /// </summary>
	  /// <param name="pa"> read into this array </param>
	  /// <param name="start"> starting at pa[start] </param>
	  /// <param name="n"> read this many elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void readLong(long[] pa, int start, int n) throws IOException
	  public virtual void readLong(long[] pa, int start, int n)
	  {
		for (int i = 0; i < n; i++)
		{
		  pa[start + i] = readLong();
		}
	  }


	  /// <summary>
	  /// Reads a <code>float</code> from this file. This method reads an
	  /// <code>int</code> value as if by the <code>readInt</code> method
	  /// and then converts that <code>int</code> to a <code>float</code>
	  /// using the <code>intBitsToFloat</code> method in class
	  /// <code>Float</code>.
	  /// <para/>
	  /// This method blocks until the four bytes are read, the end of the
	  /// stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> the next four bytes of this file, interpreted as a
	  ///         <code>float</code>. </returns>
	  /// <exception cref="EOFException"> if this file reaches the end before reading
	  ///         four bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final float readFloat() throws IOException
	  public virtual float readFloat()
	  {
		return BitConverter.Int32BitsToSingle(readInt());
	  }

	  /// <summary>
	  /// Read an array of floats
	  /// </summary>
	  /// <param name="pa"> read into this array </param>
	  /// <param name="start"> starting at pa[start] </param>
	  /// <param name="n"> read this many elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void readFloat(float[] pa, int start, int n) throws IOException
	  public virtual void readFloat(float[] pa, int start, int n)
	  {
		for (int i = 0; i < n; i++)
		{
		  pa[start + i] = BitConverter.Int32BitsToSingle(readInt());
		}
	  }


	  /// <summary>
	  /// Reads a <code>double</code> from this file. This method reads a
	  /// <code>long</code> value as if by the <code>readLong</code> method
	  /// and then converts that <code>long</code> to a <code>double</code>
	  /// using the <code>longBitsToDouble</code> method in
	  /// class <code>Double</code>.
	  /// <para/>
	  /// This method blocks until the eight bytes are read, the end of the
	  /// stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> the next eight bytes of this file, interpreted as a
	  ///         <code>double</code>. </returns>
	  /// <exception cref="EOFException"> if this file reaches the end before reading
	  ///         eight bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final double readDouble() throws IOException
	  public virtual double readDouble()
	  {
		return BitConverter.Int64BitsToDouble(readLong());
	  }

	  /// <summary>
	  /// Read an array of doubles
	  /// </summary>
	  /// <param name="pa"> read into this array </param>
	  /// <param name="start"> starting at pa[start] </param>
	  /// <param name="n"> read this many elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void readDouble(double[] pa, int start, int n) throws IOException
	  public virtual void readDouble(double[] pa, int start, int n)
	  {
		for (int i = 0; i < n; i++)
		{
		  pa[start + i] = BitConverter.Int64BitsToDouble(readLong());
		}
	  }

	  /// <summary>
	  /// Reads the next line of text from this file. This method successively
	  /// reads bytes from the file, starting at the current file pointer,
	  /// until it reaches a line terminator or the end
	  /// of the file. Each byte is converted into a character by taking the
	  /// byte's value for the lower eight bits of the character and setting the
	  /// high eight bits of the character to zero. This method does not,
	  /// therefore, support the full Unicode character set.
	  /// </summary>
	  /// <returns> the next line of text from this file, or null if end
	  ///         of file is encountered before even one byte is read. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final String readLine() throws IOException
	  public virtual string readLine()
	  {
		return readLine(Encoding.UTF8);
	  }

	  /// <summary>
	  /// Read the next line of text as the specified charset
	  /// The charset parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="charset"> - character encoding to use </param>
	  /// <returns> the next line of text </returns>
	  /// <exception cref="IOException"> </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public String readLine(Charset charset) throws IOException
	  public virtual string readLine(Charset charset)
	  {
		StringBuilder input = new StringBuilder();
		int bytes_per_char = (charset == Encoding.Unicode || charset == Encoding.BigEndianUnicode || charset == Encoding.Unicode) ? 2 : 1;

		char c = (char) 0xFFFF;
		bool eol = false;

		while (!eol)
		{
		  try
		  {
			// read 2 bytes at a time for UTF-16, else 1 byte at a time
			// NOTE: endianness of the file mst match endianness of the charset
			c = bytes_per_char > 1 ? readChar() : (char) read();
			switch (c)
			{
			  case (char) 0xFFFF:
			  case '\n':
				eol = true;
				break;
			  case '\r':
				eol = true;
				long cur = FilePointer;
				char next = bytes_per_char > 1 ? readChar() : (char) read();
				if (next != '\n')
				{
				  seek(cur);
				}
				break;
			  default:
				input.Append(c);
				break;
			}
		  }
		  catch (System.IO.EndOfStreamException)
		  {
			eol = true;
		  }
		}

		if ((c == (char) 0xFFFF) && (input.Length == 0))
		{
		  return null;
		}
		return input.ToString();
	  }

	  /// <summary>
	  /// Reads in a string from this file. The string has been encoded
	  /// using a modified UTF-8 format.
	  /// <para/>
	  /// The first two bytes are read as if by
	  /// <code>readUnsignedShort</code>. This value gives the number of
	  /// following bytes that are in the encoded string, not
	  /// the length of the resulting string. The following bytes are then
	  /// interpreted as bytes encoding characters in the UTF-8 format
	  /// and are converted into characters.
	  /// <para/>
	  /// This method blocks until all the bytes are read, the end of the
	  /// stream is detected, or an exception is thrown.
	  /// </summary>
	  /// <returns> a Unicode string. </returns>
	  /// <exception cref="EOFException"> if this file reaches the end before
	  ///         reading all the bytes. </exception>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
	  /// <exception cref="UTFDataFormatException"> if the bytes do not represent
	  ///         valid UTF-8 encoding of a Unicode string. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final String readUTF() throws IOException
	  public virtual string readUTF()
	  {
		// LOOK: uses standard UTF-8 instead of Java modified UTF-8
		int utflen = readUnsignedShort();
		byte[] buf = new byte[utflen];
		readFully(buf);
		return StringHelper.NewString(buf, 0, utflen, Encoding.UTF8);
	  }

	  /// <summary>
	  /// Read a String of known length.
	  /// </summary>
	  /// <param name="nbytes"> number of bytes to read </param>
	  /// <returns> String wrapping the bytes. </returns>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public String readString(int nbytes) throws IOException
	  public virtual string readString(int nbytes)
	  {
		return readString(nbytes, Encoding.UTF8);
	  }

	  /// <summary>
	  /// Read a String of known length as the specified charset.
	  /// The charset parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="nbytes"> number of bytes to read </param>
	  /// <param name="charset"> the {@link Charset charset} to be used to decode the bytes </param>
	  /// <returns> String wrapping the bytes. </returns>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public String readString(int nbytes, Charset charset) throws IOException
	  public virtual string readString(int nbytes, Charset charset)
	  {
		byte[] data = new byte[nbytes];
		readFully(data);
		return StringHelper.NewString(data, charset);
	  }

	  /// <summary>
	  /// Read a String of max length, zero terminate.
	  /// </summary>
	  /// <param name="nbytes"> number of bytes to read </param>
	  /// <returns> String wrapping the bytes. </returns>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public String readStringMax(int nbytes) throws IOException
	  public virtual string readStringMax(int nbytes)
	  {
		return readStringMax(nbytes, Encoding.UTF8);
	  }

	  /// <summary>
	  /// Read a String of max length as the specified charset, zero terminate.
	  /// The charset parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="nbytes"> number of bytes to read </param>
	  /// <param name="charset"> the {@link Charset charset} to be used to decode the bytes </param>
	  /// <returns> String wrapping the bytes. </returns>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public String readStringMax(int nbytes, Charset charset) throws IOException
	  public virtual string readStringMax(int nbytes, Charset charset)
	  {
		int bytes_per_char = (charset == Encoding.Unicode || charset == Encoding.BigEndianUnicode || charset == Encoding.Unicode) ? 2 : 1;
		byte[] b = new byte[nbytes];
		readFully(b);
		int count;
		int nzeros = 0;
		for (count = 0; count < nbytes; count++)
		{
		  if (b[count] == 0)
		  {
			nzeros++;
			// break if all bytes in char are zero
			if (nzeros == bytes_per_char)
			{
			  break;
			}
		  }
		  else // reset nzeros for non-zero byte
		  {
			nzeros = 0;
		  }
		}
		return StringHelper.NewString(b, 0, count, charset);
	  }

	  //
	  // DataOutput methods.
	  //

	  /// <summary>
	  /// Writes a <code>boolean</code> to the file as a 1-byte value. The
	  /// value <code>true</code> is written out as the value
	  /// <code>(byte)1</code>; the value <code>false</code> is written out
	  /// as the value <code>(byte)0</code>.
	  /// </summary>
	  /// <param name="v"> a <code>boolean</code> value to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeBoolean(boolean v) throws IOException
	  public virtual void writeBoolean(bool v)
	  {
		write(v ? 1 : 0);
	  }

	  /// <summary>
	  /// Write an array of booleans
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeBoolean(boolean[] pa, int start, int n) throws IOException
	  public virtual void writeBoolean(bool[] pa, int start, int n)
	  {
		for (int i = 0; i < n; i++)
		{
		  writeBoolean(pa[start + i]);
		}
	  }

	  /// <summary>
	  /// Writes a <code>byte</code> to the file as a 1-byte value.
	  /// </summary>
	  /// <param name="v"> a <code>byte</code> value to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeByte(int v) throws IOException
	  public virtual void writeByte(int v)
	  {
		write(v);
	  }

	  /// <summary>
	  /// Writes a <code>short</code> to the file as two bytes, high byte first.
	  /// </summary>
	  /// <param name="v"> a <code>short</code> to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeShort(int v) throws IOException
	  public virtual void writeShort(int v)
	  {
		writeShort(v, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Writes a <code>short</code> to the file as two bytes with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>short</code> to be written. </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeShort(int v, int endian) throws IOException
	  public virtual void writeShort(int v, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeShort(v, bo);
	  }

	  /// <summary>
	  /// Writes a <code>short</code> to the file as two bytes with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>short</code> to be written. </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void writeShort(int v, ByteOrder bo) throws IOException
	  public virtual void writeShort(int v, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		if (bo == NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN)
		{
		  write((v) & 0xFF);
		  write((v >> 8) & 0xFF);
		}
		else
		{
		  write((v >> 8) & 0xFF);
		  write((v) & 0xFF);
		}
	  }

	  /// <summary>
	  /// Write an array of shorts
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> this number of elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeShort(short[] pa, int start, int n) throws IOException
	  public virtual void writeShort(short[] pa, int start, int n)
	  {
		writeShort(pa, start, n, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Write an array of shorts with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> this number of elements </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeShort(short[] pa, int start, int n, int endian) throws IOException
	  public virtual void writeShort(short[] pa, int start, int n, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeShort(pa, start, n, bo);
	  }

	  /// <summary>
	  /// Write an array of shorts with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> this number of elements </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeShort(short[] pa, int start, int n, ByteOrder bo) throws IOException
	  public virtual void writeShort(short[] pa, int start, int n, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		for (int i = 0; i < n; i++)
		{
		  writeShort(pa[start + i], bo);
		}
	  }

	  /// <summary>
	  /// Writes a <code>char</code> to the file as a 2-byte value, high byte first.
	  /// </summary>
	  /// <param name="v"> a <code>char</code> value to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeChar(int v) throws IOException
	  public virtual void writeChar(int v)
	  {
		writeChar(v, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Writes a <code>char</code> to the file as a 2-byte value with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>char</code> value to be written. </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void writeChar(int v, int endian) throws IOException
	  public virtual void writeChar(int v, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeChar(v, bo);
	  }

	  /// <summary>
	  /// Writes a <code>char</code> to the file as a 2-byte value with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>char</code> value to be written. </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void writeChar(int v, ByteOrder bo) throws IOException
	  public virtual void writeChar(int v, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		if (bo == NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN)
		{
		  write((v) & 0xFF);
		  write((v >> 8) & 0xFF);
		}
		else
		{
		  write((v >> 8) & 0xFF);
		  write((v) & 0xFF);
		}
	  }

	  /// <summary>
	  /// Write an array of chars
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> this number of elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeChar(char[] pa, int start, int n) throws IOException
	  public virtual void writeChar(char[] pa, int start, int n)
	  {
		writeChar(pa, start, n, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Write an array of chars with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> this number of elements </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeChar(char[] pa, int start, int n, int endian) throws IOException
	  public virtual void writeChar(char[] pa, int start, int n, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeChar(pa, start, n, bo);
	  }

	  /// <summary>
	  /// Write an array of chars with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> this number of elements </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeChar(char[] pa, int start, int n, ByteOrder bo) throws IOException
	  public virtual void writeChar(char[] pa, int start, int n, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		for (int i = 0; i < n; i++)
		{
		  writeChar(pa[start + i], bo);
		}
	  }

	  /// <summary>
	  /// Writes an <code>int</code> to the file as four bytes, high byte first.
	  /// </summary>
	  /// <param name="v"> an <code>int</code> to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeInt(int v) throws IOException
	  public virtual void writeInt(int v)
	  {
		writeInt(v, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Writes an <code>int</code> to the file as four bytes with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> an <code>int</code> to be written. </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeInt(int v, int endian) throws IOException
	  public virtual void writeInt(int v, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeInt(v, bo);
	  }

	  /// <summary>
	  /// Writes an <code>int</code> to the file as four bytes with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> an <code>int</code> to be written. </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeInt(int v, ByteOrder bo) throws IOException
	  public virtual void writeInt(int v, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		if (bo == NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN)
		{
		  write((v) & 0xFF);
		  write((v >> 8) & 0xFF);
		  write((v >> 16) & 0xFF);
		  write((v >> 24) & 0xFF);
		}
		else
		{
		  write((v >> 24) & 0xFF);
		  write((v >> 16) & 0xFF);
		  write((v >> 8) & 0xFF);
		  write((v) & 0xFF);
		}
	  }

	  /// <summary>
	  /// Write an array of ints
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeInt(int[] pa, int start, int n) throws IOException
	  public virtual void writeInt(int[] pa, int start, int n)
	  {
		writeInt(pa, start, n, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Write an array of ints with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeInt(int[] pa, int start, int n, int endian) throws IOException
	  public virtual void writeInt(int[] pa, int start, int n, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeInt(pa, start, n, bo);
	  }

	  /// <summary>
	  /// Write an array of ints with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeInt(int[] pa, int start, int n, ByteOrder bo) throws IOException
	  public virtual void writeInt(int[] pa, int start, int n, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		for (int i = 0; i < n; i++)
		{
		  writeInt(pa[start + i], bo);
		}
	  }

	  /// <summary>
	  /// Writes a <code>long</code> to the file as eight bytes, high byte first.
	  /// </summary>
	  /// <param name="v"> a <code>long</code> to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeLong(long v) throws IOException
	  public virtual void writeLong(long v)
	  {
		writeLong(v, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Writes a <code>long</code> to the file as eight bytes with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>long</code> to be written. </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeLong(long v, int endian) throws IOException
	  public virtual void writeLong(long v, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeLong(v, bo);
	  }

	  /// <summary>
	  /// Writes a <code>long</code> to the file as eight bytes with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>long</code> to be written. </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeLong(long v, ByteOrder bo) throws IOException
	  public virtual void writeLong(long v, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		if (bo == NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN)
		{
		  write((int) (v) & 0xFF);
		  write((int) (v >> 8) & 0xFF);
		  write((int) (v >> 16) & 0xFF);
		  write((int) (v >> 24) & 0xFF);
		  write((int) (v >> 32) & 0xFF);
		  write((int) (v >> 40) & 0xFF);
		  write((int) (v >> 48) & 0xFF);
		  write((int) (v >> 56) & 0xFF);
		}
		else
		{
		  write((int) (v >> 56) & 0xFF);
		  write((int) (v >> 48) & 0xFF);
		  write((int) (v >> 40) & 0xFF);
		  write((int) (v >> 32) & 0xFF);
		  write((int) (v >> 24) & 0xFF);
		  write((int) (v >> 16) & 0xFF);
		  write((int) (v >> 8) & 0xFF);
		  write((int) (v) & 0xFF);
		}
	  }

	  /// <summary>
	  /// Write an array of longs
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeLong(long[] pa, int start, int n) throws IOException
	  public virtual void writeLong(long[] pa, int start, int n)
	  {
		writeLong(pa, start, n, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Write an array of longs with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeLong(long[] pa, int start, int n, int endian) throws IOException
	  public virtual void writeLong(long[] pa, int start, int n, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeLong(pa, start, n, bo);
	  }

	  /// <summary>
	  /// Write an array of longs with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeLong(long[] pa, int start, int n, ByteOrder bo) throws IOException
	  public virtual void writeLong(long[] pa, int start, int n, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		for (int i = 0; i < n; i++)
		{
		  writeLong(pa[start + i], bo);
		}
	  }

	  /// <summary>
	  /// Converts the float argument to an <code>int</code> using the
	  /// <code>floatToIntBits</code> method in class <code>Float</code>,
	  /// and then writes that <code>int</code> value to the file as a
	  /// 4-byte quantity, high byte first.
	  /// </summary>
	  /// <param name="v"> a <code>float</code> value to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeFloat(float v) throws IOException
	  public virtual void writeFloat(float v)
	  {
		writeInt(BitConverter.SingleToInt32Bits(v), NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Converts the float argument to an <code>int</code> using the
	  /// <code>floatToIntBits</code> method in class <code>Float</code>,
	  /// and then writes that <code>int</code> value to the file as a
	  /// 4-byte quantity, with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>float</code> value to be written. </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeFloat(float v, int endian) throws IOException
	  public virtual void writeFloat(float v, int endian)
	  {
		writeInt(BitConverter.SingleToInt32Bits(v), endian);
	  }

	  /// <summary>
	  /// Converts the float argument to an <code>int</code> using the
	  /// <code>floatToIntBits</code> method in class <code>Float</code>,
	  /// and then writes that <code>int</code> value to the file as a
	  /// 4-byte quantity, with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>float</code> value to be written. </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeFloat(float v, ByteOrder bo) throws IOException
	  public virtual void writeFloat(float v, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		writeInt(BitConverter.SingleToInt32Bits(v), bo);
	  }

	  /// <summary>
	  /// Write an array of floats
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeFloat(float[] pa, int start, int n) throws IOException
	  public virtual void writeFloat(float[] pa, int start, int n)
	  {
		writeFloat(pa, start, n, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Write an array of floats with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeFloat(float[] pa, int start, int n, int endian) throws IOException
	  public virtual void writeFloat(float[] pa, int start, int n, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeFloat(pa, start, n, bo);
	  }

	  /// <summary>
	  /// Write an array of floats with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeFloat(float[] pa, int start, int n, ByteOrder bo) throws IOException
	  public virtual void writeFloat(float[] pa, int start, int n, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		for (int i = 0; i < n; i++)
		{
		  writeFloat(pa[start + i], bo);
		}
	  }

	  /// <summary>
	  /// Converts the double argument to a <code>long</code> using the
	  /// <code>doubleToLongBits</code> method in class <code>Double</code>,
	  /// and then writes that <code>long</code> value to the file as an
	  /// 8-byte quantity, high byte first.
	  /// </summary>
	  /// <param name="v"> a <code>double</code> value to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeDouble(double v) throws IOException
	  public virtual void writeDouble(double v)
	  {
		writeLong(BitConverter.DoubleToInt64Bits(v), NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Converts the double argument to a <code>long</code> using the
	  /// <code>doubleToLongBits</code> method in class <code>Double</code>,
	  /// and then writes that <code>long</code> value to the file as an
	  /// 8-byte quantity, with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>double</code> value to be written. </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeDouble(double v, int endian) throws IOException
	  public virtual void writeDouble(double v, int endian)
	  {
		writeLong(BitConverter.DoubleToInt64Bits(v), endian);
	  }

	  /// <summary>
	  /// Converts the double argument to a <code>long</code> using the
	  /// <code>doubleToLongBits</code> method in class <code>Double</code>,
	  /// and then writes that <code>long</code> value to the file as an
	  /// 8-byte quantity, with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="v"> a <code>double</code> value to be written. </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeDouble(double v, ByteOrder bo) throws IOException
	  public virtual void writeDouble(double v, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		writeLong(BitConverter.DoubleToInt64Bits(v), bo);
	  }

	  /// <summary>
	  /// Write an array of doubles
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeDouble(double[] pa, int start, int n) throws IOException
	  public virtual void writeDouble(double[] pa, int start, int n)
	  {
		writeDouble(pa, start, n, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Write an array of doubles with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeDouble(double[] pa, int start, int n, int endian) throws IOException
	  public virtual void writeDouble(double[] pa, int start, int n, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeDouble(pa, start, n, bo);
	  }

	  /// <summary>
	  /// Write an array of doubles with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="pa"> write from this array </param>
	  /// <param name="start"> starting with this element in the array </param>
	  /// <param name="n"> write this number of elements </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeDouble(double[] pa, int start, int n, ByteOrder bo) throws IOException
	  public virtual void writeDouble(double[] pa, int start, int n, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		for (int i = 0; i < n; i++)
		{
		  writeDouble(pa[start + i], bo);
		}
	  }

	  /// <summary>
	  /// Writes the string to the file as a sequence of bytes. Each
	  /// character in the string is written out, in sequence, by discarding
	  /// its high eight bits.
	  /// </summary>
	  /// <param name="s"> a string of bytes to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeBytes(String s) throws IOException
	  public virtual void writeBytes(string s)
	  {
		int len = s.Length;
		for (int i = 0; i < len; i++)
		{
		  write((byte) s[i]);
		}
	  }

	  /// <summary>
	  /// Writes the character array to the file as a sequence of bytes. Each
	  /// character in the string is written out, in sequence, by discarding
	  /// its high eight bits.
	  /// </summary>
	  /// <param name="b"> a character array of bytes to be written. </param>
	  /// <param name="off"> the index of the first character to write. </param>
	  /// <param name="len"> the number of characters to write. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeBytes(char[] b, int off, int len) throws IOException
	  public virtual void writeBytes(char[] b, int off, int len)
	  {
		for (int i = off; i < len; i++)
		{
		  write((byte) b[i]);
		}
	  }

	  /// <summary>
	  /// Writes a string to the file as a sequence of characters. Each
	  /// character is written to the data output stream as if by the
	  /// <code>writeChar</code> method.
	  /// </summary>
	  /// <param name="s"> a <code>String</code> value to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeChars(String s) throws IOException
	  public virtual void writeChars(string s)
	  {
		writeChars(s, NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Writes a string to the file as a sequence of characters. Each
	  /// character is written to the data output stream as if by the
	  /// <code>writeChar</code> method, with the provided endianness.
	  /// The endian parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="s"> a <code>String</code> value to be written. </param>
	  /// <param name="endian"> Endianness of the file as an int (0 = big endian, 1 = little endian) </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void writeChars(String s, int endian) throws IOException
	  public virtual void writeChars(string s, int endian)
	  {
		NetcdfSharp.java.nio.ByteOrder bo = endian == LITTLE_ENDIAN ? NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN : NetcdfSharp.java.nio.ByteOrder.BIG_ENDIAN;
		writeChars(s, bo);
	  }

	  /// <summary>
	  /// Writes a string to the file as a sequence of characters. Each
	  /// character is written to the data output stream as if by the
	  /// <code>writeChar</code> method, with the provided byte order.
	  /// The byte order parameter is an extension not implemented in NetcdfSharp.java.io.RandomAccessFile.
	  /// </summary>
	  /// <param name="s"> a <code>String</code> value to be written. </param>
	  /// <param name="bo"> Endianness of the file as a ByteOrder </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void writeChars(String s, ByteOrder bo) throws IOException
	  public virtual void writeChars(string s, NetcdfSharp.java.nio.ByteOrder bo)
	  {
		int len = s.Length;
		for (int i = 0; i < len; i++)
		{
		  int v = s[i];
		  writeChar(v, bo);
		}
	  }

	  /// <summary>
	  /// Writes a string to the file using UTF-8 encoding in a
	  /// machine-independent manner.
	  /// <para/>
	  /// First, two bytes are written to the file as if by the
	  /// <code>writeShort</code> method giving the number of bytes to
	  /// follow. This value is the number of bytes actually written out,
	  /// not the length of the string. Following the length, each character
	  /// of the string is output, in sequence, using the UTF-8 encoding
	  /// for each character.
	  /// </summary>
	  /// <param name="str"> a string to be written. </param>
	  /// <exception cref="IOException"> if an I/O error occurs. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final void writeUTF(String str) throws IOException
	  public virtual void writeUTF(string str)
	  {
		int strlen = str.Length;
		int utflen = 0;

		for (int i = 0; i < strlen; i++)
		{
		  int c = str[i];
		  if ((c >= 0x0001) && (c <= 0x007F))
		  {
			utflen++;
		  }
		  else if (c > 0x07FF)
		  {
			utflen += 3;
		  }
		  else
		  {
			utflen += 2;
		  }
		}
		if (utflen > 65535)
		{
		  throw new NetcdfSharp.java.io.UTFDataFormatException();
		}

		write((utflen >> 8) & 0xFF);
		write((utflen) & 0xFF);
		for (int i = 0; i < strlen; i++)
		{
		  int c = str[i];
		  if ((c >= 0x0001) && (c <= 0x007F))
		  {
			write(c);
		  }
		  else if (c > 0x07FF)
		  {
			write(0xE0 | ((c >> 12) & 0x0F));
			write(0x80 | ((c >> 6) & 0x3F));
			write(0x80 | ((c) & 0x3F));
		  }
		  else
		  {
			write(0xC0 | ((c >> 6) & 0x1F));
			write(0x80 | ((c) & 0x3F));
		  }
		}
	  }

	  /// <summary>
	  /// Create a string representation of this object.
	  /// </summary>
	  /// <returns> a string representation of the state of the object. </returns>
	  public override string ToString()
	  {
		return location;
	  }

	  /////////////////////////////////////////////////

	  /// <summary>
	  /// Search forward from the current pos, looking for a match.
	  /// </summary>
	  /// <param name="match"> the match to look for. </param>
	  /// <param name="maxBytes"> maximum number of bytes to search. use -1 for all </param>
	  /// <returns> true if found, file position will be at the start of the match. </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public boolean searchForward(KMPMatch match, int maxBytes) throws IOException
	  public virtual bool searchForward(KMPMatch match, int maxBytes)
	  {
		long start = FilePointer;
		long last = (maxBytes < 0) ? length() : Math.Min(length(), start + maxBytes);
		long needToScan = last - start;

		// check what ever is now in the buffer
		int bytesAvailable = (int) (dataEnd - filePosition);
		if (bytesAvailable < 1)
		{
		  seek(filePosition); // read a new buffer
		  bytesAvailable = (int) (dataEnd - filePosition);
		}
		int bufStart = (int) (filePosition - bufferStart);
		int scanBytes = (int) Math.Min(bytesAvailable, needToScan);
		int pos = match.indexOf(buffer, bufStart, scanBytes);
		if (pos >= 0)
		{
		  seek(bufferStart + pos);
		  return true;
		}

		int matchLen = match.MatchLength;
		needToScan -= scanBytes - matchLen;

		while (needToScan > matchLen)
		{
		  readBuffer(dataEnd - matchLen); // force new buffer

		  scanBytes = (int) Math.Min(buffer.Length, needToScan);
		  pos = match.indexOf(buffer, 0, scanBytes);
		  if (pos > 0)
		  {
			seek(bufferStart + pos);
			return true;
		  }

		  needToScan -= scanBytes - matchLen;
		}

		// failure
		seek(last);
		return false;
	  }

	}

}
