// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2020 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3
{
	using NetcdfSharp.ucar.nc2;
	using ArrayChar = NetcdfSharp.ucar.ma2.ArrayChar;
	using ArrayObject = NetcdfSharp.ucar.ma2.ArrayObject;
	using ArrayStructure = NetcdfSharp.ucar.ma2.ArrayStructure;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using IOServiceProvider = NetcdfSharp.ucar.nc2.iosp.IOServiceProvider;
	using IOServiceProviderWriter = NetcdfSharp.ucar.nc2.iosp.IOServiceProviderWriter;
	using Layout = NetcdfSharp.ucar.nc2.iosp.Layout;
	using LayoutRegular = NetcdfSharp.ucar.nc2.iosp.LayoutRegular;
	using LayoutRegularSegmented = NetcdfSharp.ucar.nc2.iosp.LayoutRegularSegmented;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcRange = NetcdfSharp.ucar.ma2.NcRange;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using StructureData = NetcdfSharp.ucar.ma2.StructureData;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;

	/// <summary>
	/// IOServiceProviderWriter for Netcdf3 files. </summary>
	public class N3iospWriter : N3iospNew, IOServiceProviderWriter
	{
		// Default fill values, used unless _FillValue variable attribute is set.
		private const byte NC_FILL_BYTE = (byte)0x81; // -127 as unsigned byte
		private static readonly char NC_FILL_CHAR = (char)0;
		private static readonly short NC_FILL_SHORT = (short)-32767;
		private const int NC_FILL_INT = -2147483647;
		private const float NC_FILL_FLOAT = 9.9692099683868690e+36f; // near 15 * 2^119
		private const double NC_FILL_DOUBLE = 9.9692099683868690e+36;

		////////////////////////////////////////////////////////////////////////////////////////////////////////
		private bool fill = true;
		private IOServiceProvider iosp = null;

		public N3iospWriter(IOServiceProvider iosp)
		{
			this.iosp = iosp;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void openForWriting(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		public virtual void openForWriting(RandomAccessFile raf, NetcdfFile ncfile, CancelTask cancelTask)
		{
			// Cant call superclass open, do some duplicate code here
			this.raf = raf;
			this.location = (raf != null) ? raf.Location : null;
			this.ncfile = ncfile;

			string location = raf.Location;
			if (!location.StartsWith("http:", StringComparison.Ordinal))
			{
				File file = new File(location);
				if (file.exists())
				{
					lastModified = file.lastModified();
				}
			}

			raf.order(RandomAccessFile.BIG_ENDIAN);
			this.header = new N3headerWriter(this, raf, ncfile);
			Group.Builder rootGroup = Group.builder().setName("").setNcfile(ncfile);
			header.read(raf, rootGroup, null);
			ncfile.setRootGroup(rootGroup.build());
			ncfile.finish();
		}

		public virtual bool Fill
		{
			set
			{
				this.fill = value;
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void create(String filename, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, int extra, long preallocateSize, boolean largeFile) throws NetcdfSharp.java.io.IOException
		public virtual void create(string filename, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, int extra, long preallocateSize, bool largeFile)
		{
			this.ncfile = ncfile;

			// finish any structures
			ncfile.finish();

			raf = new RandomAccessFile(filename, "rw");
			raf.order(RandomAccessFile.BIG_ENDIAN);

			if (preallocateSize > 0)
			{
				NetcdfSharp.java.io.RandomAccessFile myRaf = raf.getRandomAccessFile();
				myRaf.setLength(preallocateSize);
			}

			N3headerWriter headerw = new N3headerWriter(this, raf, ncfile);
			headerw.create(extra, largeFile, null);
			this.header = headerw;

			if (fill)
			{
				fillNonRecordVariables();
			}
			// else
			// raf.setMinLength(recStart); // make sure file length is long enough, even if not written to.
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public boolean rewriteHeader(boolean largeFile) throws NetcdfSharp.java.io.IOException
		public virtual bool rewriteHeader(bool largeFile)
		{
			return ((N3headerWriter)header).rewriteHeader(largeFile, null);
		}

		//////////////////////////////////////////////////////////////////////////////////////
		// write

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void writeData(Variable v2, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.ucar.ma2.Array values) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		public virtual void writeData(Variable v2, Section section, NcArray values)
		{
			N3headerNew.Vinfo vinfo = (N3headerNew.Vinfo)v2.SPobject;
			DataType dataType = v2.DataType;

			if (v2.Unlimited)
			{
				NcRange firstRange = section.getRange(0);
				Numrecs = firstRange.last() + 1;
			}

			if (v2 is Structure)
			{
				if (!(values is ArrayStructure))
				{
					throw new ArgumentException("writeData for Structure: data must be ArrayStructure");
				}

				if (v2.Rank == 0)
				{
					throw new ArgumentException("writeData for Structure: must have rank > 0");
				}

				Dimension d = v2.getDimension(0);
				if (!d.Unlimited)
				{
					throw new ArgumentException("writeData for Structure: must have unlimited dimension");
				}

				writeRecordData((Structure)v2, section, (ArrayStructure)values);

			}
			else
			{
				Layout layout = (!v2.Unlimited) ? new LayoutRegular(vinfo.begin, v2.ElementSize, v2.Shape, section) : new LayoutRegularSegmented(vinfo.begin, v2.ElementSize, header.recsize, v2.Shape, section);
				writeData(values, layout, dataType);
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public int appendStructureData(Structure s, NetcdfSharp.ucar.ma2.StructureData sdata) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		public virtual int appendStructureData(Structure s, StructureData sdata)
		{
			int recnum = header.numrecs;
			Numrecs = recnum + 1;
			writeRecordData(s, recnum, sdata);
			return recnum;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void writeRecordData(NetcdfSharp.ucar.nc2.Structure s, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.ucar.ma2.ArrayStructure structureArray) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		private void writeRecordData(NetcdfSharp.ucar.nc2.Structure s, Section section, ArrayStructure structureArray)
		{
			int countSrcRecnum = 0;
			NcRange recordRange = section.getRange(0);
			foreach (int recnum in recordRange)
			{
				StructureData sdata = structureArray.getStructureData(countSrcRecnum);
				writeRecordData(s, recnum, sdata);
				countSrcRecnum++;
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void writeRecordData(NetcdfSharp.ucar.nc2.Structure s, int recnum, NetcdfSharp.ucar.ma2.StructureData sdata) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		private void writeRecordData(NetcdfSharp.ucar.nc2.Structure s, int recnum, StructureData sdata)
		{

			StructureMembers members = sdata.StructureMembers;

			// loop over members
			foreach (Variable vm in s.Variables)
			{
				StructureMembers.Member m = members.findMember(vm.ShortName);
				if (null == m)
				{
					continue; // this means that the data is missing from the ArrayStructure
				}

				// convert String member data into CHAR data
				NcArray data = sdata.getArray(m);
				if (data is ArrayObject && vm.DataType == DataType.CHAR && vm.Rank > 0)
				{
					int strlen = vm.getShape(vm.Rank - 1);
					data = ArrayChar.makeFromStringArray((ArrayObject)data, strlen); // turn it into an ArrayChar
				}

				// layout of the destination
				N3headerNew.Vinfo vinfo = (N3headerNew.Vinfo)vm.SPobject;
				long begin = vinfo.begin + recnum * header.recsize; // this assumes unlimited dimension
				Section memberSection = vm.ShapeAsSection;
				Layout layout = new LayoutRegular(begin, vm.ElementSize, vm.Shape, memberSection);

				try
				{
					writeData(data, layout, vm.DataType);
				}
				catch (Exception e)
				{
					log.error("Error writing member=" + vm.ShortName + " in struct=" + s.FullName, e);
					throw new IOException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// write data to a file for a variable.
		/// </summary>
		/// <param name="values"> write this data. </param>
		/// <param name="index"> handles skipping around in the file. </param>
		/// <param name="dataType"> dataType of the variable </param>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void writeData(NetcdfSharp.ucar.ma2.Array values, NetcdfSharp.ucar.nc2.iosp.Layout index, NetcdfSharp.ucar.ma2.DataType dataType) throws NetcdfSharp.java.io.IOException
		private void writeData(NcArray values, Layout index, DataType dataType)
		{
			if ((dataType == DataType.BYTE) || (dataType == DataType.CHAR))
			{
				IndexIterator ii = values.IndexIterator;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.seek(chunk.SrcPos);
					for (int k = 0; k < chunk.Nelems; k++)
					{
						raf.write(ii.ByteNext);
					}
				}
				return;

			}
			else if (dataType == DataType.STRING)
			{ // LOOK not legal
				IndexIterator ii = values.IndexIterator;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.seek(chunk.SrcPos);
					for (int k = 0; k < chunk.Nelems; k++)
					{
						string val = (string)ii.ObjectNext;
						if (!string.ReferenceEquals(val, null))
						{
							raf.write(Encoding.UTF8.GetBytes(val)); // LOOK ??
						}
					}
				}
				return;

			}
			else if (dataType == DataType.SHORT)
			{
				IndexIterator ii = values.IndexIterator;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.seek(chunk.SrcPos);
					for (int k = 0; k < chunk.Nelems; k++)
					{
						raf.writeShort(ii.ShortNext);
					}
				}
				return;

			}
			else if (dataType == DataType.INT)
			{
				IndexIterator ii = values.IndexIterator;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.seek(chunk.SrcPos);
					for (int k = 0; k < chunk.Nelems; k++)
					{
						raf.writeInt(ii.IntNext);
					}
				}
				return;

			}
			else if (dataType == DataType.FLOAT)
			{
				IndexIterator ii = values.IndexIterator;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.seek(chunk.SrcPos);
					for (int k = 0; k < chunk.Nelems; k++)
					{
						raf.writeFloat(ii.FloatNext);
					}
				}
				return;

			}
			else if (dataType == DataType.DOUBLE)
			{
				IndexIterator ii = values.IndexIterator;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.seek(chunk.SrcPos);
					for (int k = 0; k < chunk.Nelems; k++)
					{
						raf.writeDouble(ii.DoubleNext);
					}
				}
				return;
			}

			throw new System.InvalidOperationException("dataType= " + dataType);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void setNumrecs(int n) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		private int Numrecs
		{
			set
			{
				if (value <= header.numrecs)
				{
					return;
				}
				int startRec = header.numrecs;

				// fileUsed = recStart + recsize * value;
				((N3headerWriter)header).Numrecs = value;
				// this.numrecs = value;

				// TODO udim.setLength : need UnlimitedDimension extends Dimension?
				// need to let unlimited dimension know of new shape
				foreach (Dimension dim in ncfile.RootGroup.Dimensions)
				{
					if (dim.Unlimited)
					{
						dim.Length = value;
					}
				}

				// need to let all unlimited variables know of new shape TODO immutable??
				foreach (Variable v in ncfile.Variables)
				{
					if (v.Unlimited)
					{
						v.resetShape();
						v.setCachedData(null, false);
					}
				}

				// extend file, handle filling
				if (fill)
				{
					fillRecordVariables(startRec, value);
				}
				else
				{
					raf.MinLength = header.calcFileSize();
				}
			}
		}

		/// <summary>
		/// Update the value of an existing attribute on disk, not in memory. Attribute is found by name, which must match
		/// exactly.
		/// You cannot make an attribute longer, or change the number of values.
		/// For strings: truncate if longer, zero fill if shorter. Strings are padded to 4 byte boundaries, ok to use padding
		/// if it exists.
		/// For numerics: must have same number of values.
		/// </summary>
		/// <param name="v2"> variable, or null for global attribute </param>
		/// <param name="att"> replace with this value </param>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void updateAttribute(NetcdfSharp.ucar.nc2.Variable v2, Attribute att) throws NetcdfSharp.java.io.IOException
		public virtual void updateAttribute(NetcdfSharp.ucar.nc2.Variable v2, NcAttribute att)
		{
			((N3headerWriter)header).updateAttribute(v2, att);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void flush() throws NetcdfSharp.java.io.IOException
		public override void flush()
		{
			if (raf != null)
			{
				raf.flush();
				((N3headerWriter)header).writeNumrecs();
				raf.flush();
			}
		}

		/////////////////////////////////////////////////////////////

		// fill buffer with fill value

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void fillNonRecordVariables() throws NetcdfSharp.java.io.IOException
		private void fillNonRecordVariables()
		{
			// run through each variable
			foreach (Variable v in ncfile.Variables)
			{
				if (v.Unlimited)
				{
					continue;
				}
				try
				{
					writeData(v, v.ShapeAsSection, makeConstantArray(v));
				}
				catch (InvalidRangeException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace); // shouldnt happen
				}
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void fillRecordVariables(int recStart, int recEnd) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		private void fillRecordVariables(int recStart, int recEnd)
		{
			// do each record completely, should be a bit more efficient

			for (int i = recStart; i < recEnd; i++)
			{ // do one record at a time
				NcRange r = new NcRange(i, i);

				// run through each variable
				foreach (Variable v in ncfile.Variables)
				{
					if (!v.Unlimited || (v is Structure))
					{
						continue;
					}
					Section.Builder recordSection = Section.builder().appendRanges(v.Ranges);
					recordSection.setRange(0, r);
					writeData(v, recordSection.build(), makeConstantArray(v));
				}
			}
		}

		private NcArray makeConstantArray(Variable v)
		{
			Type classType = v.DataType.getPrimitiveClassType();
			// int [] shape = v.getShape();
			NcAttribute att = v.findAttribute(CDM.FILL_VALUE);

			object storage = null;
			if (classType == typeof(double))
			{
				double[] storageP = new double[1];
				storageP[0] = (att == null) ? NC_FILL_DOUBLE : Convert.ToDouble(att.NumericValue);
				storage = storageP;

			}
			else if (classType == typeof(float))
			{
				float[] storageP = new float[1];
				storageP[0] = (att == null) ? NC_FILL_FLOAT : Convert.ToSingle(att.NumericValue);
				storage = storageP;

			}
			else if (classType == typeof(int))
			{
				int[] storageP = new int[1];
				storageP[0] = (att == null) ? NC_FILL_INT : Convert.ToInt32(att.NumericValue);
				storage = storageP;

			}
			else if (classType == typeof(short))
			{
				short[] storageP = new short[1];
				storageP[0] = (att == null) ? NC_FILL_SHORT : Convert.ToInt16(att.NumericValue);
				storage = storageP;

			}
			else if (classType == typeof(byte))
			{
				byte[] storageP = new byte[1];
				storageP[0] = (att == null) ? NC_FILL_BYTE : Convert.ToByte(att.NumericValue);
				storage = storageP;

			}
			else if (classType == typeof(char))
			{
				char[] storageP = new char[1];
				storageP[0] = (att != null) && (att.StringValue.Length > 0) ? att.StringValue[0] : NC_FILL_CHAR;
				storage = storageP;
			}

			return NcArray.factoryConstant(v.DataType, v.Shape, storage);
		}

		//////////////////////////////////////////////////////////////////////////////////////////////
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public boolean syncExtend() throws NetcdfSharp.java.io.IOException
		public override bool syncExtend()
		{
			bool result = ((N3headerWriter)header).synchNumrecs();
			if (result && log.isDebugEnabled())
			{
				log.debug(" N3iosp syncExtend " + raf.Location + " numrecs =" + header.numrecs);
			}
			return result;
		}
	}

}
