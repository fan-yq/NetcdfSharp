// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.IO;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp
{
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using Group = NetcdfSharp.ucar.nc2.Group;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using ParsedSectionSpec = NetcdfSharp.ucar.nc2.ParsedSectionSpec;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using StructureDataIterator = NetcdfSharp.ucar.ma2.StructureDataIterator;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// This is the service provider interface for the low-level I/O access classes (read only).
	/// This is only used by service implementors.
	/// 
	/// An implementation must have a no-argument constructor.
	/// 
	/// The NetcdfFile class manages all registered IOServiceProvider classes.
	/// When NetcdfFiles.open() is called:
	/// <ol>
	/// <li>the file is opened as a NetcdfSharp.ucar.unidata.io.RandomAccessFile;</li>
	/// <li>the file is handed to the isValidFile() method of each registered
	/// IOServiceProvider class (until one returns true, which means it can read the file).</li>
	/// <li>the open() method on the resulting IOServiceProvider class is handed the file.</li>
	/// 
	/// @author caron
	/// </summary>
	public interface IOServiceProvider : IComparable<IOServiceProvider>
	{

		/// <summary>
		/// Check if this is a valid file for this IOServiceProvider.
		/// You must make this method thread safe, ie dont keep any state.
		/// </summary>
		/// <param name="raf"> RandomAccessFile </param>
		/// <returns> true if valid. </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> if read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException;
		bool isValidFile(RandomAccessFile raf);

		/// <summary>
		/// Open existing file, and populate ncfile with it. This method is only called by the
		/// NetcdfFile constructor on itself. The provided NetcdfFile object will be empty
		/// except for the location String and the IOServiceProvider associated with this
		/// NetcdfFile object.
		/// </summary>
		/// <param name="raf"> the file to work on, it has already passed the isValidFile() test. </param>
		/// <param name="ncfile"> add objects to this empty NetcdfFile </param>
		/// <param name="cancelTask"> used to monitor user cancellation; may be null. </param>
		/// <exception cref="IOException"> if read error </exception>
		/// @deprecated Use build(RandomAccessFile raf, Group.Builder rootGroup, CancelTask cancelTask) 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("Use build(RandomAccessFile raf, Group.Builder rootGroup, CancelTask cancelTask)") void open(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException;
		[Obsolete("Use build(RandomAccessFile raf, Group.Builder rootGroup, CancelTask cancelTask)")]
		void open(RandomAccessFile raf, NetcdfFile ncfile, CancelTask cancelTask);

		/// <summary>
		/// If this iosp implements build().
		/// </summary>
		bool Builder { get; }

		/// <summary>
		/// Open existing file, and populate it. Note that you cannot reference the NetcdfFile within this routine.
		/// This is the bridge to immutable objects that will be used exclusively in version 6.
		/// </summary>
		/// <param name="raf"> the file to work on, it has already passed the isValidFile() test. </param>
		/// <param name="rootGroup"> add objects to the root group. </param>
		/// <param name="cancelTask"> used to monitor user cancellation; may be null. </param>
		/// <exception cref="IOException"> if read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void build(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.Group.Builder rootGroup, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException;
		void build(RandomAccessFile raf, Group.Builder rootGroup, CancelTask cancelTask);

		/// <summary>
		/// Sometimes the builder needs access to the finished objects. This is called after ncfile.build() </summary>
		void buildFinish(NetcdfFile ncfile);

		/// <summary>
		/// Read data from a top level Variable and return a memory resident Array. This Array has the same element type as the
		/// Variable, and the
		/// requested shape.
		/// </summary>
		/// <param name="v2"> a top-level Variable </param>
		/// <param name="section"> the section of data to read. There must be a Range for each Dimension in the variable, in order.
		///        Note: no nulls allowed.
		///        IOSP may not modify. </param>
		/// <returns> the requested data in a memory-resident Array </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> if read error </exception>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if invalid section </exception>
		/// <seealso cref="NetcdfSharp.ucar.ma2.NcRange"/>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: NetcdfSharp.ucar.ma2.Array readData(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
		NetcdfSharp.ucar.ma2.NcArray readData(Variable v2, Section section);

		/// <summary>
		/// Read data from a top level Variable and send data to a WritableByteChannel.
		/// Must be in big-endian order.
		/// </summary>
		/// <param name="v2"> a top-level Variable </param>
		/// <param name="section"> the section of data to read.
		///        There must be a Range for each Dimension in the variable, in order.
		///        Note: no nulls allowed. IOSP may not modify. </param>
		/// <param name="channel"> write data to this WritableByteChannel </param>
		/// <returns> the number of bytes written to the channel </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> if read error </exception>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if invalid section </exception>
		/// @deprecated do not use 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("do not use") long readToByteChannel(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.java.nio.channels.WritableByteChannel channel) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
		[Obsolete("do not use")]
		long readToByteChannel(Variable v2, Section section, WritableByteChannel channel);

		/// @deprecated do not use 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("do not use") long streamToByteChannel(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.java.nio.channels.WritableByteChannel channel) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
		[Obsolete("do not use")]
		long streamToByteChannel(Variable v2, Section section, WritableByteChannel channel);

		/// <summary>
		/// Read data from a top level Variable and send data to a OutputStream.
		/// Must be in big-endian order, following ncstream conventions.
		/// Default implementation just reads to memory and writes to stream.
		/// Allow override for possible performance improvements.
		/// </summary>
		/// <param name="v2"> a top-level Variable </param>
		/// <param name="section"> the section of data to read.
		///        There must be a Range for each Dimension in the variable, in order.
		///        Note: no nulls allowed. IOSP may not modify. </param>
		/// <param name="out"> write data to this OutputStream </param>
		/// <returns> the number of bytes written to the channel </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> if read error </exception>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if invalid section </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: long readToOutputStream(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.java.io.OutputStream out) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
		long readToOutputStream(Variable v2, Section section, Stream @out);

		/// <summary>
		/// Allows reading sections of nested variables
		/// </summary>
		/// <param name="cer"> section specification : what data is wanted </param>
		/// <returns> requested data array </returns>
		/// <exception cref="IOException"> on read error </exception>
		/// <exception cref="InvalidRangeException"> if section spec is invalid </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: NetcdfSharp.ucar.ma2.Array readSection(NetcdfSharp.ucar.nc2.ParsedSectionSpec cer) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
		NetcdfSharp.ucar.ma2.NcArray readSection(ParsedSectionSpec cer);

		/// <summary>
		/// Get the structure iterator. iosps with top level sequences must override.
		/// Not threadsafe; do not use multiple StructureDataIterator for the same iosp.
		/// </summary>
		/// <param name="s"> the Structure </param>
		/// <param name="bufferSize"> the buffersize </param>
		/// <returns> the data iterator </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> if problem reading data </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: NetcdfSharp.ucar.ma2.StructureDataIterator getStructureIterator(NetcdfSharp.ucar.nc2.Structure s, int bufferSize) throws NetcdfSharp.java.io.IOException;
		StructureDataIterator getStructureIterator(Structure s, int bufferSize);

		/// <summary>
		/// Close the file.
		/// It is the IOServiceProvider's job to close the file (even though it didnt open it),
		/// and to free any other resources it has used.
		/// </summary>
		/// <exception cref="IOException"> if read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void close() throws NetcdfSharp.java.io.IOException;
		void close();

		/// <summary>
		/// Extend the NetcdfFile if the underlying dataset has changed
		/// in a way that is compatible with the current metadata.
		/// For example, if the unlimited dimension has grown.
		/// </summary>
		/// <returns> true if the NetcdfFile was extended. </returns>
		/// <exception cref="IOException"> if a read error occured when accessing the underlying dataset. </exception>
		/// @deprecated Do not use. 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("Do not use.") boolean syncExtend() throws NetcdfSharp.java.io.IOException;
		[Obsolete("Do not use.")]
		bool syncExtend();

		/// <summary>
		/// Release any system resources like file handles.
		/// Optional, implement only if you are able to reacquire.
		/// Used when object is made inactive in cache.
		/// </summary>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void release() throws NetcdfSharp.java.io.IOException;
		void release();

		/// <summary>
		/// Reacquire any resources like file handles
		/// Used when reactivating in cache.
		/// </summary>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void reacquire() throws NetcdfSharp.java.io.IOException;
		void reacquire();

		// public long getLastModified(); LOOK: dont add this for backwards compatibility. Probably add back in in version 5

		/// <summary>
		/// A way to communicate arbitrary information to and from an iosp.
		/// </summary>
		/// <param name="message"> opaque message sent to the IOSP object when its opened (not when isValidFile() is called) </param>
		/// <returns> opaque Object, may be null. </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable Object sendIospMessage(@Nullable Object message);
		object sendIospMessage(object message);

		/// <summary>
		/// Debug info for this object.
		/// </summary>
		/// <param name="o"> which object </param>
		/// <returns> debug info for this object </returns>
		string toStringDebug(object o);

		/// <summary>
		/// Show debug / underlying implementation details
		/// </summary>
		/// <returns> debug info </returns>
		string DetailInfo { get; }

		/// <summary>
		/// Get a unique id for this file type.
		/// </summary>
		/// <returns> registered id of the file type </returns>
		/// <seealso cref=""https://www.unidata.ucar.edu/software/netcdf-java/formats/FileTypes.html""/>
		string FileTypeId { get; }

		/// <summary>
		/// Get the version of this file type.
		/// </summary>
		/// <returns> version of the file type </returns>
		/// <seealso cref=""https://www.unidata.ucar.edu/software/netcdf-java/formats/FileTypes.html""/>
		string FileTypeVersion { get; }

		/// <summary>
		/// Get a human-readable description for this file type.
		/// </summary>
		/// <returns> description of the file type </returns>
		/// <seealso cref=""https://www.unidata.ucar.edu/software/netcdf-java/formats/FileTypes.html""/>
		string FileTypeDescription { get; }

		/// <summary>
		/// Used to determine the ordering for dynamically loaded IOServiceProviders.
		/// </summary>
		public enum SortGroup
		{
			GROUP_1,
			LAST_GROUP,
		}

		/// <summary>
		/// Get the SortGroup for this IOServiceProvider. This determines the order in which the dynamically loaded
		/// IOServiceProviders are checked to see if they can open a file with isValidFile().
		/// </summary>
		/// <returns> SortGroup, by default the SortGroup.LAST_GROUP will be used. </returns>
		SortGroup getSortGroup();

		/// <summary>
		/// Use the SortGroup to determine the ordering for dynamically loaded IOServiceProviders.
		/// </summary>
		int compareTo(IOServiceProvider other)
		{
			return getSortGroup().CompareTo(other.getSortGroup());
		}
	}

}
