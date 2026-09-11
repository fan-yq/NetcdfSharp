// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

/*
 * Copyright (c) 1998-2025 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2
{
	using DatasetUrl = NetcdfSharp.ucar.nc2.dataset.DatasetUrl;
	using N3headerNew = NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3.N3headerNew;
	using N3iospNew = NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3.N3iospNew;
	using AbstractIOServiceProvider = NetcdfSharp.ucar.nc2.iosp.AbstractIOServiceProvider;
	using IOServiceProvider = NetcdfSharp.ucar.nc2.iosp.IOServiceProvider;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using EscapeStrings = NetcdfSharp.ucar.nc2.util.EscapeStrings;
	using IO = NetcdfSharp.ucar.nc2.util.IO;
	using UncompressInputStream = NetcdfSharp.ucar.unidata.io.UncompressInputStream;
	using CBZip2InputStream = NetcdfSharp.ucar.unidata.io.bzip2.CBZip2InputStream;
	using RandomAccessFileProvider = NetcdfSharp.ucar.unidata.io.spi.RandomAccessFileProvider;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Static helper methods for NetcdfFile objects.
	/// These use builders and new versions of Iosp's when available.
	/// 
	/// @author caron
	/// @since 10/3/2019.
	/// </summary>
	public class NetcdfFiles
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(NetcdfFiles));
	  private static readonly IList<IOServiceProvider> registeredProviders = new List<IOServiceProvider>();
	  private static readonly IList<RandomAccessFileProvider> registeredRandomAccessFileProviders = new List<RandomAccessFileProvider>();
	  private const int default_buffersize = 8092;
	  private static readonly StringLocker stringLocker = new StringLocker();
	  private static readonly IList<string> possibleCompressedSuffixes = new List<string> {"Z", "zip", "gzip", "gz", "bz2"};
	  private static bool loadWarnings = false;
	  private static bool userLoads;

	  // load core service providers
	  static NetcdfFiles()
	  {

		// IOSPs can be loaded by reflection.
		// Most IOSPs are loaded using the ServiceLoader mechanism. One problem with this is that we no longer
		// control the order which IOSPs try to open. So its harder to avoid mis-behaving and slow IOSPs from
		// making open() slow.
		// Register iosp's that are part of cdm-core. This ensures that they are tried first.
		try
		{
		  registerIOProvider("NetcdfSharp.ucar.nc2.internal.iosp.hdf5.H5iospNew");
		}
		catch (Exception e)
		{
		  if (loadWarnings)
		  {
			log.info("Cant load class H5iospNew", e);
		  }
		}
		try
		{
		  registerIOProvider("NetcdfSharp.ucar.nc2.stream.NcStreamIosp");
		}
		catch (Exception e)
		{
		  if (loadWarnings)
		  {
			log.info("Cant load class NcStreamIosp", e);
		  }
		}
		try
		{
		  registerIOProvider("NetcdfSharp.ucar.nc2.internal.iosp.hdf4.H4iosp");
		}
		catch (Exception e)
		{
		  if (loadWarnings)
		  {
			log.info("Cant load class H4iosp", e);
		  }
		}

		// register RandomAccessFile providers that are part of cdm-core. This ensures that they are tried first.
		try
		{
		  registerRandomAccessFileProvider("NetcdfSharp.ucar.unidata.io.http.HTTPRandomAccessFile$Provider");
		}
		catch (Exception e)
		{
		  if (loadWarnings)
		  {
			log.info("Cant load class HTTPRandomAccessFileProvider", e);
		  }
		}

		try
		{
		  registerRandomAccessFileProvider("NetcdfSharp.ucar.unidata.io.InMemoryRandomAccessFile$Provider");
		}
		catch (Exception e)
		{
		  if (loadWarnings)
		  {
			log.info("Cant load class InMemoryRandomAccessFileProvider", e);
		  }
		}

		// if a user explicitly registers an IOSP or RandomAccessFile implementation via
		// registerIOProvider or registerRandomAccessFileProvider, this ensures they are tried first,
		// even before the core implementations.
		// if false, user registered implementations will be tried after the core implementations.
		// Implementations loaded by the ServiceLoader mechanism are only tried after core and explicitly
		// loaded implementations are tried.
		userLoads = true;
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Register an IOServiceProvider, using its class string name.
	  /// </summary>
	  /// <param name="className"> Class that implements IOServiceProvider. </param>
	  /// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
	  /// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
	  /// <exception cref="ClassNotFoundException"> if class not found. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void registerIOProvider(String className) throws IllegalAccessException, InstantiationException, ClassNotFoundException
	  public static void registerIOProvider(string className)
	  {
		Type ioClass = Type.GetType(className) ?? typeof(NetcdfFile).Assembly.GetType(className);
		registerIOProvider(ioClass);
	  }

	  /// <summary>
	  /// Register an IOServiceProvider. A new instance will be created when one of its files is opened.
	  /// </summary>
	  /// <param name="iospClass"> Class that implements IOServiceProvider. </param>
	  /// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
	  /// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
	  /// <exception cref="ClassCastException"> if class doesnt implement IOServiceProvider interface. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void registerIOProvider(Class iospClass) throws IllegalAccessException, InstantiationException
	  public static void registerIOProvider(Type iospClass)
	  {
		registerIOProvider(iospClass, false);
	  }

	  /// <summary>
	  /// Register an IOServiceProvider. A new instance will be created when one of its files is opened.
	  /// </summary>
	  /// <param name="iospClass"> Class that implements IOServiceProvider. </param>
	  /// <param name="last"> true=>insert at the end of the list; otherwise front </param>
	  /// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
	  /// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
	  /// <exception cref="ClassCastException"> if class doesnt implement IOServiceProvider interface. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static void registerIOProvider(Class iospClass, boolean last) throws IllegalAccessException, InstantiationException
	  private static void registerIOProvider(Type iospClass, bool last)
	  {
		IOServiceProvider spi;
		spi = (IOServiceProvider) System.Activator.CreateInstance(iospClass); // fail fast
		if (userLoads && !last)
		{
		  registeredProviders.Insert(0, spi); // put user stuff first
		}
		else
		{
		  registeredProviders.Add(spi);
		}
	  }

	  /// <summary>
	  /// Register a RandomAccessFile Provider, using its class string name.
	  /// </summary>
	  /// <param name="className"> Class that implements RandomAccessFileProvider. </param>
	  /// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
	  /// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
	  /// <exception cref="ClassNotFoundException"> if class not found. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void registerRandomAccessFileProvider(String className) throws IllegalAccessException, InstantiationException, ClassNotFoundException
	  public static void registerRandomAccessFileProvider(string className)
	  {
		Type rafClass = Type.GetType(className) ?? typeof(NetcdfFile).Assembly.GetType(className);
		registerRandomAccessFileProvider(rafClass);
	  }

	  /// <summary>
	  /// Register a RandomAccessFile Provider. A new instance will be created when one of its files is opened.
	  /// </summary>
	  /// <param name="rafClass"> Class that implements RandomAccessFileProvider. </param>
	  /// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
	  /// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
	  /// <exception cref="ClassCastException"> if class doesnt implement IOServiceProvider interface. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void registerRandomAccessFileProvider(Class rafClass) throws IllegalAccessException, InstantiationException
	  public static void registerRandomAccessFileProvider(Type rafClass)
	  {
		registerRandomAccessFileProvider(rafClass, false);
	  }

	  /// <summary>
	  /// Register a RandomAccessFile Provider. A new instance will be created when one of its files is opened.
	  /// </summary>
	  /// <param name="rafClass"> Class that implements RandomAccessFileProvider. </param>
	  /// <param name="last"> true=>insert at the end of the list; otherwise front </param>
	  /// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
	  /// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
	  /// <exception cref="ClassCastException"> if class doesnt implement IOServiceProvider interface. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static void registerRandomAccessFileProvider(Class rafClass, boolean last) throws IllegalAccessException, InstantiationException
	  private static void registerRandomAccessFileProvider(Type rafClass, bool last)
	  {
		RandomAccessFileProvider rafProvider;
		rafProvider = (RandomAccessFileProvider) System.Activator.CreateInstance(rafClass); // fail fast
		if (userLoads && !last)
		{
		  registeredRandomAccessFileProviders.Insert(0, rafProvider); // put user stuff first
		}
		else
		{
		  registeredRandomAccessFileProviders.Add(rafProvider);
		}
	  }

	  ///////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Open an existing netcdf file (read only).
	  /// </summary>
	  /// <param name="location"> location of file. </param>
	  /// <returns> the NetcdfFile. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> if error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static NetcdfFile open(String location) throws NetcdfSharp.java.io.IOException
	  public static NetcdfFile open(string location)
	  {
		return open(location, -1, null);
	  }

	  /// <summary>
	  /// Open an existing file (read only), with option of cancelling.
	  /// </summary>
	  /// <param name="location"> location of the file. </param>
	  /// <param name="cancelTask"> allow task to be cancelled; may be null. </param>
	  /// <returns> NetcdfFile object, or null if cant find IOServiceProver </returns>
	  /// <exception cref="IOException"> if error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static NetcdfFile open(String location, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public static NetcdfFile open(string location, CancelTask cancelTask)
	  {
		return open(location, -1, cancelTask);
	  }

	  /// <summary>
	  /// Open an existing file (read only), with option of cancelling, setting the RandomAccessFile buffer size for
	  /// efficiency.
	  /// </summary>
	  /// <param name="location"> location of file. </param>
	  /// <param name="buffer_size"> RandomAccessFile buffer size, if <= 0, use default size </param>
	  /// <param name="cancelTask"> allow task to be cancelled; may be null. </param>
	  /// <returns> NetcdfFile object, or null if cant find IOServiceProver </returns>
	  /// <exception cref="IOException"> if error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static NetcdfFile open(String location, int buffer_size, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public static NetcdfFile open(string location, int buffer_size, CancelTask cancelTask)
	  {
		return open(location, buffer_size, cancelTask, null);
	  }

	  /// <summary>
	  /// Open an existing file (read only), with option of cancelling, setting the RandomAccessFile buffer size for
	  /// efficiency, with an optional special object for the iosp.
	  /// </summary>
	  /// <param name="location"> location of file. This may be a
	  ///        <ol>
	  ///        <li>local netcdf-3 filename (with a file: prefix or no prefix)
	  ///        <li>remote netcdf-3 filename (with an http: prefix)
	  ///        <li>local netcdf-4 filename (with a file: prefix or no prefix)
	  ///        <li>local hdf-5 filename (with a file: prefix or no prefix)
	  ///        <li>local iosp filename (with a file: prefix or no prefix)
	  ///        </ol>
	  ///        If file ends with ".Z", ".zip", ".gzip", ".gz", or ".bz2", it will uncompress/unzip and write to new file
	  ///        without the suffix,
	  ///        then use the uncompressed file. It will look for the uncompressed file before it does any of that. Generally
	  ///        it prefers to
	  ///        place the uncompressed file in the same directory as the original file. If it does not have write permission
	  ///        on that directory, it will use the directory defined by NetcdfSharp.ucar.nc2.util.DiskCache class.
	  /// </param>
	  /// <param name="buffer_size"> RandomAccessFile buffer size, if <= 0, use default size </param>
	  /// <param name="cancelTask"> allow task to be cancelled; may be null. </param>
	  /// <param name="iospMessage"> special iosp tweaking (sent before open is called), may be null </param>
	  /// <returns> NetcdfFile object, or null if can't find IOServiceProver </returns>
	  /// <exception cref="IOException"> if error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static NetcdfFile open(String location, int buffer_size, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask, Object iospMessage) throws NetcdfSharp.java.io.IOException
	  public static NetcdfFile open(string location, int buffer_size, CancelTask cancelTask, object iospMessage)
	  {

		NetcdfSharp.ucar.unidata.io.RandomAccessFile raf = getRaf(location, buffer_size);
		try
		{
		  return open(raf, location, cancelTask, iospMessage);
		}
		catch (Exception t)
		{
		  raf.close();
		  throw new IOException(t.Message, t);
		}
	  }

	  /// <summary>
	  /// Open an existing file (read only), specifying which IOSP is to be used.
	  /// </summary>
	  /// <param name="location"> location of file </param>
	  /// <param name="iospClassName"> fully qualified class name of the IOSP class to handle this file </param>
	  /// <param name="bufferSize"> RandomAccessFile buffer size, if <= 0, use default size </param>
	  /// <param name="cancelTask"> allow task to be cancelled; may be null. </param>
	  /// <param name="iospMessage"> special iosp tweaking (sent before open is called), may be null </param>
	  /// <returns> NetcdfFile object, or null if cant find IOServiceProver </returns>
	  /// <exception cref="IOException"> if read error </exception>
	  /// <exception cref="ClassNotFoundException"> cannot find iospClassName in the class path </exception>
	  /// <exception cref="InstantiationException"> if class cannot be instantiated </exception>
	  /// <exception cref="IllegalAccessException"> if class is not accessible </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static NetcdfFile open(String location, String iospClassName, int bufferSize, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask, Object iospMessage) throws ClassNotFoundException, IllegalAccessException, InstantiationException, NetcdfSharp.java.io.IOException
	  public static NetcdfFile open(string location, string iospClassName, int bufferSize, CancelTask cancelTask, object iospMessage)
	  {

		Type iospClass = Type.GetType(iospClassName) ?? typeof(NetcdfFile).Assembly.GetType(iospClassName);
		IOServiceProvider spi = (IOServiceProvider) System.Activator.CreateInstance(iospClass); // fail fast

		// send iospMessage before iosp is opened
		if (iospMessage != null)
		{
		  spi.sendIospMessage(iospMessage);
		}

		if (bufferSize <= 0)
		{
		  bufferSize = default_buffersize;
		}

		NetcdfSharp.ucar.unidata.io.RandomAccessFile raf = NetcdfSharp.ucar.unidata.io.RandomAccessFile.acquire(canonicalizeUriString(location), bufferSize);

		NetcdfFile result = build(spi, raf, location, cancelTask);

		// send after iosp is opened
		if (iospMessage != null)
		{
		  spi.sendIospMessage(iospMessage);
		}

		return result;
	  }

	  /// <summary>
	  /// Find out if the location can be opened, but dont actually open it.
	  /// 
	  /// In order for a location to be openable by netCDF-java, we must have 1) a proper
	  /// <seealso cref="NetcdfSharp.ucar.unidata.io.RandomAccessFile"/> implementation, and 2) a proper <seealso cref="IOServiceProvider"/>
	  /// implementation.
	  /// </summary>
	  /// <param name="location"> location of file </param>
	  /// <returns> true if can be opened </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static boolean canOpen(String location) throws NetcdfSharp.java.io.IOException
	  public static bool canOpen(string location)
	  {
		bool canOpen = false;
		// do we have a RandomAccessFile class that will work?
		using (NetcdfSharp.ucar.unidata.io.RandomAccessFile raf = getRaf(location, -1))
		{
		  if (raf != null)
		  {
			log.info(string.Format("{0} can be accessed with {1}", raf.Location, raf.GetType()));
			// do we have an appropriate IOSP?
			IOServiceProvider iosp = getIosp(raf);
			if (iosp != null)
			{
			  canOpen = true;
			  log.info(string.Format("{0} can be opened by {1}", raf.Location, iosp.GetType()));
			}
		  }
		}
		return canOpen;
	  }

	  /// <summary>
	  /// Removes the {@code "file:"} or {@code "file://"} prefix from the location, if necessary. Also replaces
	  /// back slashes with forward slashes.
	  /// </summary>
	  /// <param name="location"> a URI string. </param>
	  /// <returns> a canonical URI string. </returns>
	  public static string canonicalizeUriString(string location)
	  {
		if (string.ReferenceEquals(location, null))
		{
		  return null;
		}
		// get rid of file prefix, if any
		string uriString = location.Trim();
		if (uriString.StartsWith("file://", StringComparison.Ordinal))
		{
		  uriString = uriString.Substring(7);
		}
		else if (uriString.StartsWith("file:", StringComparison.Ordinal))
		{
		  uriString = uriString.Substring(5);
		}

		// get rid of crappy microsnot \ replace with happy /
		return StringUtil2.replace(uriString, '\\', "/");
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static NetcdfSharp.ucar.unidata.io.RandomAccessFile downloadAndDecompress(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, String uriString, int buffer_size) throws NetcdfSharp.java.io.IOException
	  private static NetcdfSharp.ucar.unidata.io.RandomAccessFile downloadAndDecompress(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, string uriString, int buffer_size)
	  {
		int pos = uriString.LastIndexOf('/');

		if (pos < 0)
		{
		  pos = uriString.LastIndexOf(':');
		}

		string tmp = System.IO.Path.GetTempPath();
		string filename = uriString.Substring(pos + 1);
		string sep = File.separator;

		uriString = new File(tmp + sep + filename).getPath();
		copy(raf, new FileStream(uriString, FileMode.Create, FileAccess.Write), 1 << 20);
		try
		{
		  string uncompressedFileName = makeUncompressed(uriString);
		  // LOOK - this will only return one type of RandomAccessFile, which is OK for now,
		  // but needs to be addressed with RandomAccessDirectory
		  return NetcdfSharp.ucar.unidata.io.RandomAccessFile.acquire(uncompressedFileName, buffer_size);
		}
		catch (Exception)
		{
		  throw new IOException();
		}
	  }

	  /// <summary>
	  /// Get the appropriate RandomAccessFile for accessing an object at the provided location
	  /// </summary>
	  /// <param name="location"> a URI string. </param>
	  /// <param name="buffer_size"> size of the RandomAccessFileBuffer </param>
	  /// <returns> RandomAccessFile for the object at location </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static NetcdfSharp.ucar.unidata.io.RandomAccessFile getRaf(String location, int buffer_size) throws NetcdfSharp.java.io.IOException
	  public static NetcdfSharp.ucar.unidata.io.RandomAccessFile getRaf(string location, int buffer_size)
	  {
		string uriString = removeFragment(location.Trim());
		if (buffer_size <= 0)
		{
		  buffer_size = default_buffersize;
		}

		NetcdfSharp.ucar.unidata.io.RandomAccessFile raf = null;

		foreach (RandomAccessFileProvider provider in registeredRandomAccessFileProviders)
		{
		  if (provider.isOwnerOf(location))
		  {
			raf = provider.open(location, buffer_size);
			// might cause issues if the end of a resource location string
			// cannot be reliably used to determine compression
			if (looksCompressed(uriString) && !raf.Directory)
			{ // do not decompress directories all at once
			  raf = downloadAndDecompress(raf, uriString, buffer_size);
			}
			break;
		  }
		}

		if (raf == null)
		{
		  // look for dynamically loaded RandomAccessFile Providers
		  foreach (RandomAccessFileProvider provider in ServiceLoader.load(typeof(RandomAccessFileProvider)))
		  {
			if (provider.isOwnerOf(location))
			{
			  raf = provider.open(location, buffer_size);
			  // might cause issues if the end of a resource location string
			  // cannot be used to determine compression
			  if (looksCompressed(uriString) && !raf.Directory)
			  { // do not decompress directories all at once
				raf = downloadAndDecompress(raf, uriString, buffer_size);
			  }
			  break;
			}
		  }
		}

		if (raf == null)
		{
		  // ok, treat as a local file
		  // get rid of crappy microsnot \ replace with happy /
		  uriString = StringUtil2.replace(uriString, '\\', "/");

		  if (uriString.StartsWith("file:", StringComparison.Ordinal))
		  {
			// uriString = uriString.substring(5);
			uriString = StringUtil2.unescape(uriString.Substring(5)); // 11/10/2010 from erussell@ngs.org
		  }

		  string uncompressedFileName = null;
		  if (looksCompressed(uriString))
		  {
			try
			{
			  stringLocker.control(uriString); // Avoid race condition where the decompressed file is trying to be read by
											   // one thread while another is decompressing it
			  uncompressedFileName = makeUncompressed(uriString);
			}
			catch (Exception e)
			{
			  log.warn("Failed to uncompress {}, err= {}; try as a regular file.", uriString, e.Message);
			  // allow to fall through to open the "compressed" file directly - may be a misnamed suffix
			}
			finally
			{
			  stringLocker.release(uriString);
			}
		  }

		  if (!string.ReferenceEquals(uncompressedFileName, null))
		  {
			// open uncompressed file as a RandomAccessFile.
			raf = NetcdfSharp.ucar.unidata.io.RandomAccessFile.acquire(uncompressedFileName, buffer_size);
		  }
		  else
		  {
			// normal case - not compressed
			raf = NetcdfSharp.ucar.unidata.io.RandomAccessFile.acquire(uriString, buffer_size);
		  }
		}

		if (raf == null)
		{
		  throw new IOException("Could not find an appropriate RandomAccessFileProvider to open " + location);
		}

		return raf;
	  }

	  private static string removeFragment(string uriString)
	  {
		IList<string> protocols = DatasetUrl.getProtocols(uriString);
		// only remove fragment from non-local files
		return (protocols.Count == 0 || protocols.Contains("file")) ? uriString : uriString.Split('#')[0];
	  }

	  private static bool looksCompressed(string filename)
	  {
		// return true if filename ends with a compressed suffix or contains a compressed suffix followed by a delimiter
		// (i.e. path to a compressed entry)
		return possibleCompressedSuffixes.Any(compressedSuffix => filename.EndsWith("." + compressedSuffix, StringComparison.Ordinal)) || possibleCompressedSuffixes.Any(compressedSuffix => filename.Contains("." + compressedSuffix + "/"));
	  }

	  private static string findCompressedSuffix(string filename)
	  {
		if (possibleCompressedSuffixes.Any(compressedSuffix => filename.EndsWith("." + compressedSuffix, StringComparison.Ordinal)))
		{
		  return filename.Substring(filename.LastIndexOf('.') + 1);
		}
		return possibleCompressedSuffixes.FirstOrDefault(compressedSuffix => filename.Contains("." + compressedSuffix + "/")) ?? "";
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static String makeUncompressed(String filename) throws Exception
	  private static string makeUncompressed(string filename)
	  {
		string suffix = findCompressedSuffix(filename);
		int pos = filename.LastIndexOf(suffix, StringComparison.Ordinal);
		log.debug("suffix {}, pos {}", suffix, pos);

		string basepath = filename.Substring(0, pos - 1);
		string itempath = filename.Substring(pos + suffix.Length);
		// rebuild filepath without suffix (same as base path if there is not item path)
		string uncompressedFilename = basepath + itempath;
		// name of parent file
		string baseFilename = basepath + "." + suffix;

		log.debug("basepath '{}'", basepath);
		log.debug("itempath '{}'", itempath);
		log.debug("uncompressedFilename '{}'", uncompressedFilename);
		log.debug("baseFilename '{}'", baseFilename);

		// coverity claims resource leak, but attempts to fix break. so beware
		// see if already decompressed (cache removed; look in the same directory)
		File uncompressedFile = new File(uncompressedFilename);
		if (uncompressedFile.exists() && uncompressedFile.length() > 0)
		{
		  // see if its locked - another thread is writing it
		  FileStream stream = null;
		  try
		  {
			stream = new FileStream(uncompressedFile.GetPath(), FileMode.Open, FileAccess.Read);
			// obtain the lock
#if NET5_0_OR_GREATER
			while (OperatingSystem.IsWindows())
			{ // loop waiting for the lock
			  try
			  {
				stream.Lock(0, 1); // throws IOException while another thread is writing
				break;

			  }
			  catch (IOException)
			  { // not sure why lock() doesnt block
				try
				{
				  Thread.Sleep(100); // msecs
				}
				catch (InterruptedException)
				{
				  break;
				}
			  }
			}
#endif

			if (NetcdfFile.debugCompress)
			{
			  log.info("found uncompressed {} for {}", uncompressedFile, filename);
			}
			return uncompressedFile.getPath();

		  }
		  finally
		  {
			if (stream != null)
			{
			  stream.Close();
			}
		  }
		}

		// ok gonna write it
		// make sure compressed file exists
		File file = new File(baseFilename);
		if (!file.exists())
		{
		  return null; // bail out */
		}

		using (FileStream fout = new FileStream(uncompressedFile.GetPath(), FileMode.Create, FileAccess.Write))
		{

		  // obtain the lock
#if NET5_0_OR_GREATER
		  while (OperatingSystem.IsWindows())
		  { // loop waiting for the lock
			try
			{
			  fout.Lock(0, 1); // throws IOException while another thread is writing
			  break;

			}
			catch (IOException)
			{ // not sure why lock() doesnt block
			  try
			  {
				Thread.Sleep(100); // msecs
			  }
			  catch (InterruptedException)
			  {
			  }
			}
		  }
#endif

		  try
		  {
			if (suffix.Equals("Z", StringComparison.OrdinalIgnoreCase))
			{
			  // Z file can only contain one file - copy the whole thing
			  using (Stream @in = new UncompressInputStream(new BufferedInputStream(new FileStream(baseFilename, FileMode.Open, FileAccess.Read))))
			  {
				copy(@in, fout, 100000);
			  }
			  if (NetcdfFile.debugCompress)
			  {
				log.info("uncompressed {} to {}", filename, uncompressedFile);
			  }

			}
			else if (suffix.Equals("zip", StringComparison.OrdinalIgnoreCase))
			{
			  // find specified zip entry, if it exists
			  using (ZipArchive zin = new ZipArchive(new BufferedInputStream(new FileStream(baseFilename, FileMode.Open, FileAccess.Read)), ZipArchiveMode.Read))
			  {
				// If a desired zipentry ID was appended to method's filename parameter, then itempath
				// is of length > 1 and ID starts at itempath char offset 1.
				string itemName = (itempath.Length > 1) ? itempath.Substring(1) : "";
				log.debug("seeking zip itemName '{}'", itempath, itemName);
				int zeIndex = 0;
				ZipArchiveEntry ze = zin.Entries.Count > 0 ? zin.Entries[0] : null;

				while (ze != null)
				{
				  if (itempath.Length == 0 || ze.FullName.Equals(itemName))
				  {
					using (Stream @in = ze.Open())
					{
					  copy(@in, fout, 100000);
					}
					if (NetcdfFile.debugCompress)
					{
					  log.debug("unzipped {} entry {} to {}", filename, ze.FullName, uncompressedFile);
					}
					break;
				  }
				  zeIndex++;
				  ze = (zeIndex < zin.Entries.Count) ? zin.Entries[zeIndex] : null;
				}
			  }

			}
			else if (suffix.Equals("bz2", StringComparison.OrdinalIgnoreCase))
			{
			  // bz2 can only contain one file - copy the whole thing
			  using (Stream @in = new CBZip2InputStream(new BufferedInputStream(new FileStream(baseFilename, FileMode.Open, FileAccess.Read)), true))
			  {
				copy(@in, fout, 100000);
			  }
			  if (NetcdfFile.debugCompress)
			  {
				log.info("unbzipped {} to {}", filename, uncompressedFile);
			  }

			}
			else if (suffix.Equals("gzip", StringComparison.OrdinalIgnoreCase) || suffix.Equals("gz", StringComparison.OrdinalIgnoreCase))
			{
			  // gzip/gz concatenates streams - copy the whole thing
			  using (Stream @in = new GZipStream(new BufferedInputStream(new FileStream(baseFilename, FileMode.Open, FileAccess.Read)), CompressionMode.Decompress))
			  {
				copy(@in, fout, 100000);
			  }

			  if (NetcdfFile.debugCompress)
			  {
				log.info("ungzipped {} to {}", filename, uncompressedFile);
			  }
			}
		  }
		  catch (Exception)
		  {

			// appears we have to close before we can delete LOOK
			// fout.close();
			// fout = null;

			// dont leave bad files around
			if (uncompressedFile.exists())
			{
			  if (!uncompressedFile.delete())
			  {
				log.warn("failed to delete uncompressed file (IOException) {}", uncompressedFile);
			  }
			}
			throw;

		  }
		}

		return uncompressedFile.getPath();
	  }

	  // LOOK why not use util.IO ?
	  private static void copy(Stream @in, Stream @out, int bufferSize)
	  {
		byte[] buffer = new byte[bufferSize];
		while (true)
		{
		  int bytesRead = @in.Read(buffer, 0, buffer.Length);
		  if (bytesRead == -1)
		  {
			break;
		  }
		  @out.Write(buffer, 0, bytesRead);
		}
	  }

	  private static void copy(RandomAccessFile @in, Stream @out, int bufferSize)
	  {
		long length = @in.length();
		byte[] buffer = new byte[bufferSize];
		int bytesRead = 0;
		while (length > 0)
		{
		  if (length > bufferSize)
		  {
			@in.readFully(buffer, 0, bufferSize);
			bytesRead = bufferSize;
		  }
		  else if (length <= bufferSize)
		  {
			@in.readFully(buffer, 0, (int) length);
			bytesRead = (int) length;
		  }
		  length -= bufferSize;
		  @out.Write(buffer, 0, bytesRead);
		}
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Read a local CDM file into memory. All reads are then done from memory.
	  /// </summary>
	  /// <param name="filename"> location of CDM file, must be a local file. </param>
	  /// <returns> a NetcdfFile, which is completely in memory </returns>
	  public static NetcdfFile openInMemory(string filename)
	  {
		File file = new File(filename);
		MemoryStream bos = new MemoryStream((int) file.length());
		using (Stream @in = new BufferedInputStream(new FileStream(filename, FileMode.Open, FileAccess.Read)))
		{
		  IO.copy(@in, bos);
		}
		return openInMemory(filename, bos.ToArray());
	  }

	  /// <summary>
	  /// Read a remote CDM file into memory. All reads are then done from memory.
	  /// </summary>
	  /// <param name="uri"> location of CDM file, must be accessible through url.toURL().openStream(). </param>
	  /// <returns> a NetcdfFile, which is completely in memory </returns>
	  public static NetcdfFile openInMemory(URI uri)
	  {
		URL url = uri.toURL();
		byte[] contents;
		using (Stream @in = url.openStream())
		{
		  contents = IO.readContentsToByteArray(@in);
		}
		return openInMemory(uri.ToString(), contents);
	  }

	  /// <summary>
	  /// Open an in-memory netcdf file.
	  /// </summary>
	  /// <param name="name"> name of the dataset. Typically use the filename or URI. </param>
	  /// <param name="data"> in-memory netcdf file </param>
	  /// <returns> memory-resident NetcdfFile </returns>
	  public static NetcdfFile openInMemory(string name, byte[] data)
	  {
		NetcdfSharp.ucar.unidata.io.InMemoryRandomAccessFile raf = new NetcdfSharp.ucar.unidata.io.InMemoryRandomAccessFile(name, data);
		return open(raf, name, null, null);
	  }

	  /// <summary>
	  /// Open an in-memory netcdf file, with a specific iosp.
	  /// </summary>
	  /// <param name="name"> name of the dataset. Typically use the filename or URI. </param>
	  /// <param name="data"> in-memory netcdf file </param>
	  /// <param name="iospClassName"> fully qualified class name of the IOSP class to handle this file </param>
	  /// <returns> NetcdfFile object, or null if cant find IOServiceProver </returns>
	  public static NetcdfFile openInMemory(string name, byte[] data, string iospClassName)
	  {
		NetcdfSharp.ucar.unidata.io.InMemoryRandomAccessFile raf = new NetcdfSharp.ucar.unidata.io.InMemoryRandomAccessFile(name, data);
		Type iospClass = Type.GetType(iospClassName) ?? typeof(NetcdfFile).Assembly.GetType(iospClassName);
		IOServiceProvider spi = (IOServiceProvider) System.Activator.CreateInstance(iospClass);
		return build(spi, raf, name, null);
	  }

	  /// <summary>
	  /// Open a RandomAccessFile as a NetcdfFile, if possible.
	  /// </summary>
	  /// <param name="raf"> The open raf, is not cloised by this method. </param>
	  /// <param name="location"> human readable locatoin of this dataset. </param>
	  /// <param name="cancelTask"> used to monitor user cancellation; may be null. </param>
	  /// <param name="iospMessage"> send this message to iosp; may be null. </param>
	  /// <returns> NetcdfFile or throw an Exception. </returns>
	  public static NetcdfFile open(RandomAccessFile raf, string location, CancelTask cancelTask, object iospMessage)
	  {
		IOServiceProvider spi = getIosp(raf);
		if (spi == null)
		{
		  raf.close();
		  throw new IOException("Cant read " + location + ": not a valid CDM file.");
		}

		// send iospMessage before the iosp is opened
		if (iospMessage != null)
		{
		  spi.sendIospMessage(iospMessage);
		}

		if (log.isDebugEnabled())
		{
		  log.debug("Using IOSP {}", spi.GetType().FullName);
		}

		NetcdfFile ncfile = ((AbstractIOServiceProvider) spi).Builder ? build(spi, raf, location, cancelTask) : new NetcdfFile(spi, raf, location, cancelTask);
		spi.buildFinish(ncfile);

		// send iospMessage after iosp is opened
		if (iospMessage != null)
		{
		  spi.sendIospMessage(iospMessage);
		}

		return ncfile;
	  }

	  private static IOServiceProvider getIosp(RandomAccessFile raf)
	  {
		if (NetcdfFile.debugSPI)
		{
		  log.info("NetcdfFile try to open = {}", raf.Location);
		}

		// Registered providers override defaults.
		foreach (IOServiceProvider registeredSpi in registeredProviders)
		{
		  if (NetcdfFile.debugSPI)
		  {
			log.info(" try iosp = {}", registeredSpi.GetType().FullName);
		  }

		  if (registeredSpi.isValidFile(raf))
		  {
			// need a new instance for thread safety
			Type c = registeredSpi.GetType();
			try
			{
			  return (IOServiceProvider) System.Activator.CreateInstance(c);
			}
			catch (System.MissingMethodException)
			{
			  throw new IOException("IOServiceProvider " + c.FullName + "must have no-arg constructor.");
			}
			catch (System.MemberAccessException e)
			{
			  throw new IOException("IOServiceProvider " + c.FullName + " IllegalAccessException: " + e.Message);
			}
		  }
		}

		if (N3headerNew.isValidFile(raf))
		{
		  return new N3iospNew();

		}
		else
		{
		  // look for dynamically loaded IOSPs, and sort before using
		  IList<IOServiceProvider> sortedIosps = Lists.newArrayList<IOServiceProvider>(ServiceLoader.load(typeof(IOServiceProvider)));
		  Collections.sort(sortedIosps);

		  foreach (IOServiceProvider loadedSpi in sortedIosps)
		  {
			if (loadedSpi.isValidFile(raf))
			{
			  Type c = loadedSpi.GetType();
			  try
			  {
				return (IOServiceProvider) System.Activator.CreateInstance(c);
			  }
			  catch (System.MissingMethodException)
			  {
				throw new IOException("IOServiceProvider " + c.FullName + "must have no-arg constructor.");
			  }
			  catch (System.MemberAccessException e)
			  {
				throw new IOException("IOServiceProvider " + c.FullName + " IllegalAccessException: " + e.Message);
			  }
			}
		  }
		}
		return null;
	  }

	  /// <summary>
	  /// Build a NetcdfFile using the new builder pattern.
	  /// </summary>
	  public static NetcdfFile build(IOServiceProvider spi, RandomAccessFile raf, string location, CancelTask cancelTask)
	  {
		NetcdfFile.Builder builder = NetcdfFile.builder().setIosp((AbstractIOServiceProvider) spi).setLocation(location);

		try
		{
		  Group.Builder root = Group.builder().setName("");
		  spi.build(raf, root, cancelTask);
		  builder.setRootGroup(root);

		  string id = root.getAttributeContainer().findAttributeString("_Id", null);
		  if (id != null)
		  {
			builder.setId(id);
		  }
		  string title = root.getAttributeContainer().findAttributeString("_Title", null);
		  if (title != null)
		  {
			builder.setTitle(title);
		  }

		}
		catch (Exception)
		{
		  try
		  {
			raf.close();
		  }
		  catch (Exception)
		  {
		  }
		  try
		  {
			spi.close();
		  }
		  catch (Exception)
		  {
		  }
		  throw; // preserve stack trace
		}

		return builder.build();
	  }

	  ///////////////////////////////////////////////////////////////////////
	  // All CDM naming convention enforcement should be here.
	  // DAP conventions should be in DODSNetcdfFile
	  // TODO move this elsewhere.

	  // reservedFullName defines the characters that must be escaped
	  // when a short name is inserted into a full name
	  public const string reservedFullName = ".\\";

	  // reservedSectionSpec defines the characters that must be escaped
	  // when a short name is inserted into a section specification.
	  private const string reservedSectionSpec = "();,.\\";

	  // reservedSectionCdl defines the characters that must be escaped
	  // when what?
	  private const string reservedCdl = "[ !\"#$%&'()*,:;<=>?[]^`{|}~\\";

	  /// <summary>
	  /// Create a valid CDM object name.
	  /// Control chars (< 0x20) are not allowed.
	  /// Trailing and leading blanks are not allowed and are stripped off.
	  /// A space is converted into an underscore "_".
	  /// A forward slash "/" is converted into an underscore "_".
	  /// </summary>
	  /// <param name="shortName"> from this name </param>
	  /// <returns> valid CDM object name </returns>
	  public static string makeValidCdmObjectName(string shortName)
	  {
		if (string.ReferenceEquals(shortName, null))
		{
		  return null;
		}
		return StringUtil2.makeValidCdmObjectName(shortName);
	  }

	  /// <summary>
	  /// Escape special characters in a netcdf short name when
	  /// it is intended for use in CDL.
	  /// </summary>
	  /// <param name="vname"> the name </param>
	  /// <returns> escaped version of it </returns>
	  public static string makeValidCDLName(string vname)
	  {
		return EscapeStrings.backslashEscape(vname, reservedCdl);
	  }

	  /// <summary>
	  /// Escape special characters in a netcdf short name when
	  /// it is intended for use in a fullname
	  /// </summary>
	  /// <param name="vname"> the name </param>
	  /// <returns> escaped version of it </returns>
	  public static string makeValidPathName(string vname)
	  {
		return EscapeStrings.backslashEscape(vname, reservedFullName);
	  }

	  /// <summary>
	  /// Escape special characters in a netcdf short name when
	  /// it is intended for use in a sectionSpec
	  /// </summary>
	  /// <param name="vname"> the name </param>
	  /// <returns> escaped version of it </returns>
	  public static string makeValidSectionSpecName(string vname)
	  {
		return EscapeStrings.backslashEscape(vname, reservedSectionSpec);
	  }

	  /// <summary>
	  /// Unescape any escaped characters in a name.
	  /// </summary>
	  /// <param name="vname"> the escaped name </param>
	  /// <returns> unescaped version of it </returns>
	  internal static string makeNameUnescaped(string vname)
	  {
		return EscapeStrings.backslashUnescape(vname);
	  }

	  /// <summary>
	  /// Create a Groups's full name with appropriate backslash escaping. </summary>
	  public static string makeFullName(Group g)
	  {
		// return makeFullName(g, reservedFullName);
		Group parent = g.ParentGroup;
		if ((parent == null) || parent.Root) // common case?
		{
		  return EscapeStrings.backslashEscape(g.ShortName, reservedFullName);
		}
		StringBuilder sbuff = new StringBuilder();
		appendGroupName(sbuff, parent, reservedFullName);
		sbuff.Append(EscapeStrings.backslashEscape(g.ShortName, reservedFullName));
		return sbuff.ToString();
	  }

	  /// <summary>
	  /// Create a Variable's full name with appropriate backslash escaping.
	  /// Warning: do not use for a section spec.
	  /// </summary>
	  /// <param name="v"> the Variable </param>
	  /// <returns> full name </returns>
	  public static string makeFullName(Variable v)
	  {
		return makeFullName(v, reservedFullName);
	  }

	  /// <summary>
	  /// Create a Variable's full name with
	  /// appropriate backslash escaping for use in a section spec.
	  /// </summary>
	  /// <param name="v"> the cdm node </param>
	  /// <returns> full name </returns>
	  public static string makeFullNameSectionSpec(Variable v)
	  {
		return makeFullName(v, reservedSectionSpec);
	  }

	  /// <summary>
	  /// Given a CDMNode, create its full name with
	  /// appropriate backslash escaping of the specified characters.
	  /// </summary>
	  /// <param name="node"> the cdm node </param>
	  /// <param name="reservedChars"> the set of characters to escape </param>
	  /// <returns> full name </returns>
	  private static string makeFullName(Variable node, string reservedChars)
	  {
		Group parent = node.getParentGroup();
		if (((parent == null) || parent.Root) && !node.MemberOfStructure) // common case?
		{
		  return EscapeStrings.backslashEscape(node.ShortName, reservedChars);
		}
		StringBuilder sbuff = new StringBuilder();
		appendGroupName(sbuff, parent, reservedChars);
		appendStructureName(sbuff, node, reservedChars);
		return sbuff.ToString();
	  }

	  private static void appendGroupName(StringBuilder sbuff, Group g, string reserved)
	  {
		if (g == null)
		{
		  return;
		}
		if (g.ParentGroup == null)
		{
		  return;
		}
		appendGroupName(sbuff, g.ParentGroup, reserved);
		sbuff.Append(EscapeStrings.backslashEscape(g.ShortName, reserved));
		sbuff.Append("/");
	  }

	  private static void appendStructureName(StringBuilder sbuff, Variable n, string reserved)
	  {
		if (n.MemberOfStructure)
		{
		  appendStructureName(sbuff, n.ParentStructure, reserved);
		  sbuff.Append(".");
		}
		sbuff.Append(EscapeStrings.backslashEscape(n.ShortName, reserved));
	  }

	  /// <summary>
	  /// Create a synthetic full name from a group plus a string
	  /// </summary>
	  /// <param name="parent"> parent group </param>
	  /// <param name="name"> synthetic name string </param>
	  /// <returns> synthetic name </returns>
	  protected internal static string makeFullNameWithString(Group parent, string name)
	  {
		name = makeValidPathName(name); // escape for use in full name
		StringBuilder sbuff = new StringBuilder();
		appendGroupName(sbuff, parent, null);
		sbuff.Append(name);
		return sbuff.ToString();
	  }

	}

}
