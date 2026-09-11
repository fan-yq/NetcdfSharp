// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf5

{

	using AbstractIOServiceProvider = NetcdfSharp.ucar.nc2.iosp.AbstractIOServiceProvider;
	using ArrayStructure = NetcdfSharp.ucar.ma2.ArrayStructure;
	using ArrayStructureBB = NetcdfSharp.ucar.ma2.ArrayStructureBB;
	using ArrayStructureW = NetcdfSharp.ucar.ma2.ArrayStructureW;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using DataFormatType = NetcdfSharp.ucar.nc2.constants.DataFormatType;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using HdfEos = NetcdfSharp.ucar.nc2.@internal.iosp.hdf4.HdfEos;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using IospHelper = NetcdfSharp.ucar.nc2.iosp.IospHelper;
	using Layout = NetcdfSharp.ucar.nc2.iosp.Layout;
	using LayoutBB = NetcdfSharp.ucar.nc2.iosp.LayoutBB;
	using LayoutRegular = NetcdfSharp.ucar.nc2.iosp.LayoutRegular;
	using N3iospFillValues = NetcdfSharp.ucar.nc2.iosp.netcdf3.N3iospFillValues;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcIndex = NetcdfSharp.ucar.ma2.NcIndex;
	using NetcdfFileFormat = NetcdfSharp.ucar.nc2.write.NetcdfFileFormat;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using StructureData = NetcdfSharp.ucar.ma2.StructureData;
	using StructureDataW = NetcdfSharp.ucar.ma2.StructureDataW;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;

	/// <summary>
	/// HDF5 I/O
	/// 
	/// @author caron
	/// </summary>
	public class H5iospNew : AbstractIOServiceProvider
	{
		public const string IOSP_MESSAGE_INCLUDE_ORIGINAL_ATTRIBUTES = "IncludeOrgAttributes";

		public const int VLEN_T_SIZE = 16; // Appears to be no way to compute on the fly.

		internal static bool debug;
		internal static bool debugPos;
		internal static bool debugHeap;
		internal static bool debugHeapStrings;
		internal static bool debugFilter;
		internal static bool debugRead;
		internal static bool debugFilterIndexer;
		internal static bool debugChunkIndexer;
		internal static bool debugVlen;
		internal static bool debugStructure;
		// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		internal static bool useHdfEos_Conflict = true;

		internal static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(H5iospNew));

		public static NetcdfSharp.ucar.nc2.util.DebugFlags DebugFlags
		{
			set
			{
				debug = value.isSet("H5iosp/read");
				debugPos = value.isSet("H5iosp/filePos");
				debugHeap = value.isSet("H5iosp/Heap");
				debugFilter = value.isSet("H5iosp/filter");
				debugFilterIndexer = value.isSet("H5iosp/filterIndexer");
				debugChunkIndexer = value.isSet("H5iosp/chunkIndexer");
				debugVlen = value.isSet("H5iosp/vlen");

				H5headerNew.DebugFlags = value;
				if (debugFilter)
				{
					H5tiledLayoutBB.debugFilter = debugFilter;
				}
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
		public override bool isValidFile(RandomAccessFile raf)
		{
			return H5headerNew.isValidFile(raf);
		}

		public override string FileTypeId
		{
			get
			{
				if (isEos)
				{
					return "HDF5-EOS";
				}
				if (header.Netcdf4)
				{
					return DataFormatType.NETCDF4.Description;
				}
				return DataFormatType.HDF5.Description;
			}
		}

		public override string FileTypeDescription
		{
			get
			{
				return "Hierarchical Data Format, version 5";
			}
		}

		public virtual RandomAccessFile RandomAccessFile
		{
			get
			{
				return raf;
			}
		}

		public static void useHdfEos(bool val)
		{
			useHdfEos_Conflict = val;
		}

		public override bool Builder
		{
			get
			{
				return true;
			}
		}

		//////////////////////////////////////////////////////////////////////////////////

		private H5headerNew header;
		private bool isEos;
		internal bool includeOriginalAttributes;
		private Charset valueCharset;

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void build(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Group.Builder rootGroup, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		public override void build(RandomAccessFile raf, Group.Builder rootGroup, CancelTask cancelTask)
		{
			base.open(raf, rootGroup.getNcfile(), cancelTask);

			raf.order(RandomAccessFile.BIG_ENDIAN);
			header = new H5headerNew(rootGroup, this);
			header.read(null);

			// check if its an HDF5-EOS file
			if (useHdfEos_Conflict)
			{
				rootGroup.findGroupLocal(HdfEos.HDF5_GROUP).ifPresent(eosGroup =>
				{
					try
					{
						isEos = HdfEos.amendFromODL(raf.Location, header, eosGroup);
					}
					catch (IOException)
					{
						log.warn(" HdfEos.amendFromODL failed");
					}
				});
			}
		}

		public override object sendIospMessage(object message)
		{
			if (message is Charset)
			{
				setValueCharset((Charset)message);
			}
			if (message.Equals(NetcdfFile.IOSP_MESSAGE_GET_NETCDF_FILE_FORMAT))
			{
				if (!header.Netcdf4)
				{
					return null;
				}
				return header.Classic ? NetcdfFileFormat.NETCDF4_CLASSIC : NetcdfFileFormat.NETCDF4;
			}
			return base.sendIospMessage(message);
		}

		/// <summary>
		/// Return <seealso cref="Charset value charset"/> if it was defined. Definition of charset
		/// occurs by sending a charset as a message using the <seealso cref="sendIospMessage"/>
		/// method.
		/// </summary>
		/// <returns> <seealso cref="Charset value charset"/> if it was defined. </returns>
		public virtual Optional<Charset> getValueCharset() => Optional<Charset>.ofNullable(valueCharset);

		public virtual void setValueCharset(Charset charset) { this.valueCharset = charset; }


		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void open(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		public override void open(RandomAccessFile raf, NetcdfFile ncfile, CancelTask cancelTask)
		{
			base.open(raf, ncfile, cancelTask);
			Group.Builder rootGroup = Group.builder().setName("").setNcfile(ncfile);
			header = new H5headerNew(rootGroup, this);
			header.read(null);
			ncfile.setRootGroup(rootGroup.build());

			// check if its an HDF5-EOS file
			if (useHdfEos_Conflict)
			{
				rootGroup.findGroupLocal(HdfEos.HDF5_GROUP).ifPresent(eosGroup =>
				{
					try
					{
						isEos = HdfEos.amendFromODL(raf.Location, header, eosGroup);
					}
					catch (IOException)
					{
						log.warn(" HdfEos.amendFromODL failed");
					}
				});
			}

			ncfile.finish();
		}

		public virtual H5headerNew Header
		{
			get
			{
				return header;
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.Array readData(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section section) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		public override NcArray readData(Variable v2, Section section)
		{
			H5headerNew.Vinfo vinfo = (H5headerNew.Vinfo)v2.SPobject;
			if (debugRead)
			{
				// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Console.Write("{0} read {1}%n", v2.FullName, section);
			}
			return readData(v2, vinfo.dataPos, section);
		}

		// all the work is here, so can be called recursively
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private NetcdfSharp.ucar.ma2.Array readData(NetcdfSharp.ucar.nc2.Variable v2, long dataPos, NetcdfSharp.ucar.ma2.Section wantSection) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		private NcArray readData(Variable v2, long dataPos, Section wantSection)
		{
			H5headerNew.Vinfo vinfo = (H5headerNew.Vinfo)v2.SPobject;
			DataType dataType = v2.DataType;
			object data;
			Layout layout;

			if (vinfo.useFillValue_Conflict)
			{ // fill value only
				object pa = IospHelper.makePrimitiveArray((int)wantSection.computeSize(), dataType, vinfo.FillValue);
				if (dataType == DataType.CHAR)
				{
					pa = IospHelper.convertByteToChar((byte[])pa);
				}
				return NcArray.factory(dataType, wantSection.Shape, pa);
			}

			if (vinfo.mfp != null)
			{ // filtered
				if (debugFilter)
				{
					Console.WriteLine("read variable filtered " + v2.FullName + " vinfo = " + vinfo);
				}
				Debug.Assert(vinfo.isChunked);
				ByteOrder bo = (vinfo.typeInfo.endian == 0) ? ByteOrder.BIG_ENDIAN : ByteOrder.LITTLE_ENDIAN;
				layout = new H5tiledLayoutBB(v2, wantSection, raf, vinfo.mfp.Filters, bo);
				if (vinfo.typeInfo.isVString)
				{
					data = readFilteredStringData((LayoutBB)layout);
				}
				else
				{
					data = IospHelper.readDataFill((LayoutBB)layout, v2.DataType, vinfo.FillValue);
				}

			}
			else
			{ // normal case
				if (debug)
				{
					Console.WriteLine("read variable " + v2.FullName + " vinfo = " + vinfo);
				}

				DataType readDtype = v2.DataType;
				int elemSize = v2.ElementSize;
				object fillValue = vinfo.FillValue;
				int endian = vinfo.typeInfo.endian;

				// fill in the wantSection
				wantSection = Section.fill(wantSection, v2.Shape);

				if (vinfo.typeInfo.hdfType == 2)
				{ // time
					readDtype = vinfo.mdt.timeType;
					elemSize = readDtype.getSize();
					fillValue = N3iospFillValues.getFillValueDefault(readDtype);

				}
				else if (vinfo.typeInfo.hdfType == 8)
				{ // enum
					H5headerNew.TypeInfo baseInfo = vinfo.typeInfo.@base;
					readDtype = baseInfo.dataType.Value;
					elemSize = readDtype.getSize();
					fillValue = N3iospFillValues.getFillValueDefault(readDtype);
					endian = baseInfo.endian;

				}
				else if (vinfo.typeInfo.hdfType == 9)
				{ // vlen
					elemSize = vinfo.typeInfo.byteSize;
					endian = vinfo.typeInfo.endian;
					// wantSection = wantSection.removeVlen(); // remove vlen dimension
				}

				if (vinfo.isChunked)
				{
					layout = new H5tiledLayout((H5headerNew.Vinfo)v2.SPobject, readDtype, wantSection);
				}
				else
				{
					layout = new LayoutRegular(dataPos, elemSize, v2.Shape, wantSection);
				}
				data = readData(vinfo, v2, layout, readDtype, wantSection.Shape, fillValue, endian);
			}

			if (data is NcArray)
			{
				return (NcArray)data;
			}
			else if (dataType == DataType.STRUCTURE)
			{
				return convertStructure((Structure)v2, layout, wantSection.Shape, (byte[])data); // LOOK
			}
			else
			{
				return NcArray.factory(dataType, wantSection.Shape, data);
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private String[] readFilteredStringData(NetcdfSharp.ucar.nc2.iosp.LayoutBB layout) throws NetcdfSharp.java.io.IOException
		private string[] readFilteredStringData(LayoutBB layout)
		{
			int size = (int)layout.TotalNelems;
			string[] sa = new string[size];
			while (layout.hasNext())
			{
				LayoutBB.ChunkBB chunk = (LayoutBB.ChunkBB)layout.next();
				ByteBuffer bb = chunk.ByteBuffer;
				// bb.position(chunk.getSrcElem());
				if (debugHeapStrings)
				{
					// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
					Console.Write("readFilteredStringData chunk={0}%n", chunk);
				}
				int destPos = (int)chunk.DestElem;
				for (int i = 0; i < chunk.Nelems; i++)
				{ // 16 byte "heap ids"
					sa[destPos++] = header.readHeapString(bb, (chunk.SrcElem + i) * 16); // LOOK does this handle section
																						 // correctly ??
				}
			}
			return sa;
		}

		/// <summary>
		/// Read data subset from file for a variable, return NcArray or java primitive array.
		/// </summary>
		/// <param name="v"> the variable to read. </param>
		/// <param name="layout"> handles skipping around in the file. </param>
		/// <param name="dataType"> dataType of the data to read </param>
		/// <param name="shape"> the shape of the output </param>
		/// <param name="fillValue"> fill value as a wrapped primitive </param>
		/// <returns> primitive array or NcArray with data read in </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> if read error </exception>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if invalid section </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private Object readData(H5headerNew.Vinfo vinfo, NetcdfSharp.ucar.nc2.Variable v, NetcdfSharp.ucar.nc2.iosp.Layout layout, NetcdfSharp.ucar.ma2.DataType dataType, int[] shape, Object fillValue, int endian) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		private object readData(H5headerNew.Vinfo vinfo, Variable v, Layout layout, DataType dataType, int[] shape, object fillValue, int endian)
		{

			H5headerNew.TypeInfo typeInfo = vinfo.typeInfo;

			// special processing
			if (typeInfo.hdfType == 2)
			{ // time
				object data = IospHelper.readDataFill(raf, layout, dataType, fillValue, endian, true);
				NcArray timeArray = NcArray.factory(dataType, shape, data);

				// now transform into an ISO Date String
				string[] stringData = new string[(int)timeArray.Size];
				int count = 0;
				while (timeArray.hasNext())
				{
					long time = timeArray.nextLong();
					stringData[count++] = CalendarDate.of(time).ToString();
				}
				return NcArray.factory(DataType.STRING, shape, stringData);
			}

			if (typeInfo.hdfType == 8)
			{ // enum
				object data = IospHelper.readDataFill(raf, layout, dataType, fillValue, endian);
				return NcArray.factory(dataType, shape, data);
			}

			if (typeInfo.isVlen)
			{ // vlen (not string)
				DataType readType = dataType;
				if (typeInfo.@base.hdfType == 7) // reference
				{
					readType = DataType.LONG;
				}

				// general case is to read an array of vlen objects
				// each vlen generates an NcArray - so return ArrayObject of NcArray
				// boolean scalar = false; // layout.getTotalNelems() == 1; // if scalar, return just the len NcArray // remove
				// 12/25/10 jcaron
				NcArray[] data = new NcArray[(int)layout.TotalNelems];
				int count = 0;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					if (chunk == null)
					{
						continue;
					}
					for (int i = 0; i < chunk.Nelems; i++)
					{
						long address = chunk.SrcPos + layout.ElemSize * i;
						NcArray vlenArray = header.getHeapDataArray(address, readType, endian);
						data[count++] = (typeInfo.@base.hdfType == 7) ? convertReference(vlenArray) : vlenArray;
					}
				}
				int prefixrank = 0;
				for (int i = 0; i < shape.Length; i++)
				{ // find leftmost vlen
					if (shape[i] < 0)
					{
						prefixrank = i;
						break;
					}
				}
				NcArray result;
				if (prefixrank == 0) // if scalar, return just the singleton vlen array
				{
					result = data[0];
				}
				else
				{
					int[] newshape = new int[prefixrank];
					System.Array.Copy(shape, 0, newshape, 0, prefixrank);
					result = NcArray.makeVlenArray(newshape, data);
				}
				return result;
			}

			if (dataType == DataType.STRUCTURE)
			{ // LOOK what about subset ?
				int recsize = layout.ElemSize;
				long size = recsize * layout.TotalNelems;
				byte[] byteArray = new byte[(int)size];
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					if (chunk == null)
					{
						continue;
					}
					if (debugStructure)
					{
						Console.WriteLine(" readStructure " + v.getFullName() + " chunk= " + chunk + " index.getElemSize= " + layout.ElemSize);
					}
					// copy bytes directly into the underlying byte[] LOOK : assumes contiguous layout ??
					raf.seek(chunk.SrcPos);
					raf.readFully(byteArray, (int)chunk.DestElem * recsize, chunk.Nelems * recsize);
				}

				// place data into an ArrayStructureBB
				return convertStructure((Structure)v, layout, shape, byteArray); // LOOK
			}

			// normal case
			return readDataPrimitive(layout, dataType, shape, fillValue, endian, true);
		}

		internal virtual NcArray convertReference(NcArray refArray)
		{
			int nelems = (int)refArray.Size;
			NcIndex ima = refArray.Index;
			string[] result = new string[nelems];
			for (int i = 0; i < nelems; i++)
			{
				long reference = refArray.getLong(ima.set(i));
				string name = header.getDataObjectName(reference);
				result[i] = name != null ? name : reference.ToString();
				if (debugVlen)
				{
					Console.WriteLine(" convertReference 0x" + Convert.ToString(reference, 16) + " to " + result[i]);
				}
			}
			return NcArray.factory(DataType.STRING, new int[] { nelems }, result);
		}

		private ArrayStructure convertStructure(Structure s, Layout layout, int[] shape, byte[] byteArray)
		{
			// create StructureMembers - must set offsets
			StructureMembers sm = s.makeStructureMembers();
			int calcSize = ArrayStructureBB.setOffsets(sm); // standard

			// special offset setting
			bool hasHeap = convertStructure(s, sm);

			int recSize = layout.ElemSize;
			if (recSize < calcSize)
			{
				log.error("calcSize = {} actualSize = {}%n", calcSize, recSize);
				throw new IOException("H5iosp illegal structure size " + s.getFullName());
			}
			sm.StructureSize = recSize;

			// place data into an ArrayStructureBB
			ByteBuffer bb = ByteBuffer.wrap(byteArray);
			ArrayStructureBB asbb = new ArrayStructureBB(sm, shape, bb, 0);

			// strings and vlens are stored on the heap, and must be read separately
			if (hasHeap)
			{
				int destPos = 0;
				for (int i = 0; i < layout.TotalNelems; i++)
				{ // loop over each structure
					convertHeap(asbb, destPos, sm);
					destPos += layout.ElemSize;
				}
			}
			return asbb;
		}

		// recursive
		private bool convertStructure(Structure s, StructureMembers sm)
		{
			bool hasHeap = false;
			foreach (StructureMembers.Member m in sm.Members)
			{
				Variable v2 = s.findVariable(m.Name);
				Debug.Assert(v2 != null);
				H5headerNew.Vinfo vm = (H5headerNew.Vinfo)v2.SPobject;

				// apparently each member may have separate byte order (!!!??)
				if (vm.typeInfo.endian >= 0)
				{
					m.DataObject = vm.typeInfo.endian == RandomAccessFile.LITTLE_ENDIAN ? ByteOrder.LITTLE_ENDIAN : ByteOrder.BIG_ENDIAN;
				}

				// vm.dataPos : offset since start of Structure
				m.DataParam = (int)vm.dataPos;

				// track if there is a heap
				if (v2.DataType == DataType.STRING || v2.VariableLength)
				{
					hasHeap = true;
				}

				// recurse
				if (v2 is Structure)
				{
					Structure nested = (Structure)v2;
					StructureMembers nestSm = nested.makeStructureMembers();
					m.StructureMembers = nestSm;
					hasHeap |= convertStructure(nested, nestSm);
				}
			}
			return hasHeap;
		}

		internal virtual void convertHeap(ArrayStructureBB asbb, int pos, StructureMembers sm)
		{
			ByteBuffer bb = asbb.ByteBuffer;
			foreach (StructureMembers.Member m in sm.Members)
			{
				if (m.DataType == DataType.STRING)
				{
					m.DataObject = ByteOrder.nativeOrder(); // the index is always written in "native order"
					int size = m.Size;
					int destPos = pos + m.DataParam;
					string[] result = new string[size];
					for (int i = 0; i < size; i++)
					{
						result[i] = header.readHeapString(bb, destPos + i * 16); // 16 byte "heap ids" are in the ByteBuffer
					}

					int index = asbb.addObjectToHeap(result);
					bb.order(ByteOrder.nativeOrder()); // the string index is always written in "native order"
					bb.putInt(destPos, index); // overwrite with the index into the StringHeap

				}
				else if (m.VariableLength)
				{
					int startPos = pos + m.DataParam;
					bb.order(ByteOrder.LITTLE_ENDIAN);

					ByteOrder bo = (ByteOrder)m.DataObject;
					int endian = bo.Equals(ByteOrder.LITTLE_ENDIAN) ? RandomAccessFile.LITTLE_ENDIAN : RandomAccessFile.BIG_ENDIAN;
					// Compute rank and size upto the first (and ideally last) VLEN
					int[] fieldshape = m.Shape;
					int prefixrank = 0;
					int size = 1;
					for (; prefixrank < fieldshape.Length; prefixrank++)
					{
						if (fieldshape[prefixrank] < 0)
						{
							break;
						}
						size *= fieldshape[prefixrank];
					}
					Debug.Assert(size == m.Size, "Internal error: field size mismatch");
					NcArray[] fieldarray = new NcArray[size]; // hold all the vlen instance data
															  // destPos will point to each vlen instance in turn
															  // assuming we have 'size' such instances in a row.
					int destPos = startPos;
					for (int i = 0; i < size; i++)
					{
						// vlenarray extracts the i'th vlen contents (struct not supported).
						NcArray vlenArray = header.readHeapVlen(bb, destPos, m.DataType, endian);
						fieldarray[i] = vlenArray;
						destPos += VLEN_T_SIZE; // Apparently no way to compute VLEN_T_SIZE on the fly
					}
					NcArray result;
					if (prefixrank == 0) // if scalar, return just the singleton vlen array
					{
						result = fieldarray[0];
					}
					else
					{
						int[] newshape = new int[prefixrank];
						System.Array.Copy(fieldshape, 0, newshape, 0, prefixrank);
						// result = Array.makeObjectArray(m.getDataType(), fieldarray[0].getClass(), newshape, fieldarray);
						result = NcArray.makeVlenArray(newshape, fieldarray);
					}
					int index = asbb.addObjectToHeap(result);
					bb.order(ByteOrder.nativeOrder());
					bb.putInt(startPos, index); // overwrite with the index into the Heap
				}
			}
		}

		/// <summary>
		/// Read data subset from file for a variable, create primitive array.
		/// </summary>
		/// <param name="layout"> handles skipping around in the file. </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <param name="shape"> the shape of the output </param>
		/// <param name="fillValue"> fill value as a wrapped primitive </param>
		/// <param name="endian"> byte order </param>
		/// <returns> primitive array with data read in </returns>
		internal virtual object readDataPrimitive(Layout layout, DataType dataType, int[] shape, object fillValue, int endian, bool convertChar)
		{

			if (dataType == DataType.STRING)
			{
				int size = (int)layout.TotalNelems;
				string[] sa = new string[size];
				int count = 0;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					if (chunk == null)
					{
						continue;
					}
					for (int i = 0; i < chunk.Nelems; i++)
					{ // 16 byte "heap ids"
						sa[count++] = header.readHeapString(chunk.SrcPos + layout.ElemSize * i);
					}
				}
				return sa;
			}

			if (dataType == DataType.OPAQUE)
			{
				NcArray opArray = NcArray.factory(DataType.OPAQUE, shape);
				Debug.Assert((new Section(shape).computeSize() == layout.TotalNelems));

				int count = 0;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					if (chunk == null)
					{
						continue;
					}
					int recsize = layout.ElemSize;
					for (int i = 0; i < chunk.Nelems; i++)
					{
						byte[] pa = new byte[recsize];
						raf.seek(chunk.SrcPos + i * recsize);
						raf.readFully(pa, 0, recsize);
						opArray.setObject(count++, ByteBuffer.wrap(pa));
					}
				}
				return opArray;
			}

			// normal case
			return IospHelper.readDataFill(raf, layout, dataType, fillValue, endian, convertChar);
		}

		// old way
		private StructureData readStructure(Structure s, ArrayStructureW asw, long dataPos)
		{
			StructureDataW sdata = new StructureDataW(asw.StructureMembers);
			if (debug)
			{
				Console.WriteLine(" readStructure " + s.getFullName() + " dataPos = " + dataPos);
			}

			foreach (Variable v2 in s.Variables)
			{
				H5headerNew.Vinfo vinfo = (H5headerNew.Vinfo)v2.SPobject;
				if (debug)
				{
					Console.WriteLine(" readStructureMember " + v2.getFullName() + " vinfo = " + vinfo);
				}
				NcArray dataArray = readData(v2, dataPos + vinfo.dataPos, v2.ShapeAsSection);
				sdata.setMemberData(v2.ShortName, dataArray);
			}

			return sdata;
		}

		//////////////////////////////////////////////////////////////////////////
		// override base class

		public override void close()
		{
			base.close();
			header.close();
		}

		public override void reacquire()
		{
			base.reacquire();
			// LOOK headerParser.raf = this.raf;
		}

		public override string toStringDebug(object o)
		{
			if (o is Variable)
			{
				Variable v = (Variable)o;
				H5headerNew.Vinfo vinfo = (H5headerNew.Vinfo)v.SPobject;
				return vinfo.ToString();
			}
			return null;
		}

	}

}
