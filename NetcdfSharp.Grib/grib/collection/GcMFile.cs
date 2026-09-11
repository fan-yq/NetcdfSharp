// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/*
 * Copyright (c) 1998-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using IO = NetcdfSharp.ucar.nc2.util.IO;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// MFile stored in GC index
	/// 
	/// @author caron
	/// @since 2/19/14
	/// </summary>
	public class GcMFile : MFile
	{

	  internal static IList<GcMFile> makeFiles(MFile directory, IList<MFile> files, ISet<int> allFileSet)
	  {
		IList<GcMFile> result = new List<GcMFile>(files.Count);
		string dirPath = StringUtil2.replace(directory.Path, '\\', "/");

		foreach (int index in allFileSet)
		{
		  MFile file = files[index];
		  string filename;
		  if (file.Path.StartsWith(dirPath, StringComparison.Ordinal))
		  {
			filename = file.Name;
			if (filename.StartsWith("/", StringComparison.Ordinal))
			{
			  filename = filename.Substring(1);
			}
		  }
		  else
		  {
			filename = file.Path; // when does this happen ??
		  }
		  result.Add(new GcMFile(directory, filename, file.LastModified, file.Length, index));
		}
		return result;
	  }

	  ////////////////////////////////////////////////////////////////////////////////////////
	  public readonly MFile directory;
	  public readonly string name;
	  public readonly long lastModified, length;
	  public readonly int index;

	  internal GcMFile(MFile directory, string name, long lastModified, long length, int index)
		  : base(directory != null && directory.Path != null ? directory.Path + "/" + name : name)
	  {
		this.directory = directory;
		this.name = name;
		this.lastModified = lastModified;
		this.index = index;
		this.length = length;
	  }

	  public override long LastModified
	  {
		  get
		  {
			return lastModified;
		  }
	  }

	  public override long Length
	  {
		  get
		  {
			return length;
		  }
	  }

	  public override bool isDirectory()
	  {
		return false;
	  }

	  public override string Path
	  {
		  get
		  {
			MFile file = directory.getChild(name);
			return file == null ? null : file.Path;
		  }
	  }

	  public override string Name
	  {
		  get
		  {
			return name;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.thredds.inventory.MFile getParent() throws NetcdfSharp.java.io.IOException
	  public override MFile Parent
	  {
		  get
		  {
			return directory;
		  }
	  }

	  public virtual int CompareTo(MFile o)
	  {
		return string.CompareOrdinal(name, o.Name);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public Object getAuxInfo()
	  public virtual object AuxInfo
	  {
		  get
		  {
			return null;
		  }
		  set
		  {
		  }
	  }


	  public override MFile getDirectory()
	  {
		return directory;
	  }

	  public override string ToString()
	  {
		return "GcMFile{" + "directory=" + directory.Path + ", name='" + name + '\'' + ", lastModified=" + lastModified + ", length=" + length + ", index=" + index + '}';
	  }

	  public override bool exists()
	  {
		MFile file = directory.getChild(name);
		return file != null && file.exists();
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.java.io.InputStream getInputStream() throws NetcdfSharp.java.io.FileNotFoundException
	  public override Stream InputStream
	  {
		  get
		  {
			MFile file = directory.getChild(name);
			if (file == null)
			{
			  throw new FileNotFoundException(name);
			}
			return file.InputStream;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void writeToStream(NetcdfSharp.java.io.OutputStream outputStream) throws NetcdfSharp.java.io.IOException
	  public virtual void writeToStream(Stream outputStream)
	  {
		IO.copyFile(Path, outputStream);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void writeToStream(NetcdfSharp.java.io.OutputStream outputStream, long offset, long maxBytes) throws NetcdfSharp.java.io.IOException
	  public virtual void writeToStream(Stream outputStream, long offset, long maxBytes)
	  {
		using (RandomAccessFile randomAccessFile = RandomAccessFile.acquire(Path))
		{
		  IO.copyRafB(randomAccessFile, offset, maxBytes, outputStream);
		}
	  }

	  public override GcMFile getChild(string newFilename)
	  {
		throw new System.NotSupportedException("GcMFile::getChild not implemented. Filename: " + Name);
	  }

	  public virtual string relativize(MFile other)
	  {
		throw new System.NotSupportedException("GcMFile::relativize not implemented. Filename: " + Name);
	  }
	}

}
