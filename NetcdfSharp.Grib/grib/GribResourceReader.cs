// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.IO;
using System.IO.Compression;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib
{

	/*
	 * Static methods to read resource files.
	 *
	 * @author caron 07/28/14
	 * 
	 * @version 2.0
	 */
	public class GribResourceReader
	{
	  // The GRIB tables are embedded as one compressed zip (see NetcdfSharp.Grib.csproj
	  // PackGribTables + tools/bufr-pack-resources.py); decompress the requested entry.
	  private const string archiveResource = "/resources/gribTables.zip";
	  private static byte[] archiveBytes; // full zip, loaded lazily and kept in memory
	  private static readonly object archiveLock = new object();

	  /// <summary>
	  /// Get the input stream to the given resource
	  /// </summary>
	  /// <param name="resourceName"> The resource name. May be a resource on the class path or a file </param>
	  /// <returns> The input stream to the resource </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static InputStream getInputStream(String resourceName) throws FileNotFoundException
	  public static Stream getInputStream(string resourceName)
	  {

		// Try the embedded table archive first (entry names are relative to resources/)
		Stream s = openFromArchive(normalizeEntryName(resourceName));
		if (s != null)
		{
		  return s;
		}

		// Try embedded resources (legacy per-file)
		s = typeof(GribResourceReader).Assembly.GetManifestResourceStream(resourceName);
		if (s != null)
		{
		  return s;
		}

		// Try the file system
		string path = resourceName;
		if (System.IO.File.Exists(path))
		{
		  return System.IO.File.Open(path, FileMode.Open, FileAccess.Read);
		}

		// give up
		throw new FileNotFoundException("Cant find resource " + resourceName);
	  }

	  // Like getInputStream, but returns null when the resource is absent
	  // (mirrors Java ClassLoader.getResourceAsStream).
	  public static Stream getInputStreamOrNull(string resourceName)
	  {
		try
		{
		  return getInputStream(resourceName);
		}
		catch (FileNotFoundException)
		{
		  return null;
		}
	  }

	  // "resources/grib1/wmoTable3.xml" or "/resources/grib2/wmo/X.xml" -> "grib1/wmoTable3.xml" / "grib2/wmo/X.xml"
	  private static string normalizeEntryName(string resourceName)
	  {
		if (resourceName == null)
		{
		  return null;
		}
		string name = resourceName.Replace('\\', '/');
		if (name.StartsWith("/", StringComparison.Ordinal))
		{
		  name = name.Substring(1);
		}
		if (name.StartsWith("resources/", StringComparison.Ordinal))
		{
		  name = name.Substring("resources/".Length);
		}
		return name;
	  }

	  // decompress a single entry of the embedded zip; returns null when the entry is absent.
	  // the caller consumes and disposes the stream
	  private static Stream openFromArchive(string entryName)
	  {
		if (string.IsNullOrEmpty(entryName))
		{
		  return null;
		}

		byte[] zip = archiveBytes;
		if (zip == null)
		{
		  lock (archiveLock)
		  {
			if (archiveBytes == null)
			{
			  using (Stream zs = typeof(GribResourceReader).Assembly.GetManifestResourceStream(archiveResource))
			  {
				if (zs == null)
				{
				  return null; // no archive embedded (e.g. building only a single project)
				}
				using (MemoryStream ms = new MemoryStream())
				{
				  zs.CopyTo(ms);
				  archiveBytes = ms.ToArray();
				}
			  }
			}
			zip = archiveBytes;
		  }
		}

		using (MemoryStream ms = new MemoryStream(zip, false))
		using (ZipArchive za = new ZipArchive(ms, ZipArchiveMode.Read))
		{
		  ZipArchiveEntry entry = za.GetEntry(entryName);
		  if (entry == null)
		  {
			return null;
		  }
		  using (Stream es = entry.Open())
		  {
			MemoryStream result = new MemoryStream();
			es.CopyTo(result);
			result.Position = 0;
			return result;
		  }
		}
	  }

	}


}
