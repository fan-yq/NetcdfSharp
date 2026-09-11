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
namespace NetcdfSharp.ucar.nc2.iosp
{
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using StructureData = NetcdfSharp.ucar.ma2.StructureData;

	/// <summary>
	/// This is an interface to Netcdf-3 and Netcdf-4 file writing.
	/// </summary>
	/// @deprecated This will be internal in ver6. 
	[Obsolete("This will be internal in ver6.")]
	public interface IOServiceProviderWriter : IOServiceProvider
	{
		/// <summary>
		/// Create new file, populate it from the objects in ncfile.
		/// </summary>
		/// <param name="filename"> name of file to create. </param>
		/// <param name="ncfile"> get everything but data from here </param>
		/// <param name="extra"> if > 0, pad header with extra bytes </param>
		/// <param name="preallocateSize"> if > 0, set length of file to this upon creation - this (usually) pre-allocates contiguous
		///        storage. </param>
		/// <param name="largeFile"> if want large file format </param>
		/// <exception cref="NetcdfSharp.java.io.IOException"> if I/O error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void create(String filename, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, int extra, long preallocateSize, boolean largeFile) throws NetcdfSharp.java.io.IOException;
		void create(string filename, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, int extra, long preallocateSize, bool largeFile);

		/// <summary>
		/// Open existing file and allow writing. Netcdf-4 writing is general. Netcdf-3 writing is restricted to writing data
		/// into
		/// existing variables.
		/// </summary>
		/// <param name="raf"> the file to work on. </param>
		/// <param name="ncfile"> add objects to this empty NetcdfFile </param>
		/// <param name="cancelTask"> used to monitor user cancellation; may be null. </param>
		/// <exception cref="IOException"> if I/O error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void openForWriting(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException;
		void openForWriting(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask);

		/// <summary>
		/// Set the fill flag.
		/// For new files, set in the create() method. This method is to set fill for existing files that you want to write.
		/// If true, the data is first written with fill values.
		/// Set to false if you expect to write all data values, set to true if you want to be
		/// sure that unwritten data values have the fill value in it.
		/// </summary>
		/// <param name="fill"> set fill mode true or false </param>
		bool Fill { set; }

		/// <summary>
		/// Write data into a variable.
		/// </summary>
		/// <param name="v2"> variable to write; must already exist. </param>
		/// <param name="section"> the section of data to write.
		///        There must be a Range for each Dimension in the variable, in order.
		///        The shape must match the shape of values.
		///        The origin and stride indicate where the data is placed into the stored Variable array. </param>
		/// <param name="values"> data to write. The shape must match section.getShape(). </param>
		/// <exception cref="IOException"> if I/O error </exception>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if invalid section </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void writeData(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.ucar.ma2.Array values) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
		void writeData(NetcdfSharp.ucar.nc2.Variable v2, Section section, NetcdfSharp.ucar.ma2.NcArray values);

		/// <summary>
		/// Append a structureData along the unlimited dimension
		/// </summary>
		/// <param name="s"> belongs to this structure </param>
		/// <param name="sdata"> the stuctureData to append </param>
		/// <returns> the recnum where it was written </returns>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: int appendStructureData(NetcdfSharp.ucar.nc2.Structure s, NetcdfSharp.ucar.ma2.StructureData sdata) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
		int appendStructureData(Structure s, StructureData sdata);


		/// <summary>
		/// if theres room before data, rewrite header without moving the data. netcdf3 only
		/// </summary>
		/// <returns> true if it worked </returns>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: boolean rewriteHeader(boolean largeFile) throws NetcdfSharp.java.io.IOException;
		bool rewriteHeader(bool largeFile);

		/// <summary>
		/// Update the value of an existing attribute. Attribute is found by name, which must match exactly.
		/// You cannot make an attribute longer, or change the number of values.
		/// For strings: truncate if longer, zero fill if shorter. Strings are padded to 4 byte boundaries, ok to use padding
		/// if it exists.
		/// For numerics: must have same number of values.
		/// </summary>
		/// <param name="v2"> variable, or null for global attribute </param>
		/// <param name="att"> replace with this value </param>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void updateAttribute(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.nc2.Attribute att) throws NetcdfSharp.java.io.IOException;
		void updateAttribute(NetcdfSharp.ucar.nc2.Variable v2, NcAttribute att);

		/// <summary>
		/// Flush all data buffers to disk.
		/// </summary>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void flush() throws NetcdfSharp.java.io.IOException;
		void flush();

	}

}
