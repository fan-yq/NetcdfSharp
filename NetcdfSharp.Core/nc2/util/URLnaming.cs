// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.util
{
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Networking utilities.
	/// 
	/// TODO move to util.net
	/// </summary>
	public class URLnaming
	{

	  /// try to figure out if we need to add file: to the location when writing
	  public static string canonicalizeWrite(string location)
	  {
		try
		{
		  URI refURI = URI.create(location);
		  if (refURI.isAbsolute())
		  {
			return location;
		  }
		}
		catch (Exception)
		{
		  // return "file:" + location;
		}
		return "file:" + location;
	  }

	  /// <summary>
	  /// This augments URI.resolve(), by also dealing with file: URIs.
	  /// If baseURi is not a file: scheme, then URI.resolve is called.
	  /// Otherwise the last "/" is found in the base, and the ref is appended to it.
	  /// <para>
	  /// For file: baseURLS: only relative URLS not starting with / are supported. This is
	  /// apparently different from the behavior of URI.resolve(), so may be trouble,
	  /// but it allows NcML absolute location to be specified without the file: prefix.
	  /// <p/>
	  /// Example :
	  /// 
	  /// <pre>
	  /// base:     file://my/guide/collections/designfaq.ncml
	  /// ref:      sub/my.nc
	  /// resolved: file://my/guide/collections/sub/my.nc
	  /// </pre>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="baseUri"> base URI as a Strng </param>
	  /// <param name="relativeUri"> relative URI, as a String </param>
	  /// <returns> the resolved URI as a String </returns>
	  public static string resolve(string baseUri, string relativeUri)
	  {
		if ((string.ReferenceEquals(baseUri, null)) || (string.ReferenceEquals(relativeUri, null)))
		{
		  return relativeUri;
		}

		if (relativeUri.StartsWith("file:", StringComparison.Ordinal))
		{
		  return relativeUri;
		}

		// deal with a base file URL
		if (baseUri.StartsWith("file:", StringComparison.Ordinal))
		{

		  // the case where the relativeURL is absolute.
		  // unfortunately, we may get an Exception
		  try
		  {
			URI uriRelative = URI.create(relativeUri);
			if (uriRelative.isAbsolute())
			{
			  return relativeUri;
			}
		  }
		  catch (Exception)
		  {
			// empty
		  }

		  if ((relativeUri.Length > 0) && (relativeUri[0] == '#'))
		  {
			return baseUri + relativeUri;
		  }

		  if ((relativeUri.Length > 0) && (relativeUri[0] == '/'))
		  {
			return relativeUri;
		  }

		  baseUri = StringUtil2.substitute(baseUri, "\\", "/"); // assumes forward slash
		  int pos = baseUri.LastIndexOf('/');
		  if (pos > 0)
		  {
			string baseDir = baseUri.Substring(0, pos + 1);
			if (relativeUri.Equals("."))
			{
			  return baseDir;
			}
			else
			{
			  return baseDir + relativeUri;
			}
		  }
		}

		if (baseUri.StartsWith("cdms3:", StringComparison.Ordinal))
		{
		  if (relativeUri.StartsWith("cdms3:", StringComparison.Ordinal))
		  {
			return relativeUri;
		  }
		  else
		  {
			MFile absoluteMFile;
			try
			{
			  absoluteMFile = MFiles.create(baseUri).getParent().getChild(relativeUri);
			}
			catch (IOException)
			{
			  return relativeUri;
			}
			return absoluteMFile == null ? relativeUri : absoluteMFile.getPath();
		  }
		}

		// non-file URLs

		// relativeUri = canonicalizeRead(relativeUri);
		try
		{
		  URI relativeURI = URI.create(relativeUri);
		  if (relativeURI.isAbsolute())
		  {
			return relativeUri;
		  }

		  // otherwise let the URI class resolve it
		  URI baseURI = URI.create(baseUri);
		  URI resolvedURI = baseURI.resolve(relativeUri);
		  return resolvedURI.toASCIIString();

		}
		catch (ArgumentException)
		{
		  return relativeUri;
		}
	  }

	  public static string resolveFile(string baseDir, string filepath)
	  {
		if (string.ReferenceEquals(baseDir, null))
		{
		  return filepath;
		}
		if (string.ReferenceEquals(filepath, null))
		{
		  return null;
		}
		File file = new File(filepath);
		if (file.isAbsolute())
		{
		  return filepath;
		}

		if (baseDir.StartsWith("file:", StringComparison.Ordinal))
		{
		  baseDir = baseDir.Substring(5);
		}

		MFile @base = MFiles.create(baseDir);
		if (!@base.isDirectory())
		{
		  try
		  {
			@base = @base.getParent();
		  }
		  catch (IOException)
		  {
			return filepath;
		  }
		}
		if (@base == null)
		{
		  return filepath;
		}
		MFile absoluteFile = @base.getChild(filepath);
		return absoluteFile == null ? filepath : absoluteFile.getPath();
	  }

	}

}
