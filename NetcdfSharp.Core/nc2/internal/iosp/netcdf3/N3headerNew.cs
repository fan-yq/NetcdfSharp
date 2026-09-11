// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3
{
	using NetcdfSharp.ucar.ma2;
	using NetcdfSharp.ucar.nc2;
	using NetcdfFileFormat = NetcdfSharp.ucar.nc2.write.NetcdfFileFormat;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Netcdf version 3 header. Read-only version using Builders for immutablility.
	/// </summary>
	public class N3headerNew
	{
		private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(N3headerNew));

		internal static readonly byte[] MAGIC = new byte[] { 0x43, 0x44, 0x46, 0x01 };
		// 64-bit offset format : only affects the variable offset value
		internal static readonly byte[] MAGIC_LONG = new byte[] { 0x43, 0x44, 0x46, 0x02 };
		internal const int MAGIC_DIM = 10;
		internal const int MAGIC_VAR = 11;
		internal const int MAGIC_ATT = 12;

		public static bool disallowFileTruncation; // see NetcdfFile.setDebugFlags
		public static bool debugHeaderSize; // see NetcdfFile.setDebugFlags

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
		public static bool isValidFile(RandomAccessFile raf)
		{
			// fail fast on directory
			if (raf.Directory)
			{
				return false;
			}
			switch (NetcdfFileFormat.findNetcdfFormatType(raf).innerEnumValue)
			{
				case NetcdfFileFormat.InnerEnum.NETCDF3:
				case NetcdfFileFormat.InnerEnum.NETCDF3_64BIT_OFFSET:
					return true;
				case NetcdfFileFormat.InnerEnum.NETCDF3_64BIT_DATA:
					log.debug("NETCDF3_64BIT_DATA (CDF5) not supported");
					return false;
				default:
					return false;
			}
		}

		// variable info for reading/writing
		internal class Vinfo
		{
			internal string name;
			internal long vsize; // size of array in bytes. if isRecord, size per record.
			internal long begin; // offset of start of data from start of file
			internal bool isRecord; // is it a record variable?
			internal long attsPos; // attributes start here - used for update

			internal Vinfo(string name, long vsize, long begin, bool isRecord, long attsPos)
			{
				this.name = name;
				this.vsize = vsize;
				this.begin = begin;
				this.isRecord = isRecord;
				this.attsPos = attsPos;
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected internal RandomAccessFile raf;

		// N3iospNew needs access to these
		private bool isStreaming; // is streaming (numrecs = -1)
		internal int numrecs; // number of records written
		internal long recsize; // size of each record (padded)
		internal long recStart = int.MaxValue; // where the record data starts

		internal bool useLongOffset;
		private N3iospNew n3iospNew;
		internal long nonRecordDataSize; // size of non-record variables
		internal Dimension udim; // the unlimited dimension
		private IList<Vinfo> vars = new List<Vinfo>();
		internal long dataStart = long.MaxValue; // where the data starts

		private readonly Charset valueCharset;

		internal N3headerNew(N3iospNew n3iospNew)
		{
			this.n3iospNew = n3iospNew;
			this.valueCharset = n3iospNew.getValueCharset().orElse(Encoding.UTF8);
		}

		/// <summary>
		/// Return defined <seealso cref="Charset value charset"/> that
		/// will be used by reading HDF3 header.
		/// </summary>
		/// <returns> <seealso cref="Charset charset"/> </returns>
		private Charset ValueCharset
		{
			get
			{
				return valueCharset;
			}
		}

		/// <summary>
		/// Read the header and populate the ncfile
		/// </summary>
		/// <param name="raf"> read from this file </param>
		/// <param name="root"> the root Group builder to populate. </param>
		/// <param name="debugOut"> optional for debug message, may be null </param>
		/// <exception cref="IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void read(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Group.Builder root, Formatter debugOut) throws NetcdfSharp.java.io.IOException
		internal virtual void read(RandomAccessFile raf, Group.Builder root, Formatter debugOut)
		{
			this.raf = raf;

			long actualSize = raf.length();
			nonRecordDataSize = 0; // length of non-record data
			recsize = 0; // length of single record
			recStart = int.MaxValue; // where the record data starts

			// netcdf magic number
			long pos = 0;
			raf.order(RandomAccessFile.BIG_ENDIAN);
			raf.seek(pos);

			byte[] b = new byte[4];
			raf.readFully(b);
			if (!isMagicBytes(b))
			{
				throw new IOException("Not a netCDF file " + raf.Location);
			}
			if ((b[3] != 1) && (b[3] != 2))
			{
				throw new IOException("Not a netCDF file " + raf.Location);
			}
			useLongOffset = (b[3] == 2);

			// number of records
			numrecs = raf.readInt();
			if (debugOut != null)
			{
				debugOut.format("numrecs= %d%n", numrecs);
			}
			if (numrecs == -1)
			{
				isStreaming = true;
				numrecs = 0;
			}

			// dimensions
			int numdims = 0;
			int magic = raf.readInt();
			if (magic == 0)
			{
				raf.readInt(); // skip 32 bits
			}
			else
			{
				if (magic != MAGIC_DIM)
				{
					throw new IOException("Misformed netCDF file - dim magic number wrong " + raf.Location);
				}
				numdims = raf.readInt();
				if (debugOut != null)
				{
					debugOut.format("numdims= %d%n", numdims);
				}
			}

			// Must keep dimensions in strict order
			// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
			// ORIGINAL LINE: List<Variable.Builder<?>> uvars = new ArrayList<>();
			IList<Variable.Builder> uvars = new List<Variable.Builder>(); // vars that have the unlimited dimension
			List<Dimension> fileDimensions = new List<Dimension>();
			for (int i = 0; i < numdims; i++)
			{
				if (debugOut != null)
				{
					debugOut.format("  dim %d pos= %d%n", i, raf.FilePointer);
				}
				string name = readString();
				int len = raf.readInt();
				Dimension dim;
				if (len == 0)
				{
					dim = Dimension.builder().setName(name).setIsUnlimited(true).setLength(numrecs).build();
					udim = dim;
				}
				else
				{
					dim = new Dimension(name, len);
				}
				fileDimensions.Add(dim);
				root.addDimension(dim);
				if (debugOut != null)
				{
					debugOut.format(" added dimension %s%n", dim);
				}
			}

			// global attributes
			readAtts(root.AttributeContainer, debugOut);

			// variables
			int nvars = 0;
			magic = raf.readInt();
			if (magic == 0)
			{
				raf.readInt(); // skip 32 bits
			}
			else
			{
				if (magic != MAGIC_VAR)
				{
					throw new IOException("Misformed netCDF file  - var magic number wrong " + raf.Location);
				}
				nvars = raf.readInt();
				if (debugOut != null)
				{
					debugOut.format("numdims= %d%n", numdims);
				}
			}
			if (debugOut != null)
			{
				debugOut.format("num variables= %d%n", nvars);
			}

			// loop over variables
			for (int i = 0; i < nvars; i++)
			{
				string name = readString();
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> var = Variable.builder().setName(name);
				Variable.Builder var = Variable.builder().setName(name);

				// get element count in non-record dimensions
				long velems = 1;
				bool isRecord = false;
				int rank = raf.readInt();
				IList<Dimension> dims = new List<Dimension>();
				for (int j = 0; j < rank; j++)
				{
					int dimIndex = raf.readInt();
					Dimension dim = fileDimensions[dimIndex];
					if (dim.Unlimited)
					{
						isRecord = true;
						uvars.Add(var); // track record variables
					}
					else
					{
						velems *= dim.Length;
					}

					dims.Add(dim);
				}
				var.addDimensions(dims);

				if (debugOut != null)
				{
					debugOut.format("---name=<%s> dims = [", name);
					foreach (Dimension dim in dims)
					{
						debugOut.format("%s ", dim.ShortName);
					}
					debugOut.format("]%n");
				}

				// variable attributes
				long varAttsPos = raf.FilePointer;
				readAtts(var.getAttributeContainer(), debugOut);

				// data type
				int type = raf.readInt();
				DataType dataType = getDataType(type);
				var.setDataType(dataType);

				// size and beginning data position in file
				long vsize = raf.readInt();
				long begin = useLongOffset ? raf.readLong() : (long)raf.readInt();

				if (debugOut != null)
				{
					debugOut.format(" name= %s type=%d vsize=%s velems=%d begin= %d isRecord=%s attsPos=%d%n", name, type, vsize, velems, begin, isRecord, varAttsPos);
					long calcVsize = (velems + padding(velems)) * dataType.getSize();
					if (vsize != calcVsize)
					{
						debugOut.format(" *** readVsize %d != calcVsize %d%n", vsize, calcVsize);
					}
				}
				if (vsize < 0)
				{
					vsize = (velems + padding(velems)) * dataType.getSize();
				}

				Vinfo vinfo = new Vinfo(name, vsize, begin, isRecord, varAttsPos);
				vars.Add(vinfo);
				var.setSPobject(vinfo);

				// track how big each record is
				if (isRecord)
				{
					recsize += vsize;
					recStart = Math.Min(recStart, begin);
				}
				else
				{
					nonRecordDataSize = Math.Max(nonRecordDataSize, begin + vsize);
				}

				dataStart = Math.Min(dataStart, begin);
				root.addVariable(var);
			}

			pos = raf.FilePointer;

			// if nvars == 0
			if (dataStart == long.MaxValue)
			{
				dataStart = pos;
			}

			if (nonRecordDataSize > 0) // if there are non-record variables
			{
				nonRecordDataSize -= dataStart;
			}
			if (uvars.Count == 0) // if there are no record variables
			{
				recStart = 0;
			}

			// check for streaming file - numrecs must be calculated
			// Example: TestDir.cdmUnitTestDir + "ft/station/madis2.nc"
			if (isStreaming)
			{
				long recordSpace = actualSize - recStart;
				numrecs = recsize == 0 ? 0 : (int)(recordSpace / recsize);

				// set size of the unlimited dimension, reset the record variables
				if (udim != null)
				{
					udim = udim.toBuilder().setLength(numrecs).build();
					root.replaceDimension(udim);
					foreach (var v in uvars)
					{
						v.replaceDimensionByName(udim);
					}
				}
			}

			// Check if file affected by bug CDM-52 (netCDF-Java library used incorrect padding when
			// the file contained only one record variable and it was of type byte, char, or short).
			// Example TestDir.cdmLocalTestDataDir + "byteArrayRecordVarPaddingTest-bad.nc"
			if (uvars.Count == 1)
			{
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> uvar = uvars.get(0);
				Variable.Builder uvar = uvars[0];
				DataType dtype = uvar.dataType;
				if ((dtype == DataType.CHAR) || (dtype == DataType.BYTE) || (dtype == DataType.SHORT))
				{
					long vsize = dtype.getSize(); // works for all netcdf-3 data types
					IList<Dimension> dims = uvar.getDimensions();
					foreach (Dimension curDim in dims)
					{
						if (!curDim.Unlimited)
						{
							vsize *= curDim.Length;
						}
					}
					Vinfo vinfo = (Vinfo)uvar.spiObject;
					if (vsize != vinfo.vsize)
					{
						log.info("Misformed netCDF file - file written with incorrect padding for record variable (CDM-52): fvsize=" + vinfo.vsize + "!= calc size =" + vsize);
						recsize = vsize;
						vinfo.vsize = vsize;
					}
				}
			}

			if (debugHeaderSize)
			{
				Console.WriteLine("  filePointer = " + pos + " dataStart=" + dataStart);
				Console.WriteLine("  recStart = " + recStart + " dataStart+nonRecordDataSize =" + (dataStart + nonRecordDataSize));
				Console.WriteLine("  nonRecordDataSize size= " + nonRecordDataSize);
				Console.WriteLine("  recsize= " + recsize);
				Console.WriteLine("  numrecs= " + numrecs);
				Console.WriteLine("  actualSize= " + actualSize);
			}

			// check for truncated files
			// theres a "wart" that allows a file to be up to 3 bytes smaller than you expect.
			long calcSize = dataStart + nonRecordDataSize + recsize * numrecs;
			if (calcSize > actualSize + 3)
			{
				if (disallowFileTruncation)
				{
					throw new IOException("File is truncated, calculated size= " + calcSize + " actual = " + actualSize);
				}
				else
				{
					// log.info("File is truncated calculated size= "+calcSize+" actual = "+actualSize);
					raf.setExtendMode();
				}
			}

			// add a record structure if asked to do so
			if (n3iospNew.useRecordStructure && uvars.Count > 0)
			{
				makeRecordStructure(root, uvars);
			}
		}

		/// <summary>
		/// Check if the given bytes correspond to
		/// <seealso cref="MAGIC magic bytes"/> of the header.
		/// </summary>
		/// <param name="bytes"> given bytes. </param>
		/// <returns> <code>true</code> if the given bytes correspond to
		///         <seealso cref="MAGIC magic bytes"/> of the header. Otherwise <code>false</code>. </returns>
		private bool isMagicBytes(byte[] bytes)
		{
			for (int i = 0; i < 3; i++)
			{
				if (bytes[i] != MAGIC[i])
				{
					return false;
				}
			}
			return true;
		}

		internal virtual long calcFileSize()
		{
			if (udim != null)
			{
				return recStart + recsize * numrecs;
			}
			else
			{
				return dataStart + nonRecordDataSize;
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void showDetail(Formatter out) throws NetcdfSharp.java.io.IOException
		internal virtual void showDetail(Formatter @out)
		{
			long actual = raf.length();
			@out.format("  raf length= %s %n", actual);
			@out.format("  isStreaming= %s %n", isStreaming);
			@out.format("  useLongOffset= %s %n", useLongOffset);
			@out.format("  dataStart= %d%n", dataStart);
			@out.format("  nonRecordData size= %d %n", nonRecordDataSize);
			@out.format("  unlimited dimension = %s %n", udim);

			if (udim != null)
			{
				@out.format("  record Data starts = %d %n", recStart);
				@out.format("  recsize = %d %n", recsize);
				@out.format("  numrecs = %d %n", numrecs);
			}

			long calcSize = calcFileSize();
			@out.format("  computedSize = %d %n", calcSize);
			if (actual < calcSize)
			{
				@out.format("  TRUNCATED!! actual size = %d (%d bytes) %n", actual, (calcSize - actual));
			}
			else if (actual != calcSize)
			{
				@out.format(" actual size larger = %d (%d byte extra) %n", actual, (actual - calcSize));
			}

			@out.format("%n  %20s____start_____size__unlim%n", "name");
			foreach (Vinfo vinfo in this.vars)
			{
				@out.format("  %20s %8d %8d  %s %n", vinfo.name, vinfo.begin, vinfo.vsize, vinfo.isRecord);
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private int readAtts(AttributeContainerMutable atts, Formatter fout) throws NetcdfSharp.java.io.IOException
		private int readAtts(AttributeContainerMutable atts, Formatter fout)
		{
			int natts = 0;
			int magic = raf.readInt();
			if (magic == 0)
			{
				raf.readInt(); // skip 32 bits
			}
			else
			{
				if (magic != MAGIC_ATT)
				{
					throw new IOException("Misformed netCDF file  - att magic number wrong");
				}
				natts = raf.readInt();
			}
			if (fout != null)
			{
				fout.format(" num atts= %d%n", natts);
			}

			for (int i = 0; i < natts; i++)
			{
				if (fout != null)
				{
					fout.format("***att %d pos= %d%n", i, raf.FilePointer);
				}
				string name = readString();
				int type = raf.readInt();
				NcAttribute att;

				if (type == 2)
				{
					if (fout != null)
					{
						fout.format(" begin read String val pos= %d%n", raf.FilePointer);
					}
					string val = readString(ValueCharset);
					if (string.ReferenceEquals(val, null))
					{
						val = "";
					}
					if (fout != null)
					{
						fout.format(" end read String val pos= %d%n", raf.FilePointer);
					}
					att = new NcAttribute(name, val);

				}
				else
				{
					if (fout != null)
					{
						fout.format(" begin read val pos= %d%n", raf.FilePointer);
					}
					int nelems = raf.readInt();

					DataType dtype = getDataType(type);
					NcAttribute.Builder builder = NcAttribute.builder(name).setDataType(dtype);

					if (nelems > 0)
					{
						int[] shape = new int[] { nelems };
						NcArray arr = NcArray.factory(dtype, shape);
						IndexIterator ii = arr.IndexIterator;
						int nbytes = 0;
						for (int j = 0; j < nelems; j++)
						{
							nbytes += readAttributeValue(dtype, ii);
						}

						builder.setValues(arr); // no validation !!
						skip(nbytes);
					}
					att = builder.build();

					if (fout != null)
					{
						fout.format(" end read val pos= %d%n", raf.FilePointer);
					}
				}

				atts.addAttribute(att);
				if (fout != null)
				{
					fout.format("  %s%n", att);
				}
			}

			return natts;
		}

		internal int readAttributeValue(DataType type, IndexIterator ii)
		{
			if (type == DataType.BYTE)
			{
				byte b = (byte)raf.read();
				// if (debug) out.println(" byte val = "+b);
				ii.ByteNext = b;
				return 1;

			}
			else if (type == DataType.CHAR)
			{
				char c = (char)raf.read();
				// if (debug) out.println(" char val = "+c);
				ii.CharNext = c;
				return 1;

			}
			else if (type == DataType.SHORT)
			{
				short s = raf.readShort();
				// if (debug) out.println(" short val = "+s);
				ii.ShortNext = s;
				return 2;

			}
			else if (type == DataType.INT)
			{
				int i = raf.readInt();
				// if (debug) out.println(" int val = "+i);
				ii.IntNext = i;
				return 4;

			}
			else if (type == DataType.FLOAT)
			{
				float f = raf.readFloat();
				// if (debug) out.println(" float val = "+f);
				ii.FloatNext = f;
				return 4;

			}
			else if (type == DataType.DOUBLE)
			{
				double d = raf.readDouble();
				// if (debug) out.println(" double val = "+d);
				ii.DoubleNext = d;
				return 8;
			}
			return 0;
		}

		// read a string = (nelems, byte array), then skip to 4 byte boundary
		internal string readString()
		{
			return readString(Encoding.UTF8);
		}

		private string readString(Charset charset)
		{
			int nelems = raf.readInt();
			byte[] b = new byte[nelems];
			raf.readFully(b);
			skip(nelems); // pad to 4 byte boundary

			if (nelems == 0)
			{
				return null;
			}

			// null terminates
			int count = 0;
			while (count < nelems)
			{
				if (b[count] == 0)
				{
					break;
				}
				count++;
			}

			return StringHelper.NewString(b, 0, count, charset); // all strings are considered to be UTF-8 unicode.
		}

		// skip to a 4 byte boundary in the file
		internal void skip(int nbytes)
		{
			int pad = padding(nbytes);
			if (pad > 0)
			{
				raf.seek(raf.FilePointer + pad);
			}
		}

		// find number of bytes needed to pad to a 4 byte boundary
		internal static int padding(int nbytes)
		{
			int pad = nbytes % 4;
			if (pad != 0)
			{
				pad = 4 - pad;
			}
			return pad;
		}

		// find number of bytes needed to pad to a 4 byte boundary
		internal static int padding(long nbytes)
		{
			int pad = (int)(nbytes % 4);
			if (pad != 0)
			{
				pad = 4 - pad;
			}
			return pad;
		}

		internal static DataType getDataType(int type)
		{
			switch (type)
			{
				case 1:
					return DataType.BYTE;
				case 2:
					return DataType.CHAR;
				case 3:
					return DataType.SHORT;
				case 4:
					return DataType.INT;
				case 5:
					return DataType.FLOAT;
				case 6:
					return DataType.DOUBLE;
				default:
					throw new ArgumentException("unknown type == " + type);
			}
		}

		internal static int getType(DataType dt)
		{
			if (dt == DataType.BYTE)
			{
				return 1;
			}
			else if ((dt == DataType.CHAR) || (dt == DataType.STRING))
			{
				return 2;
			}
			else if (dt == DataType.SHORT)
			{
				return 3;
			}
			else if (dt == DataType.INT)
			{
				return 4;
			}
			else if (dt == DataType.FLOAT)
			{
				return 5;
			}
			else if (dt == DataType.DOUBLE)
			{
				return 6;
			}

			throw new ArgumentException("unknown DataType == " + dt);
		}

		private bool makeRecordStructure(Group.Builder root, IList<Variable.Builder> uvars)
		{
			var recordStructure = Structure.builder1().setName1("record");
			recordStructure.setParentGroupBuilder1(root).setDimensionsByName(udim.ShortName);
			foreach (Variable.Builder v in uvars)
			{
				Variable.Builder memberV = v.makeSliceBuilder(0, 0); // set unlimited dimension to 0
				recordStructure.addMemberVariable(memberV);
			}
			root.addVariable(recordStructure);
			uvars.Add(recordStructure);
			return true;
		}

	}

}
