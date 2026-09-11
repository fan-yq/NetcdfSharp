// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetcdfSharp.java.util;

/*
 * Copyright (c) 1998-2020 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3
{
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using Dimension = NetcdfSharp.ucar.nc2.Dimension;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// Class to write a netcdf3 header. </summary>
	internal class N3headerWriter : N3headerNew
	{
		private const long MAX_UNSIGNED_INT = 0x00000000ffffffffL;

		private NetcdfFile ncfile;
		private ImmutableList<Variable> uvars; // vars that have the unlimited dimension
		private long globalAttsPos; // global attributes start here - used for update

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="n3iospNew"> the iosp </param>
		/// <param name="raf"> write to this file </param>
		/// <param name="ncfile"> the header of this NetcdfFile </param>
		internal N3headerWriter(N3iospNew n3iospNew, RandomAccessFile raf, NetcdfFile ncfile) : base(n3iospNew)
		{
			this.raf = raf;
			this.ncfile = ncfile;
		}

		/// <summary>
		/// Write the header out, based on ncfile structures.
		/// </summary>
		/// <param name="extra"> if > 0, pad header with extra bytes </param>
		/// <param name="largeFile"> if large file format </param>
		/// <param name="fout"> debugging output sent to here </param>
		/// <exception cref="IOException"> on write error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void create(int extra, boolean largeFile, NetcdfSharp.java.util.Formatter fout) throws NetcdfSharp.java.io.IOException
		internal virtual void create(int extra, bool largeFile, Formatter fout)
		{
			writeHeader(extra, largeFile, false, fout);
		}

		/// <summary>
		/// Sneaky way to make the header bigger, if there is room for it
		/// </summary>
		/// <param name="largeFile"> is large file format </param>
		/// <param name="fout"> put debug messages here, mnay be null </param>
		/// <returns> true if it worked </returns>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: boolean rewriteHeader(boolean largeFile, NetcdfSharp.java.util.Formatter fout) throws NetcdfSharp.java.io.IOException
		internal virtual bool rewriteHeader(bool largeFile, Formatter fout)
		{
			int want = sizeHeader(largeFile);
			if (want > dataStart)
			{
				return false;
			}

			writeHeader(0, largeFile, true, fout);
			return true;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void writeHeader(int extra, boolean largeFile, boolean keepDataStart, NetcdfSharp.java.util.Formatter fout) throws NetcdfSharp.java.io.IOException
		internal virtual void writeHeader(int extra, bool largeFile, bool keepDataStart, Formatter fout)
		{
			this.useLongOffset = largeFile;
			this.nonRecordDataSize = 0; // length of non-record data
			recsize = 0; // length of single record
			recStart = long.MaxValue; // where the record data starts

			// magic number
			raf.seek(0);
			raf.write(largeFile ? N3headerNew.MAGIC_LONG : N3headerNew.MAGIC);

			// numrecs
			raf.writeInt(0);

			// dims
			IList<Dimension> dims = ncfile.RootGroup.Dimensions;
			int numdims = dims.Count;
			if (numdims == 0)
			{
				raf.writeInt(0);
				raf.writeInt(0);
			}
			else
			{
				raf.writeInt(N3headerNew.MAGIC_DIM);
				raf.writeInt(numdims);
			}
			for (int i = 0; i < numdims; i++)
			{
				Dimension dim = dims[i];
				if (fout != null)
				{
					fout.format("  dim %d pos %d%n", i, raf.FilePointer);
				}
				writeString(dim.ShortName);
				raf.writeInt(dim.Unlimited ? 0 : dim.Length);
				if (dim.Unlimited)
				{
					udim = dim;
				}
			}

			// global attributes
			globalAttsPos = raf.FilePointer; // position where global attributes start
			writeAtts(ncfile.RootGroup.attributes(), fout);

			// variables
			IList<Variable> vars = ncfile.Variables;

			// Track record variables.
			ImmutableList<Variable>.Builder uvarb = ImmutableList<Variable>.builder();
			foreach (Variable curVar in vars)
			{
				if (curVar.Unlimited)
				{
					uvarb.add(curVar);
				}
			}
			uvars = uvarb.build();
			writeVars(vars, largeFile, fout);

			// now calculate where things go
			if (!keepDataStart)
			{
				dataStart = raf.FilePointer;
				if (extra > 0)
				{
					dataStart += extra;
				}
			}
			long pos = dataStart;

			// non-record variable starting positions
			foreach (Variable var in vars)
			{
				N3headerNew.Vinfo vinfo = (N3headerNew.Vinfo)var.SPobject;
				if (!vinfo.isRecord)
				{
					raf.seek(vinfo.begin);

					if (largeFile)
					{
						raf.writeLong(pos);
					}
					else
					{
						if (pos > int.MaxValue)
						{
							throw new ArgumentException("Variable starting pos=" + pos + " may not exceed " + int.MaxValue);
						}
						raf.writeInt((int)pos);
					}

					vinfo.begin = pos;
					if (fout != null)
					{
						fout.format("  %s begin at = %d end= %d%n", var.FullName, vinfo.begin, (vinfo.begin + vinfo.vsize));
					}
					pos += vinfo.vsize;

					// track how big each record is
					nonRecordDataSize = Math.Max(nonRecordDataSize, vinfo.begin + vinfo.vsize);
				}
			}

			recStart = pos; // record variables start here

			// record variable starting positions
			foreach (Variable var in vars)
			{
				N3headerNew.Vinfo vinfo = (N3headerNew.Vinfo)var.SPobject;
				if (vinfo.isRecord)
				{
					raf.seek(vinfo.begin);

					if (largeFile)
					{
						raf.writeLong(pos);
					}
					else
					{
						raf.writeInt((int)pos);
					}

					vinfo.begin = pos;
					if (fout != null)
					{
						fout.format(" %s record begin at = %d%n", var.FullName, dataStart);
					}
					pos += vinfo.vsize;

					// track how big each record is
					recsize += vinfo.vsize;
					recStart = Math.Min(recStart, vinfo.begin);
				}
			}

			if (nonRecordDataSize > 0) // if there are non-record variables
			{
				nonRecordDataSize -= dataStart;
			}
			if (uvars.isEmpty()) // if there are no record variables
			{
				recStart = 0;
			}
		}

		// calculate the size writing a header would take
		internal virtual int sizeHeader(bool largeFile)
		{
			int size = 4; // magic number
			size += 4; // numrecs

			// dims
			size += 8; // magic, ndims
			foreach (Dimension dim in ncfile.RootGroup.Dimensions)
			{
				size += sizeString(dim.ShortName) + 4; // name, len
			}

			// global attributes
			size += sizeAtts(ncfile.GlobalAttributes);

			// variables
			size += 8; // magic, nvars
			foreach (Variable var in ncfile.Variables)
			{
				size += sizeString(var.ShortName);

				// dimensions
				size += 4; // ndims
				size += 4 * var.Dimensions.Count; // dim id

				// variable attributes
				size += sizeAtts(var.attributes());

				size += 8; // data type, variable size
				size += (largeFile) ? 8 : 4;
			}

			return size;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void writeAtts(Iterable<NetcdfSharp.ucar.nc2.Attribute> atts, NetcdfSharp.java.util.Formatter fout) throws NetcdfSharp.java.io.IOException
		private void writeAtts(IEnumerable<NcAttribute> atts, Formatter fout)
		{

			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final NetcdfSharp.java.util.List<NetcdfSharp.ucar.nc2.Attribute> attributesToWrite = NetcdfSharp.java.util.stream.StreamSupport.stream(atts.spliterator(), false).filter(att -> !NetcdfSharp.ucar.nc2.constants.CDM.isspecial(att)).collect(NetcdfSharp.java.util.stream.Collectors.toList());
			IList<NcAttribute> attributesToWrite = atts.Where(att => !CDM.isspecial(att)).ToList();

			int n = Iterables.size(attributesToWrite);
			if (n == 0)
			{
				raf.writeInt(0);
				raf.writeInt(0);
			}
			else
			{
				raf.writeInt(MAGIC_ATT);
				raf.writeInt(n);
			}

			int count = 0;
			foreach (NcAttribute att in attributesToWrite)
			{
				if (fout != null)
				{
					fout.format("***att %d pos= %d%n", count, raf.FilePointer);
				}

				writeString(att.ShortName);
				int type = getType(att.DataType);
				raf.writeInt(type);

				if (type == 2)
				{
					writeStringValues(att);
				}
				else
				{
					int nelems = att.Length;
					raf.writeInt(nelems);
					int nbytes = 0;
					for (int j = 0; j < nelems; j++)
					{
						nbytes += writeAttributeValue(att.getNumericValue(j));
					}
					pad(nbytes, (byte)0);
					if (fout != null)
					{
						fout.format(" end write val pos= %d%n", raf.FilePointer);
					}
				}
				if (fout != null)
				{
					fout.format("  %s%n", att);
				}
			}
		}

		private int sizeAtts(IEnumerable<NcAttribute> atts)
		{
			int size = 8; // magic, natts

			foreach (NcAttribute att in atts)
			{
				size += sizeString(att.ShortName);
				size += 4; // type

				int type = getType(att.DataType);
				if (type == 2)
				{
					size += sizeStringValues(att);
				}
				else
				{
					size += 4; // nelems
					int nelems = att.Length;
					int nbytes = 0;
					for (int j = 0; j < nelems; j++)
					{
						nbytes += sizeAttributeValue(att.getNumericValue(j));
					}
					size += nbytes;
					size += padding(nbytes);
				}
			}
			return size;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void writeStringValues(NetcdfSharp.ucar.nc2.Attribute att) throws NetcdfSharp.java.io.IOException
		private void writeStringValues(NcAttribute att)
		{
			int n = att.Length;
			if (n == 1)
			{
				writeString(att.StringValue);
			}
			else
			{
				StringBuilder values = new StringBuilder();
				for (int i = 0; i < n; i++)
				{
					values.Append(att.getStringValue(i));
				}
				writeString(values.ToString());
			}
		}

		private int sizeStringValues(NcAttribute att)
		{
			int size = 0;
			int n = att.Length;
			if (n == 1)
			{
				size += sizeString(att.StringValue);
			}
			else
			{
				StringBuilder values = new StringBuilder();
				for (int i = 0; i < n; i++)
				{
					values.Append(att.getStringValue(i));
				}
				size += sizeString(values.ToString());
			}

			return size;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private int writeAttributeValue(Number numValue) throws NetcdfSharp.java.io.IOException
		private int writeAttributeValue(object numValue)
		{
			if (numValue is byte?)
			{
				raf.write(Convert.ToByte(numValue));
				return 1;

			}
			else if (numValue is short?)
			{
				raf.writeShort(Convert.ToInt16(numValue));
				return 2;

			}
			else if (numValue is int?)
			{
				raf.writeInt(Convert.ToInt32(numValue));
				return 4;

			}
			else if (numValue is float?)
			{
				raf.writeFloat(Convert.ToSingle(numValue));
				return 4;

			}
			else if (numValue is double?)
			{
				raf.writeDouble(Convert.ToDouble(numValue));
				return 8;
			}

			// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new System.InvalidOperationException("unknown attribute type == " + numValue.GetType().FullName);
		}

		private int sizeAttributeValue(object numValue)
		{

			if (numValue is byte?)
			{
				return 1;

			}
			else if (numValue is short?)
			{
				return 2;

			}
			else if (numValue is int?)
			{
				return 4;

			}
			else if (numValue is float?)
			{
				return 4;

			}
			else if (numValue is double?)
			{
				return 8;
			}

			// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new System.InvalidOperationException("unknown attribute type == " + numValue.GetType().FullName);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void writeVars(NetcdfSharp.java.util.List<NetcdfSharp.ucar.nc2.Variable> vars, boolean largeFile, NetcdfSharp.java.util.Formatter fout) throws NetcdfSharp.java.io.IOException
		private void writeVars(IList<Variable> vars, bool largeFile, Formatter fout)
		{
			int n = vars.Count;
			if (n == 0)
			{
				raf.writeInt(0);
				raf.writeInt(0);
			}
			else
			{
				raf.writeInt(MAGIC_VAR);
				raf.writeInt(n);
			}

			foreach (Variable var in vars)
			{
				writeString(var.ShortName);

				// dimensions
				long vsize = var.DataType.getSize(); // works for all netcdf-3 data types
				IList<Dimension> dims = var.Dimensions;
				raf.writeInt(dims.Count);
				foreach (Dimension dim in dims)
				{
					int dimIndex = findDimensionIndex(ncfile, dim);
					raf.writeInt(dimIndex);

					if (!dim.Unlimited)
					{
						vsize *= dim.Length;
					}
				}
				long unpaddedVsize = vsize;
				vsize += padding(vsize);

				// variable attributes
				long varAttsPos = raf.FilePointer;
				writeAtts(var.attributes(), fout);

				// data type, variable size, beginning file position
				DataType dtype = var.DataType;
				int type = getType(dtype);
				raf.writeInt(type);

				int vsizeWrite = (vsize < MAX_UNSIGNED_INT) ? (int)vsize : -1;
				raf.writeInt(vsizeWrite);
				long pos = raf.FilePointer;
				if (largeFile)
				{
					raf.writeLong(0); // come back to this later
				}
				else
				{
					raf.writeInt(0); // come back to this later
				}

				// From nc3 file format specification
				// (https://www.unidata.ucar.edu/software/netcdf/docs/netcdf.html#NetCDF-Classic-Format):
				// Note on padding: In the special case of only a single record variable of character,
				// byte, or short type, no padding is used between data values.
				// 2/15/2011: we will continue to write the (incorrect) padded vsize into the header, but we will use the unpadded
				// size to read/write
				if (uvars.size() == 1 && uvars.get(0) == var)
				{
					if ((dtype == DataType.CHAR) || (dtype == DataType.BYTE) || (dtype == DataType.SHORT))
					{
						vsize = unpaddedVsize;
					}
				}
				var.SPobject = new N3headerNew.Vinfo(var.ShortName, vsize, pos, var.Unlimited, varAttsPos);
			}
		}

		// write a string then pad to 4 byte boundary
		private void writeString(string s)
		{
			byte[] b = Encoding.UTF8.GetBytes(s); // all strings are encoded in UTF-8 Unicode.
			raf.writeInt(b.Length);
			raf.write(b);
			pad(b.Length, (byte)0);
		}

		private int sizeString(string s)
		{
			int size = s.Length + 4;
			return size + padding(s.Length);
		}

		private int findDimensionIndex(NetcdfFile ncfile, Dimension wantDim)
		{
			IList<Dimension> dims = ncfile.RootGroup.Dimensions;
			for (int i = 0; i < dims.Count; i++)
			{
				Dimension dim = dims[i];
				if (dim.Equals(wantDim))
				{
					return i;
				}
			}
			throw new System.InvalidOperationException("unknown Dimension == " + wantDim);
		}

		// pad to a 4 byte boundary
		private void pad(int nbytes, byte fill)
		{
			int pad = padding(nbytes);
			for (int i = 0; i < pad; i++)
			{
				raf.write(fill);
			}
		}

		internal virtual void writeNumrecs()
		{
			// set number of records in the header
			raf.seek(4);
			raf.writeInt(numrecs);
		}

		internal virtual void initFromExisting(N3iospNew existingIosp)
		{
			N3headerNew existingHeader = existingIosp.header;
			this.dataStart = existingHeader.dataStart;
			this.nonRecordDataSize = existingHeader.nonRecordDataSize;
			this.numrecs = existingHeader.numrecs;
			this.recsize = existingHeader.recsize;
			this.recStart = existingHeader.recStart;
			this.useLongOffset = existingHeader.useLongOffset;
			this.udim = existingHeader.udim;
		}

		internal virtual void setNumrecs(int n)
		{
			this.numrecs = n;
		}

		internal virtual int Numrecs
		{
			get
			{
				return numrecs;
			}
			set
			{
				setNumrecs(value);
			}
		}

		// TODO udim.setLength : need UnlimitedDimension extends Dimension?
		internal virtual bool synchNumrecs()
		{
			// check number of records in the header
			// gotta bypass the RAF buffer
			int n = raf.readIntUnbuffered(4);
			if (n == this.numrecs)
			{
				return false;
			}
			if (n < 0) // streaming
			{
				return false;
			}

			// update everything
			this.numrecs = n;

			// set it in the unlimited dimension
			udim.Length = this.numrecs;

			// set it in all of the record variables
			foreach (Variable uvar in uvars)
			{
				uvar.resetShape();
				uvar.invalidateCache();
			}

			return true;
		}

		///////////////////////////////////////////////////////////////////////////////
		// tricky - perhaps deprecate ?

		internal virtual void updateAttribute(Variable v2, NcAttribute att)
		{
			long pos;
			if (v2 == null)
			{
				pos = findAtt(globalAttsPos, att.ShortName);
			}
			else
			{
				N3headerNew.Vinfo vinfo = (N3headerNew.Vinfo)v2.SPobject;
				pos = findAtt(vinfo.attsPos, att.ShortName);
			}

			raf.seek(pos);
			int type = raf.readInt();
			DataType have = getDataType(type);
			DataType want = att.DataType;
			if (want == DataType.STRING)
			{
				want = DataType.CHAR;
			}
			if (want != have)
			{
				throw new ArgumentException("Update Attribute must have same type or original = " + have + " att = " + att);
			}

			if (type == 2)
			{ // String
				string s = att.StringValue;
				int org = raf.readInt();
				int size = org + padding(org); // ok to use the padding
				int max = Math.Min(size, s.Length); // cant make any longer than size
				if (max > org)
				{ // adjust if its using the padding, but not if its shorter
					raf.seek(pos + 4);
					raf.writeInt(max);
				}

				byte[] b = new byte[size];
				for (int i = 0; i < max; i++)
				{
					b[i] = (byte)s[i];
				}
				raf.write(b);

			}
			else
			{
				int nelems = raf.readInt();
				int max = Math.Min(nelems, att.Length); // cant make any longer
				for (int j = 0; j < max; j++)
				{
					writeAttributeValue(att.getNumericValue(j));
				}
			}
		}

		private long findAtt(long start_pos, string want)
		{
			raf.seek(start_pos + 4);

			int natts = raf.readInt();
			for (int i = 0; i < natts; i++)
			{
				string name = readString();
				if (name.Equals(want))
				{
					return raf.FilePointer;
				}

				int type = raf.readInt();

				if (type == 2)
				{
					readString();
				}
				else
				{
					int nelems = raf.readInt();
					DataType dtype = getDataType(type);
					int[] shape = new int[] { nelems };
					NcArray arr = NcArray.factory(dtype, shape);
					IndexIterator ii = arr.IndexIterator;
					int nbytes = 0;
					for (int j = 0; j < nelems; j++)
					{
						nbytes += readAttributeValue(dtype, ii);
					}
					skip(nbytes);
				}
			}

			throw new ArgumentException("no such attribute " + want);
		}

	}

}
