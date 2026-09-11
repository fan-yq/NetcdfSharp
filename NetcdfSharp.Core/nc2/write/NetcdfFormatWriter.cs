// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2020 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.write
{
	using Preconditions = NetcdfSharp.com.google.common.@base.Preconditions;
	using ArrayChar = NetcdfSharp.ucar.ma2.ArrayChar;
	using ArrayObject = NetcdfSharp.ucar.ma2.ArrayObject;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using StructureData = NetcdfSharp.ucar.ma2.StructureData;
	using NetcdfSharp.ucar.nc2;
	using N3iospNew = NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3.N3iospNew;
	using N3iospWriter = NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3.N3iospWriter;
	using IOServiceProvider = NetcdfSharp.ucar.nc2.iosp.IOServiceProvider;
	using IOServiceProviderWriter = NetcdfSharp.ucar.nc2.iosp.IOServiceProviderWriter;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;

	/// <summary>
	/// Writes Netcdf 3 or 4 formatted files to disk.
	/// Note that there is no redefine mode. Once you call build(), you cannot add new metadata, you can only write data.
	/// 
	/// <pre>
	/// NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(testFile.getPath());
	/// writerb.addDimension(Dimension.builder().setName("vdim").setIsUnlimited(true).build());
	/// writerb.addVariable("v", DataType.BYTE, "vdim");
	/// using (NetcdfFormatWriter writer = writerb.build()) {
	///   writer.write("v", dataArray);
	/// }
	/// </pre>
	/// </summary>
	public class NetcdfFormatWriter : IDisposable
	{
		private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(NetcdfFormatWriter));

		/// <summary>
		/// Open an existing Netcdf file for writing data.
		/// Cannot add new objects, you can only read/write data to existing Variables.
		/// </summary>
		/// <param name="location"> name of existing file to open. </param>
		/// <returns> existing file that can be written to </returns>
		/// <exception cref="IOException"> on I/O error </exception>
		public static NetcdfFormatWriter.Builder openExisting(string location)
		{
			return builder().setNewFile(false).setLocation(location);
		}

		/// <summary>
		/// Create a new Netcdf3 file.
		/// </summary>
		/// <param name="location"> name of new file to open; if it exists, will overwrite it. </param>
		/// <returns> new NetcdfFormatWriter </returns>
		public static NetcdfFormatWriter.Builder createNewNetcdf3(string location)
		{
			return builder().setNewFile(true).setFormat(NetcdfFileFormat.NETCDF3).setLocation(location);
		}

		/// <summary>
		/// Create a new Netcdf3 file with 64-bit offsets.
		/// </summary>
		/// <param name="location"> name of new file to open; if it exists, will overwrite it. </param>
		/// <returns> new NetcdfFormatWriter </returns>
		public static NetcdfFormatWriter.Builder createNewNetcdf3c64(string location)
		{
			return builder().setNewFile(true).setFormat(NetcdfFileFormat.NETCDF3_64BIT_OFFSET).setLocation(location);
		}

		/// <summary>
		/// Obtain a Builder to set custom options </summary>
		public static Builder builder()
		{
			return new Builder();
		}

		public class Builder
		{
			internal string location;
			internal NetcdfFileFormat format = NetcdfFileFormat.NETCDF3;
			internal bool isNewFile = true;
			internal bool fill = true;
			internal int extraHeaderBytes;
			internal long preallocateSize;
			internal IOServiceProvider iosp;

			internal Group.Builder rootGroup = Group.builder().setName("");

			/// <summary>
			/// The file location </summary>
			public virtual Builder setLocation(string location)
			{
				this.location = location;
				return this;
			}

			public virtual Builder setIosp(IOServiceProvider iosp)
			{
				this.iosp = iosp;
				return this;
			}

			public virtual IOServiceProvider Iosp
			{
				get
				{
					return iosp;
				}
			}

			/// <summary>
			/// Set the format version. Only needed when its a new file. Default is NetcdfFileFormat.NETCDF3 </summary>
			public virtual Builder setFormat(NetcdfFileFormat format)
			{
				this.format = format;
				return this;
			}

			public virtual NetcdfFileFormat Format
			{
				get
				{
					return this.format;
				}
			}

			/// <summary>
			/// True if its a new file, false if its an existing file. Default true. </summary>
			public virtual Builder setNewFile(bool newFile)
			{
				isNewFile = newFile;
				return this;
			}

			/// <summary>
			/// Set the fill flag. Only used by netcdf-3.
			/// If true, the data is first written with fill values.
			/// Default is fill = true, to follow the C library.
			/// Set false if you expect to write all data values, which makes writing faster.
			/// Set true if you want to be sure that unwritten data values are set to the fill value.
			/// </summary>
			public virtual Builder setFill(bool fill)
			{
				this.fill = fill;
				return this;
			}

			/// <summary>
			/// Set extra bytes to reserve in the header. Only used by netcdf-3.
			/// This can prevent rewriting the entire file on redefine.
			/// </summary>
			/// <param name="extraHeaderBytes"> # bytes extra for the header </param>
			public virtual Builder setExtraHeader(int extraHeaderBytes)
			{
				this.extraHeaderBytes = extraHeaderBytes;
				return this;
			}

			/// <summary>
			/// Preallocate the file size, for efficiency. Only used by netcdf-3. </summary>
			public virtual Builder setPreallocateSize(long preallocateSize)
			{
				this.preallocateSize = preallocateSize;
				return this;
			}

			/// <summary>
			/// Add a global attribute </summary>
			public virtual Builder addAttribute(NcAttribute att)
			{
				if (!isNewFile)
				{
					throw new System.NotSupportedException("Cant add attribute to existing netcdf-3 files");
				}
				rootGroup.addAttribute(att);
				return this;
			}

			/// <summary>
			/// Add a dimension to the root group. </summary>
			public virtual Dimension addDimension(string dimName, int length)
			{
				if (!isNewFile)
				{
					throw new System.NotSupportedException("Cant add dimension to existing netcdf-3 files");
				}
				Dimension dim = new Dimension(dimName, length);
				rootGroup.addDimension(dim);
				return dim;
			}

			/// <summary>
			/// Add a dimension to the root group. </summary>
			public virtual Dimension addDimension(Dimension dim)
			{
				if (!isNewFile)
				{
					throw new System.NotSupportedException("Cant add dimension to existing netcdf-3 files");
				}
				rootGroup.addDimension(dim);
				return dim;
			}

			/// <summary>
			/// Add an unlimited dimension to the root group. </summary>
			public virtual Dimension addUnlimitedDimension(string dimName)
			{
				return addDimension(Dimension.builder().setName(dimName).setIsUnlimited(true).build());
			}

			/// <summary>
			/// Get the root group </summary>
			public virtual Group.Builder RootGroup
			{
				get
				{
					return rootGroup;
				}
			}

			/// <summary>
			/// Set the root group. This allows all metadata to be built externally. </summary>
			public virtual Builder setRootGroup(Group.Builder rootGroup)
			{
				this.rootGroup = rootGroup;
				return this;
			}

			/// <summary>
			/// Add a Variable to the root group. </summary>
			public virtual Variable.Builder addVariable(string shortName, DataType dataType, string dimString)
			{
				if (!isNewFile)
				{
					throw new System.NotSupportedException("Cant add variable to existing netcdf-3 files");
				}
				Variable.Builder vb = Variable.builder().setName(shortName).setDataType(dataType).setParentGroupBuilder(rootGroup).setDimensionsByName(dimString);
				rootGroup.addVariable(vb);
				return vb;
			}

			/// <summary>
			/// Add a Variable to the root group. </summary>
			public virtual Variable.Builder addVariable(string shortName, DataType dataType, IList<Dimension> dims)
			{
				if (!isNewFile)
				{
					throw new System.NotSupportedException("Cant add variable to existing netcdf-3 files");
				}
				Variable.Builder vb = Variable.builder().setName(shortName).setDataType(dataType).setParentGroupBuilder(rootGroup).setDimensions(dims);
				rootGroup.addVariable(vb);
				return vb;
			}

			/// <summary>
			/// Add a Structure to the root group. </summary>
			public virtual Structure.BuilderStruct addStructure(string shortName, string dimString)
			{
				if (!isNewFile)
				{
					throw new System.NotSupportedException("Cant add structure to existing netcdf-3 files");
				}
				Structure.BuilderStruct vb = Structure.builder1().setName1(shortName).setParentGroupBuilder1(rootGroup);
				vb.setDimensionsByName(dimString);
				rootGroup.addVariable(vb);
				return vb;
			}

			public virtual long calcSize()
			{
				return calcSize(this.rootGroup);
			}

			// Note that we have enough info to try to estimate effects of compression, if its a Netcdf4 file.
			internal virtual long calcSize(Group.Builder group)
			{
				long totalSizeOfVars = 0;
				foreach (Variable.Builder var in this.rootGroup.vbuilders)
				{
					totalSizeOfVars += Dimensions.getSize(var.getDimensions()) * var.getElementSize();
				}
				foreach (Group.Builder nested in group.gbuilders)
				{
					totalSizeOfVars += calcSize(nested);
				}
				return totalSizeOfVars;
			}

			/// <summary>
			/// Once this is called, do not use the Builder again. </summary>
			public virtual NetcdfFormatWriter build()
			{
				return new NetcdfFormatWriter(this);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		private readonly string location;
		private readonly NetcdfFileFormat format;
		private readonly bool isNewFile;
		private readonly bool fill;
		private readonly int extraHeaderBytes;
		private readonly long preallocateSize;

		private readonly Group rootGroup;
		private readonly NetcdfFile ncout;
		private readonly IOServiceProviderWriter spiw;
		private readonly NetcdfSharp.ucar.unidata.io.RandomAccessFile existingRaf;

		private bool isClosed = false;

		private NetcdfFormatWriter(Builder builder)
		{
			this.location = builder.location;
			this.format = builder.format;
			this.isNewFile = builder.isNewFile;
			this.fill = builder.fill;
			this.extraHeaderBytes = builder.extraHeaderBytes;
			this.preallocateSize = builder.preallocateSize;

			if (format.isNetcdf4Format())
			{
				throw new System.NotSupportedException("netCDF-4 writing is not supported (no JNA/native netcdf-c binding)");
			}

			this.ncout = NetcdfFile.builder().setRootGroup(builder.rootGroup).setLocation(builder.location).build();
			this.rootGroup = this.ncout.RootGroup;

			if (!isNewFile)
			{
				existingRaf = new NetcdfSharp.ucar.unidata.io.RandomAccessFile(location, "rw");
				NetcdfFileFormat existingVersion = NetcdfFileFormat.findNetcdfFormatType(existingRaf);
				if (format != null && format != existingVersion)
				{
					existingRaf.close();
					throw new ArgumentException("Existing file at location" + location + " (" + existingVersion + ") does not match requested version " + format);
				}
			}
			else
			{
				existingRaf = null;
			}

			spiw = new N3iospWriter(new N3iospNew());

			// If anything fails, make sure that resources are closed.
			try
			{
				if (!isNewFile)
				{
					spiw.openForWriting(existingRaf, this.ncout, null);
				}
				else
				{
					spiw.create(location, this.ncout, extraHeaderBytes, preallocateSize, testIfLargeFile());
				}
				spiw.Fill = fill;
			}
			catch
			{
				spiw.close();
				throw;
			}
		}

		// TODO should not be used to read data, close and reopen
		public virtual NetcdfFile OutputFile
		{
			get
			{
				return this.ncout;
			}
		}

		public virtual NetcdfFileFormat Format
		{
			get
			{
				return format;
			}
		}

		public virtual Variable findVariable(string fullNameEscaped)
		{
			return this.ncout.findVariable(fullNameEscaped);
		}

		public virtual Dimension findDimension(string dimName)
		{
			return this.ncout.RootGroup.findDimension(dimName);
		}

		public virtual NcAttribute findGlobalAttribute(string attName)
		{
			return this.ncout.RootGroup.findAttribute(attName);
		}

		private bool testIfLargeFile()
		{
			if (format == NetcdfFileFormat.NETCDF3_64BIT_OFFSET)
			{
				return true;
			}

			if (format == NetcdfFileFormat.NETCDF3)
			{
				long totalSizeOfVars = calcSize(ncout.RootGroup);
				long maxSize = int.MaxValue;
				if (totalSizeOfVars > maxSize)
				{
					log.debug("Request size = {} Mbytes", totalSizeOfVars / 1000 / 1000);
					return true;
				}
			}
			return false;
		}

		public virtual long calcSize()
		{
			return calcSize(ncout.RootGroup);
		}

		// Note that we have enough info to try to estimate effects of compression, if its a Netcdf4 file.
		private long calcSize(Group group)
		{
			long totalSizeOfVars = 0;
			foreach (Variable var in group.Variables)
			{
				totalSizeOfVars += Dimensions.getSize(var.Dimensions) * var.ElementSize;
			}
			foreach (Group nested in group.Groups)
			{
				totalSizeOfVars += calcSize(nested);
			}
			return totalSizeOfVars;
		}

		////////////////////////////////////////////
		//// use these calls to write data to the file

		/// <summary>
		/// Write data to the given variable, origin assumed to be 0.
		/// </summary>
		/// <param name="v"> variable to write to </param>
		/// <param name="values"> write this array; must be same type and rank as Variable </param>
		/// <exception cref="IOException"> if I/O error </exception>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if values Array has illegal shape </exception>
		public virtual void write(Variable v, NcArray values)
		{
			write(v, new int[values.Rank], values);
		}

		/// <summary>
		/// Write data to the named variable, origin assumed to be 0. </summary>
		public virtual void write(string varName, NcArray values)
		{
			Variable v = findVariable(varName);
			Preconditions.checkNotNull(v);
			write(v, values);
		}

		/// <summary>
		/// Write data to the given variable.
		/// </summary>
		/// <param name="v"> variable to write to </param>
		/// <param name="origin"> offset within the variable to start writing. </param>
		/// <param name="values"> write this array; must be same type and rank as Variable </param>
		/// <exception cref="IOException"> if I/O error </exception>
		/// <exception cref="InvalidRangeException"> if values Array has illegal shape </exception>
		public virtual void write(Variable v, int[] origin, NcArray values)
		{
			spiw.writeData(v, new Section(origin, values.Shape), values);
		}

		/// <summary>
		/// Write data to the named variable.
		/// </summary>
		/// <param name="varName"> name of variable to write to </param>
		/// <param name="origin"> offset within the variable to start writing. </param>
		/// <param name="values"> write this array; must be same type and rank as Variable </param>
		/// <exception cref="IOException"> if I/O error </exception>
		/// <exception cref="InvalidRangeException"> if values Array has illegal shape </exception>
		public virtual void write(string varName, int[] origin, NcArray values)
		{
			Variable v = findVariable(varName);
			Preconditions.checkNotNull(v);
			write(v, origin, values);
		}

		/// <summary>
		/// Write String data to a CHAR variable, origin assumed to be 0.
		/// </summary>
		/// <param name="v"> variable to write to </param>
		/// <param name="values"> write this array; must be ArrayObject of String </param>
		/// <exception cref="IOException"> if I/O error </exception>
		/// <exception cref="InvalidRangeException"> if values Array has illegal shape </exception>
		public virtual void writeStringDataToChar(Variable v, NcArray values)
		{
			writeStringDataToChar(v, new int[values.Rank], values);
		}

		/// <summary>
		/// Write String data to a CHAR variable.
		/// </summary>
		/// <param name="v"> variable to write to </param>
		/// <param name="origin"> offset to start writing, ignore the strlen dimension. </param>
		/// <param name="values"> write this array; must be ArrayObject of String </param>
		/// <exception cref="IOException"> if I/O error </exception>
		/// <exception cref="InvalidRangeException"> if values Array has illegal shape </exception>
		public virtual void writeStringDataToChar(Variable v, int[] origin, NcArray values)
		{
			if (values.ElementType != typeof(string))
			{
				throw new ArgumentException("values must be an ArrayObject of String ");
			}

			if (v.DataType != DataType.CHAR)
			{
				throw new ArgumentException("variable " + v.FullName + " is not type CHAR");
			}

			int rank = v.Rank;
			int strlen = v.getShape(rank - 1);

			// turn it into an ArrayChar
			ArrayChar cvalues = ArrayChar.makeFromStringArray((ArrayObject)values, strlen);

			int[] corigin = new int[rank];
			System.Array.Copy(origin, 0, corigin, 0, rank - 1);

			write(v, corigin, cvalues);
		}

		public virtual int appendStructureData(Structure s, StructureData sdata)
		{
			return spiw.appendStructureData(s, sdata);
		}

		/// <summary>
		/// Update the value of an existing attribute. Attribute is found by name, which must match exactly.
		/// You cannot make an attribute longer, or change the number of values.
		/// For strings: truncate if longer, zero fill if shorter. Strings are padded to 4 byte boundaries, ok to use padding
		/// if it exists.
		/// For numerics: must have same number of values.
		/// This is really a netcdf-3 writing only. netcdf-4 attributes can be changed without rewriting.
		/// </summary>
		/// <param name="v2"> variable, or null for global attribute </param>
		/// <param name="att"> replace with this value </param>
		/// <exception cref="IOException"> if I/O error </exception>
		public virtual void updateAttribute(NetcdfSharp.ucar.nc2.Variable v2, NcAttribute att)
		{
			spiw.updateAttribute(v2, att);
		}

		/// <summary>
		/// Flush anything written to disk. </summary>
		public virtual void flush()
		{
			spiw.flush();
		}

		/// <summary>
		/// close the file. </summary>
		public virtual void close()
		{
			if (!isClosed)
			{
				flush();
				spiw.close();
				isClosed = true;
			}
		}

		/// <summary>
		/// Abort writing to this file. The file is closed. </summary>
		public virtual void abort()
		{
			if (!isClosed)
			{
				spiw.close();
				isClosed = true;
			}
		}

		public void Dispose()
		{
			close();
		}
	}
}
