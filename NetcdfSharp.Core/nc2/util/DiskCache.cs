// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.util
{
	using JDateTime = NetcdfSharp.org.joda.time.JDateTime;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// This is a general purpose utility for determining a place to write files and cache them, eg for
	/// uncompressing files. This class does not scour itself.
	/// <para>
	/// Note that when a file in the cache is accessed, its lastModified date is set, which is used for the LRU scouring.
	/// </para>
	/// <para>
	/// The cdm library sometimes needs to write files, eg
	/// to uncompress them, or for grib index files, etc. The first choice is to write these files
	/// in the same directory that the original file lives in. However, that directory may not be
	/// writeable, so we need to find a place to write them to.
	/// We also want to use the file if it already exists.
	/// </para>
	/// <para>
	/// </para>
	/// <para>
	/// A writeable cache "root directory" is set:
	/// <ol>
	/// <li>explicitly by setRootDirectory()
	/// <li>through the system property "nj22.cache" if it exists
	/// <li>by appending "/nj22/cache" to the system property "user.home" if it exists
	/// <li>by appending "/nj22/cache" to the system property "user.dir" if it exists
	/// <li>use "./nj22/cache" if none of the above
	/// </ol>
	/// </para>
	/// <para>
	/// </para>
	/// <para>
	/// Scenario 1: want to uncompress a file; check to see if already have done so,
	/// otherwise get a File that can be written to.
	/// 
	/// <pre>
	/// // see if already uncompressed
	/// File uncompressedFile = FileCache.getFile(uncompressedFilename, false);
	/// if (!uncompressedFile.exists()) {
	///   // nope, uncompress it
	///   UncompressInputStream.uncompress(uriString, uncompressedFile);
	/// }
	/// doSomething(uncompressedFile);
	/// </pre>
	/// </para>
	/// <para>
	/// </para>
	/// <para>
	/// Scenario 2: want to write a derived file always in the cache.
	/// 
	/// <pre>
	/// File derivedFile = FileCache.getCacheFile(derivedFilename);
	/// if (!derivedFile.exists()) {
	///   createDerivedFile(derivedFile);
	/// }
	/// doSomething(derivedFile);
	/// </pre>
	/// </para>
	/// <para>
	/// </para>
	/// <para>
	/// Scenario 3: same as scenario 1, but use the default Cache policy:
	/// 
	/// <pre>
	/// File wf = FileCache.getFileStandardPolicy(uncompressedFilename);
	/// if (!wf.exists()) {
	///   writeToFile(wf);
	///   wf.close();
	/// }
	/// doSomething(wf);
	/// </pre>
	/// 
	/// TODO will move in ver 6
	/// </para>
	/// </summary>
	public class DiskCache
	{
	  private static NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger("cacheLogger");
	  private static string root;
	  private static bool standardPolicy;
	  private static bool checkExist;

	  /// <summary>
	  /// debug only
	  /// </summary>
	  public static bool simulateUnwritableDir;

	  static DiskCache()
	  {
		root = JSystem.getProperty("nj22.cache");

		if (string.ReferenceEquals(root, null))
		{
		  string home = JSystem.getProperty("user.home");

		  if (string.ReferenceEquals(home, null))
		  {
			home = JSystem.getProperty("user.dir");
		  }

		  if (string.ReferenceEquals(home, null))
		  {
			home = ".";
		  }

		  root = home + "/.unidata/cache/";
		}

		string policy = JSystem.getProperty("nj22.cachePolicy");
		if (!string.ReferenceEquals(policy, null))
		{
		  standardPolicy = policy.Equals("true", StringComparison.OrdinalIgnoreCase);
		}
	  }

	  /// <summary>
	  /// Set the cache root directory. Create it if it doesnt exist.
	  /// </summary>
	  /// <param name="cacheDir"> the cache directory </param>
	  public static string RootDirectory
	  {
		  set
		  {
			if (!value.EndsWith("/", StringComparison.Ordinal))
			{
			  value = value + "/";
			}
			root = StringUtil2.replace(value, '\\', "/"); // no nasty backslash

			makeRootDirectory();
		  }
		  get
		  {
			return root;
		  }
	  }

	  /// <summary>
	  /// Make sure that the current root directory exists.
	  /// </summary>
	  public static void makeRootDirectory()
	  {
		File dir = new File(root);
		if (!dir.exists())
		{
		  if (!dir.mkdirs())
		  {
			throw new System.InvalidOperationException("DiskCache.setRootDirectory(): could not create root directory <" + root + ">.");
		  }
		}
		checkExist = true;
	  }


	  /// <summary>
	  /// Set the standard policy used in getWriteableFileStandardPolicy().
	  /// Default is to try to create the file in the same directory as the original.
	  /// Setting alwaysInCache to true means to always create it in the cache directory.
	  /// </summary>
	  /// <param name="alwaysInCache"> make this the default policy </param>
	  public static bool CachePolicy
	  {
		  set
		  {
			standardPolicy = value;
		  }
	  }

	  /// <summary>
	  /// Get a File if it exists. If not, get a File that can be written to.
	  /// Use the standard policy to decide where to place it.
	  /// <para>
	  /// Things are a bit complicated, because in order to guarantee a file in
	  /// an arbitrary location can be written to, we have to try to open it as a
	  /// FileOutputStream. If we do, we don't want to open it twice,
	  /// so we return a WriteableFile that contains an opened FileOutputStream.
	  /// If it already exists, or we get it from cache, we don't need to open it.
	  /// </para>
	  /// <para>
	  /// In any case, you must call WriteableFile.close() to make sure its closed.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="fileLocation"> normal file location </param>
	  /// <returns> WriteableFile holding the writeable File and a possibly opened FileOutputStream (append false) </returns>
	  public static File getFileStandardPolicy(string fileLocation)
	  {
		return getFile(fileLocation, standardPolicy);
	  }

	  /// <summary>
	  /// Get a File if it exists. If not, get a File that can be written to.
	  /// If alwaysInCache, look only in the cache, otherwise, look in the "normal" location first,
	  /// then in the cache.
	  /// <para>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="fileLocation"> normal file location </param>
	  /// <param name="alwaysInCache"> true if you want to look only in the cache </param>
	  /// <returns> a File that either exists or is writeable. </returns>
	  public static File getFile(string fileLocation, bool alwaysInCache)
	  {
		if (alwaysInCache)
		{
		  return getCacheFile(fileLocation);

		}
		else
		{
		  File f = new File(fileLocation);
		  if (f.exists())
		  {
			return f;
		  }

		  // now comes the tricky part to make sure we can open and write to it
		  try
		  {
			if (!simulateUnwritableDir && f.createNewFile())
			{
			  bool ret = f.delete();
			  Debug.Assert(ret);
			  return f;
			}
		  }
		  catch (IOException)
		  {
			// cant write to it - drop through
		  }

		  return getCacheFile(fileLocation);
		}
	  }

	  /// <summary>
	  /// Get a file in the cache.
	  /// File may or may not exist. We assume its always writeable.
	  /// If it does exist, set its LastModifiedDate to current time.
	  /// </summary>
	  /// <param name="fileLocation"> normal file location </param>
	  /// <returns> equivalent File in the cache. </returns>
	  public static File getCacheFile(string fileLocation)
	  {
		File f = new File(makeCachePath(fileLocation));
		if (f.exists())
		{
		  if (!f.setLastModified(DateTimeHelper.CurrentUnixTimeMillis()))
		  {
			logger.warn("Failed to setLastModified on " + f.getPath());
		  }
		}

		if (!checkExist)
		{
		  File dir = f.getParentFile();
		  if (!dir.exists() && !dir.mkdirs())
		  {
			logger.warn("Failed to mkdirs on " + dir.getPath());
		  }
		  checkExist = true;
		}
		return f;
	  }

	  /// <summary>
	  /// Make the cache filename
	  /// </summary>
	  /// <param name="fileLocation"> normal file location </param>
	  /// <returns> cache filename </returns>
	  private static string makeCachePath(string fileLocation)
	  {
		Escaper urlPathEscaper = UrlEscapers.urlPathSegmentEscaper();

		fileLocation = fileLocation.Replace('\\', '/'); // LOOK - use better normalization code eg Spring StringUtils
		string cachePath = urlPathEscaper.escape(fileLocation);

		// We need to escape ":" on windows or else our cache file will be something
		// like root + http, or root + C. We're already using UrlEscapers, so let's
		// replace ":" with "%3A"
		cachePath = cachePath.Replace(":", "%3A");

		return root + cachePath;
	  }

	  public static void showCache(PrintStream pw)
	  {
		pw.println("Cache files");
		pw.println("Size   LastModified       Filename");
		File dir = new File(root);
		File[] children = dir.listFiles();
		if (children == null)
		{
		  return;
		}
		foreach (File file in children)
		{
		  string org = EscapeStrings.urlDecode(file.getName());
		  pw.println(" " + file.length() + " " + new JDateTime(file.lastModified()) + " " + org);
		}
	  }

	  /// <summary>
	  /// Remove all files with date < cutoff.
	  /// </summary>
	  /// <param name="cutoff"> earliest date to allow </param>
	  /// <param name="sbuff"> write results here, null is ok. </param>
	  public static void cleanCache(JDateTime cutoff, StringBuilder sbuff)
	  {
		if (sbuff != null)
		{
		  sbuff.Append("CleanCache files before ").Append(cutoff).Append("\n");
		}
		File dir = new File(root);
		File[] children = dir.listFiles();
		if (children == null)
		{
		  return;
		}
		foreach (File file in children)
		{
		  JDateTime lastMod = new JDateTime(file.lastModified());
		  if (lastMod < cutoff)
		  {
			bool ret = file.delete();
			if (sbuff != null)
			{
			  sbuff.Append(" delete ").Append(file).Append(" (").Append(lastMod).Append(")\n");
			  if (!ret)
			  {
				sbuff.Append("Error deleting ").Append(file).Append("\n");
			  }
			}
		  }
		}
	  }

	  /// <summary>
	  /// Remove files if needed to make cache have less than maxBytes bytes file sizes.
	  /// This will remove oldest files first.
	  /// </summary>
	  /// <param name="maxBytes"> max number of bytes in cache. </param>
	  /// <param name="sbuff"> write results here, null is ok. </param>
	  public static void cleanCache(long maxBytes, StringBuilder sbuff)
	  {
		File dir = new File(root);

		File[] files = dir.listFiles();
		if (files != null)
		{
		  cleanCache(maxBytes, new FileAgeComparator(new List<File> {files}), sbuff);
		}
	  }

	  /// <summary>
	  /// Remove files if needed to make cache have less than maxBytes bytes file sizes.
	  /// This will remove files in sort order defined by fileComparator.
	  /// The first files in the sort order are kept, until the max bytes is exceeded, then they are deleted.
	  /// </summary>
	  /// <param name="maxBytes"> max number of bytes in cache. </param>
	  /// <param name="fileComparator"> sort files first with this </param>
	  /// <param name="sbuff"> write results here, null is ok. </param>
	  public static void cleanCache(long maxBytes, IComparer<File> fileComparator, StringBuilder sbuff)
	  {
		if (sbuff != null)
		{
		  sbuff.Append("DiskCache clean maxBytes= ").Append(maxBytes).Append(" on dir ").Append(root).Append("\n");
		}

		File dir = new File(root);
		long total = 0, total_delete = 0;

		File[] files = dir.listFiles();
		if (files != null)
		{
		  IList<File> fileList = files;
		  fileList.Sort(fileComparator);

		  foreach (File file in fileList)
		  {
			if (file.length() + total > maxBytes)
			{
			  total_delete += file.length();
			  if (sbuff != null)
			  {
				sbuff.Append(" delete ").Append(file).Append(" (").Append(file.length()).Append(")\n");
			  }
			  if (!file.delete() && sbuff != null)
			  {
				sbuff.Append("Error deleting ").Append(file).Append("\n");
			  }
			}
			else
			{
			  total += file.length();
			}
		  }
		}
		if (sbuff != null)
		{
		  sbuff.Append("Total bytes deleted= ").Append(total_delete).Append("\n");
		  sbuff.Append("Total bytes left in cache= ").Append(total).Append("\n");
		}
	  }

	  // reverse sort - latest come first
	  private class FileAgeComparator : IComparer<File>
	  {
		internal static readonly IDictionary<File, long> lastModified = new Dictionary<File, long>();

		public FileAgeComparator(IList<File> fileList)
		{
		  fileList.ForEach(file => lastModified.put(file, file.lastModified()));
		}

		public virtual int Compare(File f1, File f2)
		{
		  return lastModified.GetOrDefault(f2, JDateTime.now().getMillis()).compareTo(lastModified.GetOrDefault(f1, JDateTime.now().getMillis()));
		}
	  }

	}

}


