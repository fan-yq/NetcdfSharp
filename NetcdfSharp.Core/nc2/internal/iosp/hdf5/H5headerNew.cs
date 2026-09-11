// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Signedness = NetcdfSharp.ucar.ma2.Signedness;

/*
 * Copyright (c) 1998-2026 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf5
{
	using ArrayChar = NetcdfSharp.ucar.ma2.ArrayChar;
	using ArrayObject = NetcdfSharp.ucar.ma2.ArrayObject;
	using ArrayStructure = NetcdfSharp.ucar.ma2.ArrayStructure;
	using ArrayStructureBB = NetcdfSharp.ucar.ma2.ArrayStructureBB;
	using AttributeContainer = NetcdfSharp.ucar.nc2.AttributeContainer;
	using AttributeContainerMutable = NetcdfSharp.ucar.nc2.AttributeContainerMutable;
	using Builder = NetcdfSharp.ucar.nc2.Group.Builder;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataBTree = NetcdfSharp.ucar.nc2.iosp.hdf5.DataBTree;
	using DataObject = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.DataObject;
	using DataObjectFacade = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.DataObjectFacade;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using Dimension = NetcdfSharp.ucar.nc2.Dimension;
	using EnumTypedef = NetcdfSharp.ucar.nc2.EnumTypedef;
	using GlobalHeap = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.GlobalHeap;
	using Group = NetcdfSharp.ucar.nc2.Group;
	using H5Group = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.H5Group;
	using H5headerIF = NetcdfSharp.ucar.nc2.iosp.hdf5.H5headerIF;
	using HdfEos = NetcdfSharp.ucar.nc2.@internal.iosp.hdf4.HdfEos;
	using HdfHeaderIF = NetcdfSharp.ucar.nc2.@internal.iosp.hdf4.HdfHeaderIF;
	using HeaderMessage = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.HeaderMessage;
	using HeapIdentifier = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.HeapIdentifier;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using IospHelper = NetcdfSharp.ucar.nc2.iosp.IospHelper;
	using Layout = NetcdfSharp.ucar.nc2.iosp.Layout;
	using LayoutBB = NetcdfSharp.ucar.nc2.iosp.LayoutBB;
	using LayoutRegular = NetcdfSharp.ucar.nc2.iosp.LayoutRegular;
	using MemTracker = NetcdfSharp.ucar.nc2.iosp.hdf5.MemTracker;
	using MessageAttribute = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.MessageAttribute;
	using MessageComment = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.MessageComment;
	using MessageDataspace = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.MessageDataspace;
	using MessageDatatype = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.MessageDatatype;
	using MessageFillValue = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.MessageFillValue;
	using MessageFillValueOld = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.MessageFillValueOld;
	using MessageFilter = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.MessageFilter;
	using MessageType = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.MessageType;
	using N3iospFillValues = NetcdfSharp.ucar.nc2.iosp.netcdf3.N3iospFillValues;
	using Nc4 = NetcdfSharp.ucar.nc2.iosp.netcdf4.Nc4;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using NcIndex = NetcdfSharp.ucar.ma2.NcIndex;
	using NetcdfFileFormat = NetcdfSharp.ucar.nc2.write.NetcdfFileFormat;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using StructureMember = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5objects.StructureMember;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// Read all of the metadata of an HD5 file. </summary>
	public class H5headerNew : H5headerIF, HdfHeaderIF
	{
		private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(H5headerNew));

		// special attribute names in HDF5
		public const string HDF5_CLASS = "CLASS";
		public const string HDF5_DIMENSION_LIST = "DIMENSION_LIST";
		public const string HDF5_DIMENSION_SCALE = "DIMENSION_SCALE";
		public const string HDF5_DIMENSION_LABELS = "DIMENSION_LABELS";
		public const string HDF5_DIMENSION_NAME = "NAME";
		public const string HDF5_REFERENCE_LIST = "REFERENCE_LIST";

		// debugging
		private static bool debugVlen;
		private static bool debug1, debugDetail, debugPos, debugHeap, debugV;
		private static bool debugGroupBtree, debugDataBtree, debugBtree2;
		private static bool debugContinueMessage, debugTracker, debugSoftLink, debugHardLink, debugSymbolTable;
		private static bool warnings = true, debugReference, debugCreationOrder, debugStructure;
		private static bool debugDimensionScales;

		// NULL string value, following netCDF-C, set to NIL
		private const string NULL_STRING_VALUE = "NIL";

		public static bool Warnings
		{
			set
			{
				warnings = value;
			}
		}

		public static NetcdfSharp.ucar.nc2.util.DebugFlags DebugFlags
		{
			set
			{
				debug1 = value.isSet("H5header/header");
				debugBtree2 = value.isSet("H5header/btree2");
				debugContinueMessage = value.isSet("H5header/continueMessage");
				debugDetail = value.isSet("H5header/headerDetails");
				debugDataBtree = value.isSet("H5header/dataBtree");
				debugGroupBtree = value.isSet("H5header/groupBtree");
				debugHeap = value.isSet("H5header/Heap");
				debugPos = value.isSet("H5header/filePos");
				debugReference = value.isSet("H5header/reference");
				debugSoftLink = value.isSet("H5header/softLink");
				debugHardLink = value.isSet("H5header/hardLink");
				debugSymbolTable = value.isSet("H5header/symbolTable");
				debugTracker = value.isSet("H5header/memTracker");
				debugV = value.isSet("H5header/Variable");
				debugStructure = value.isSet("H5header/structure");
			}
		}

		private static readonly byte[] magic = new byte[] { unchecked((byte)0x89), (byte)'H', (byte)'D', (byte)'F', (byte)'\r', (byte)'\n', 0x1a, (byte)'\n' };
		private static readonly string magicString = StringHelper.NewString(magic, Encoding.UTF8);
		private const long maxHeaderPos = 50000; // header's gotta be within this
		private const bool transformReference = true;

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
		public static bool isValidFile(RandomAccessFile raf)
		{
			// fail fast on directory
			if (raf.Directory)
			{
				return false;
			}
			// For HDF5, we need to search forward
			long filePos = 0;
			long size = raf.length();
			while ((filePos < size - 8) && (filePos < maxHeaderPos))
			{
				byte[] buff = new byte[magic.Length];
				raf.seek(filePos);
				if (raf.read(buff) < magic.Length)
				{
					return false;
				}
				if (NetcdfFileFormat.memequal(buff, magic, magic.Length))
				{
					return true;
				}
				// The offsets that the header can be at
				filePos = (filePos == 0) ? 512 : 2 * filePos;
			}
			return false;
		}

		////////////////////////////////////////////////////////////////////////////////


		/*
		 * Implementation notes
		 * any field called address is actually relative to the base address.
		 * any field called filePos or dataPos is a byte offset within the file.
		 *
		 * it appears theres no sure fire way to tell if the file was written by netcdf4 library
		 * 1) if one of the the NETCF4-XXX atts are set
		 * 2) dimension scales:
		 * 1) all dimensions have a dimension scale
		 * 2) they all have the same length as the dimension
		 * 3) all variables' dimensions have a dimension scale
		 */

		private readonly Group.Builder root;
		private readonly H5iospNew h5iosp;

		private long baseAddress;
		internal byte sizeOffsets, sizeLengths;
		internal bool isOffsetLong, isLengthLong;

		/*
		 * Cant always tell if written with netcdf library. if all dimensions have coordinate variables, eg:
		 * Q:/cdmUnitTest/formats/netcdf4/ncom_relo_fukushima_1km_tmp_2011040800_t000.nc4
		 */
		private bool isNetcdf4;
		private H5Group h5rootGroup;
		private IDictionary<string, DataObjectFacade> symlinkMap = new Dictionary<string, DataObjectFacade>(200);
		private IDictionary<long, DataObject> addressMap = new Dictionary<long, DataObject>(200);

		private H5objects h5objects;
		private PrintWriter debugOut;
		private MemTracker memTracker;

		private readonly Charset valueCharset;

		internal H5headerNew(Group.Builder root, H5iospNew h5iosp)
		{
			this.root = root;
			this.h5iosp = h5iosp;
			valueCharset = h5iosp.getValueCharset().orElse(Encoding.UTF8);
		}

		/// <summary>
		/// Return defined <seealso cref="Charset value charset"/> that
		/// will be used by reading HDF5 header.
		/// </summary>
		/// <returns> <seealso cref="Charset charset"/> </returns>
		protected internal virtual Charset ValueCharset
		{
			get
			{
				return valueCharset;
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public void read(NetcdfSharp.java.io.PrintWriter debugPS) throws NetcdfSharp.java.io.IOException
		public virtual void read(PrintWriter debugPS)
		{
			if (debugPS != null)
			{
				debugOut = debugPS;
			}
			else if (debug1 || debugContinueMessage || debugCreationOrder || debugDetail || debugDimensionScales || debugGroupBtree || debugHardLink || debugHeap || debugPos || debugReference || debugTracker || debugV || debugSoftLink || warnings)
			{
				debugOut = new PrintWriter(JSystem.@out);
			}
			h5objects = new H5objects(this, debugOut, memTracker);

			long actualSize = RandomAccessFile.length();

			if (debugTracker)
			{
				memTracker = new MemTracker(actualSize);
			}

			// find the superblock - no limits on how far in
			bool ok = false;
			long filePos = 0;
			while ((filePos < actualSize - 8))
			{
				RandomAccessFile.seek(filePos);
				string magic = RandomAccessFile.readString(8);
				if (magic.Equals(magicString))
				{
					ok = true;
					break;
				}
				filePos = (filePos == 0) ? 512 : 2 * filePos;
			}
			if (!ok)
			{
				throw new IOException("Not a netCDF4/HDF5 file ");
			}
			if (debug1)
			{
				log.debug("H5header opened file to read:'{}' size= {}", RandomAccessFile.Location, actualSize);
			}
			// now we are positioned right after the header

			// header information is in le byte order
			RandomAccessFile.order(RandomAccessFile.LITTLE_ENDIAN);

			long superblockStart = RandomAccessFile.FilePointer - 8;
			if (debugTracker)
			{
				memTracker.add("header", 0, superblockStart);
			}

			// superblock version
			byte versionSB = RandomAccessFile.readByte();

			if (versionSB < 2)
			{
				readSuperBlock1(superblockStart, versionSB);
			}
			else if (versionSB == 2)
			{
				readSuperBlock2(superblockStart);
			}
			else
			{
				throw new IOException("Unknown superblock version= " + versionSB);
			}

			// now look for symbolic links LOOK this doesnt work; probably remove 10/27/14 jc
			replaceSymbolicLinks(h5rootGroup);

			// recursively run through all the dataObjects and add them to the ncfile
			bool allSharedDimensions = makeNetcdfGroup(root, h5rootGroup);
			if (allSharedDimensions)
			{
				isNetcdf4 = true;
			}

			if (debugTracker)
			{
				Formatter f = new Formatter();
				memTracker.report(f);
				log.debug(f.ToString());
			}

			debugOut = null;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void readSuperBlock1(long superblockStart, byte versionSB) throws NetcdfSharp.java.io.IOException
		private void readSuperBlock1(long superblockStart, byte versionSB)
		{
			byte versionFSS, versionGroup, versionSHMF;
			short btreeLeafNodeSize, btreeInternalNodeSize;
			int fileFlags;

			long heapAddress;
			long eofAddress;
			long driverBlockAddress;

			versionFSS = RandomAccessFile.readByte();
			versionGroup = RandomAccessFile.readByte();
			RandomAccessFile.readByte(); // skip 1 byte
			versionSHMF = RandomAccessFile.readByte();
			if (debugDetail)
			{
				log.debug(" versionSB= " + versionSB + " versionFSS= " + versionFSS + " versionGroup= " + versionGroup + " versionSHMF= " + versionSHMF);
			}

			sizeOffsets = RandomAccessFile.readByte();
			isOffsetLong = (sizeOffsets == 8);

			sizeLengths = RandomAccessFile.readByte();
			isLengthLong = (sizeLengths == 8);
			if (debugDetail)
			{
				log.debug(" sizeOffsets= {} sizeLengths= {}", sizeOffsets, sizeLengths);
				log.debug(" isLengthLong= {} isOffsetLong= {}", isLengthLong, isOffsetLong);
			}

			RandomAccessFile.read(); // skip 1 byte
									 // log.debug(" position="+mapBuffer.position());

			btreeLeafNodeSize = RandomAccessFile.readShort();
			btreeInternalNodeSize = RandomAccessFile.readShort();
			if (debugDetail)
			{
				log.debug(" btreeLeafNodeSize= {} btreeInternalNodeSize= {}", btreeLeafNodeSize, btreeInternalNodeSize);
			}
			// log.debug(" position="+mapBuffer.position());

			fileFlags = RandomAccessFile.readInt();
			if (debugDetail)
			{
				log.debug(" fileFlags= 0x{}", Convert.ToString(fileFlags, 16));
			}

			if (versionSB == 1)
			{
				short storageInternalNodeSize = RandomAccessFile.readShort();
				RandomAccessFile.skipBytes(2);
			}

			baseAddress = readOffset();
			heapAddress = readOffset();
			eofAddress = readOffset();
			driverBlockAddress = readOffset();

			if (baseAddress != superblockStart)
			{
				baseAddress = superblockStart;
				eofAddress += superblockStart;
				if (debugDetail)
				{
					log.debug(" baseAddress set to superblockStart");
				}
			}

			if (debugDetail)
			{
				log.debug(" baseAddress= 0x{}", Convert.ToString(baseAddress, 16));
				log.debug(" global free space heap Address= 0x{}", Convert.ToString(heapAddress, 16));
				log.debug(" eof Address={}", eofAddress);
				log.debug(" raf length= {}", RandomAccessFile.length());
				log.debug(" driver BlockAddress= 0x{}", Convert.ToString(driverBlockAddress, 16));
				log.debug("");
			}
			if (debugTracker)
			{
				memTracker.add("superblock", superblockStart, RandomAccessFile.FilePointer);
			}

			// look for file truncation
			long fileSize = RandomAccessFile.length();
			if (fileSize < eofAddress)
			{
				throw new IOException("File is truncated should be= " + eofAddress + " actual = " + fileSize + "%nlocation= " + RandomAccessFile.Location);
			}

			// next comes the root object's SymbolTableEntry
			// extract the root group object, recursively read all objects
			h5rootGroup = h5objects.readRootSymbolTable(RandomAccessFile.FilePointer);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void readSuperBlock2(long superblockStart) throws NetcdfSharp.java.io.IOException
		private void readSuperBlock2(long superblockStart)
		{
			sizeOffsets = RandomAccessFile.readByte();
			isOffsetLong = (sizeOffsets == 8);

			sizeLengths = RandomAccessFile.readByte();
			isLengthLong = (sizeLengths == 8);
			if (debugDetail)
			{
				log.debug(" sizeOffsets= {} sizeLengths= {}", sizeOffsets, sizeLengths);
				log.debug(" isLengthLong= {} isOffsetLong= {}", isLengthLong, isOffsetLong);
			}

			byte fileFlags = RandomAccessFile.readByte();
			if (debugDetail)
			{
				log.debug(" fileFlags= 0x{}", Convert.ToString(fileFlags, 16));
			}

			baseAddress = readOffset();
			long extensionAddress = readOffset();
			long eofAddress = readOffset();
			long rootObjectAddress = readOffset();
			int checksum = RandomAccessFile.readInt();

			if (debugDetail)
			{
				log.debug(" baseAddress= 0x{}", Convert.ToString(baseAddress, 16));
				log.debug(" extensionAddress= 0x{}", Convert.ToString(extensionAddress, 16));
				log.debug(" eof Address={}", eofAddress);
				log.debug(" rootObjectAddress= 0x{}", Convert.ToString(rootObjectAddress, 16));
				log.debug("");
			}

			if (debugTracker)
			{
				memTracker.add("superblock", superblockStart, RandomAccessFile.FilePointer);
			}

			if (baseAddress != superblockStart)
			{
				baseAddress = superblockStart;
				eofAddress += superblockStart;
				if (debugDetail)
				{
					log.debug(" baseAddress set to superblockStart");
				}
			}

			// look for file truncation
			long fileSize = RandomAccessFile.length();
			if (fileSize < eofAddress)
			{
				throw new IOException("File is truncated should be= " + eofAddress + " actual = " + fileSize);
			}

			h5rootGroup = h5objects.readRootObject(rootObjectAddress);
		}

		private void replaceSymbolicLinks(H5Group group)
		{
			if (group == null)
			{
				return;
			}

			IList<DataObjectFacade> objList = group.nestedObjects;
			int count = 0;
			while (count < objList.Count)
			{
				DataObjectFacade dof = objList[count];

				if (dof.group != null)
				{ // group - recurse
					replaceSymbolicLinks(dof.group);

				}
				else if (dof.linkName != null)
				{ // symbolic links
					DataObjectFacade link;
					if (!symlinkMap.TryGetValue(dof.linkName, out link))
					{
						link = null;
					}
					if (link == null)
					{
						log.warn(" WARNING Didnt find symbolic link={} from {}", dof.linkName, dof.name);
						objList.RemoveAt(count);
						continue;
					}

					// dont allow loops
					if (link.group != null)
					{
						if (group.isChildOf(link.group))
						{
							log.warn(" ERROR Symbolic Link loop found ={}", dof.linkName);
							objList.RemoveAt(count);
							continue;
						}
					}

					// dont allow in the same group. better would be to replicate the group with the new name
					if (dof.parent == link.parent)
					{
						objList.Remove(dof);
						count--; // negate the incr
					}
					else // replace
					{
						objList[count] = link;
					}

					if (debugSoftLink)
					{
						log.debug("  Found symbolic link={}", dof.linkName);
					}
				}

				count++;
			}
		}

		internal virtual void addSymlinkMap(string name, DataObjectFacade facade)
		{
			symlinkMap[name] = facade;
		}

		///////////////////////////////////////////////////////////////
		// construct netcdf objects

		private bool makeNetcdfGroup(Group.Builder parentGroup, H5Group h5group)
		{

			/*
			 * 6/21/2013 new algorithm for dimensions.
			 * 1. find all objects with all CLASS = "DIMENSION_SCALE", make into a dimension. use shape(0) as length. keep in
			 * order
			 * 2. if also a variable (NAME != "This is a ...") then first dim = itself, second matches length, if multiple
			 * match, use :_Netcdf4Coordinates = 0, 3 and order of dimensions.
			 * 3. use DIMENSION_LIST to assign dimensions to data variables.
			 */

			// 1. find all objects with all CLASS = "DIMENSION_SCALE", make into a dimension. use shape(0) as length. keep in
			// order
			foreach (DataObjectFacade facade in h5group.nestedObjects)
			{
				if (facade.isVariable)
				{
					findDimensionScales(parentGroup, h5group, facade);
				}
			}

			// 2. if also a variable (NAME != "This is a ...") then first dim = itself, second matches length, if multiple
			// match, use :_Netcdf4Coordinates = 0, 3 and order of dimensions.
			foreach (DataObjectFacade facade in h5group.nestedObjects)
			{
				if (facade.is2DCoordinate)
				{
					findDimensionScales2D(h5group, facade);
				}
			}

			bool allHaveSharedDimensions = true;

			// 3. use DIMENSION_LIST to assign dimensions to other variables.
			foreach (DataObjectFacade facade in h5group.nestedObjects)
			{
				if (facade.isVariable)
				{
					allHaveSharedDimensions &= findSharedDimensions(parentGroup, h5group, facade);
				}
			}

			createDimensions(parentGroup, h5group);

			// process types first
			foreach (DataObjectFacade facadeNested in h5group.nestedObjects)
			{
				if (facadeNested.isTypedef)
				{
					if (debugReference && facadeNested.dobj.mdt.type == 7)
					{
						log.debug("{}", facadeNested);
					}

					if (facadeNested.dobj.mdt.map != null)
					{
						EnumTypedef enumTypedef = parentGroup.findEnumTypedef(facadeNested.name, true).orElse(null);
						if (enumTypedef == null)
						{
							DataType basetype;
							switch (facadeNested.dobj.mdt.byteSize)
							{
								case 1:
									basetype = DataType.ENUM1;
									break;
								case 2:
									basetype = DataType.ENUM2;
									break;
								default:
									basetype = DataType.ENUM4;
									break;
							}
							enumTypedef = new EnumTypedef(facadeNested.name, facadeNested.dobj.mdt.map, basetype);
							parentGroup.addEnumTypedef(enumTypedef);
						}
					}
					if (debugV)
					{
						log.debug("  made enumeration {}", facadeNested.name);
					}
				}

			} // loop over typedefs

			// nested objects - groups and variables
			foreach (DataObjectFacade facadeNested in h5group.nestedObjects)
			{

				if (facadeNested.isGroup)
				{
					H5Group h5groupNested = h5objects.readH5Group(facadeNested);
					if (facadeNested.group == null) // hard link with cycle
					{
						continue; // just skip it
					}
					Group.Builder nestedGroup = Group.builder().setName(facadeNested.name);
					parentGroup.addGroup(nestedGroup);
					allHaveSharedDimensions &= makeNetcdfGroup(nestedGroup, h5groupNested);
					if (debug1)
					{
						log.debug("--made Group " + nestedGroup.shortName + " add to " + parentGroup.shortName);
					}

				}
				else if (facadeNested.isVariable)
				{
					if (debugReference && facadeNested.dobj.mdt.type == 7)
					{
						log.debug("{}", facadeNested);
					}

					Variable.Builder v = makeVariable(parentGroup, facadeNested);
					if (v != null)
					{
						parentGroup.addVariable(v);

						if (v.dataType.isEnum())
						{
							string enumTypeName = v.getEnumTypeName();
							if (enumTypeName == null)
							{
								log.warn("EnumTypedef is missing for variable: {}", v.shortName);
								throw new System.InvalidOperationException("EnumTypedef is missing for variable: " + v.shortName);
							}
							// This code apparently addresses the possibility of an anonymous enum LOOK ??
							if (enumTypeName.Length == 0)
							{
								EnumTypedef enumTypedef = parentGroup.findEnumTypedef(facadeNested.name, true).get();
								if (enumTypedef == null)
								{
									enumTypedef = new EnumTypedef(facadeNested.name, facadeNested.dobj.mdt.map);
									parentGroup.addEnumTypedef(enumTypedef);
									v.setEnumTypeName(enumTypedef.ShortName);
								}
							}
						}

						Vinfo vinfo = (Vinfo)v.spiObject;
						if (debugV)
						{
							log.debug("  made Variable " + v.shortName + "  vinfo= " + vinfo + "\n" + v);
						}
					}
				}

			} // loop over nested objects

			// create group attributes last. need enums to be found first
			IList<MessageAttribute> fatts = filterAttributes(h5group.facade.dobj.attributes);
			foreach (MessageAttribute matt in fatts)
			{
				try
				{
					makeAttributes(null, matt, parentGroup.getAttributeContainer());
				}
				catch (InvalidRangeException e)
				{
					throw new IOException(e.Message);
				}
			}

			// add system attributes
			processSystemAttributes(h5group.facade.dobj.messages, parentGroup.getAttributeContainer());
			return allHaveSharedDimensions;
		}

		/////////////////////////
		/*
		 * from https://www.unidata.ucar.edu/software/netcdf/docs/netcdf.html#NetCDF_002d4-Format
		 * C.3.7 Attributes
		 *
		 * Attributes in HDF5 and netCDF-4 correspond very closely. Each attribute in an HDF5 file is represented as an
		 * attribute
		 * in the netCDF-4 file, with the exception of the attributes below, which are ignored by the netCDF-4 API.
		 *
		 * _Netcdf4Coordinates An integer array containing the dimension IDs of a variable which is a multi-dimensional
		 * coordinate variable.
		 * _nc3_strict When this (scalar, H5T_NATIVE_INT) attribute exists in the root group of the HDF5 file, the netCDF API
		 * will enforce
		 * the netCDF classic model on the data file.
		 * REFERENCE_LIST This attribute is created and maintained by the HDF5 dimension scale API.
		 * CLASS This attribute is created and maintained by the HDF5 dimension scale API.
		 * DIMENSION_LIST This attribute is created and maintained by the HDF5 dimension scale API.
		 * NAME This attribute is created and maintained by the HDF5 dimension scale API.
		 *
		 * ----------
		 * from dim_scales_wk9 - Nunes.ppt
		 *
		 * Attribute named "CLASS" with the value "DIMENSION_SCALE"
		 * Optional attribute named "NAME"
		 * Attribute references to any associated Dataset
		 *
		 * -------------
		 * from https://www.unidata.ucar.edu/mailing_lists/archives/netcdfgroup/2008/msg00093.html
		 *
		 * Then comes the part you will have to do for your datasets. You open the data
		 * dataset, get an ID, DID variable here, open the latitude dataset, get its ID,
		 * DSID variable here, and "link" the 2 with this call
		 *
		 * if (H5DSattach_scale(did,dsid,DIM0) < 0)
		 *
		 * what this function does is to associated the dataset DSID (latitude) with the
		 * dimension* specified by the parameter DIM0 (0, in this case, the first
		 * dimension of the 2D array) of the dataset DID
		 *
		 * If you open HDF Explorer and expand the attributes of the "data" dataset you
		 * will see an attribute called DIMENSION_LIST.
		 * This is done by this function. It is an array that contains 2 HDF5 references,
		 * one for the latitude dataset, other for the longitude)
		 *
		 * If you expand the "lat" dataset , you will see that it contains an attribute
		 * called REFERENCE_LIST. It is a compound type that contains
		 * 1) a reference to my "data" dataset
		 * 2) the index of the data dataset this scale is to be associated with (0
		 * for the lat, 1 for the lon)
		 */

		// find the Dimension Scale objects, turn them into shared dimensions
		// always has attribute CLASS = "DIMENSION_SCALE"
		// note that we dont bother looking at their REFERENCE_LIST
		private void findDimensionScales(Group.Builder g, H5Group h5group, DataObjectFacade facade)
		{
			System.Collections.IEnumerator iter = facade.dobj.attributes.GetEnumerator();
			while (iter.MoveNext())
			{
				MessageAttribute matt = (MessageAttribute)iter.Current;
				if (matt.name.Equals(HDF5_CLASS))
				{
					NcAttribute att = makeAttribute(matt);
					if (att == null)
					{
						throw new System.InvalidOperationException();
					}
					string val = att.getStringValue();
					if (val.Equals(HDF5_DIMENSION_SCALE) && facade.dobj.mds.ndims > 0)
					{

						// create a dimension - always use the first dataspace length
						facade.dimList = addDimension(g, h5group, facade.name, facade.dobj.mds.dimLength[0], facade.dobj.mds.maxLength[0] == -1);
						facade.hasNetcdfDimensions = true;
						if (!h5iosp.includeOriginalAttributes)
						{
							// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
							// iter.remove();
						}

						if (facade.dobj.mds.ndims > 1)
						{
							facade.is2DCoordinate = true;
						}
					}
				}
			}
		}

		private void findDimensionScales2D(H5Group h5group, DataObjectFacade facade)
		{
			int[] lens = facade.dobj.mds.dimLength;
			if (lens.Length > 2)
			{
				log.warn("DIMENSION_LIST: dimension scale > 2 = {}", facade.Name);
				return;
			}

			// first dimension is itself
			string name = facade.Name;
			int pos = name.LastIndexOf('/');
			string dimName = (pos >= 0) ? name.Substring(pos + 1) : name;

			StringBuilder sbuff = new StringBuilder();
			sbuff.Append(dimName);
			sbuff.Append(" ");

			// second dimension is really an anonymous dimension, ironically now we go through amazing hoops to keep it shared
			// 1. use dimids if they exist
			// 2. if length matches and unique, use it
			// 3. if no length matches or multiple matches, then use anonymous

			int want_len = lens[1]; // second dimension
			Dimension match = null;
			bool unique = true;
			foreach (Dimension d in h5group.dimList)
			{
				if (d.Length == want_len)
				{
					if (match == null)
					{
						match = d;
					}
					else
					{
						unique = false;
					}
				}
			}
			if (match != null && unique)
			{
				sbuff.Append(match.ShortName); // 2. if length matches and unique, use it

			}
			else
			{
				if (match == null)
				{ // 3. if no length matches or multiple matches, then use anonymous
					log.warn("DIMENSION_LIST: dimension scale {} has second dimension {} but no match", facade.Name, want_len);
					sbuff.Append(want_len);
				}
				else
				{
					log.warn("DIMENSION_LIST: dimension scale {} has second dimension {} but multiple matches", facade.Name, want_len);
					sbuff.Append(want_len);
				}
			}

			facade.dimList = sbuff.ToString();
		}

		// look for references to dimension scales, ie the variables that use them
		// return true if this variable is compatible with netcdf4 data model
		private bool findSharedDimensions(Group.Builder g, H5Group h5group, DataObjectFacade facade)
		{
			System.Collections.IEnumerator iter = facade.dobj.attributes.GetEnumerator();
			while (iter.MoveNext())
			{
				MessageAttribute matt = (MessageAttribute)iter.Current;
				// find the dimensions - set length to maximum
				// DIMENSION_LIST contains, for each dimension, a list of references to Dimension Scales
				switch (matt.name)
				{
					case HDF5_DIMENSION_LIST: // references : may extend the dimension length
						{
							NcAttribute att = makeAttribute(matt); // this reads in the data

							if (att == null)
							{
								log.warn("DIMENSION_LIST: failed to read on variable {}", facade.Name);

							}
							else if (att.Length != facade.dobj.mds.dimLength.Length)
							{ // some attempts to writing hdf5 directly fail here
								log.warn("DIMENSION_LIST: must have same number of dimension scales as dimensions att={} on variable {}", att, facade.Name);

							}
							else
							{
								StringBuilder sbuff = new StringBuilder();
								for (int i = 0; i < att.Length; i++)
								{
									string name = att.getStringValue(i);
									string dimName = extendDimension(g, h5group, name, facade.dobj.mds.dimLength[i]);
									sbuff.Append(dimName).Append(" ");
								}
								facade.dimList = sbuff.ToString();
								facade.hasNetcdfDimensions = true;
								if (debugDimensionScales)
								{
									log.debug("Found dimList '{}' for group '{}' matt={}", facade.dimList, g.shortName, matt);
								}
								if (!h5iosp.includeOriginalAttributes)
								{
									// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
									// iter.remove();
								}
							}

							break;
						}
					case HDF5_DIMENSION_NAME:
						{
							NcAttribute att = makeAttribute(matt);
							if (att == null)
							{
								throw new System.InvalidOperationException();
							}
							string val = att.getStringValue();
							if (val.StartsWith("This is a netCDF dimension but not a netCDF variable", StringComparison.Ordinal))
							{
								facade.isVariable = false;
								isNetcdf4 = true;
							}
							if (!h5iosp.includeOriginalAttributes)
							{
								// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
								// iter.remove();
							}
							if (debugDimensionScales)
							{
								log.debug("Found {}", val);
							}

							break;
						}
					case HDF5_REFERENCE_LIST:
						if (!h5iosp.includeOriginalAttributes)
						{
							// JAVA TO C# CONVERTER TASK: .NET enumerators are read-only:
							// iter.remove();
						}
						break;
				}
			}
			return facade.hasNetcdfDimensions || facade.dobj.mds.dimLength.Length == 0;

		}

		// add a dimension, return its name
		private string addDimension(Group.Builder parent, H5Group h5group, string name, int length, bool isUnlimited)
		{
			int pos = name.LastIndexOf('/');
			string dimName = (pos >= 0) ? name.Substring(pos + 1) : name;

			Dimension d;
			if (!h5group.dimMap.TryGetValue(dimName, out d))
			{
				d = null;
			}
			if (d == null)
			{ // create if not found
				d = Dimension.builder().setName(name).setIsUnlimited(isUnlimited).setLength(length).build();
				h5group.dimMap[dimName] = d;
				h5group.dimList.Add(d);
				parent.addDimension(d);
				if (debugDimensionScales)
				{
					log.debug("addDimension name=" + name + " dim= " + d + " to group " + parent.shortName);
				}

			}
			else
			{ // check has correct length
				if (d.Length != length)
				{
					throw new System.InvalidOperationException("addDimension: DimScale has different length than dimension it references dimScale=" + dimName);
				}
			}

			return d.ShortName;
		}

		// look for unlimited dimensions without dimension scale - must get length from the variable
		private string extendDimension(Group.Builder parent, H5Group h5group, string name, int length)
		{
			int pos = name.LastIndexOf('/');
			string dimName = (pos >= 0) ? name.Substring(pos + 1) : name;

			Dimension d;
			if (!h5group.dimMap.TryGetValue(dimName, out d))
			{
				d = null;
			}
			if (d == null)
			{
				d = parent.findDimension(dimName).orElse(null); // then look in parent groups
			}

			if (d != null)
			{
				if (d.Unlimited && (length > d.Length))
				{
					parent.replaceDimension(d.toBuilder().setLength(length).build());
				}

				if (!d.Unlimited && (length != d.Length))
				{
					throw new System.InvalidOperationException("extendDimension: DimScale has different length than dimension it references dimScale=" + dimName);
				}
				return d.ShortName;
			}

			return dimName;
		}

		private void createDimensions(Group.Builder g, H5Group h5group)
		{
			foreach (Dimension d in h5group.dimList)
			{
				g.addDimensionIfNotExists(d);
			}
		}

		private IList<MessageAttribute> filterAttributes(IList<MessageAttribute> attList)
		{
			IList<MessageAttribute> result = new List<MessageAttribute>(attList.Count);
			foreach (MessageAttribute matt in attList)
			{
				if (matt.name.Equals(Nc4.NETCDF4_COORDINATES) || matt.name.Equals(Nc4.NETCDF4_DIMID) || matt.name.Equals(Nc4.NETCDF4_STRICT))
				{
					isNetcdf4 = true;
				}
				else
				{
					result.Add(matt);
				}
			}
			return result;
		}

		/// <summary>
		/// Create Attribute objects from the MessageAttribute and add to list
		/// </summary>
		/// <param name="sb"> if attribute for a Structure, then deconstruct and add to member variables </param>
		/// <param name="matt"> attribute message </param>
		/// <param name="attContainer"> add Attribute to this </param>
		private void makeAttributes(Structure.BuilderStruct sb, MessageAttribute matt, AttributeContainerMutable attContainer)
		{
			MessageDatatype mdt = matt.mdt;

			if (mdt.type == 6)
			{ // structure
				Vinfo vinfo = new Vinfo(this, matt.mdt, matt.mds, matt.dataPos);
				ArrayStructure attData = (ArrayStructure)readAttributeData(matt, vinfo, DataType.STRUCTURE);

				if (null == sb)
				{
					// flatten and add to list
					foreach (StructureMembers.Member sm in attData.StructureMembers.Members)
					{
						NcArray memberData = attData.extractMemberArray(sm);
						attContainer.addAttribute(new NcAttribute(matt.name + "." + sm.Name, memberData));
					}

				}
				else if (matt.name.Equals(CDM.FIELD_ATTS))
				{
					// flatten and add to list
					foreach (StructureMembers.Member sm in attData.StructureMembers.Members)
					{
						string memberName = sm.Name;
						int pos = memberName.IndexOf(":");
						if (pos < 0)
						{
							continue; // LOOK
						}
						string fldName = memberName.Substring(0, pos);
						string attName = memberName.Substring(pos + 1);
						NcArray memberData = attData.extractMemberArray(sm);
						Variable.Builder vb = sb.findMemberVariable(fldName);
						if (vb != null)
						{
							vb.getAttributeContainer().addAttribute(new NcAttribute(attName, memberData));
						}
					}

				}
				else
				{ // assign separate attribute for each member
					StructureMembers attMembers = attData.StructureMembers;
					foreach (Variable.Builder v in sb.vbuilders)
					{
						// does the compound attribute have a member with same name as nested variable ?
						StructureMembers.Member sm = attMembers.findMember(v.shortName);
						if (null != sm)
						{
							// if so, add the att to the member variable, using the name of the compound attribute
							NcArray memberData = attData.extractMemberArray(sm);
							v.addAttribute(new NcAttribute(matt.name, memberData)); // LOOK check for missing values
						}
					}

					// look for unassigned members, add to the list
					foreach (StructureMembers.Member sm in attData.StructureMembers.Members)
					{
						Variable.Builder vb = sb.findMemberVariable(sm.Name);
						if (vb == null)
						{
							NcArray memberData = attData.extractMemberArray(sm);
							attContainer.addAttribute(new NcAttribute(matt.name + "." + sm.Name, memberData));
						}
					}
				}

			}
			else
			{
				// make a single attribute
				NcAttribute att = makeAttribute(matt);
				if (att != null)
				{
					attContainer.addAttribute(att);
				}
			}

			// reading attribute values might change byte order during a read
			// put back to little endian for further header processing
			RandomAccessFile.order(RandomAccessFile.LITTLE_ENDIAN);
		}

		private NcAttribute makeAttribute(MessageAttribute matt)
		{
			Vinfo vinfo = new Vinfo(this, matt.mdt, matt.mds, matt.dataPos);
			DataType? dtype = vinfo.NCDataType;

			// check for empty attribute case
			if (matt.mds.type == 2)
			{
				if (dtype == DataType.CHAR)
				{
					return new NcAttribute(matt.name, DataType.STRING); // empty char considered to be a null string attr
				}
				else
				{
					return new NcAttribute(matt.name, dtype.Value);
				}
			}

			NcArray attData;
			try
			{
				attData = readAttributeData(matt, vinfo, dtype);

			}
			catch (InvalidRangeException)
			{
				log.warn("failed to read Attribute " + matt.name + " HDF5 file=" + RandomAccessFile.Location);
				return null;
			}

			NcAttribute result;
			if (attData.isVlen())
			{
				IList<object> dataList = new List<object>();
				while (attData.hasNext())
				{
					NcArray nested = (NcArray)attData.next();
					while (nested.hasNext())
					{
						dataList.Add(nested.next());
					}
				}
				result = new NcAttribute(matt.name, (System.Collections.IList)dataList, matt.mdt.unsigned);

			}
			else
			{
				result = new NcAttribute(matt.name, attData);
			}

			RandomAccessFile.order(RandomAccessFile.LITTLE_ENDIAN);
			return result;
		}

		// read attribute values without creating a Variable
		private NcArray readAttributeData(MessageAttribute matt, Vinfo vinfo, DataType? dataType)
		{
			int[] shape = matt.mds.dimLength;

			// Structures
			if (dataType == DataType.STRUCTURE)
			{
				bool hasStrings = false;

				StructureMembers.Builder builder = StructureMembers.builder().setName(matt.name);
				foreach (StructureMember h5sm in matt.mdt.members)
				{

					// from tkunicki@usgs.gov 2/19/2010 - fix for compound attributes
					// DataType dt = getNCtype(h5sm.mdt.type, h5sm.mdt.byteSize);
					// StructureMembers.Member m = sm.addMember(h5sm.name, null, null, dt, new int[] {1});

					DataType? dt;
					int[] dim;
					switch (h5sm.mdt.type)
					{
						case 9: // STRING
							dt = DataType.STRING;
							dim = new int[] { 1 };
							break;
						case 10: // ARRAY
							dt = getNCtype(h5sm.mdt.@base.type, h5sm.mdt.@base.byteSize, h5sm.mdt.unsigned);
							dim = h5sm.mdt.dim;
							break;
						default: // PRIMITIVE
							dt = getNCtype(h5sm.mdt.type, h5sm.mdt.byteSize, h5sm.mdt.unsigned);
							dim = new int[] { 1 };
							break;
					}
					if (dt == null)
					{
						log.debug("SKIPPING DataType for structure member " + h5sm.name);
						continue;
					}
					StructureMembers.MemberBuilder mb = builder.addMember(h5sm.name, null, null, dt.Value, dim);

					if (h5sm.mdt.endian >= 0) // apparently each member may have separate byte order (!!!??)
					{
						mb.setDataObject(h5sm.mdt.endian == RandomAccessFile.LITTLE_ENDIAN ? ByteOrder.LITTLE_ENDIAN : ByteOrder.BIG_ENDIAN);
					}
					mb.setDataParam(h5sm.offset); // offset since start of Structure
					if (dt == DataType.STRING)
					{
						hasStrings = true;
					}
				}

				int recsize = matt.mdt.byteSize;
				Layout layoutA = new LayoutRegular(matt.dataPos, recsize, shape, new Section(shape));
				builder.setStructureSize(recsize);
				StructureMembers sm = builder.build();

				// place data into an ArrayStructureBB for efficiency
				ArrayStructureBB asbb = new ArrayStructureBB(sm, shape);
				byte[] byteArray = asbb.ByteBuffer.array();
				while (layoutA.hasNext())
				{
					Layout.Chunk chunk = layoutA.next();
					if (chunk == null)
					{
						continue;
					}
					if (debugStructure)
					{
						log.debug(" readStructure " + matt.name + " chunk= " + chunk + " index.getElemSize= " + layoutA.ElemSize);
					}

					// copy bytes directly into the underlying byte[]
					RandomAccessFile.seek(chunk.SrcPos);
					RandomAccessFile.readFully(byteArray, (int)chunk.DestElem * recsize, chunk.Nelems * recsize);
				}

				// strings are stored on the heap, and must be read separately
				if (hasStrings)
				{
					int destPos = 0;
					for (int i = 0; i < layoutA.TotalNelems; i++)
					{ // loop over each structure
						h5iosp.convertHeap(asbb, destPos, sm);
						destPos += layoutA.ElemSize;
					}
				}
				return asbb;
			} // Structure case

			// Strings
			if ((vinfo.typeInfo.hdfType == 9) && (vinfo.typeInfo.isVString))
			{
				Layout layoutB = new LayoutRegular(matt.dataPos, matt.mdt.byteSize, shape, new Section(shape));
				ArrayObject.D1 dataB = (ArrayObject.D1)NcArray.factory(DataType.STRING, new int[] { (int)layoutB.TotalNelems });
				int count = 0;
				while (layoutB.hasNext())
				{
					Layout.Chunk chunk = layoutB.next();
					if (chunk == null)
					{
						continue;
					}
					for (int i = 0; i < chunk.Nelems; i++)
					{
						long address = chunk.SrcPos + layoutB.ElemSize * i;
						string sval = readHeapString(address);
						dataB.set(count++, sval);
					}
				}
				return dataB;
			} // vlen case

			// Vlen (non-String)
			if (vinfo.typeInfo.hdfType == 9)
			{ // vlen
				int endianC = vinfo.typeInfo.endian;
				DataType readType = dataType.Value;
				if (vinfo.typeInfo.@base.hdfType == 7)
				{ // reference
					readType = DataType.LONG;
					endianC = 1; // apparently always LE
				}

				Layout layoutC = new LayoutRegular(matt.dataPos, matt.mdt.byteSize, shape, new Section(shape));

				// general case is to read an array of vlen objects
				// each vlen generates an NcArray - so return ArrayObject of NcArray
				bool scalar = layoutC.TotalNelems == 1; // if scalar, return just the len NcArray
				NcArray[] dataC = new NcArray[(int)layoutC.TotalNelems];
				int count = 0;
				while (layoutC.hasNext())
				{
					Layout.Chunk chunk = layoutC.next();
					if (chunk == null)
					{
						continue;
					}
					for (int i = 0; i < chunk.Nelems; i++)
					{
						long address = chunk.SrcPos + layoutC.ElemSize * i;
						NcArray vlenArray = getHeapDataArray(address, readType, endianC);
						if (vinfo.typeInfo.@base.hdfType == 7)
						{
							dataC[count++] = h5iosp.convertReference(vlenArray);
						}
						else
						{
							dataC[count++] = vlenArray;
						}
					}
				}
				// return (scalar) ? data[0] : Array.makeObjectArray(readType, data[0].getClass(), shape, data);
				return (scalar) ? dataC[0] : NcArray.makeVlenArray(shape, dataC);

			} // vlen case

			// NON-STRUCTURE CASE
			DataType readDtype = dataType.Value;
			int elemSize = dataType.Value.getSize();
			int endian = vinfo.typeInfo.endian;

			if (vinfo.typeInfo.hdfType == 2)
			{ // time
				readDtype = vinfo.mdt.timeType;
				elemSize = readDtype.getSize();

			}
			else if (vinfo.typeInfo.hdfType == 3)
			{ // char
				if (vinfo.mdt.byteSize > 1)
				{
					int[] newShape = new int[shape.Length + 1];
					System.Array.Copy(shape, 0, newShape, 0, shape.Length);
					newShape[shape.Length] = vinfo.mdt.byteSize;
					shape = newShape;
				}

			}
			else if (vinfo.typeInfo.hdfType == 5)
			{ // opaque
				elemSize = vinfo.mdt.byteSize;

			}
			else if (vinfo.typeInfo.hdfType == 8)
			{ // enum
				TypeInfo baseInfo = vinfo.typeInfo.@base;
				readDtype = baseInfo.dataType.Value;
				elemSize = readDtype.getSize();
				endian = baseInfo.endian;
			}

			Layout layout = new LayoutRegular(matt.dataPos, elemSize, shape, new Section(shape));
			object data = h5iosp.readDataPrimitive(layout, dataType.Value, shape, null, endian, false);
			NcArray dataArray;

			if ((dataType == DataType.CHAR))
			{
				if (vinfo.mdt.byteSize > 1)
				{ // chop back into pieces
					byte[] bdata = (byte[])data;
					int strlen = vinfo.mdt.byteSize;
					int n = bdata.Length / strlen;
					ArrayObject.D1 sarray = (ArrayObject.D1)NcArray.factory(DataType.STRING, new int[] { n });
					for (int i = 0; i < n; i++)
					{
						string sval = convertString(bdata, i * strlen, strlen);
						sarray.set(i, sval);
					}
					dataArray = sarray;

				}
				else
				{
					string sval = convertString((byte[])data);
					ArrayObject.D1 sarray = (ArrayObject.D1)NcArray.factory(DataType.STRING, new int[] { 1 });
					sarray.set(0, sval);
					dataArray = sarray;
				}

			}
			else
			{
				dataArray = (data is NcArray) ? (NcArray)data : NcArray.factory(readDtype, shape, data);
			}

			// convert attributes to enum strings
			if ((vinfo.typeInfo.hdfType == 8) && (matt.mdt.map != null))
			{
				dataArray = convertEnums(matt.mdt.map, dataType.Value, dataArray);
			}

			return dataArray;
		}

		private string convertString(byte[] b)
		{
			// null terminates
			int count = 0;
			while (count < b.Length)
			{
				if (b[count] == 0)
				{
					break;
				}
				count++;
			}
			return StringHelper.NewString(b, 0, count, valueCharset); // all strings are considered to be UTF-8 unicode
		}

		private string convertString(byte[] b, int start, int len)
		{
			// null terminates
			int count = start;
			while (count < start + len)
			{
				if (b[count] == 0)
				{
					break;
				}
				count++;
			}
			return StringHelper.NewString(b, start, count - start, valueCharset); // all strings are considered to be UTF-8
																				  // unicode
		}

		protected internal virtual NcArray convertEnums(IDictionary<int, string> map, DataType dataType, NcArray values)
		{
			int nelems = (int)values.Size;
			string[] sa = new string[nelems];
			int count = 0;
			values.resetLocalIterator();
			while (values.hasNext())
			{
				int ival;
				if (dataType == DataType.ENUM1)
				{
					ival = DataTypes.unsignedByteToShort(values.nextByte());
				}
				else if (dataType == DataType.ENUM2)
				{
					ival = DataTypes.unsignedShortToInt(values.nextShort());
				}
				else
				{
					ival = values.nextInt();
				}
				string sval;
				map.TryGetValue(ival, out sval);
				if (sval == null)
				{
					sval = "Unknown enum value=" + ival;
				}
				sa[count++] = sval;
			}
			return NcArray.factory(DataType.STRING, values.Shape, sa);
		}

		private Variable.Builder makeVariable(Group.Builder parentGroup, DataObjectFacade facade)
		{
			try
			{
				Vinfo vinfo = new Vinfo(this, facade);
				if (vinfo.NCDataType == null)
				{
					log.debug("SKIPPING DataType= " + vinfo.typeInfo.hdfType + " for variable " + facade.name);
					return null;
				}

				NcAttribute fillAttribute = null;
				foreach (HeaderMessage mess in facade.dobj.messages)
				{
					if (mess.mtype == MessageType.FillValue)
					{
						MessageFillValue fvm = (MessageFillValue)mess.messData;
						if (fvm.hasFillValue)
						{
							vinfo.fillValue = fvm.value;
						}
					}
					else if (mess.mtype == MessageType.FillValueOld)
					{
						MessageFillValueOld fvm = (MessageFillValueOld)mess.messData;
						if (fvm.size > 0)
						{
							vinfo.fillValue = fvm.value;
						}
					}

					object fillValue = vinfo.getFillValueNonDefault();
					if (fillValue != null)
					{
						object defFillValue = N3iospFillValues.getFillValueDefault(vinfo.typeInfo.dataType.Value);
						if (!fillValue.Equals(defFillValue))
						{
							fillAttribute = new NcAttribute(CDM.FILL_VALUE, fillValue, vinfo.typeInfo.unsigned);
						}
					}
				}

				long dataAddress = facade.dobj.msl.dataAddress;

				// deal with unallocated data
				if (dataAddress == -1)
				{
					vinfo.useFillValue_Conflict = true;

					// if didnt find, use zeroes !!
					if (vinfo.fillValue == null)
					{
						vinfo.fillValue = new byte[vinfo.typeInfo.dataType.Value.getSize()];
					}
				}

				Variable.Builder vb;
				Structure.BuilderStruct sb = null;
				if (facade.dobj.mdt.type == 6)
				{ // Compound
					string vname = facade.name;
					vb = sb = Structure.builder1().setName1(vname);
					vb.setParentGroupBuilder(parentGroup);
					if (!makeVariableShapeAndType(parentGroup, sb, facade.dobj.mdt, facade.dobj.mds, vinfo, facade.dimList))
					{
						return null;
					}
					addMembersToStructure(parentGroup, sb, facade.dobj.mdt);
					sb.setElementSize(facade.dobj.mdt.byteSize);

				}
				else
				{
					string vname = facade.name;
					if (vname.StartsWith(Nc4.NETCDF4_NON_COORD, StringComparison.Ordinal))
					{
						vname = vname.Substring(Nc4.NETCDF4_NON_COORD.Length); // skip prefix
					}
					vb = Variable.builder().setName(vname);
					vb.setParentGroupBuilder(parentGroup);
					if (!makeVariableShapeAndType(parentGroup, vb, facade.dobj.mdt, facade.dobj.mds, vinfo, facade.dimList))
					{
						return null;
					}

					// special case of variable length strings
					if (vb.dataType == DataType.STRING)
					{
						vb.setElementSize(16); // because the array has elements that are HeapIdentifier
					}
					else if (vb.dataType == DataType.OPAQUE) // special case of opaque
					{
						vb.setElementSize(facade.dobj.mdt.@base.byteSize);
					}
				}

				vb.setSPobject(vinfo);

				// look for attributes
				IList<MessageAttribute> fatts = filterAttributes(facade.dobj.attributes);
				foreach (MessageAttribute matt in fatts)
				{
					try
					{
						makeAttributes(sb, matt, vb.getAttributeContainer());
					}
					catch (InvalidRangeException e)
					{
						throw new IOException(e.Message);
					}
				}

				AttributeContainerMutable atts = vb.getAttributeContainer();
				processSystemAttributes(facade.dobj.messages, atts);
				if (fillAttribute != null && atts.findAttribute(CDM.FILL_VALUE) == null)
				{
					vb.addAttribute(fillAttribute);
				}
				// if (vinfo.typeInfo.unsigned)
				// v.addAttribute(new Attribute(CDM.UNSIGNED, "true"));
				if (facade.dobj.mdt.type == 5)
				{
					string desc = facade.dobj.mdt.opaque_desc;
					if ((desc != null) && (desc.Length != 0))
					{
						vb.addAttribute(new NcAttribute("_opaqueDesc", desc));
					}
				}

				int[] shape = makeVariableShape(facade.dobj.mdt, facade.dobj.mds, facade.dimList);
				if (vinfo.isChunked)
				{ // make the data btree, but entries are not read in
					vinfo.btree = new DataBTree(this, dataAddress, shape, vinfo.storageSize, memTracker);

					if (vinfo.isChunked)
					{ // add an attribute describing the chunk size
						IList<int> chunksize = new List<int>();
						for (int i = 0; i < vinfo.storageSize.Length - 1; i++) // skip last one - its the element size
						{
							chunksize.Add(vinfo.storageSize[i]);
						}
						vb.addAttribute(NcAttribute.builder(CDM.CHUNK_SIZES).setValues(chunksize.Cast<object>().ToList(), true).build());
					}
				}

				if (transformReference && (facade.dobj.mdt.type == 7) && (facade.dobj.mdt.referenceType == 0))
				{ // object reference
				  // NetcdfSharp.java.lang.System.@out.printf("new transform object Reference: facade= %s variable name=%s%n", facade.name,
				  // vb.shortName);
					vb.setDataType(DataType.STRING);
					NcArray rawData = vinfo.readArray();
					NcArray refData = findReferenceObjectNames(rawData);
					vb.setCachedData(refData, true); // so H5iosp.read() is never called
					vb.addAttribute(new NcAttribute("_HDF5ReferenceType", "values are names of referenced Variables"));
				}

				if (transformReference && (facade.dobj.mdt.type == 7) && (facade.dobj.mdt.referenceType == 1))
				{ // region reference
					if (warnings)
					{
						log.warn("transform region Reference: facade=" + facade.name + " variable name=" + vb.shortName);
					}

					// fake data for now
					vb.setDataType(DataType.LONG);
					NcArray newData = NcArray.factory(DataType.LONG, shape);
					vb.setCachedData(newData, true); // so H5iosp.read() is never called
					vb.addAttribute(new NcAttribute("_HDF5ReferenceType", "values are regions of referenced Variables"));
				}

				// debugging
				vinfo.setOwner(vb);
				// TODO: handle logging warnings and unknown filters at the Filter level

				if (debug1)
				{
					log.debug("makeVariable " + vb.shortName + "; vinfo= " + vinfo);
				}

				return vb;

			}
			catch (InvalidRangeException e)
			{
				log.error(e.Message);
				return null;
			}
		}

		// convert an array of lons which are data object references to an array of strings,
		// the names of the data objects (dobj.who)
		private NcArray findReferenceObjectNames(NcArray data)
		{
			int nelems = (int)data.Size;
			string[] result = new string[nelems];
			int count = 0;
			data.resetLocalIterator();
			while (data.hasNext())
			{
				long objId = data.nextLong();
				DataObject dobj = getDataObject(objId, null);
				if (dobj == null)
				{
					log.warn("readReferenceObjectNames cant find obj= {}", objId);
				}
				else
				{
					if (debugReference)
					{
						log.debug(" Referenced object= {}", dobj.who);
					}
					result[count] = dobj.who;
				}
				count++;
			}
			return NcArray.factory(DataType.STRING, data.Shape, result);
		}

		private void addMembersToStructure(Group.Builder parent, Structure.BuilderStruct s, MessageDatatype mdt)
		{
			foreach (StructureMember m in mdt.members)
			{
				Variable.Builder v = makeVariableMember(parent, m.name, m.offset, m.mdt);
				if (v != null)
				{
					s.addMemberVariable(v);
					if (debug1)
					{
						log.debug("  made Member Variable " + v.shortName + "\n" + v);
					}
				}
			}
		}

		// Used for Structure Members
		private Variable.Builder makeVariableMember(Group.Builder parentGroup, string name, long dataPos, MessageDatatype mdt)
		{

			Vinfo vinfo = new Vinfo(this, mdt, null, dataPos); // LOOK need mds
			if (vinfo.NCDataType == null)
			{
				log.debug("SKIPPING DataType= " + vinfo.typeInfo.hdfType + " for variable " + name);
				return null;
			}

			try
			{
				if (mdt.type == 6)
				{
					var sb = Structure.builder1().setName1(name).setParentGroupBuilder1(parentGroup);
					makeVariableShapeAndType(parentGroup, sb, mdt, null, vinfo, null);
					addMembersToStructure(parentGroup, sb, mdt);
					sb.setElementSize(mdt.byteSize);

					sb.setSPobject(vinfo);
					vinfo.setOwner(sb);
					return sb;

				}
				else
				{
					Variable.Builder vb = Variable.builder().setName(name).setParentGroupBuilder(parentGroup);
					makeVariableShapeAndType(parentGroup, vb, mdt, null, vinfo, null);

					// special case of variable length strings
					if (vb.dataType == DataType.STRING)
					{
						vb.setElementSize(16); // because the array has elements that are HeapIdentifier
					}
					else if (vb.dataType == DataType.OPAQUE) // special case of opaque
					{
						vb.setElementSize(mdt.@base.byteSize);
					}

					vb.setSPobject(vinfo);
					vinfo.setOwner(vb);
					return vb;
				}
			}
			catch (InvalidRangeException e)
			{
				log.error(e.Message);
				return null;
			}
		}

		private void processSystemAttributes(IList<HeaderMessage> messages, AttributeContainer attContainer)
		{
			foreach (HeaderMessage mess in messages)
			{
				if (mess.mtype == MessageType.Comment)
				{
					MessageComment m = (MessageComment)mess.messData;
					attContainer.addAttribute(new NcAttribute("_comment", m.comment));
				}
			}
		}

		// get the shape of the Variable
		private int[] makeVariableShape(MessageDatatype mdt, MessageDataspace msd, string dimNames)
		{
			int[] shape = (msd != null) ? msd.dimLength : new int[0];
			if (shape == null)
			{
				shape = new int[0]; // scaler
			}

			// merge the shape for array type (10)
			if (mdt.type == 10)
			{
				int len = shape.Length + mdt.dim.Length;
				if (mdt.isVlenType())
				{
					len++;
				}
				int[] combinedDim = new int[len];
				System.Array.Copy(shape, 0, combinedDim, 0, shape.Length);
				System.Array.Copy(mdt.dim, 0, combinedDim, shape.Length, mdt.dim.Length); // // type 10 is the inner dimensions
				if (mdt.isVlenType())
				{
					combinedDim[len - 1] = -1;
				}
				shape = combinedDim;
			}

			// dimension names were not passed in
			if (dimNames == null)
			{
				if (mdt.type == 3)
				{ // fixed length string - DataType.CHAR, add string length
					if (mdt.byteSize != 1)
					{ // scalar string member variable
						int[] rshape = new int[shape.Length + 1];
						System.Array.Copy(shape, 0, rshape, 0, shape.Length);
						rshape[shape.Length] = mdt.byteSize;
						return rshape;
					}
				}
				else if (mdt.isVlenType())
				{ // variable length (not a string)
					if ((shape.Length == 1) && (shape[0] == 1))
					{ // replace scalar with vlen
						return new int[] { -1 };

					}
					else if (mdt.type != 10)
					{ // add vlen dimension already done above for array
						int[] rshape = new int[shape.Length + 1];
						System.Array.Copy(shape, 0, rshape, 0, shape.Length);
						rshape[shape.Length] = -1;
						return rshape;
					}
				}
			}
			return shape;
		}

		// set the type and shape of the Variable
		private bool makeVariableShapeAndType(Group.Builder parent, Variable.Builder v, MessageDatatype mdt, MessageDataspace msd, Vinfo vinfo, string dimNames)
		{

			int[] shape = makeVariableShape(mdt, msd, dimNames);

			// set dimensions on the variable
			if (dimNames != null)
			{ // dimensions were passed in
				if ((mdt.type == 9) && !mdt.isVString)
				{
					v.setDimensionsByName(dimNames + " *");
				}
				else
				{
					v.setDimensionsByName(dimNames);
				}
			}
			else
			{
				for (int i = 0; i < shape.Length; i++)
				{
					if ((shape[i] < 1) && (shape[i] != -1))
					{
						throw new InvalidRangeException("shape[" + i + "]=" + shape[i] + " must be > 0");
					}
				}
				v.setDimensionsAnonymous(shape);
			}

			// set the type
			DataType? dt = vinfo.NCDataType;
			if (dt == null)
			{
				return false;
			}
			v.setDataType(dt.Value);

			// set the enumTypedef
			if (dt.Value.isEnum())
			{
				// dmh: An HDF5 file, at least as used by netcdf-4, may define an enumeration
				// type one or more times:
				// 1. There may be an explicit, independent enum type definition.
				// 2. A variable/HDF5-Dataset may define an implicit enum type with the same name as the variable.
				// 3. A variable may define an implicit enum type that is a copy of a case 1 enum type;
				// the implicit enum type will have the same name as the independent enum type.
				//
				// The algorithm to infer (and if necessary, create) the proper EnumTypeDef is as follows:
				// Step 1. If there exists a case 1 enum type with the same name as the variable's enum type,
				// then use that.
				// Step 2. If the variable's enum type has the same name as the variable, then we need to
				// look for a case 1 enum type that is structurally the same as the variable's enum type.
				// If such exists, then use that.
				// Step 3: Otherwise, create a new enum type and use that. The new enum type
				// will have these properties:
				// a. It is defined in the same group as the variable
				// b. It has a mutated name similar to the variable's name, namely <variablename>_enum_t.

				EnumTypedef actualEnumTypedef = null; // The final chosen EnumTypedef

				// Step 1:
				// See if an independent enum type already exists with the same name
				EnumTypedef candidate = parent.findEnumTypedef(mdt.enumTypeName, true).orElse(null);
				actualEnumTypedef = candidate;

				// Step 2:
				// See if an independent enum type already exists that is structurally similar.
				if (actualEnumTypedef == null && mdt.enumTypeName.Equals(v.shortName))
				{
					// Materialize a enum type def for search purposes; name is irrelevant
					EnumTypedef template = new EnumTypedef(mdt.enumTypeName, mdt.map);
					// Search for a structurally similar enum type def
					candidate = parent.findSimilarEnumTypedef(template, true).orElse(null);
					actualEnumTypedef = candidate;
				}

				// Step 3: Create an independent type
				if (actualEnumTypedef == null)
				{
					string newname = null;
					if (mdt.enumTypeName.Equals(v.shortName))
					{
						// Create mutated name to avoid name conflict
						newname = mdt.enumTypeName + "_enum_t";
					}
					else
					{
						newname = mdt.enumTypeName;
					}
					actualEnumTypedef = new EnumTypedef(newname, mdt.map);
					// Add to the current group(builder)
					parent.addEnumTypedef(actualEnumTypedef);
				}

				// associate with the variable
				v.setEnumTypeName(actualEnumTypedef.ShortName);
			}
			return true;
		}

		public virtual Group.Builder RootGroup
		{
			get
			{
				return root;
			}
		}

		public virtual void makeVinfoForDimensionMapVariable(Group.Builder parent, Variable.Builder v)
		{
			// this is a self contained variable, doesnt need any extra info
			Vinfo vinfo = new Vinfo(this);
			vinfo.owner = v;
		}

		public virtual string readStructMetadata(Variable.Builder structMetadataVar)
		{
			Vinfo vinfo = (Vinfo)structMetadataVar.spiObject;
			return vinfo.readString();
		}

		// Holder of all H5 specific information for a Variable, needed to do IO.
		public class Vinfo
		{
			internal readonly H5headerNew outerInstance;
			internal Variable.Builder owner; // debugging
			internal DataObjectFacade facade; // debugging

			internal long dataPos; // for regular variables, needs to be absolute, with baseAddress added if needed
								   // for member variables, is the offset from start of structure

			internal TypeInfo typeInfo;
			internal int[] storageSize; // for type 1 (continuous) : mds.dimLength;
										// for type 2 (chunked) : msl.chunkSize (last number is element size)
										// null for attributes

			internal bool isvlen; // VLEN, but not vlenstring

			// chunked stuff
			internal bool isChunked;
			internal DataBTree btree; // only if isChunked

			internal MessageDatatype mdt;
			internal MessageDataspace mds;
			internal MessageFilter mfp;

			internal bool useFillValue_Conflict;
			internal byte[] fillValue;

			public virtual DataBTree Btree
			{
				get
				{
					return btree;
				}
			}

			public virtual string getCompression()
			{
				if (mfp == null)
				{
					return null;
				}
				Formatter f = new Formatter();
				foreach (H5objects.Filter filt in mfp.filters)
				{
					f.format("name: %s , id: %d", filt.getName(), filt.getId());
				}
				return f.ToString();
			}

			public virtual int[] getChunking()
			{
				return storageSize;
			}

			public virtual bool useFillValue()
			{
				return useFillValue_Conflict;
			}

			public virtual long[] countStorageSize(Formatter f)
			{
				long[] result = new long[2];
				if (btree == null)
				{
					if (f != null)
					{
						f.format("btree is null%n");
					}
					return result;
				}
				if (useFillValue_Conflict)
				{
					if (f != null)
					{
						f.format("useFillValue - no data is stored%n");
					}
					return result;
				}

				int count = 0;
				long total = 0;
				DataBTree.DataChunkIterator iter = btree.getDataChunkIteratorFilter(null);
				while (iter.hasNext())
				{
					DataBTree.DataChunk dc = iter.next();
					if (f != null)
					{
						f.format(" %s%n", dc);
					}
					total += dc.size;
					count++;
				}

				result[0] = total;
				result[1] = count;
				return result;
			}

			internal Vinfo()
			{
				// nuthing
			}

			internal Vinfo(H5headerNew outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="facade"> DataObjectFacade: always has an mdt and an msl </param>
			internal Vinfo(H5headerNew outerInstance, DataObjectFacade facade)
			{
				this.outerInstance = outerInstance;
				this.facade = facade;
				// LOOK if compact, do not use fileOffset
				this.dataPos = (facade.dobj.msl.type == 0) ? facade.dobj.msl.dataAddress : outerInstance.getFileOffset(facade.dobj.msl.dataAddress);
				this.mdt = facade.dobj.mdt;
				this.mds = facade.dobj.mds;
				this.mfp = facade.dobj.mfp;

				isvlen = this.mdt.isVlen;
				if (!facade.dobj.mdt.isOK && warnings)
				{
					log.debug("WARNING HDF5 file " + outerInstance.RandomAccessFile.Location + " not handling " + facade.dobj.mdt);
					return; // not a supported datatype
				}

				this.isChunked = (facade.dobj.msl.type == 2);
				if (isChunked)
				{
					this.storageSize = facade.dobj.msl.chunkSize;
				}
				else
				{
					this.storageSize = facade.dobj.mds.dimLength;
				}

				// figure out the data type
				this.typeInfo = calcNCtype(facade.dobj.mdt);
			}

			/// <summary>
			/// Constructor, used for reading attributes
			/// </summary>
			/// <param name="mdt"> datatype </param>
			/// <param name="mds"> dataspace </param>
			/// <param name="dataPos"> start of data in file </param>
			internal Vinfo(H5headerNew outerInstance, MessageDatatype mdt, MessageDataspace mds, long dataPos)
			{
				this.outerInstance = outerInstance;
				this.mdt = mdt;
				this.mds = mds;
				this.dataPos = dataPos;

				if (!mdt.isOK && warnings)
				{
					log.debug("WARNING HDF5 file " + outerInstance.RandomAccessFile.Location + " not handling " + mdt);
					return; // not a supported datatype
				}

				isvlen = this.mdt.isVlen;

				// figure out the data type
				// this.hdfType = mdt.type;
				this.typeInfo = calcNCtype(mdt);
			}

			internal virtual void setOwner(Variable.Builder owner)
			{
				this.owner = owner;
				if (btree != null)
				{
					btree.Owner = owner;
				}
			}

			internal virtual TypeInfo calcNCtype(MessageDatatype mdt)
			{
				int hdfType = mdt.type;
				int byteSize = mdt.byteSize;
				byte[] flags = mdt.flags;
				// boolean unsigned = mdt.unsigned;

				TypeInfo tinfo = new TypeInfo(hdfType, byteSize);

				if (hdfType == 0)
				{ // int, long, short, byte
					tinfo.dataType = outerInstance.getNCtype(hdfType, byteSize, mdt.unsigned);
					tinfo.endian = ((flags[0] & 1) == 0) ? RandomAccessFile.LITTLE_ENDIAN : RandomAccessFile.BIG_ENDIAN;
					tinfo.unsigned = ((flags[0] & 8) == 0);

				}
				else if (hdfType == 1)
				{ // floats, doubles
					tinfo.dataType = outerInstance.getNCtype(hdfType, byteSize, mdt.unsigned);
					tinfo.endian = ((flags[0] & 1) == 0) ? RandomAccessFile.LITTLE_ENDIAN : RandomAccessFile.BIG_ENDIAN;

				}
				else if (hdfType == 2)
				{ // time
					tinfo.dataType = DataType.STRING;
					tinfo.endian = ((flags[0] & 1) == 0) ? RandomAccessFile.LITTLE_ENDIAN : RandomAccessFile.BIG_ENDIAN;

				}
				else if (hdfType == 3)
				{ // fixed length strings map to CHAR. String is used for Vlen type = 1.
					tinfo.dataType = DataType.CHAR;
					tinfo.vpad = (flags[0] & 0xf);
					// when elem length = 1, there is a problem with dimensionality.
					// eg char cr(2); has a storage_size of [1,1].

				}
				else if (hdfType == 4)
				{ // bit field
					tinfo.dataType = outerInstance.getNCtype(hdfType, byteSize, mdt.unsigned);

				}
				else if (hdfType == 5)
				{ // opaque
					tinfo.dataType = DataType.OPAQUE;

				}
				else if (hdfType == 6)
				{ // structure
					tinfo.dataType = DataType.STRUCTURE;

				}
				else if (hdfType == 7)
				{ // reference
					tinfo.endian = RandomAccessFile.LITTLE_ENDIAN;
					tinfo.dataType = DataType.LONG; // file offset of the referenced object
													// LOOK - should get the object, and change type to whatever it is (?)

				}
				else if (hdfType == 8)
				{ // enums
					if (tinfo.byteSize == 1)
					{
						tinfo.dataType = DataType.ENUM1;
					}
					else if (tinfo.byteSize == 2)
					{
						tinfo.dataType = DataType.ENUM2;
					}
					else if (tinfo.byteSize == 4)
					{
						tinfo.dataType = DataType.ENUM4;
					}
					else
					{
						log.warn("Illegal byte suze for enum type = {}", tinfo.byteSize);
						throw new System.InvalidOperationException("Illegal byte suze for enum type = " + tinfo.byteSize);
					}

					// enumMap = mdt.map;

				}
				else if (hdfType == 9)
				{ // variable length array
					tinfo.isVString = mdt.isVString;
					tinfo.isVlen = mdt.isVlen;
					if (mdt.isVString)
					{
						tinfo.vpad = ((flags[0] >> 4) & 0xf);
						tinfo.dataType = DataType.STRING;
					}
					else
					{
						tinfo.dataType = outerInstance.getNCtype(mdt.@base.type, mdt.@base.byteSize, mdt.@base.unsigned);
						tinfo.endian = mdt.@base.endian;
						tinfo.unsigned = mdt.@base.unsigned;
					}
				}
				else if (hdfType == 10)
				{ // array : used for structure members
					tinfo.endian = (mdt.flags[0] & 1) == 0 ? RandomAccessFile.LITTLE_ENDIAN : RandomAccessFile.BIG_ENDIAN;
					if (mdt.isVStringType())
					{
						tinfo.dataType = DataType.STRING;
					}
					else
					{
						int basetype = mdt.getBaseType();
						tinfo.dataType = outerInstance.getNCtype(basetype, mdt.getBaseSize(), mdt.unsigned);
					}
				}
				else if (warnings)
				{
					log.debug("WARNING not handling hdf dataType = " + hdfType + " size= " + byteSize);
				}

				if (mdt.@base != null)
				{
					tinfo.@base = calcNCtype(mdt.@base);
				}
				return tinfo;
			}

			public override string ToString()
			{
				StringBuilder buff = new StringBuilder();
				buff.Append("dataPos=").Append(dataPos).Append(" datatype=").Append(typeInfo);
				if (isChunked)
				{
					buff.Append(" isChunked (");
					foreach (int size in storageSize)
					{
						buff.Append(size).Append(" ");
					}
					buff.Append(")");
				}
				if (mfp != null)
				{
					buff.Append(" hasFilter");
				}
				buff.Append("; // ").Append(extraInfo());
				if (null != facade)
				{
					buff.Append("\n").Append(facade);
				}

				return buff.ToString();
			}

			public virtual string extraInfo()
			{
				StringBuilder buff = new StringBuilder();
				if ((typeInfo.dataType != DataType.CHAR) && (typeInfo.dataType != DataType.STRING))
				{
					buff.Append(typeInfo.unsigned ? " unsigned" : " signed");
				}
				if (typeInfo.endian >= 0)
				{
					buff.Append((typeInfo.endian == RandomAccessFile.LITTLE_ENDIAN) ? " LittleEndian" : " BigEndian");
				}
				if (useFillValue_Conflict)
				{
					buff.Append(" useFillValue");
				}
				return buff.ToString();
			}

			internal virtual DataType? NCDataType
			{
				get
				{
					return typeInfo.dataType;
				}
			}

			/// <summary>
			/// Get the Fill Value, return default if one was not set.
			/// </summary>
			/// <returns> wrapped primitive (Byte, Short, Integer, Double, Float, Long), or null if none </returns>
			internal virtual object FillValue
			{
				get
				{
					return (fillValue == null) ? N3iospFillValues.getFillValueDefault(typeInfo.dataType.Value) : getFillValueNonDefault();
				}
			}

			internal virtual object getFillValueNonDefault()
			{
				if (fillValue == null)
				{
					return null;
				}

				if ((typeInfo.dataType.Value.getPrimitiveClassType() == typeof(byte)) || (typeInfo.dataType == DataType.CHAR))
				{
					return fillValue[0];
				}

				ByteBuffer bbuff = ByteBuffer.wrap(fillValue);
				if (typeInfo.endian >= 0)
				{
					bbuff.order(typeInfo.endian == RandomAccessFile.LITTLE_ENDIAN ? ByteOrder.LITTLE_ENDIAN : ByteOrder.BIG_ENDIAN);
				}

				if (typeInfo.dataType.Value.getPrimitiveClassType() == typeof(short))
				{
					ShortBuffer tbuff = bbuff.asShortBuffer();
					return tbuff.get();

				}
				else if (typeInfo.dataType.Value.getPrimitiveClassType() == typeof(int))
				{
					IntBuffer tbuff = bbuff.asIntBuffer();
					return tbuff.get();

				}
				else if (typeInfo.dataType.Value.getPrimitiveClassType() == typeof(long))
				{
					LongBuffer tbuff = bbuff.asLongBuffer();
					return tbuff.get();

				}
				else if (typeInfo.dataType == DataType.FLOAT)
				{
					FloatBuffer tbuff = bbuff.asFloatBuffer();
					return tbuff.get();

				}
				else if (typeInfo.dataType == DataType.DOUBLE)
				{
					DoubleBuffer tbuff = bbuff.asDoubleBuffer();
					return tbuff.get();
				}

				return null;
			}

			// limited reader; Variable is not built yet.
			internal virtual NcArray readArray()
			{
				int[] shape = mds.dimLength;
				DataType dataType = typeInfo.dataType.Value;

				if (useFillValue_Conflict)
				{
					object pa = IospHelper.makePrimitiveArray((int)NcIndex.computeSize(shape), dataType, FillValue);
					if (dataType == DataType.CHAR)
					{
						pa = IospHelper.convertByteToChar((byte[])pa);
					}
					return NcArray.factory(dataType, shape, pa);
				}

				// filtered, must read and decode by chunk
				if (mfp != null)
				{
					ByteOrder bo = (typeInfo.endian == 0) ? ByteOrder.BIG_ENDIAN : ByteOrder.LITTLE_ENDIAN;
					LayoutBB layoutBB;
					try
					{
						layoutBB = new H5tiledLayoutBB(this, shape, dataType.getSize(), new Section(shape), outerInstance.RandomAccessFile, mfp.filters, bo);
					}
					catch (InvalidRangeException)
					{
						// should not happen because we passed in the full shape
						throw new System.InvalidOperationException();
					}
					object dataF = IospHelper.readDataFill(layoutBB, dataType, FillValue);
					return NcArray.factory(dataType, shape, dataF);
				}

				Layout layout;
				try
				{
					if (isChunked)
					{
						layout = new H5tiledLayout(this, dataType, new Section(shape));
					}
					else
					{
						layout = new LayoutRegular(dataPos, dataType.getSize(), shape, null);
					}
				}
				catch (InvalidRangeException)
				{
					// cant happen because we use null for wantSection
					throw new System.InvalidOperationException();
				}
				object data = IospHelper.readDataFill(outerInstance.RandomAccessFile, layout, dataType, FillValue, typeInfo.endian, false);
				return NcArray.factory(dataType, shape, data);
			}

			// limited reader; Variable is not built yet.
			internal virtual string readString()
			{
				int[] shape = new int[] { mdt.byteSize };
				DataType dataType = typeInfo.dataType.Value;

				if (useFillValue_Conflict)
				{
					object pa = IospHelper.makePrimitiveArray((int)NcIndex.computeSize(shape), dataType, FillValue);
					if (dataType == DataType.CHAR)
					{
						pa = IospHelper.convertByteToChar((byte[])pa);
					}
					NcArray dataArray = NcArray.factory(dataType, shape, pa);
					return (dataArray is ArrayChar.D1) ? ((ArrayChar)dataArray).String : "";
				}

				Layout layout;
				try
				{
					if (isChunked)
					{
						layout = new H5tiledLayout(this, dataType, new Section(shape));
					}
					else
					{
						layout = new LayoutRegular(dataPos, dataType.getSize(), shape, null);
					}
				}
				catch (InvalidRangeException)
				{
					// cant happen because we use null for wantSection
					throw new System.InvalidOperationException();
				}
				object data = IospHelper.readDataFill(outerInstance.RandomAccessFile, layout, dataType, FillValue, typeInfo.endian, true);

				string result = "";
				if (data is string)
				{
					// Sometimes StructMetadata.0 is stored as a string,
					// and IospHelper returns it directly as a string, so pass it along
					result = (string)data;
				}
				else
				{
					NcArray dataArray = NcArray.factory(dataType, shape, data);
					// read and parse the ODL
					if (dataArray is ArrayChar.D1)
					{
						ArrayChar ca = (ArrayChar)dataArray;
						result = ca.String; // common case only StructMetadata.0, avoid extra copy
					}
					else if (dataArray is ArrayObject.D0)
					{
						ArrayObject ao = (ArrayObject)dataArray;
						result = (string)ao.getObject(0);
					}
					else if (dataArray is ArrayObject.D1)
					{
						result = (string)dataArray.getObject(0);
					}
					else
					{
						log.error("Unsupported array type {} for StructMetadata", dataArray.ElementType);
					}
				}
				return result;
			}
		}

		internal virtual DataType? getNCtype(int hdfType, int size, bool unsigned)
		{
			if ((hdfType == 0) || (hdfType == 4))
			{ // integer, bit field
				Signedness signedness = unsigned ? Signedness.UNSIGNED : Signedness.SIGNED;

				if (size == 1)
				{
					return DataType.BYTE.withSignedness(signedness);
				}
				else if (size == 2)
				{
					return DataType.SHORT.withSignedness(signedness);
				}
				else if (size == 4)
				{
					return DataType.INT.withSignedness(signedness);
				}
				else if (size == 8)
				{
					return DataType.LONG.withSignedness(signedness);
				}
				else if (warnings)
				{
					log.debug("WARNING HDF5 file " + RandomAccessFile.Location + " not handling hdf integer type (" + hdfType + ") with size= " + size);
					log.warn("HDF5 file " + RandomAccessFile.Location + " not handling hdf integer type (" + hdfType + ") with size= " + size);
					return null;
				}

			}
			else if (hdfType == 1)
			{
				if (size == 4)
				{
					return DataType.FLOAT;
				}
				else if (size == 8)
				{
					return DataType.DOUBLE;
				}
				else if (warnings)
				{
					log.debug("WARNING HDF5 file " + RandomAccessFile.Location + " not handling hdf float type with size= " + size);
					log.warn("HDF5 file " + RandomAccessFile.Location + " not handling hdf float type with size= " + size);
					return null;
				}

			}
			else if (hdfType == 3)
			{ // fixed length strings. String is used for Vlen type = 1
				return DataType.CHAR;

			}
			else if (hdfType == 6)
			{
				return DataType.STRUCTURE;

			}
			else if (hdfType == 7)
			{ // reference
				return DataType.ULONG;

			}
			else if (hdfType == 9)
			{
				return null; // dunno

			}
			else if (warnings)
			{
				log.warn("HDF5 file " + RandomAccessFile.Location + " not handling hdf type = " + hdfType + " size= " + size);
			}
			else
			{
				log.debug("HDF5 file " + RandomAccessFile.Location + " not handling hdf type = " + hdfType + " size= " + size);
			}
			return null;
		}

		public class TypeInfo
		{
			internal int hdfType, byteSize;
			internal DataType? dataType;
			internal int endian = -1; // 1 = RandomAccessFile.LITTLE_ENDIAN || 0 = RandomAccessFile.BIG_ENDIAN
			internal bool unsigned;
			internal bool isVString; // is it a vlen string
			internal bool isVlen; // vlen but not string
			internal int vpad; // string padding
			internal TypeInfo @base; // vlen, enum

			internal TypeInfo(int hdfType, int byteSize)
			{
				this.hdfType = hdfType;
				this.byteSize = byteSize;
			}

			public override string ToString()
			{
				StringBuilder buff = new StringBuilder();
				buff.Append("hdfType=").Append(hdfType).Append(" byteSize=").Append(byteSize).Append(" dataType=").Append(dataType);
				buff.Append(" unsigned=").Append(unsigned).Append(" isVString=").Append(isVString).Append(" vpad=").Append(vpad).Append(" endian=").Append(endian);
				if (@base != null)
				{
					buff.Append("\n   base=").Append(@base);
				}
				return buff.ToString();
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// Heaps

		/// <summary>
		/// Fetch a Vlen data array.
		/// </summary>
		/// <param name="globalHeapIdAddress"> address of the heapId, used to get the String out of the heap </param>
		/// <param name="dataType"> type of data </param>
		/// <param name="endian"> byteOrder of the data (0 = BE, 1 = LE) </param>
		/// <returns> the NcArray read from the heap </returns>
		internal virtual NcArray getHeapDataArray(long globalHeapIdAddress, DataType dataType, int endian)
		{
			HeapIdentifier heapId = h5objects.readHeapIdentifier(globalHeapIdAddress);
			if (debugHeap)
			{
				log.debug(" heapId= {}", heapId);
			}
			return getHeapDataArray(heapId, dataType, endian);
		}

		internal virtual NcArray getHeapDataArray(HeapIdentifier heapId, DataType dataType, int endian)
		{
			GlobalHeap.HeapObject ho = heapId.getHeapObject();
			if (ho == null)
			{
				throw new InvalidRangeException("Illegal Heap address, HeapObject = " + heapId);
			}
			if (debugHeap)
			{
				log.debug(" HeapObject= {}", ho);
			}
			if (endian >= 0)
			{
				RandomAccessFile.order(endian);
			}

			if (DataType.FLOAT == dataType)
			{
				float[] pa = new float[heapId.nelems];
				RandomAccessFile.seek(ho.dataPos);
				RandomAccessFile.readFloat(pa, 0, pa.Length);
				return NcArray.factory(dataType, new int[] { pa.Length }, pa);

			}
			else if (DataType.DOUBLE == dataType)
			{
				double[] pa = new double[heapId.nelems];
				RandomAccessFile.seek(ho.dataPos);
				RandomAccessFile.readDouble(pa, 0, pa.Length);
				return NcArray.factory(dataType, new int[] { pa.Length }, pa);

			}
			else if (dataType.getPrimitiveClassType() == typeof(byte))
			{
				byte[] pa = new byte[heapId.nelems];
				RandomAccessFile.seek(ho.dataPos);
				RandomAccessFile.readFully(pa, 0, pa.Length);
				return NcArray.factory(dataType, new int[] { pa.Length }, pa);

			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				short[] pa = new short[heapId.nelems];
				RandomAccessFile.seek(ho.dataPos);
				RandomAccessFile.readShort(pa, 0, pa.Length);
				return NcArray.factory(dataType, new int[] { pa.Length }, pa);

			}
			else if (dataType.getPrimitiveClassType() == typeof(int))
			{
				int[] pa = new int[heapId.nelems];
				RandomAccessFile.seek(ho.dataPos);
				RandomAccessFile.readInt(pa, 0, pa.Length);
				return NcArray.factory(dataType, new int[] { pa.Length }, pa);

			}
			else if (dataType.getPrimitiveClassType() == typeof(long))
			{
				long[] pa = new long[heapId.nelems];
				RandomAccessFile.seek(ho.dataPos);
				RandomAccessFile.readLong(pa, 0, pa.Length);
				return NcArray.factory(dataType, new int[] { pa.Length }, pa);
			}

			throw new System.NotSupportedException("getHeapDataAsArray dataType=" + dataType);
		}

		/// <summary>
		/// Fetch a String from the heap.
		/// </summary>
		/// <param name="heapIdAddress"> address of the heapId, used to get the String out of the heap </param>
		/// <returns> String the String read from the heap </returns>
		internal virtual string readHeapString(long heapIdAddress)
		{
			HeapIdentifier heapId = h5objects.readHeapIdentifier(heapIdAddress);
			if (heapId.isEmpty())
			{
				return NULL_STRING_VALUE;
			}
			GlobalHeap.HeapObject ho = heapId.getHeapObject();
			if (ho == null)
			{
				throw new System.InvalidOperationException("Cant find Heap Object,heapId=" + heapId);
			}
			if (ho.dataSize > 1000 * 1000)
			{
				return string.Format("Bad HeapObject.dataSize={0}", ho);
			}
			RandomAccessFile.seek(ho.dataPos);
			return RandomAccessFile.readString((int)ho.dataSize, valueCharset);
		}

		/// <summary>
		/// Fetch a String from the heap, when the heap identifier has already beed read into a ByteBuffer at given pos
		/// </summary>
		/// <param name="bb"> heap id is here </param>
		/// <param name="pos"> at this position </param>
		/// <returns> String the String read from the heap </returns>
		internal virtual string readHeapString(ByteBuffer bb, int pos)
		{
			HeapIdentifier heapId = h5objects.readHeapIdentifier(bb, pos);
			if (heapId.isEmpty())
			{
				return NULL_STRING_VALUE;
			}
			GlobalHeap.HeapObject ho = heapId.getHeapObject();
			if (ho == null)
			{
				throw new System.InvalidOperationException("Cant find Heap Object,heapId=" + heapId);
			}
			RandomAccessFile.seek(ho.dataPos);
			return RandomAccessFile.readString((int)ho.dataSize, valueCharset);
		}

		internal virtual NcArray readHeapVlen(ByteBuffer bb, int pos, DataType dataType, int endian)
		{
			HeapIdentifier heapId = h5objects.readHeapIdentifier(bb, pos);
			return getHeapDataArray(heapId, dataType, endian);
		}

		/// <summary>
		/// Get a data object's name, using the objectId you get from a reference (aka hard link).
		/// </summary>
		/// <param name="objId"> address of the data object </param>
		/// <returns> String the data object's name, or null if not found </returns>
		internal virtual string getDataObjectName(long objId)
		{
			DataObject dobj = getDataObject(objId, null);
			if (dobj == null)
			{
				log.error("H5iosp.readVlenData cant find dataObject id= {}", objId);
				return null;
			}
			else
			{
				if (debugVlen)
				{
					log.debug(" Referenced object= {}", dobj.who);
				}
				return dobj.who;
			}
		}

		//////////////////////////////////////////////////////////////
		// Internal organization of Data Objects

		/// <summary>
		/// All access to data objects come through here, so we can cache.
		/// Look in cache first; read if not in cache.
		/// </summary>
		/// <param name="address"> object address (aka id) </param>
		/// <param name="name"> optional name </param>
		/// <returns> DataObject </returns>
		internal virtual DataObject getDataObject(long address, string name)
		{
			// find it
			DataObject dobj;
			if (!addressMap.TryGetValue(address, out dobj))
			{
				dobj = null;
			}
			if (dobj != null)
			{
				if ((dobj.who == null) && name != null)
				{
					dobj.who = name;
				}
				return dobj;
			}
			// if (name == null) return null; // ??

			// read it
			dobj = h5objects.readDataObject(address, name);
			addressMap[address] = dobj; // look up by address (id)
			return dobj;
		}

		//////////////////////////////////////////////////////////////
		// utilities

		public virtual int makeIntFromBytes(byte[] bb, int start, int n)
		{
			int result = 0;
			for (int i = start + n - 1; i >= start; i--)
			{
				result <<= 8;
				byte b = bb[i];
				result += (b < 0) ? b + 256 : b;
			}
			return result;
		}

		public virtual bool OffsetLong
		{
			get
			{
				return isOffsetLong;
			}
		}

		public virtual long readLength()
		{
			return isLengthLong ? RandomAccessFile.readLong() : RandomAccessFile.readInt();
		}

		public virtual long readOffset()
		{
			return isOffsetLong ? RandomAccessFile.readLong() : RandomAccessFile.readInt();
		}

		public virtual long readAddress()
		{
			return getFileOffset(readOffset());
		}

		public virtual byte SizeLengths
		{
			get
			{
				return sizeLengths;
			}
		}

		// size of data depends on "maximum possible number"
		public virtual int getNumBytesFromMax(long maxNumber)
		{
			int size = 0;
			while (maxNumber != 0)
			{
				size++;
				maxNumber = (long)((ulong)maxNumber >> 8); // right shift with zero extension
			}
			return size;
		}

		public virtual long readVariableSizeUnsigned(int size)
		{
			long vv;
			if (size == 1)
			{
				vv = DataTypes.unsignedByteToShort(RandomAccessFile.readByte());
			}
			else if (size == 2)
			{
				if (debugPos)
				{
					log.debug("position={}", RandomAccessFile.FilePointer);
				}
				short s = RandomAccessFile.readShort();
				vv = DataTypes.unsignedShortToInt(s);
			}
			else if (size == 4)
			{
				vv = DataTypes.unsignedIntToLong(RandomAccessFile.readInt());
			}
			else if (size == 8)
			{
				vv = RandomAccessFile.readLong();
			}
			else
			{
				vv = readVariableSizeN(size);
			}
			return vv;
		}

		// Little endian
		private long readVariableSizeN(int nbytes)
		{
			int[] ch = new int[nbytes];
			for (int i = 0; i < nbytes; i++)
			{
				ch[i] = RandomAccessFile.read();
			}

			long result = ch[nbytes - 1];
			for (int i = nbytes - 2; i >= 0; i--)
			{
				result = result << 8;
				result += ch[i];
			}

			return result;
		}

		public virtual RandomAccessFile RandomAccessFile
		{
			get
			{
				return h5iosp.RandomAccessFile;
			}
		}

		public virtual long getFileOffset(long address)
		{
			return baseAddress + address;
		}

		public virtual byte SizeOffsets
		{
			get
			{
				return sizeOffsets;
			}
		}

		internal virtual H5objects getH5objects()
		{
			return h5objects;
		}

		public virtual bool Netcdf4
		{
			get
			{
				return isNetcdf4;
			}
		}

		public virtual bool Classic
		{
			get
			{
				return false; // TODO
			}
		}

		public virtual void close()
		{
			if (debugTracker)
			{
				Formatter f = new Formatter();
				memTracker.report(f);
				log.debug("{}", f);
			}
		}

		public virtual void getEosInfo(Formatter f)
		{
			HdfEos.getEosInfo(RandomAccessFile.Location, this, root, f);
		}

		// debug - hdf5Table
		public virtual IList<DataObject> getDataObjects()
		{
			List<DataObject> result = new List<DataObject>(addressMap.Values);
			result.Sort((o1, o2) => o1.address.CompareTo(o2.address));
			return result;
		}

	}

}
