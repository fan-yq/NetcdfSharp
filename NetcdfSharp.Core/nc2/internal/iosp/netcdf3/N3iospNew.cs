// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/* Copyright Unidata */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3

{

	using AbstractIOServiceProvider = NetcdfSharp.ucar.nc2.iosp.AbstractIOServiceProvider;
	using ArrayStructureBB = NetcdfSharp.ucar.ma2.ArrayStructureBB;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using DataFormatType = NetcdfSharp.ucar.nc2.constants.DataFormatType;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using IOServiceProvider = NetcdfSharp.ucar.nc2.iosp.IOServiceProvider;
	using IospHelper = NetcdfSharp.ucar.nc2.iosp.IospHelper;
	using Layout = NetcdfSharp.ucar.nc2.iosp.Layout;
	using LayoutRegular = NetcdfSharp.ucar.nc2.iosp.LayoutRegular;
	using LayoutRegularSegmented = NetcdfSharp.ucar.nc2.iosp.LayoutRegularSegmented;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcRange = NetcdfSharp.ucar.ma2.NcRange;
	using NetcdfFileFormat = NetcdfSharp.ucar.nc2.write.NetcdfFileFormat;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;
	using Vinfo = NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3.N3headerNew.Vinfo;

	/// <summary>
	/// Netcdf 3 version iosp, using Builders for immutability.
	/// 
	/// @author caron
	/// @since 9/29/2019.
	/// </summary>
	public class N3iospNew : AbstractIOServiceProvider, IOServiceProvider
	{
		protected internal static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(N3iospNew));

		// NetCDF File Format Type (defined in netcdf.h from the C library)
		private static readonly string NC_FORMATX_NC3 = NetcdfFileFormat.NETCDF3.version().ToString();

		/*
		 * CLASSIC
		 * The maximum size of a record in the classic format in versions 3.5.1 and earlier is 2^32 - 4 bytes.
		 * In versions 3.6.0 and later, there is no such restriction on total record size for the classic format
		 * or 64-bit offset format.
		 *
		 * If you don't use the unlimited dimension, only one variable can exceed 2 GiB in size, but it can be as
		 * large as the underlying file system permits. It must be the last variable in the dataset, and the offset
		 * to the beginning of this variable must be less than about 2 GiB.
		 *
		 * The limit is really 2^31 - 4. If you were to specify a variable size of 2^31 -3, for example, it would be
		 * rounded up to the nearest multiple of 4 bytes, which would be 2^31, which is larger than the largest
		 * signed integer, 2^31 - 1.
		 *
		 * If you use the unlimited dimension, record variables may exceed 2 GiB in size, as long as the offset of the
		 * start of each record variable within a record is less than 2 GiB - 4.
		 */

		/*
		 * LARGE FILE
		 * Assuming an operating system with Large File Support, the following restrictions apply to the netCDF 64-bit offset
		 * format.
		 *
		 * No fixed-size variable can require more than 2^32 - 4 bytes of storage for its data, unless it is the last
		 * fixed-size variable and there are no record variables. When there are no record variables, the last
		 * fixed-size variable can be any size supported by the file system, e.g. terabytes.
		 *
		 * A 64-bit offset format netCDF file can have up to 2^32 - 1 fixed sized variables, each under 4GiB in size.
		 * If there are no record variables in the file the last fixed variable can be any size.
		 *
		 * No record variable can require more than 2^32 - 4 bytes of storage for each record's worth of data,
		 * unless it is the last record variable. A 64-bit offset format netCDF file can have up to 2^32 - 1 records,
		 * of up to 2^32 - 1 variables, as long as the size of one record's data for each record variable except the
		 * last is less than 4 GiB - 4.
		 *
		 * Note also that all netCDF variables and records are padded to 4 byte boundaries.
		 */

		protected internal N3headerNew header;
		protected internal long lastModified; // used by sync
		private bool debugRecord = false;

		private Charset valueCharset;

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
		public override bool isValidFile(RandomAccessFile raf)
		{
			return N3headerNew.isValidFile(raf);
		}

		public override string DetailInfo
		{
			get
			{
				Formatter f = new Formatter();
				f.format("%s", base.DetailInfo);

				try
				{
					header.showDetail(f);
				}
				catch (IOException e)
				{
					return e.Message;
				}

				return f.ToString();

			}
		}

		// properties
		internal bool useRecordStructure;

		//////////////////////////////////////////////////////////////////////////////////////
		// read existing file

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated @Override public void open(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		[Obsolete]
		public override void open(RandomAccessFile raf, NetcdfFile ncfile, CancelTask cancelTask)
		{
			base.open(raf, ncfile, cancelTask);

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
			header = createHeader();

			Group.Builder rootGroup = Group.builder().setName("").setNcfile(ncfile);
			header.read(raf, rootGroup, null);
			ncfile.setRootGroup(rootGroup.build());
			ncfile.finish();
		}

		public override bool Builder
		{
			get
			{
				return true;
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void build(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Group.Builder rootGroup, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		public override void build(RandomAccessFile raf, Group.Builder rootGroup, CancelTask cancelTask)
		{
			base.open(raf, rootGroup.getNcfile(), cancelTask);

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
			header = createHeader();
			header.read(raf, rootGroup, null);
		}

		/// <summary>
		/// Create header for reading netcdf file. </summary>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private N3headerNew createHeader() throws NetcdfSharp.java.io.IOException
		private N3headerNew createHeader()
		{
			return new N3headerNew(this);
		}

		/////////////////////////////////////////////////////////////////////////////
		// data reading

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public NetcdfSharp.ucar.ma2.Array readData(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		public override NcArray readData(Variable v2, Section section)
		{
			if (v2 is Structure)
			{
				return readRecordData((Structure)v2, section);
			}

			Vinfo vinfo = (Vinfo)v2.SPobject;
			DataType dataType = v2.DataType;

			Layout layout = (!v2.Unlimited) ? new LayoutRegular(vinfo.begin, v2.ElementSize, v2.Shape, section) : new LayoutRegularSegmented(vinfo.begin, v2.ElementSize, header.recsize, v2.Shape, section);

			if (layout.TotalNelems == 0)
			{
				return NcArray.factory(dataType, section.Shape);
			}

			object data = readData(layout, dataType);
			return NcArray.factory(dataType, section.Shape, data);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public long readToByteChannel(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.java.nio.channels.WritableByteChannel channel) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		public override long readToByteChannel(Variable v2, Section section, WritableByteChannel channel)
		{

			if (v2 is Structure)
			{
				return readRecordData((Structure)v2, section, channel);
			}

			Vinfo vinfo = (Vinfo)v2.SPobject;
			DataType dataType = v2.DataType;

			Layout layout = (!v2.Unlimited) ? new LayoutRegular(vinfo.begin, v2.ElementSize, v2.Shape, section) : new LayoutRegularSegmented(vinfo.begin, v2.ElementSize, header.recsize, v2.Shape, section);

			return readData(layout, dataType, channel);
		}

		// TODO
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private long readRecordData(NetcdfSharp.ucar.nc2.Structure s, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.java.nio.channels.WritableByteChannel out) throws NetcdfSharp.java.io.IOException
		private long readRecordData(Structure s, Section section, WritableByteChannel @out)
		{
			long count = 0;

			/*
			 * RegularIndexer index = new RegularIndexer( s.getShape(), recsize, recStart, section, recsize);
			 * while (index.hasNext()) {
			 * Indexer.Chunk chunk = index.next();
			 * count += raf.readBytes( out, chunk.getFilePos(), chunk.getNelems() * s.getElementSize());
			 * }
			 */

			// LOOK this is the OTW layout based on netcdf-3
			// not sure this works but should give an idea of timing
			NcRange recordRange = section.getRange(0);
			/*
			 * int stride = recordRange.stride();
			 * if (stride == 1) {
			 * int first = recordRange.first();
			 * int n = recordRange.length();
			 * if (false) System.out.println(" read record " + first+" "+ n * header.recsize+" bytes ");
			 * return raf.readToByteChannel(out, header.recStart + first * header.recsize, n * header.recsize);
			 *
			 * } else {
			 */

			foreach (int recnum in recordRange)
			{
				if (debugRecord)
				{
					Console.WriteLine(" read record " + recnum);
				}
				raf.seek(header.recStart + recnum * header.recsize); // where the record starts
				count += raf.readToByteChannel(@out, header.recStart + recnum * header.recsize, header.recsize);
			}
			// }

			return count;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public boolean syncExtend() throws NetcdfSharp.java.io.IOException
		public override bool syncExtend()
		{
			// boolean result = header.synchNumrecs();
			// if (result && log.isDebugEnabled())
			// log.debug(" N3iosp syncExtend " + raf.getLocation() + " numrecs =" + header.numrecs);
			return true;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public void flush() throws NetcdfSharp.java.io.IOException
		public virtual void flush()
		{
			if (raf != null)
			{
				raf.flush();
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void close() throws NetcdfSharp.java.io.IOException
		public override void close()
		{
			if (raf != null)
			{
				if (header != null)
				{
					long size = header.calcFileSize();
					raf.MinLength = size;
				}
				raf.close();
			}
			raf = null;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void reacquire() throws NetcdfSharp.java.io.IOException
		public override void reacquire()
		{
			base.reacquire();
			header.raf = this.raf;
		}

		public override string toStringDebug(object o)
		{
			return null;
		}

		public override object sendIospMessage(object message)
		{
			if (message is Charset)
			{
				setValueCharset((Charset)message);
			}
			if (message.Equals(NetcdfFile.IOSP_MESSAGE_GET_NETCDF_FILE_FORMAT))
			{
				return header.useLongOffset ? NetcdfFileFormat.NETCDF3_64BIT_OFFSET : NetcdfFileFormat.NETCDF3;
			}
			if (message.Equals(NetcdfFile.IOSP_MESSAGE_ADD_RECORD_STRUCTURE))
			{
				useRecordStructure = true;
				return null;
			}
			return base.sendIospMessage(message);
		}

		/// <summary>
		/// Return <seealso cref="Charset value charset"/> if it was defined. Definition of charset
		/// occurs by sending a charset as a message using the <seealso cref="sendIospMessage"/>
		/// method.
		/// </summary>
		/// <returns> <seealso cref="Charset value charset"/> if it was defined. </returns>
		internal virtual Optional<Charset> getValueCharset()
		{
			return Optional<Charset>.ofNullable(valueCharset);
		}

		internal virtual void setValueCharset(Charset charset)
		{
			this.valueCharset = charset;
		}


		public override string FileTypeId
		{
			get
			{
				return DataFormatType.NETCDF.Description;
			}
		}

		public override string FileTypeDescription
		{
			get
			{
				return "NetCDF-3/CDM";
			}
		}

		public override string FileTypeVersion
		{
			get
			{
				return NC_FORMATX_NC3;
			}
		}

		/// <summary>
		/// Read data subset from file for a variable, create primitive array.
		/// </summary>
		/// <param name="index"> handles skipping around in the file. </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <returns> primitive array with data read in </returns>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private Object readData(NetcdfSharp.ucar.nc2.iosp.Layout index, NetcdfSharp.ucar.ma2.DataType dataType) throws NetcdfSharp.java.io.IOException
		private object readData(Layout index, DataType dataType)
		{
			return IospHelper.readDataFill(raf, index, dataType, null, -1);
		}

		/// <summary>
		/// Read data subset from file for a variable, to WritableByteChannel.
		/// Will send as bigendian, since thats what the underlying file has.
		/// </summary>
		/// <param name="index"> handles skipping around in the file. </param>
		/// <param name="dataType"> dataType of the variable </param>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private long readData(NetcdfSharp.ucar.nc2.iosp.Layout index, NetcdfSharp.ucar.ma2.DataType dataType, NetcdfSharp.java.nio.channels.WritableByteChannel out) throws NetcdfSharp.java.io.IOException
		private long readData(Layout index, DataType dataType, WritableByteChannel @out)
		{
			long count = 0;
			if (dataType.getPrimitiveClassType() == typeof(byte) || dataType == DataType.CHAR)
			{
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					count += raf.readToByteChannel(@out, chunk.SrcPos, chunk.Nelems);
				}

			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					count += raf.readToByteChannel(@out, chunk.SrcPos, 2 * chunk.Nelems);
				}

			}
			else if (dataType.getPrimitiveClassType() == typeof(int) || (dataType == DataType.FLOAT))
			{
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					count += raf.readToByteChannel(@out, chunk.SrcPos, 4 * chunk.Nelems);
				}

			}
			else if ((dataType == DataType.DOUBLE) || dataType.getPrimitiveClassType() == typeof(long))
			{
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					count += raf.readToByteChannel(@out, chunk.SrcPos, 8 * chunk.Nelems);
				}
			}

			return count;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Read data from record structure. For N3, this is the only possible structure, and there can be no nesting.
		/// Read all variables for each record, put in ByteBuffer.
		/// </summary>
		/// <param name="s"> the record structure </param>
		/// <param name="section"> the record range to read </param>
		/// <returns> an ArrayStructure, with all the data read in. </returns>
		/// <exception cref="IOException"> on error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private NetcdfSharp.ucar.ma2.Array readRecordData(NetcdfSharp.ucar.nc2.Structure s, NetcdfSharp.ucar.ma2.Section section) throws NetcdfSharp.java.io.IOException
		private NcArray readRecordData(Structure s, Section section)
		{
			// has to be 1D
			NcRange recordRange = section.getRange(0);

			// create the ArrayStructure
			StructureMembers members = s.makeStructureMembers();
			foreach (StructureMembers.Member m in members.Members)
			{
				Variable v2 = s.findVariable(m.Name);
				Vinfo vinfo = (Vinfo)v2.SPobject;
				m.DataParam = (int)(vinfo.begin - header.recStart);
			}

			// protect against too large of reads
			if (header.recsize > int.MaxValue)
			{
				throw new ArgumentException("Cant read records when recsize > " + int.MaxValue);
			}
			long nrecs = section.computeSize();
			if (nrecs * header.recsize > int.MaxValue)
			{
				throw new ArgumentException("Too large read: nrecs * recsize= " + (nrecs * header.recsize) + "bytes exceeds " + int.MaxValue);
			}

			members.StructureSize = (int)header.recsize;
			ArrayStructureBB structureArray = new ArrayStructureBB(members, new int[] { recordRange.length() });

			// loop over records
			byte[] result = structureArray.ByteBuffer.array();
			int count = 0;
			foreach (int recnum in recordRange)
			{
				if (debugRecord)
				{
					Console.WriteLine(" read record " + recnum);
				}
				raf.seek(header.recStart + recnum * header.recsize); // where the record starts

				if (recnum != header.numrecs - 1)
				{
					raf.readFully(result, (int)(count * header.recsize), (int)header.recsize);
				}
				else
				{
					// "wart" allows file to be one byte short. since its always padding, we allow
					raf.read(result, (int)(count * header.recsize), (int)header.recsize);
				}
				count++;
			}

			return structureArray;
		}
	}

}
