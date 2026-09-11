// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.IO;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp
{
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using StructureDataIterator = NetcdfSharp.ucar.ma2.StructureDataIterator;
	using Group = NetcdfSharp.ucar.nc2.Group;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using NetcdfFiles = NetcdfSharp.ucar.nc2.NetcdfFiles;
	using ParsedSectionSpec = NetcdfSharp.ucar.nc2.ParsedSectionSpec;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Format = NetcdfSharp.ucar.unidata.util.Format;

	/// <summary>
	/// Abstract base class for IOSP implementations that provides default implementations
	/// of readToByteChannel(...) and readSection(...).
	/// <para>
	/// Implementations should make sure to handle the RandomAccessFile properly by
	/// doing one of the following:
	/// <ol>
	/// <li>Write your own open(...) and close() methods that keep track of the
	/// RandomAccessFile, be sure to close the RandomAccessFile in your close()
	/// method.</li>
	/// <li>Write your own open(...) and close() methods that call the open(...)
	/// and close() methods defined here, use the "raf" variable also defined
	/// here.</li>
	/// <li>Don't write an open(...) or close() method, so that those defined
	/// here are used.</li>
	/// </ol>
	/// </para>
	/// </summary>
	public abstract class AbstractIOServiceProvider : IOServiceProvider
	{
		public virtual SortGroup getSortGroup()
		{
			return NetcdfSharp.ucar.nc2.iosp.IOServiceProvider.SortGroup.LAST_GROUP;
		}
		public virtual int CompareTo(IOServiceProvider other)
		{
			return getSortGroup().CompareTo(other.getSortGroup());
		}
		public abstract string FileTypeDescription {get;}
		public abstract string FileTypeId {get;}
		public abstract NcArray readData(NetcdfSharp.ucar.nc2.Variable v2, Section section);
		public abstract bool isValidFile(RandomAccessFile raf);
	  /// <summary>
	  /// Subclasses that use AbstractIOServiceProvider.open(...) or .close()
	  /// should use this (instead of their own private variable).
	  /// </summary>
	  protected internal RandomAccessFile raf;
	  protected internal string location;
	  protected internal int rafOrder = RandomAccessFile.BIG_ENDIAN;

	  // In general, ncfile doesnt exist until after open is called.
	  // That argues for open() changing to a builder.
	  protected internal NetcdfFile ncfile;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void open(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public virtual void open(RandomAccessFile raf, NetcdfFile ncfile, CancelTask cancelTask)
	  {
		this.raf = raf;
		this.location = (raf != null) ? raf.Location : null;
		this.ncfile = ncfile;
	  }

	  // TODO: Is there an alternative to making this method public? Maybe in 6?
	  public virtual NetcdfFile NetcdfFile
	  {
		  set
		  {
			this.ncfile = value;
		  }
	  }

	  public virtual bool Builder
	  {
		  get
		  {
			return false;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void build(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.Group.Builder rootGroup, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public virtual void build(RandomAccessFile raf, Group.Builder rootGroup, CancelTask cancelTask)
	  {
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new System.NotSupportedException(string.Format("Class {0} does not implement build() method", this.GetType().FullName));
	  }

	  public virtual void buildFinish(NetcdfFile ncfile)
	  {
		// No op
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void close() throws NetcdfSharp.java.io.IOException
	  public virtual void close()
	  {
		if (raf != null)
		{
		  raf.close();
		}
		raf = null;
	  }

	  // release any resources like file handles
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void release() throws NetcdfSharp.java.io.IOException
	  public virtual void release()
	  {
		if (raf != null)
		{
		  raf.close();
		}
		raf = null;
	  }

	  // reacquire any resources like file handles
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void reacquire() throws NetcdfSharp.java.io.IOException
	  public virtual void reacquire()
	  {
		raf = NetcdfFiles.getRaf(location, -1);
		raf.order(rafOrder);
	  }

	  // default implementation, reads into an Array, then writes to WritableByteChannel
	  // subclasses should override if possible
	  // LOOK DataOutputStream uses big-endian
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public long readToByteChannel(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.java.nio.channels.WritableByteChannel channel) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public virtual long readToByteChannel(NetcdfSharp.ucar.nc2.Variable v2, Section section, WritableByteChannel channel)
	  {

		NcArray data = readData(v2, section);
		return IospHelper.copyToByteChannel(data, channel);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public long readToOutputStream(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.java.io.OutputStream out) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public virtual long readToOutputStream(NetcdfSharp.ucar.nc2.Variable v2, Section section, Stream @out)
	  {

		NcArray data = readData(v2, section);
		return IospHelper.copyToOutputStream(data, @out);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public long streamToByteChannel(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.java.nio.channels.WritableByteChannel channel) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public virtual long streamToByteChannel(NetcdfSharp.ucar.nc2.Variable v2, Section section, WritableByteChannel channel)
	  {

		NcArray data = readData(v2, section);
		return IospHelper.copyToByteChannel(data, channel);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.ucar.ma2.Array readSection(NetcdfSharp.ucar.nc2.ParsedSectionSpec cer) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public virtual NcArray readSection(ParsedSectionSpec cer)
	  {
		return IospHelper.readSection(cer); // IOSPs can optimize by overriding
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.ucar.ma2.StructureDataIterator getStructureIterator(NetcdfSharp.ucar.nc2.Structure s, int bufferSize) throws NetcdfSharp.java.io.IOException
	  public virtual StructureDataIterator getStructureIterator(Structure s, int bufferSize)
	  {
		return null;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public Object sendIospMessage(@Nullable Object message)
	  public virtual object sendIospMessage(object message)
	  {
		if (message is string && (string)message == NetcdfFile.IOSP_MESSAGE_RANDOM_ACCESS_FILE)
		{
		  return raf;
		}
		return null;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean syncExtend() throws NetcdfSharp.java.io.IOException
	  public virtual bool syncExtend()
	  {
		return false;
	  }

	  /// <summary>
	  /// Returns the time that the underlying file(s) were last modified. If they've changed since they were stored in the
	  /// cache, they will be closed and reopened with <seealso cref="NetcdfSharp.ucar.nc2.util.cache.FileFactory"/>.
	  /// </summary>
	  /// <returns> a {@code long} value representing the time the file(s) were last modified or {@code 0L} if the
	  ///         last-modified time couldn't be determined for any reason. </returns>
	  public virtual long LastModified
	  {
		  get
		  {
			if (!string.ReferenceEquals(location, null))
			{
			  MFile file = MFiles.create(location);
			  return file.LastModified;
			}
			else
			{
			  return 0;
			}
		  }
	  }

	  public virtual string toStringDebug(object o)
	  {
		return "";
	  }

	  public virtual string DetailInfo
	  {
		  get
		  {
			if (raf == null)
			{
			  return "";
			}
			try
			{
			  Formatter fout = new Formatter();
			  double size = raf.length() / (1000.0 * 1000.0);
			  fout.format(" raf = %s%n", raf.Location);
			  fout.format(" size= %d (%s Mb)%n%n", raf.length(), Format.dfrac(size, 3));
			  return fout.ToString();

			}
			catch (IOException e)
			{
			  return e.Message;
			}
		  }
	  }

	  public virtual string FileTypeVersion
	  {
		  get
		  {
			return "N/A";
		  }
	  }

	}

}
