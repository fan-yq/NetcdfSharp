// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf4
{
	using ArrayStructure = NetcdfSharp.ucar.ma2.ArrayStructure;
	using Builder = NetcdfSharp.ucar.nc2.Group.Builder;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using Dimension = NetcdfSharp.ucar.nc2.Dimension;
	using Format = NetcdfSharp.ucar.unidata.util.Format;
	using Group = NetcdfSharp.ucar.nc2.Group;
	using H4type = NetcdfSharp.ucar.nc2.iosp.hdf4.H4type;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using NetcdfFiles = NetcdfSharp.ucar.nc2.NetcdfFiles;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;
	using TagEnum = NetcdfSharp.ucar.nc2.iosp.hdf4.TagEnum;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// Read the tags of an HDF4 file, construct CDM objects.
	/// All page references are to "HDF Specification and Developers Guide" version 4.1r5, nov 2001.
	/// 
	/// @author caron
	/// @since Jul 18, 2007
	/// </summary>
	public class H4header : HdfHeaderIF
	{
		private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(H4header));

		private static readonly byte[] H4HEAD = new byte[] { (byte)0x0e, (byte)0x03, (byte)0x13, (byte)0x01 };
		private static readonly string H4HEAD_STRING = StringHelper.NewString(H4HEAD, Encoding.UTF8);
		private const long maxHeaderPos = 500000; // header's gotta be within this

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: static boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
		internal static bool isValidFile(RandomAccessFile raf)
		{
			// fail fast on directory
			if (raf.Directory)
			{
				return false;
			}
			long pos = 0;
			long size = raf.length();

			// search forward for the header
			while ((pos < (size - H4HEAD.Length)) && (pos < maxHeaderPos))
			{
				raf.seek(pos);
				string magic = raf.readString(H4HEAD.Length);
				if (magic.Equals(H4HEAD_STRING))
				{
					return true;
				}
				pos = (pos == 0) ? 512 : 2 * pos;
			}

			return false;
		}

		private static bool debugDD; // DDH/DD
		private static bool debugTag1; // show tags after read(), before read2().
		private static bool debugTag2; // show tags after everything is done.
		private static bool debugTagDetail; // when showing tags, show detail or not
		private static bool debugConstruct; // show CDM objects as they are constructed
		private static bool debugAtt; // show CDM attributes as they are constructed
		private static bool debugLinked; // linked data
		private static bool debugChunkTable; // chunked data
		private static bool debugChunkDetail; // chunked data
		private static bool debugTracker; // memory tracker
		private static bool warnings; // log messages

		// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private static bool useHdfEos_Conflict = true; // allow to turn hdf eos processing off

		public static NetcdfSharp.ucar.nc2.util.DebugFlags DebugFlags
		{
			set
			{
				debugTag1 = value.isSet("H4header/tag1");
				debugTag2 = value.isSet("H4header/tag2");
				debugTagDetail = value.isSet("H4header/tagDetail");
				debugConstruct = value.isSet("H4header/construct");
				debugAtt = value.isSet("H4header/att");
				debugLinked = value.isSet("H4header/linked");
				debugChunkTable = value.isSet("H4header/chunkTable");
				debugChunkDetail = value.isSet("H4header/chunkDetail");
				debugTracker = value.isSet("H4header/memTracker");
				if (value.isSet("HdfEos/showWork"))
				{
					HdfEos.showWork = true;
				}
			}
		}

		public static void useHdfEos(bool val)
		{
			useHdfEos_Conflict = val;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////

		internal RandomAccessFile raf;
		private Group.Builder root;
		private bool isEos;
		internal string version = "N/A";

		private IList<Tag> alltags;
		private IDictionary<int, Tag> tagMap = new Dictionary<int, Tag>();
		private IDictionary<short, Vinfo> refnoMap = new Dictionary<short, Vinfo>();

		private MemTracker memTracker;
		private PrintWriter debugOut;

		private readonly Charset valueCharset;

		public H4header()
		{
			valueCharset = Encoding.UTF8;
			debugOut = new PrintWriter(JSystem.@out);
		}

		internal H4header(H4iosp h4iosp)
		{
			valueCharset = h4iosp.getValueCharset().orElse(Encoding.UTF8);
			debugOut = new PrintWriter(JSystem.@out);
		}

		/// <summary>
		/// Return defined <seealso cref="Charset value charset"/> that
		/// will be used by reading HDF4 header.
		/// </summary>
		/// <returns> <seealso cref="Charset value charset"/> </returns>
		protected internal virtual Charset ValueCharset
		{
			get
			{
				return valueCharset;
			}
		}

		public virtual bool Eos
		{
			get
			{
				return isEos;
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: void read(NetcdfSharp.ucar.unidata.io.RandomAccessFile myRaf, NetcdfSharp.ucar.nc2.Group.Builder root, NetcdfSharp.java.util.Formatter debugOut) throws NetcdfSharp.java.io.IOException
		internal virtual void read(RandomAccessFile myRaf, Group.Builder root, Formatter debugOut)
		{
			this.raf = myRaf;
			this.root = root;

			long actualSize = raf.length();
			memTracker = new MemTracker(this, actualSize);

			if (!isValidFile(myRaf))
			{
				throw new IOException("Not an HDF4 file ");
			}

			// now we are positioned right after the header
			memTracker.add("header", 0, raf.FilePointer);

			// header information is in big endian byte order
			raf.order(RandomAccessFile.BIG_ENDIAN);
			if (debugConstruct)
			{
				debugOut.format("H4header 0pened file to read:'" + raf.Location + "', size=" + actualSize / 1000 + " Kb");
			}

			// read the DDH and DD records
			alltags = new List<Tag>();
			long link = raf.FilePointer;
			while (link > 0)
			{
				link = readDDH(alltags, link);
			}

			// now read the individual tags where needed
			foreach (Tag tag in alltags)
			{ // LOOK could sort by file offset to minimize I/O
				tag.read();
				tagMap[tagid(tag.refno, tag.code)] = tag; // track all tags in a map, key is the "tag id".
				if (debugTag1)
				{
					Console.WriteLine(debugTagDetail ? tag.detail() : tag);
				}
			}

			// construct the netcdf objects
			construct(alltags);

			if (useHdfEos_Conflict)
			{
				isEos = HdfEos.amendFromODL(raf.Location, this, root);
				if (isEos)
				{
					adjustDimensions();
					string history = root.AttributeContainer.findAttributeString("_History", "");
					root.addAttribute(new NcAttribute("_History", history + "; HDF-EOS StructMetadata information was read"));
				}
			}

			if (debugTag2)
			{
				foreach (Tag tag in alltags)
				{
					debugOut.format("%s", debugTagDetail ? tag.detail() : tag);
				}
			}

			if (debugTracker)
			{
				memTracker.report();
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public void getEosInfo(NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
		public virtual void getEosInfo(Formatter f)
		{
			HdfEos.getEosInfo(raf.Location, this, root, f);
		}

		private static int tagid(short refno, short code)
		{
			int result = (code & 0x3FFF) << 16;
			int result2 = (refno & 0xffff);
			result = result + result2;
			return result;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void construct(NetcdfSharp.java.util.List<Tag> alltags) throws NetcdfSharp.java.io.IOException
		private void construct(IList<Tag> alltags)
		{
			// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
			// ORIGINAL LINE: NetcdfSharp.java.util.List<NetcdfSharp.ucar.nc2.Variable.Builder<?>> vars = new NetcdfSharp.java.util.ArrayList<>();
			IList<Variable.Builder> vars = new List<Variable.Builder>();

			// pass 1 : Vgroups with special classes
			foreach (Tag t in alltags)
			{
				if (t.code == 306)
				{ // raster image
					Variable.Builder v = makeImage((TagGroup)t);
					if (v != null)
					{
						vars.Add(v);
					}

				}
				else if (t.code == 1965)
				{
					TagVGroup vgroup = (TagVGroup)t;
					if (vgroup.className.StartsWith("Dim", StringComparison.Ordinal) || vgroup.className.StartsWith("UDim", StringComparison.Ordinal))
					{
						makeDimension(vgroup);
					}

					else if (vgroup.className.StartsWith("Var", StringComparison.Ordinal))
					{
						Variable.Builder v = makeVariable(vgroup);
						if (v != null)
						{
							vars.Add(v);
						}

					}
					else if (vgroup.className.StartsWith("CDF0.0", StringComparison.Ordinal))
					{
						addGlobalAttributes(vgroup);
					}
				}
			}

			// pass 2 - VHeaders, NDG
			foreach (Tag t in alltags)
			{
				if (t.used)
				{
					continue;
				}

				if (t.code == 1962)
				{ // VHeader
					TagVH tagVH = (TagVH)t;
					if (tagVH.className.StartsWith("Data", StringComparison.Ordinal))
					{
						Variable.Builder v = makeVariable(tagVH);
						if (v != null)
						{
							vars.Add(v);
						}
					}

				}
				else if (t.code == 720)
				{ // numeric data group
					Variable.Builder v = makeVariable((TagGroup)t);
					vars.Add(v);
				}
			}

			// pass 3 - misc not claimed yet
			foreach (Tag t in alltags)
			{
				if (t.used)
				{
					continue;
				}

				if (t.code == 1962)
				{ // VHeader
					TagVH vh = (TagVH)t;
					if (!vh.className.StartsWith("Att", StringComparison.Ordinal) && !vh.className.StartsWith("_HDF_CHK_TBL", StringComparison.Ordinal))
					{
						Variable.Builder v = makeVariable(vh);
						if (v != null)
						{
							vars.Add(v);
						}
					}
				}
			}

			// pass 4 - Groups
			foreach (Tag t in alltags)
			{
				if (t.used)
				{
					continue;
				}

				if (t.code == 1965)
				{ // VGroup
					TagVGroup vgroup = (TagVGroup)t;
					makeGroup(vgroup, root);
				}
			}

			// not already assigned to a group : put in root group.
			foreach (Variable.Builder v in vars)
			{
				Vinfo vinfo = (Vinfo)v.spiObject;
				// if (vinfo.group == null) {
				if (vinfo.group == null && !root.findVariableLocal(v.shortName).isPresent())
				{
					root.addVariable(v);
					vinfo.group = root;
				}
			}

			// annotations become attributes
			foreach (Tag t in alltags)
			{
				if (t is TagAnnotate)
				{
					TagAnnotate ta = (TagAnnotate)t;
					Vinfo vinfo = refnoMap[ta.obj_refno];
					if (vinfo != null)
					{
						vinfo.v.addAttribute(new NcAttribute((t.code == 105) ? "description" : CDM.LONG_NAME, ta.text));
						t.used = true;
					}
				}
			}

			// misc global attributes
			root.addAttribute(new NcAttribute("_History", "Direct read of HDF4 file through CDM library"));
			foreach (Tag t in alltags)
			{
				if (t.code == 30)
				{
					this.version = ((TagVersion)t).value();
					root.addAttribute(new NcAttribute("HDF4_Version", this.version));
					t.used = true;

				}
				else if (t.code == 100)
				{
					root.addAttribute(new NcAttribute("Title-" + t.refno, ((TagText)t).text));
					t.used = true;

				}
				else if (t.code == 101)
				{
					root.addAttribute(new NcAttribute("Description-" + t.refno, ((TagText)t).text));
					t.used = true;
				}
			}

		}

		private void adjustDimensions()
		{
			// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
			// ORIGINAL LINE: NetcdfSharp.com.google.common.collect.Multimap<NetcdfSharp.ucar.nc2.Dimension, NetcdfSharp.ucar.nc2.Variable.Builder<?>> dimUsedMap = NetcdfSharp.com.google.common.collect.ArrayListMultimap.create();
			Multimap<Dimension, Variable.Builder> dimUsedMap = ArrayListMultimap<Dimension, Variable.Builder>.create();
			root.makeDimensionMap(root, dimUsedMap);
			ISet<Dimension> dimUsed = dimUsedMap.keySet();

			// remove unused dimensions from root group
			var rootDims = root.Dimensions;
			for (int i = rootDims.Count - 1; i >= 0; i--)
			{
				Dimension dim = rootDims[i];
				if (!dimUsed.Contains(dim))
				{
					rootDims.RemoveAt(i);
				}
			}

			// push used dimensions to the lowest group that contains all variables
			foreach (Dimension dim in dimUsed)
			{
				Group.Builder lowest = null;
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: NetcdfSharp.java.util.Collection<NetcdfSharp.ucar.nc2.Variable.Builder<?>> vlist = dimUsedMap.get(dim);
				ICollection<Variable.Builder> vlist = dimUsedMap.get(dim);
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: for (NetcdfSharp.ucar.nc2.Variable.Builder<?> v : vlist)
				foreach (Variable.Builder v in vlist)
				{
					Vinfo vinfo = (Vinfo)v.spiObject;
					if (vinfo == null)
					{
						// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
						log.warn(string.Format("adjustDimensions {0} missing vinfo (new)%n", v.shortName));
						continue;
					}
					Group.Builder gb = vinfo.group;
					if (lowest == null)
					{
						lowest = gb;
					}
					else
					{
						lowest = lowest.commonParent(gb);
					}
				}
				if (lowest != null)
				{
					root.removeDimensionFromAllGroups(root, dim);
					lowest.addDimensionIfNotExists(dim);
				}
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void makeDimension(TagVGroup group) throws NetcdfSharp.java.io.IOException
		private void makeDimension(TagVGroup group)
		{
			IList<TagVH> dims = new List<TagVH>();

			Tag data = null;
			for (int i = 0; i < group.nelems; i++)
			{
				Tag tag = tagMap[tagid(group.elem_ref[i], group.elem_tag[i])];
				if (tag == null)
				{
					throw new System.InvalidOperationException();
				}
				if (tag.code == 1962)
				{
					dims.Add((TagVH)tag);
				}
				if (tag.code == 1963)
				{
					data = tag;
				}
			}
			if (dims.Count == 0)
			{
				throw new System.InvalidOperationException();
			}

			int length = 0;
			if (data != null)
			{
				raf.seek(data.offset);
				length = raf.readInt();
				data.used = true;

			}
			else
			{

				foreach (TagVH vh in dims)
				{
					vh.used = true;
					data = tagMap[tagid(vh.refno, TagEnum.VS.Code)];
					if (null != data)
					{
						data.used = true;
						raf.seek(data.offset);
						int length2 = raf.readInt();
						if (debugConstruct)
						{
							Console.WriteLine("dimension length=" + length2 + " for TagVGroup= " + group + " using data " + data.refno);
						}
						if (length2 > 0)
						{
							length = length2;
							break;
						}
					}
				}
			}

			if (data == null)
			{
				log.error("**no data for dimension TagVGroup= " + group);
				return;
			}

			if (length <= 0)
			{
				log.warn("**dimension length=" + length + " for TagVGroup= " + group + " using data " + data.refno);
			}

			bool isUnlimited = (length == 0);
			Dimension dim = Dimension.builder().setName(group.name).setIsUnlimited(isUnlimited).setLength(length).build();
			root.addDimension(dim);
		}

		private void addGlobalAttributes(TagVGroup group)
		{
			// look for attributes
			for (int i = 0; i < group.nelems; i++)
			{
				Tag tag = tagMap[tagid(group.elem_ref[i], group.elem_tag[i])];
				if (tag == null)
				{
					throw new System.InvalidOperationException();
				}
				if (tag.code == 1962)
				{
					TagVH vh = (TagVH)tag;
					if (vh.className.StartsWith("Att", StringComparison.Ordinal))
					{
						string lowername = vh.name.ToLower();
						if ((vh.nfields == 1) && (H4type.setDataType(vh.fld_type[0], null) == DataType.CHAR) && ((vh.fld_isize[0] > 4000) || lowername.StartsWith("archivemetadata", StringComparison.Ordinal) || lowername.StartsWith("coremetadata", StringComparison.Ordinal) || lowername.StartsWith("productmetadata", StringComparison.Ordinal) || lowername.StartsWith("structmetadata", StringComparison.Ordinal)))
						{
							root.addVariable(makeVariable(vh)); // // large EOS metadata - make into variable in root group
						}
						else
						{
							NcAttribute att = makeAttribute(vh);
							if (null != att)
							{
								root.addAttribute(att); // make into attribute in root group
							}
						}
					}
				}
			}

			group.used = true;
		}

		private NcAttribute makeAttribute(TagVH vh)
		{

			Tag data = tagMap[tagid(vh.refno, TagEnum.VS.Code)];
			if (data == null)
			{
				throw new System.InvalidOperationException();
			}

			// for now assume only 1
			if (vh.nfields != 1)
			{
				throw new System.InvalidOperationException();
			}

			string name = vh.name;
			short type = vh.fld_type[0];
			int size = vh.fld_isize[0];
			int nelems = vh.nvert;
			vh.used = true;
			data.used = true;

			NcAttribute att = null;
			raf.seek(data.offset);
			switch (type)
			{
				case 3:
				case 4:
					if (nelems == 1)
					{
						att = new NcAttribute(name, raf.readStringMax(size, valueCharset));
					}
					else
					{
						string[] vals = new string[nelems];
						for (int i = 0; i < nelems; i++)
						{
							vals[i] = raf.readStringMax(size, valueCharset);
						}
						att = new NcAttribute(name, NcArray.factory(DataType.STRING, new int[] { nelems }, vals));
					}
					break;
				case 5:
					if (nelems == 1)
					{
						att = new NcAttribute(name, raf.readFloat());
					}
					else
					{
						float[] vals = new float[nelems];
						for (int i = 0; i < nelems; i++)
						{
							vals[i] = raf.readFloat();
						}
						att = new NcAttribute(name, NcArray.factory(DataType.FLOAT, new int[] { nelems }, vals));
					}
					break;
				case 6:
					if (nelems == 1)
					{
						att = new NcAttribute(name, raf.readDouble());
					}
					else
					{
						double[] vals = new double[nelems];
						for (int i = 0; i < nelems; i++)
						{
							vals[i] = raf.readDouble();
						}
						att = new NcAttribute(name, NcArray.factory(DataType.DOUBLE, new int[] { nelems }, vals));
					}
					break;
				case 20:
				case 21:
					if (nelems == 1)
					{
						att = new NcAttribute(name, raf.readByte());
					}
					else
					{
						byte[] vals = new byte[nelems];
						for (int i = 0; i < nelems; i++)
						{
							vals[i] = raf.readByte();
						}
						att = new NcAttribute(name, NcArray.factory(DataType.BYTE, new int[] { nelems }, vals));
					}
					break;
				case 22:
				case 23:
					if (nelems == 1)
					{
						att = new NcAttribute(name, raf.readShort());
					}
					else
					{
						short[] vals = new short[nelems];
						for (int i = 0; i < nelems; i++)
						{
							vals[i] = raf.readShort();
						}
						att = new NcAttribute(name, NcArray.factory(DataType.SHORT, new int[] { nelems }, vals));
					}
					break;
				case 24:
				case 25:
					if (nelems == 1)
					{
						att = new NcAttribute(name, raf.readInt());
					}
					else
					{
						int[] vals = new int[nelems];
						for (int i = 0; i < nelems; i++)
						{
							vals[i] = raf.readInt();
						}
						att = new NcAttribute(name, NcArray.factory(DataType.INT, new int[] { nelems }, vals));
					}
					break;
				case 26:
				case 27:
					if (nelems == 1)
					{
						att = new NcAttribute(name, raf.readLong());
					}
					else
					{
						long[] vals = new long[nelems];
						for (int i = 0; i < nelems; i++)
						{
							vals[i] = raf.readLong();
						}
						att = new NcAttribute(name, NcArray.factory(DataType.LONG, new int[] { nelems }, vals));
					}
					break;
			}

			if (debugAtt)
			{
				Console.WriteLine("added attribute " + att);
			}
			return att;
		}

		private Group.Builder makeGroup(TagVGroup tagGroup, Group.Builder parent)
		{
			if (tagGroup.nelems < 1)
			{
				return null;
			}

			Group.Builder group = Group.builder().setName(tagGroup.name);
			parent.addGroup(group);
			tagGroup.used = true;
			tagGroup.group = group;

			for (int i = 0; i < tagGroup.nelems; i++)
			{
				Tag tag = tagMap[tagid(tagGroup.elem_ref[i], tagGroup.elem_tag[i])];
				if (tag == null)
				{
					log.error("Reference tag missing= " + tagGroup.elem_ref[i] + "/" + tagGroup.elem_tag[i] + " for group " + tagGroup.refno);
					continue;
				}

				if (tag.code == 720)
				{ // NG - prob var
					if (tag.vinfo != null)
					{
						Variable.Builder v = tag.vinfo.v;
						if (v != null)
						{
							addVariableToGroup(group, v, tag);
						}
						else
						{
							log.error("Missing variable " + tag.refno);
						}
					}
				}

				if (tag.code == 1962)
				{ // Vheader - may be an att or a var
					TagVH vh = (TagVH)tag;
					if (vh.className.StartsWith("Att", StringComparison.Ordinal))
					{
						NcAttribute att = makeAttribute(vh);
						if (null != att)
						{
							group.addAttribute(att);
						}
					}
					else if (tag.vinfo != null)
					{
						Variable.Builder v = tag.vinfo.v;
						addVariableToGroup(group, v, tag);
					}
				}

				/*
				 * original
				 * if (tag.code == 1965) { // VGroup - prob a Group
				 * TagVGroup vg = (TagVGroup) tag;
				 * if ((vg.group != null) && (vg.group.getParentGroup() == ncfile.getRootGroup())) {
				 * addGroupToGroup(group, vg.group, vg);
				 * vg.group.setParentGroup(group);
				 * } else {
				 * Group nested = makeGroup(vg, group); // danger - loops
				 * if (nested != null)
				 * addGroupToGroup(group, nested, vg);
				 * }
				 * }
				 */

				if (tag.code == 1965)
				{ // VGroup - prob a Group
					TagVGroup vg = (TagVGroup)tag;
					if ((vg.group != null) && (vg.group.getParentGroup() == root))
					{
						addGroupToGroup(group, vg.group, vg);
					}
					else
					{
						// makeGroup adds the nested group.
						makeGroup(vg, group);
					}
				}
			}

			if (debugConstruct)
			{
				Console.WriteLine("added group " + group.shortName + " from VG " + tagGroup.refno);
			}

			return group;
		}

		private void addVariableToGroup(Group.Builder g, Variable.Builder v, Tag tag)
		{
			g.findVariableLocal(v.shortName).ifPresent(varExisting => v.setName(v.shortName + tag.refno)); // disambiguate
			g.addVariable(v);
			tag.vinfo.group = g;
		}

		private void addGroupToGroup(Group.Builder parent, Group.Builder g, Tag tag)
		{
			// may have to reparent the group
			root.removeGroup(g.shortName);

			parent.findGroupLocal(g.shortName).ifPresent(groupExisting => g.setName(g.shortName + tag.refno)); // disambiguate
																											   // name

			parent.addGroup(g);
		}

		private Variable.Builder makeImage(TagGroup group)
		{
			TagRIDimension dimTag = null;
			TagNumberType ntag;
			Tag data = null;

			Vinfo vinfo = new Vinfo(this, group.refno);
			group.used = true;

			// use the list of elements in the group to find the other tags
			for (int i = 0; i < group.nelems; i++)
			{
				Tag tg = tagMap[tagid(group.elem_ref[i], group.elem_tag[i])];
				if (tg == null)
				{
					log.warn("Image Group " + group.tag() + " has missing tag=" + group.elem_ref[i] + "/" + group.elem_tag[i]);
					return null;
				}

				vinfo.tags.Add(tg);
				tg.vinfo = vinfo; // track which variable this tg belongs to
				tg.used = true; // assume if contained in Group, then used, to avoid redundant variables

				if (tg.code == 300)
				{
					dimTag = (TagRIDimension)tg;
				}
				if (tg.code == 302)
				{
					data = tg;
				}
				if (tg.code == 301)
				{
				}
			}
			if (dimTag == null)
			{
				log.warn("Image Group " + group.tag() + " missing dimension tag");
				return null;
			}
			if (data == null)
			{
				log.warn("Image Group " + group.tag() + " missing data tag");
				return null;
			}

			// get the NT tag, referred to from the dimension tag
			Tag tag = tagMap[tagid(dimTag.nt_ref, TagEnum.NT.Code)];
			if (tag == null)
			{
				log.warn("Image Group " + group.tag() + " missing NT tag");
				return null;
			}
			ntag = (TagNumberType)tag;

			if (debugConstruct)
			{
				Console.WriteLine("construct image " + group.refno);
			}
			vinfo.start = data.offset;
			vinfo.tags.Add(group);
			vinfo.tags.Add(dimTag);
			vinfo.tags.Add(data);
			vinfo.tags.Add(ntag);

			// assume dimensions are not shared for now
			if (dimTag.dims == null)
			{
				dimTag.dims = new List<Dimension>();
				dimTag.dims.Add(makeDimensionUnshared("ydim", dimTag.ydim));
				dimTag.dims.Add(makeDimensionUnshared("xdim", dimTag.xdim));
			}

			Variable.Builder vb = Variable.builder().setName("Image-" + group.refno);
			vb.setDataType(H4type.setDataType(ntag.type, null));
			vb.addDimensions(dimTag.dims);
			vinfo.setVariable(vb);

			return vb;
		}

		private Dimension makeDimensionUnshared(string dimName, int len)
		{
			return Dimension.builder(dimName, len).setIsShared(false).build();
		}

		private Structure makeChunkVariable(NetcdfFile ncfile, TagVH vh)
		{
			Vinfo vinfo = new Vinfo(this, vh.refno);
			vinfo.tags.Add(vh);
			vh.vinfo = vinfo;
			vh.used = true;

			TagData data = (TagData)tagMap[tagid(vh.refno, TagEnum.VS.Code)];
			if (data == null)
			{
				log.error("Cant find tag " + vh.refno + "/" + TagEnum.VS.Code + " for TagVH=" + vh.detail());
				return null;
			}
			vinfo.tags.Add(data);
			data.used = true;
			data.vinfo = vinfo;

			if (vh.nfields < 1)
			{
				throw new System.InvalidOperationException();
			}

			try
			{
				Structure.BuilderStruct sb = Structure.builder1().setName1(vh.name);
				vinfo.setVariable(sb);
				Structure s = new Structure(ncfile, null, null, vh.name); // LOOK cheating
				s.setSPobject(vinfo);

				if (vh.nvert > 1)
				{
					s.setDimensionsAnonymous(new int[] { vh.nvert });
				}
				else
				{
					s.setIsScalar();
				}

				for (int fld = 0; fld < vh.nfields; fld++)
				{
					Variable m = new Variable(ncfile, null, s, vh.fld_name[fld]);
					short type = vh.fld_type[fld];
					short nelems = vh.fld_order[fld];
					H4type.setDataType(type, m);
					if (nelems > 1)
					{
						m.setDimensionsAnonymous(new int[] { nelems });
					}
					else
					{
						m.setIsScalar();
					}

					m.setSPobject(new Minfo(vh.fld_offset[fld]));
					s.addMemberVariable(m);
				}

				vinfo.setData(data, vh.ivsize);
				return s;

			}
			catch (InvalidRangeException e)
			{
				throw new System.InvalidOperationException(e.Message);
			}
		}

		private Variable.Builder makeVariable(TagVH vh)
		{
			Vinfo vinfo = new Vinfo(this, vh.refno);
			vinfo.tags.Add(vh);
			vh.vinfo = vinfo;
			vh.used = true;

			TagData data = (TagData)tagMap[tagid(vh.refno, TagEnum.VS.Code)];
			if (data == null)
			{
				log.error("Cant find tag " + vh.refno + "/" + TagEnum.VS.Code + " for TagVH=" + vh.detail());
				return null;
			}
			vinfo.tags.Add(data);
			data.used = true;
			data.vinfo = vinfo;

			if (vh.nfields < 1)
			{
				throw new System.InvalidOperationException();
			}

			Variable.Builder vb;
			if (vh.nfields == 1)
			{
				// String name = createValidObjectName(vh.name);
				vb = Variable.builder().setName(vh.name);
				vinfo.setVariable(vb);
				vb.setDataType(H4type.setDataType(vh.fld_type[0], null));

				if (vh.nvert > 1)
				{

					if (vh.fld_order[0] > 1)
					{
						vb.setDimensionsAnonymous(new int[] { vh.nvert, vh.fld_order[0] });
					}
					else if (vh.fld_order[0] < 0)
					{
						vb.setDimensionsAnonymous(new int[] { vh.nvert, vh.fld_isize[0] });
					}
					else
					{
						vb.setDimensionsAnonymous(new int[] { vh.nvert });
					}

				}
				else
				{

					if (vh.fld_order[0] > 1)
					{
						vb.setDimensionsAnonymous(new int[] { vh.fld_order[0] });
					}
					else if (vh.fld_order[0] < 0)
					{
						vb.setDimensionsAnonymous(new int[] { vh.fld_isize[0] });
					}
					else
					{
						vb.setIsScalar();
					}
				}

				vinfo.setData(data, vb.dataType.getSize());

			}
			else
			{

				Structure.BuilderStruct s = Structure.builder1().setName1(vh.name);
				vinfo.setVariable(s);
				// vinfo.recsize = vh.ivsize;

				if (vh.nvert > 1)
				{
					s.setDimensionsAnonymous(new int[] { vh.nvert });
				}
				else
				{
					s.setIsScalar();
				}

				for (int fld = 0; fld < vh.nfields; fld++)
				{
					Variable.Builder m = Variable.builder().setName(vh.fld_name[fld]);
					short type = vh.fld_type[fld];
					short nelems = vh.fld_order[fld];
					m.setDataType(H4type.setDataType(type, null));
					if (nelems > 1)
					{
						m.setDimensionsAnonymous(new int[] { nelems });
					}
					else
					{
						m.setIsScalar();
					}

					m.setSPobject(new Minfo(vh.fld_offset[fld]));
					s.addMemberVariable(m);
				}

				vinfo.setData(data, vh.ivsize);
				vb = s;
			}

			if (debugConstruct)
			{
				Console.WriteLine("added variable " + vb.shortName + " from VH " + vh);
			}

			return vb;
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
			Vinfo vinfo = makeVinfo((short)-1);
			vinfo.group = parent;
			vinfo.setVariable(v);
		}

		public virtual string readStructMetadata(Variable.Builder structMetadataVar)
		{
			Vinfo vinfo = (Vinfo)structMetadataVar.spiObject;
			return vinfo.read();
		}

		// member info
		internal class Minfo
		{
			internal int offset;

			internal Minfo(int offset)
			{
				this.offset = offset;
			}
		}

		private Variable.Builder makeVariable(TagVGroup group)
		{
			Vinfo vinfo = new Vinfo(this, group.refno);
			vinfo.tags.Add(group);
			group.used = true;

			TagSDDimension dim = null;
			TagNumberType ntag = null;
			TagData data = null;
			IList<Dimension> dims = new List<Dimension>();
			for (int i = 0; i < group.nelems; i++)
			{
				Tag tag = tagMap[tagid(group.elem_ref[i], group.elem_tag[i])];
				if (tag == null)
				{
					log.error("Reference tag missing= " + group.elem_ref[i] + "/" + group.elem_tag[i]);
					continue;
				}

				vinfo.tags.Add(tag);
				tag.vinfo = vinfo; // track which variable this tag belongs to
				tag.used = true; // assume if contained in Vgroup, then not needed, to avoid redundant variables

				if (tag.code == 106)
				{
					ntag = (TagNumberType)tag;
				}
				if (tag.code == 701)
				{
					dim = (TagSDDimension)tag;
				}
				if (tag.code == 702)
				{
					data = (TagData)tag;
				}

				if (tag.code == 1965)
				{
					TagVGroup vg = (TagVGroup)tag;
					if (vg.className.StartsWith("Dim", StringComparison.Ordinal) || vg.className.StartsWith("UDim", StringComparison.Ordinal))
					{
						string dimName = NetcdfFiles.makeValidCdmObjectName(vg.name);
						Dimension d = root.findDimension(dimName).orElseThrow(() => new System.InvalidOperationException());
						dims.Add(d);
					}
				}
			}
			if (ntag == null)
			{
				log.error("ntype tag missing vgroup= " + group.refno);
				return null;
			}
			if (dim == null)
			{
				log.error("dim tag missing vgroup= " + group.refno);
				return null;
			}
			if (data == null)
			{
				log.warn("data tag missing vgroup= " + group.refno + " " + group.name);
				// return null;
			}
			Variable.Builder vb = Variable.builder().setName(group.name);
			vb.addDimensions(dims);
			vb.setDataType(H4type.setDataType(ntag.type, null));

			vinfo.setVariable(vb);
			vinfo.setData(data, vb.dataType.getSize());

			// apparently the 701 SDDimension tag overrides the VGroup dimensions
			Debug.Assert(dim.shape.Length == vb.getRank());
			bool ok = true;
			IList<Dimension> vdimensions = vb.getDimensions();
			for (int i = 0; i < dim.shape.Length; i++)
			{
				Dimension vdim = vdimensions[i];
				if (dim.shape[i] != vdim.Length)
				{
					if (warnings)
					{
						log.info(dim.shape[i] + " != " + vdim.Length + " for " + vb.shortName);
					}
					ok = false;
				}
			}

			if (!ok)
			{
				vb.setDimensionsAnonymous(dim.shape);
			}

			// look for attributes
			addVariableAttributes(group, vinfo);

			if (debugConstruct)
			{
				Console.WriteLine("added variable " + vb.shortName + " from VG " + group.refno);
				Console.WriteLine("  SDdim= " + dim.detail());
				Console.Write("  VGdim= ");
				foreach (Dimension vdim in dims)
				{
					Console.Write(vdim + " ");
				}
				Console.WriteLine();
			}

			return vb;
		}

		private Variable.Builder makeVariable(TagGroup group)
		{
			Vinfo vinfo = new Vinfo(this, group.refno);
			vinfo.tags.Add(group);
			group.used = true;

			TagSDDimension dim = null;
			TagData data = null;
			for (int i = 0; i < group.nelems; i++)
			{
				Tag tag = tagMap[tagid(group.elem_ref[i], group.elem_tag[i])];
				if (tag == null)
				{
					log.error("Cant find tag " + group.elem_ref[i] + "/" + group.elem_tag[i] + " for group=" + group.refno);
					continue;
				}
				vinfo.tags.Add(tag);
				tag.vinfo = vinfo; // track which variable this tag belongs to
				tag.used = true; // assume if contained in Group, then used, to avoid redundant variables

				if (tag.code == 701)
				{
					dim = (TagSDDimension)tag;
				}
				if (tag.code == 702)
				{
					data = (TagData)tag;
				}
			}
			if ((dim == null) || (data == null))
			{
				throw new System.InvalidOperationException();
			}

			TagNumberType nt = (TagNumberType)tagMap[tagid(dim.nt_ref, TagEnum.NT.Code)];
			if (null == nt)
			{
				throw new System.InvalidOperationException();
			}

			Variable.Builder vb = Variable.builder().setName("SDS-" + group.refno);
			vb.setDimensionsAnonymous(dim.shape);
			DataType dataType = H4type.setDataType(nt.type, null);
			vb.setDataType(dataType);

			vinfo.setVariable(vb);
			vinfo.setData(data, vb.dataType.getSize());

			// now that we know n, read attribute tags
			for (int i = 0; i < group.nelems; i++)
			{
				Tag tag = tagMap[tagid(group.elem_ref[i], group.elem_tag[i])];
				if (tag == null)
				{
					throw new System.InvalidOperationException();
				}

				if (tag.code == 704)
				{
					TagTextN labels = (TagTextN)tag;
					labels.read(dim.rank);
					tag.used = true;
					vb.addAttribute(NcAttribute.builder().setName(CDM.LONG_NAME).setValues((IList<object>)(System.Collections.IList)labels.getList(), false).build());
				}
				if (tag.code == 705)
				{
					TagTextN units = (TagTextN)tag;
					units.read(dim.rank);
					tag.used = true;
					vb.addAttribute(NcAttribute.builder().setName(CDM.UNITS).setValues((IList<object>)(System.Collections.IList)units.getList(), false).build());
				}
				if (tag.code == 706)
				{
					TagTextN formats = (TagTextN)tag;
					formats.read(dim.rank);
					tag.used = true;
					vb.addAttribute(NcAttribute.builder().setName("formats").setValues((IList<object>)(System.Collections.IList)formats.getList(), false).build());
				}
				if (tag.code == 707)
				{
					TagSDminmax minmax = (TagSDminmax)tag;
					tag.used = true;
					vb.addAttribute(new NcAttribute("min", minmax.getMin(dataType)));
					vb.addAttribute(new NcAttribute("max", minmax.getMax(dataType)));
				}
			}

			// look for VH style attributes - dunno if they are actually used
			addVariableAttributes(group, vinfo);

			if (debugConstruct)
			{
				Console.WriteLine("added variable " + vb.shortName + " from Group " + group);
				Console.WriteLine("  SDdim= " + dim.detail());
			}

			return vb;
		}

		private void addVariableAttributes(TagGroup group, Vinfo vinfo)
		{
			// look for attributes
			for (int i = 0; i < group.nelems; i++)
			{
				Tag tag = tagMap[tagid(group.elem_ref[i], group.elem_tag[i])];
				if (tag == null)
				{
					throw new System.InvalidOperationException();
				}
				if (tag.code == 1962)
				{
					TagVH vh = (TagVH)tag;
					if (vh.className.StartsWith("Att", StringComparison.Ordinal))
					{
						NcAttribute att = makeAttribute(vh);
						if (null != att)
						{
							vinfo.v.addAttribute(att);
							if (att.getShortName().Equals(CDM.FILL_VALUE))
							{
								vinfo.setFillValue(att);
							}
						}
					}
				}
			}
		}

		//////////////////////////////////////////////////////////////////////

		internal virtual Vinfo makeVinfo(short refno)
		{
			return new Vinfo(this, refno);
		}

		internal class Vinfo : IComparable<Vinfo>
		{
			private readonly H4header outerInstance;

			internal short refno;
			internal Variable.Builder v;
			internal Group.Builder group;
			internal IList<Tag> tags = new List<Tag>();

			// info about reading the data
			internal TagData data;
			internal int elemSize; // for Structures, this is recsize
			internal object fillValue;

			// below is not set until setLayoutInfo() is called
			internal bool isLinked, isCompressed, isChunked, hasNoData;

			// regular
			internal int start = -1;
			internal int length;

			// linked
			internal long[] segPos;
			internal int[] segSize;

			// chunked
			internal IList<DataChunk> chunks;
			internal int[] chunkSize;

			internal Vinfo(H4header outerInstance, short refno)
			{
				this.outerInstance = outerInstance;
				this.refno = refno;
				if (refno >= 0)
				{
					outerInstance.refnoMap[refno] = this;
				}
			}

			internal virtual void setVariable(Variable.Builder v)
			{
				this.v = v;
				v.setSPobject(this);
			}

			public virtual int CompareTo(Vinfo o)
			{
				return refno.CompareTo(o.refno);
			}

			internal virtual void setData(TagData data, int elemSize)
			{
				this.data = data;
				this.elemSize = elemSize;
				hasNoData = (data == null);
			}

			internal virtual void setFillValue(NcAttribute att)
			{
				// see IospHelper.makePrimitiveArray(int size, DataType dataType, Object fillValue)
				fillValue = (v.dataType == DataType.STRING) ? att.getStringValue() : att.getNumericValue();
			}

			// make sure needed info is present : call this when variable needs to be read
			// this allows us to defer getting layout info until then
			internal virtual NetcdfFile LayoutInfo
			{
				set
				{
					if (data == null)
					{
						return;
					}

					if (null != data.linked)
					{
						isLinked = true;
						setDataBlocks(data.linked.LinkedDataBlocks, elemSize);

					}
					else if (null != data.compress)
					{
						isCompressed = true;
						TagData compData = data.compress.getDataTag();
						tags.Add(compData);
						isLinked = (compData.linked != null);
						if (isLinked)
						{
							setDataBlocks(compData.linked.LinkedDataBlocks, elemSize);
						}
						else
						{
							start = compData.offset;
							length = compData.length;
							hasNoData = (start < 0) || (length < 0);
						}

					}
					else if (null != data.chunked)
					{
						isChunked = true;
						chunks = data.chunked.getDataChunks(value);
						chunkSize = data.chunked.chunk_length;
						isCompressed = data.chunked.isCompressed;

					}
					else
					{
						start = data.offset;
						hasNoData = (start < 0);
					}
				}
			}

			internal virtual void setDataBlocks(IList<TagLinkedBlock> linkedBlocks, int elemSize)
			{
				int nsegs = linkedBlocks.Count;
				segPos = new long[nsegs];
				segSize = new int[nsegs];
				int count = 0;
				foreach (TagLinkedBlock tag in linkedBlocks)
				{
					segPos[count] = tag.offset;

					// option 1
					// the size must be a multiple of elemSize - assume remaining is unused
					// int nelems = tag.length / elemSize;
					// segSize[count] = nelems * elemSize;

					// option 2
					// Stucture that requires requires full use of the block length.
					// structure size = 12, block size = 4096, has 4 extra bytes
					// E:/problem/AIRS.2007.10.17.L1B.Cal_Subset.v5.0.16.0.G07292194950.hdf#L1B_AIRS_Cal_Subset/Data
					// Fields/radiances
					// LOOK do we sometimes need option 1) above ??
					segSize[count] = tag.length;
					count++;
				}
			}

			internal virtual IList<DataChunk> readChunks(NetcdfFile ncfile)
			{
				return data.chunked.getDataChunks(ncfile);
			}

			internal virtual string read()
			{
				outerInstance.raf.seek(data.offset);
				return outerInstance.raf.readString(data.length, outerInstance.valueCharset);
			}

			public override string ToString()
			{
				StringBuilder sbuff = new StringBuilder();
				sbuff.Append("refno=").Append(refno).Append(" name=").Append(v.shortName).Append(" fillValue=").Append(fillValue).Append("\n");
				sbuff.Append(" isChunked=").Append(isChunked).Append(" isCompressed=").Append(isCompressed).Append(" isLinked=").Append(isLinked).Append(" hasNoData=").Append(hasNoData).Append("\n");
				sbuff.Append(" elemSize=").Append(elemSize).Append(" data start=").Append(start).Append(" length=").Append(length).Append("\n\n");
				foreach (Tag t in tags)
				{
					sbuff.Append(" ").Append(t.detail()).Append("\n");
				}
				return sbuff.ToString();
			}

		}

		//////////////////////////////////////////////////////////////////////

		private long readDDH(IList<Tag> alltags, long start)
		{
			raf.seek(start);

			int ndd = DataTypes.unsignedShortToInt(raf.readShort()); // number of DD blocks
			long link = DataTypes.unsignedIntToLong(raf.readInt()); // point to the next DDH; link == 0 means no more
			if (debugDD)
			{
				Console.WriteLine(" DDHeader ndd=" + ndd + " link=" + link);
			}

			long pos = raf.FilePointer;
			for (int i = 0; i < ndd; i++)
			{
				raf.seek(pos);
				Tag tag = factory();
				pos += 12; // tag usually changed the file pointer
				if (tag.code > 1)
				{
					alltags.Add(tag);
				}
			}
			memTracker.add("DD block", start, raf.FilePointer);
			return link;
		}

		////////////////////////////////////////////////////////////////////////////////
		// Tags

		private Tag factory()
		{
			short code = raf.readShort();
			int ccode = code & 0x3FFF;
			switch (ccode)
			{
				case 20:
					return new TagLinkedBlock(this, code);
				case 30:
					return new TagVersion(this, code);
				case 40: // Compressed
				case 61: // Chunk
				case 702: // Scientific data
				case 1963: // VData
					return new TagData(this, code);
				case 100:
				case 101:
				case 708:
					return new TagText(this, code);
				case 104:
				case 105:
					return new TagAnnotate(this, code);
				case 106:
					return new TagNumberType(this, code);
				case 300:
				case 307:
				case 308:
					return new TagRIDimension(this, code);
				case 301:
					return new TagRIPalette(this, code);
				case 306:
				case 720:
					return new TagGroup(this, code);
				case 701:
					return new TagSDDimension(this, code);
				case 704:
				case 705:
				case 706:
					return new TagTextN(this, code);
				case 707:
					return new TagSDminmax(this, code);
				case 1962:
					return new TagVH(this, code);
				case 1965:
					return new TagVGroup(this, code);
				default:
					return new Tag(this, code);
			}
		}

		// public for debugging (NetcdfSharp.ucar.nc2.ui.Hdf4Table)
		// Tag == "Data Descriptor" (DD) and (usually) a "Data Element" that the offset/length points to
		public class Tag
		{
			protected internal readonly H4header outerInstance;

			internal short code;
			internal short refno;
			internal bool extended;
			internal int offset, length;
			internal TagEnum t;
			internal bool used;
			internal Vinfo vinfo;

			// read just the DD part of the tag. see p 11
			internal Tag(H4header outerInstance, short code)
			{
				this.outerInstance = outerInstance;
				this.extended = (code & 0x4000) != 0;
				this.code = (short)(code & 0x3FFF);
				refno = outerInstance.raf.readShort();
				offset = outerInstance.raf.readInt();
				length = outerInstance.raf.readInt();
				t = TagEnum.getTag(this.code);
				if ((code > 1) && H4header.debugTracker)
				{
					outerInstance.memTracker.add(t.Name + " " + refno, offset, offset + length);
				}
			}

			// read the offset/length part of the tag. overridden by subclasses
			protected internal virtual void read()
			{
			}

			public virtual string detail()
			{
				return (used ? " " : "*") + "refno=" + refno + " tag= " + t + (extended ? " EXTENDED" : "") + " offset=" + offset + " length=" + length + (((vinfo != null) && (vinfo.v != null)) ? " VV=" + vinfo.v.shortName : "");
			}

			public override string ToString()
			{
				return (used ? " " : "*") + "refno=" + refno + " tag= " + t + (extended ? " EXTENDED" : "" + " length=" + length);
			}

			public virtual string tag()
			{
				return refno + "/" + code;
			}

			public virtual short getCode()
			{
				return code;
			}

			public virtual short getRefno()
			{
				return refno;
			}

			public virtual bool isExtended()
			{
				return extended;
			}

			public virtual int getOffset()
			{
				return offset;
			}

			public virtual int getLength()
			{
				return length;
			}

			public virtual string getType()
			{
				return t.ToString();
			}

			public virtual bool isUsed()
			{
				return used;
			}

			public virtual string getVinfo()
			{
				return (vinfo == null) ? "" : vinfo.ToString();
			}

			public virtual string getVClass()
			{
				if (this is H4header.TagVGroup)
				{
					return ((H4header.TagVGroup)this).className;
				}
				if (this is H4header.TagVH)
				{
					return ((H4header.TagVH)this).className;
				}
				return "";
			}
		}

		// 40 (not documented), 702 p 129
		internal class TagData : Tag
		{
			internal short ext_type;
			internal SpecialLinked linked;
			internal SpecialComp compress;
			internal SpecialChunked chunked;
			internal int tag_len;

			internal TagData(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				if (extended)
				{
					outerInstance.raf.seek(offset);
					ext_type = outerInstance.raf.readShort(); // note size wrong in doc

					if (ext_type == TagEnum.SPECIAL_LINKED)
					{
						linked = new SpecialLinked(outerInstance);
						linked.read();

					}
					else if (ext_type == TagEnum.SPECIAL_COMP)
					{
						compress = new SpecialComp(outerInstance);
						compress.read();

					}
					else if (ext_type == TagEnum.SPECIAL_CHUNKED)
					{
						chunked = new SpecialChunked(outerInstance);
						chunked.read();
					}
					tag_len = (int)(outerInstance.raf.FilePointer - offset);
				}
			}

			public override string detail()
			{
				if (linked != null)
				{
					return base.detail() + " ext_tag= " + ext_type + " tag_len= " + tag_len + " " + linked.detail();
				}
				else if (compress != null)
				{
					return base.detail() + " ext_tag= " + ext_type + " tag_len= " + tag_len + " " + compress.detail();
				}
				else if (chunked != null)
				{
					return base.detail() + " ext_tag= " + ext_type + " tag_len= " + tag_len + " " + chunked.detail();
				}
				else
				{
					return base.detail();
				}
			}

		}

		// p 147-150
		internal class SpecialChunked
		{
			private readonly H4header outerInstance;

			internal byte version, flag;
			internal short chunk_tbl_tag, chunk_tbl_ref;
			internal int head_len, elem_tot_length, chunk_size, nt_size, ndims;

			internal int[] dim_length, chunk_length;
			internal byte[][] dim_flag;
			internal bool isCompressed;

			internal short sp_tag_desc; // SPECIAL_XXX constant
			internal byte[] sp_tag_header;
			internal IList<DataChunk> dataChunks;

			internal SpecialChunked(H4header outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal virtual void read()
			{
				head_len = outerInstance.raf.readInt();
				version = outerInstance.raf.readByte();
				outerInstance.raf.skipBytes(3);
				flag = outerInstance.raf.readByte();
				elem_tot_length = outerInstance.raf.readInt();
				chunk_size = outerInstance.raf.readInt();
				nt_size = outerInstance.raf.readInt();

				chunk_tbl_tag = outerInstance.raf.readShort();
				chunk_tbl_ref = outerInstance.raf.readShort();
				outerInstance.raf.skipBytes(4);
				ndims = outerInstance.raf.readInt();

				dim_flag = new byte[ndims][];
				for (int i = 0; i < ndims; i++)
				{
					dim_flag[i] = new byte[4];
				}
				dim_length = new int[ndims];
				chunk_length = new int[ndims];
				for (int i = 0; i < ndims; i++)
				{
					outerInstance.raf.readFully(dim_flag[i]);
					dim_length[i] = outerInstance.raf.readInt();
					chunk_length[i] = outerInstance.raf.readInt();
				}

				int fill_val_numtype = outerInstance.raf.readInt();
				byte[] fill_value = new byte[fill_val_numtype];
				outerInstance.raf.readFully(fill_value);

				// LOOK wuzzit stuff? "specialness"
				sp_tag_desc = outerInstance.raf.readShort();
				int sp_header_len = outerInstance.raf.readInt();
				sp_tag_header = new byte[sp_header_len];
				outerInstance.raf.readFully(sp_tag_header);
			}

			internal virtual IList<DataChunk> getDataChunks(NetcdfFile ncfile)
			{
				if (dataChunks == null)
				{
					dataChunks = new List<DataChunk>();

					// read the chunk table - stored as a Structure in the data
					if (H4header.debugChunkTable)
					{
						Console.WriteLine(" TagData getChunkedTable " + detail());
					}
					TagVH chunkTableTag = (TagVH)outerInstance.tagMap[tagid(chunk_tbl_ref, chunk_tbl_tag)];
					Structure s = outerInstance.makeChunkVariable(ncfile, chunkTableTag);
					if (s == null)
					{
						throw new System.InvalidOperationException("cant parse " + chunkTableTag);
					}
					ArrayStructure sdata = (ArrayStructure)s.read();
					if (H4header.debugChunkDetail)
					{
						Console.WriteLine("getChunkedTable: " + sdata);
					}

					// construct the chunks
					StructureMembers members = sdata.getStructureMembers();
					StructureMembers.Member originM = members.findMember("origin");
					StructureMembers.Member tagM = members.findMember("chk_tag");
					StructureMembers.Member refM = members.findMember("chk_ref");
					int n = (int)sdata.getSize();
					if (H4header.debugChunkTable)
					{
						Console.WriteLine(" Reading " + n + " DataChunk tags");
					}
					for (int i = 0; i < n; i++)
					{
						int[] origin = sdata.getJavaArrayInt(i, originM);
						short tag = sdata.getScalarShort(i, tagM);
						short refno = sdata.getScalarShort(i, refM);
						TagData data = (TagData)outerInstance.tagMap[tagid(refno, tag)];
						dataChunks.Add(new DataChunk(origin, chunk_length, data));
						data.used = true;
						if (data.compress != null)
						{
							isCompressed = true;
						}
					}
				}
				return dataChunks;
			}

			public virtual string detail()
			{
				StringBuilder sbuff = new StringBuilder("SPECIAL_CHUNKED ");
				sbuff.Append(" head_len=").Append(head_len).Append(" version=").Append(version).Append(" special =").Append(flag).Append(" elem_tot_length=").Append(elem_tot_length);
				sbuff.Append(" chunk_size=").Append(chunk_size).Append(" nt_size=").Append(nt_size).Append(" chunk_tbl_tag=").Append(chunk_tbl_tag).Append(" chunk_tbl_ref=").Append(chunk_tbl_ref);
				sbuff.Append("\n flag  dim  chunk\n");
				for (int i = 0; i < ndims; i++)
				{
					sbuff.Append(" ").Append(dim_flag[i][2]).Append(",").Append(dim_flag[i][3]).Append(" ").Append(dim_length[i]).Append(" ").Append(chunk_length[i]).Append("\n");
				}
				sbuff.Append(" special=").Append(sp_tag_desc).Append(" val=");
				foreach (byte b in sp_tag_header)
				{
					sbuff.Append(" ").Append(b);
				}
				return sbuff.ToString();
			}
		}

		internal class DataChunk
		{
			internal int[] origin;
			internal TagData data;

			internal DataChunk(int[] origin, int[] chunk_length, TagData data)
			{
				// origin is in units of chunks - convert to indices
				Debug.Assert(origin.Length == chunk_length.Length);
				for (int i = 0; i < origin.Length; i++)
				{
					origin[i] *= chunk_length[i];
				}
				this.origin = origin;

				this.data = data;
				if (debugChunkTable)
				{
					Console.Write(" Chunk origin=");
					foreach (int value in origin)
					{
						Console.Write(value + " ");
					}
					Console.WriteLine(" data=" + data.detail());
				}
			}
		}

		// p 151
		internal class SpecialComp
		{
			private readonly H4header outerInstance;

			internal short version, model_type, compress_type, data_ref;
			internal int uncomp_length;
			internal TagData dataTag;

			// compress_type == 2
			internal short signFlag, fillValue;
			internal int nt, startBit, bitLength;

			// compress_type == 4
			internal short deflateLevel;

			internal SpecialComp(H4header outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal virtual void read()
			{
				version = outerInstance.raf.readShort();
				uncomp_length = outerInstance.raf.readInt();
				data_ref = outerInstance.raf.readShort();
				model_type = outerInstance.raf.readShort();
				compress_type = outerInstance.raf.readShort();

				if (compress_type == TagEnum.COMP_CODE_NBIT)
				{
					nt = outerInstance.raf.readInt();
					signFlag = outerInstance.raf.readShort();
					fillValue = outerInstance.raf.readShort();
					startBit = outerInstance.raf.readInt();
					bitLength = outerInstance.raf.readInt();

				}
				else if (compress_type == TagEnum.COMP_CODE_DEFLATE)
				{
					deflateLevel = outerInstance.raf.readShort();
				}
			}

			internal virtual TagData getDataTag()
			{
				if (dataTag == null)
				{
					dataTag = (TagData)outerInstance.tagMap[tagid(data_ref, TagEnum.COMPRESSED.Code)];
					if (dataTag == null)
					{
						throw new System.InvalidOperationException("TagCompress not found for " + detail());
					}
					dataTag.used = true;
				}
				return dataTag;
			}

			public virtual string detail()
			{
				StringBuilder sbuff = new StringBuilder("SPECIAL_COMP ");
				sbuff.Append(" version=").Append(version).Append(" uncompressed length =").Append(uncomp_length).Append(" link_ref=").Append(data_ref);
				sbuff.Append(" model_type=").Append(model_type).Append(" compress_type=").Append(compress_type);
				if (compress_type == TagEnum.COMP_CODE_NBIT)
				{
					sbuff.Append(" nt=").Append(nt).Append(" signFlag=").Append(signFlag).Append(" fillValue=").Append(fillValue).Append(" startBit=").Append(startBit).Append(" bitLength=").Append(bitLength);
				}
				else if (compress_type == TagEnum.COMP_CODE_DEFLATE)
				{
					sbuff.Append(" deflateLevel=").Append(deflateLevel);
				}
				return sbuff.ToString();
			}

		}

		// p 145
		internal class SpecialLinked
		{
			private readonly H4header outerInstance;

			internal int length, first_len;
			internal short blk_len, num_blk, link_ref;
			internal IList<TagLinkedBlock> linkedDataBlocks;

			internal SpecialLinked(H4header outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal virtual void read()
			{
				length = outerInstance.raf.readInt();
				first_len = outerInstance.raf.readInt();
				blk_len = outerInstance.raf.readShort(); // note size wrong in doc
				num_blk = outerInstance.raf.readShort(); // note size wrong in doc
				link_ref = outerInstance.raf.readShort();
			}

			internal virtual IList<TagLinkedBlock> LinkedDataBlocks
			{
				get
				{
					if (linkedDataBlocks == null)
					{
						linkedDataBlocks = new List<TagLinkedBlock>();
						if (H4header.debugLinked)
						{
							Console.WriteLine(" TagData readLinkTags " + detail());
						}
						short next = link_ref; // (short) (link_ref & 0x3FFF);
						while (next != 0)
						{
							TagLinkedBlock tag = (TagLinkedBlock)outerInstance.tagMap[tagid(next, TagEnum.LINKED.Code)];
							if (tag == null)
							{
								throw new System.InvalidOperationException("TagLinkedBlock not found for " + detail());
							}
							tag.used = true;
							tag.read2(num_blk, linkedDataBlocks);
							next = tag.next_ref; // (short) (tag.next_ref & 0x3FFF);
						}
					}
					return linkedDataBlocks;
				}
			}

			public virtual string detail()
			{
				return "SPECIAL_LINKED length=" + length + " first_len=" + first_len + " blk_len=" + blk_len + " num_blk=" + num_blk + " link_ref=" + link_ref;
			}
		}

		// 20 p 146 Also used for data blocks, which has no next_ref! (!)
		internal class TagLinkedBlock : Tag
		{
			internal short next_ref;
			internal short[] block_ref;
			internal int n;

			internal TagLinkedBlock(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			internal virtual void read2(int nb, IList<TagLinkedBlock> dataBlocks)
			{
				outerInstance.raf.seek(offset);
				next_ref = outerInstance.raf.readShort();
				block_ref = new short[nb];
				for (int i = 0; i < nb; i++)
				{
					block_ref[i] = outerInstance.raf.readShort();
					if (block_ref[i] == 0)
					{
						break;
					}
					n++;
				}

				if (H4header.debugLinked)
				{
					Console.WriteLine(" TagLinkedBlock read2 " + detail());
				}
				for (int i = 0; i < n; i++)
				{
					TagLinkedBlock tag = (TagLinkedBlock)outerInstance.tagMap[tagid(block_ref[i], TagEnum.LINKED.Code)];
					tag.used = true;
					dataBlocks.Add(tag);
					if (H4header.debugLinked)
					{
						Console.WriteLine("   Linked data= " + tag.detail());
					}
				}
			}

			public override string detail()
			{
				if (block_ref == null)
				{
					return base.detail();
				}

				StringBuilder sbuff = new StringBuilder(base.detail());
				sbuff.Append(" next_ref= ").Append(next_ref);
				sbuff.Append(" dataBlks= ");
				for (int i = 0; i < n; i++)
				{
					short refno = block_ref[i];
					sbuff.Append(refno).Append(" ");
				}
				return sbuff.ToString();
			}

		}


		// 30
		private class TagVersion : Tag
		{
			internal int major, minor, release;
			internal string name;

			internal TagVersion(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				major = outerInstance.raf.readInt();
				minor = outerInstance.raf.readInt();
				release = outerInstance.raf.readInt();
				name = outerInstance.raf.readStringMax(length - 12);
			}

			public virtual string value()
			{
				return major + "." + minor + "." + release + " (" + name + ")";
			}

			public override string detail()
			{
				return base.detail() + " version= " + major + "." + minor + "." + release + " (" + name + ")";
			}
		}

		// 100, 101
		private class TagText : Tag
		{
			internal string text;

			internal TagText(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				text = outerInstance.raf.readStringMax(length, outerInstance.valueCharset);
			}

			public override string detail()
			{
				string t = (text.Length < 60) ? text : text.Substring(0, 59);
				return base.detail() + " text= " + t;
			}
		}

		// 104, 105
		private class TagAnnotate : Tag
		{
			internal string text;
			internal short obj_tagno, obj_refno;

			internal TagAnnotate(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				obj_tagno = outerInstance.raf.readShort();
				obj_refno = outerInstance.raf.readShort();
				text = outerInstance.raf.readStringMax(length - 4, outerInstance.valueCharset).Trim();
			}

			public override string detail()
			{
				string t = (text.Length < 60) ? text : text.Substring(0, 59);
				return base.detail() + " for=" + obj_refno + "/" + obj_tagno + " text=" + t;
			}
		}

		// 106
		private class TagNumberType : Tag
		{
			internal byte version, type, nbits, type_class;

			internal TagNumberType(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				version = outerInstance.raf.readByte();
				type = outerInstance.raf.readByte();
				nbits = outerInstance.raf.readByte();
				type_class = outerInstance.raf.readByte();
			}

			public override string detail()
			{
				return base.detail() + " version=" + version + " type=" + type + " nbits=" + nbits + " type_class=" + type_class;
			}

			public override string ToString()
			{
				return base.ToString() + " type=" + H4type.setDataType(type, null) + " nbits=" + nbits;
			}

		}

		// 300, 307, 308 p119
		private class TagRIDimension : Tag
		{
			internal int xdim, ydim;
			internal short nt_ref, nelems, interlace, compress, compress_ref;
			internal IList<Dimension> dims;

			internal TagRIDimension(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				xdim = outerInstance.raf.readInt();
				ydim = outerInstance.raf.readInt();
				outerInstance.raf.skipBytes(2);
				nt_ref = outerInstance.raf.readShort();
				nelems = outerInstance.raf.readShort();
				interlace = outerInstance.raf.readShort();
				compress = outerInstance.raf.readShort();
				compress_ref = outerInstance.raf.readShort();
			}

			public override string detail()
			{
				return base.detail() + " xdim=" + xdim + " ydim=" + ydim + " nelems=" + nelems + " nt_ref=" + nt_ref + " interlace=" + interlace + " compress=" + compress + " compress_ref=" + compress_ref;
			}
		}

		// 301 p121
		private class TagRIPalette : Tag
		{
			internal int[] table;

			internal TagRIPalette(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			// cant read without info from other tags
			protected internal virtual void read(int nx, int ny)
			{
				outerInstance.raf.seek(offset);
				table = new int[nx * ny];
				outerInstance.raf.readInt(table, 0, nx * ny);
			}

		}

		// 701 p128
		private class TagSDDimension : Tag
		{
			internal short rank, nt_ref;
			internal int[] shape;
			internal short[] nt_ref_scale;

			internal TagSDDimension(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				rank = outerInstance.raf.readShort();
				shape = new int[rank];
				for (int i = 0; i < rank; i++)
				{
					shape[i] = outerInstance.raf.readInt();
				}

				outerInstance.raf.skipBytes(2);
				nt_ref = outerInstance.raf.readShort();
				nt_ref_scale = new short[rank];
				for (int i = 0; i < rank; i++)
				{
					outerInstance.raf.skipBytes(2);
					nt_ref_scale[i] = outerInstance.raf.readShort();
				}

			}

			public override string detail()
			{
				StringBuilder sbuff = new StringBuilder(base.detail());
				sbuff.Append("   dims= ");
				for (int i = 0; i < rank; i++)
				{
					sbuff.Append(shape[i]).Append(" ");
				}
				sbuff.Append("   nt= ").Append(nt_ref).Append(" nt_scale=");
				for (int i = 0; i < rank; i++)
				{
					sbuff.Append(nt_ref_scale[i]).Append(" ");
				}
				return sbuff.ToString();
			}

			public override string ToString()
			{
				StringBuilder sbuff = new StringBuilder(base.ToString());
				sbuff.Append("   dims= ");
				for (int i = 0; i < rank; i++)
				{
					sbuff.Append(shape[i]).Append(" ");
				}
				sbuff.Append("   nt= ").Append(nt_ref).Append(" nt_scale=");
				for (int i = 0; i < rank; i++)
				{
					sbuff.Append(nt_ref_scale[i]).Append(" ");
				}
				return sbuff.ToString();
			}
		}

		// 704, 705, 706 p 130
		private class TagTextN : Tag
		{
			internal string[] text;

			internal TagTextN(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			internal virtual IList<string> getList()
			{
				IList<string> result = new List<string>(text.Length);
				foreach (string s in text)
				{
					if (s.Trim().Length != 0)
					{
						result.Add(s.Trim());
					}
				}
				return result;
			}


			internal virtual void read(int n)
			{
				text = new string[n];

				outerInstance.raf.seek(offset);
				byte[] b = new byte[length];
				outerInstance.raf.readFully(b);
				int count = 0;
				int start = 0;
				for (int i = 0; i < length; i++)
				{
					if (b[i] == 0)
					{
						text[count] = StringHelper.NewString(b, start, i - start, outerInstance.valueCharset);
						count++;
						if (count == n)
						{
							break;
						}
						start = i + 1;
					}
				}
			}
		}

		// 707, p132
		private class TagSDminmax : Tag
		{
			internal ByteBuffer bb;
			internal DataType dt;

			internal TagSDminmax(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				byte[] buff = new byte[length];
				outerInstance.raf.readFully(buff);
				bb = ByteBuffer.wrap(buff);
			}

			internal virtual object getMin(DataType dataType)
			{
				dt = dataType;
				return get(dataType, 1);
			}

			internal virtual object getMax(DataType dataType)
			{
				dt = dataType;
				return get(dataType, 0);
			}

			internal virtual object get(DataType dataType, int index)
			{
				if (dataType == DataType.BYTE)
				{
					return bb.array()[index];
				}
				if (dataType == DataType.SHORT)
				{
					return bb.asShortBuffer().get(index);
				}
				if (dataType == DataType.INT)
				{
					return bb.asIntBuffer().get(index);
				}
				if (dataType == DataType.LONG)
				{
					return bb.asLongBuffer().get(index);
				}
				if (dataType == DataType.FLOAT)
				{
					return bb.asFloatBuffer().get(index);
				}
				if (dataType == DataType.DOUBLE)
				{
					return bb.asDoubleBuffer().get(index);
				}
				return double.NaN;
			}

			public override string detail()
			{
				string sbuff = base.detail() + "   min= " + getMin(dt) + "   max= " + getMax(dt);
				return sbuff;
			}
		}

		// 306 p118; 720 p 127
		private class TagGroup : Tag
		{
			internal int nelems;
			internal short[] elem_tag, elem_ref;

			internal TagGroup(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				nelems = length / 4;

				elem_tag = new short[nelems];
				elem_ref = new short[nelems];
				for (int i = 0; i < nelems; i++)
				{
					elem_tag[i] = outerInstance.raf.readShort();
					elem_ref[i] = outerInstance.raf.readShort();
				}
			}

			public override string detail()
			{
				StringBuilder sbuff = new StringBuilder(base.detail());
				sbuff.Append("\n");
				sbuff.Append("   tag ref\n   ");
				for (int i = 0; i < nelems; i++)
				{
					sbuff.Append(elem_tag[i]).Append(" ");
					sbuff.Append(elem_ref[i]).Append(" ");
					sbuff.Append("\n   ");
				}

				return sbuff.ToString();
			}
		}

		// 1965 p135
		private class TagVGroup : TagGroup
		{
			internal short extag, exref, version;
			internal string name, className;
			internal Group.Builder group;

			internal TagVGroup(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				nelems = outerInstance.raf.readShort();

				elem_tag = new short[nelems];
				for (int i = 0; i < nelems; i++)
				{
					elem_tag[i] = outerInstance.raf.readShort();
				}

				elem_ref = new short[nelems];
				for (int i = 0; i < nelems; i++)
				{
					elem_ref[i] = outerInstance.raf.readShort();
				}

				short len = outerInstance.raf.readShort();
				name = outerInstance.raf.readStringMax(len);
				len = outerInstance.raf.readShort();
				className = outerInstance.raf.readStringMax(len);

				extag = outerInstance.raf.readShort();
				exref = outerInstance.raf.readShort();
				version = outerInstance.raf.readShort();
			}

			public override string ToString()
			{
				return base.ToString() + " class= " + className + " name= " + name;
			}

			public override string detail()
			{
				StringBuilder sbuff = new StringBuilder();
				sbuff.Append(used ? " " : "*").Append("refno=").Append(refno).Append(" tag= ").Append(t).Append(extended ? " EXTENDED" : "").Append(" offset=").Append(offset).Append(" length=").Append(length).Append(((vinfo != null) && (vinfo.v != null)) ? " VV=" + vinfo.v.shortName : "");
				sbuff.Append(" class= ").Append(className);
				sbuff.Append(" extag= ").Append(extag);
				sbuff.Append(" exref= ").Append(exref);
				sbuff.Append(" version= ").Append(version);
				sbuff.Append("\n");
				sbuff.Append(" name= ").Append(name);
				sbuff.Append("\n");
				sbuff.Append("   tag ref\n   ");
				for (int i = 0; i < nelems; i++)
				{
					sbuff.Append(elem_tag[i]).Append(" ");
					sbuff.Append(elem_ref[i]).Append(" ");
					sbuff.Append("\n   ");
				}
				return sbuff.ToString();
			}
		}

		// 1962 p 136
		private class TagVH : Tag
		{
			internal short interlace, nfields, extag, exref, version;
			internal int ivsize;
			internal short[] fld_type, fld_order;
			internal int[] fld_isize, fld_offset;
			internal string[] fld_name;
			internal int nvert; // number of entries in Vdata
			internal string name, className;
			internal int tag_len;

			internal TagVH(H4header outerInstance, short code) : base(outerInstance, code)
			{
			}

			protected internal override void read()
			{
				outerInstance.raf.seek(offset);
				interlace = outerInstance.raf.readShort();
				nvert = outerInstance.raf.readInt();
				ivsize = DataTypes.unsignedShortToInt(outerInstance.raf.readShort());
				nfields = outerInstance.raf.readShort();

				fld_type = new short[nfields];
				for (int i = 0; i < nfields; i++)
				{
					fld_type[i] = outerInstance.raf.readShort();
				}

				fld_isize = new int[nfields];
				for (int i = 0; i < nfields; i++)
				{
					fld_isize[i] = DataTypes.unsignedShortToInt(outerInstance.raf.readShort());
				}

				fld_offset = new int[nfields];
				for (int i = 0; i < nfields; i++)
				{
					fld_offset[i] = DataTypes.unsignedShortToInt(outerInstance.raf.readShort());
				}

				fld_order = new short[nfields];
				for (int i = 0; i < nfields; i++)
				{
					fld_order[i] = outerInstance.raf.readShort();
				}

				fld_name = new string[nfields];
				for (int i = 0; i < nfields; i++)
				{
					short len2 = outerInstance.raf.readShort();
					fld_name[i] = outerInstance.raf.readStringMax(len2);
				}

				short len = outerInstance.raf.readShort();
				name = outerInstance.raf.readStringMax(len);

				len = outerInstance.raf.readShort();
				className = outerInstance.raf.readStringMax(len);

				extag = outerInstance.raf.readShort();
				exref = outerInstance.raf.readShort();
				version = outerInstance.raf.readShort();

				tag_len = (int)(outerInstance.raf.FilePointer - offset);
			}

			public override string ToString()
			{
				return base.ToString() + " class= " + className + " name= " + name;
			}

			public override string detail()
			{
				StringBuilder sbuff = new StringBuilder(base.detail());
				sbuff.Append(" class= ").Append(className);
				sbuff.Append(" interlace= ").Append(interlace);
				sbuff.Append(" nvert= ").Append(nvert);
				sbuff.Append(" ivsize= ").Append(ivsize);
				sbuff.Append(" extag= ").Append(extag);
				sbuff.Append(" exref= ").Append(exref);
				sbuff.Append(" version= ").Append(version);
				sbuff.Append(" tag_len= ").Append(tag_len);
				sbuff.Append("\n");
				sbuff.Append(" name= ").Append(name);
				sbuff.Append("\n");
				sbuff.Append("   name    type  isize  offset  order\n   ");
				for (int i = 0; i < nfields; i++)
				{
					sbuff.Append(fld_name[i]).Append(" ");
					sbuff.Append(fld_type[i]).Append(" ");
					sbuff.Append(fld_isize[i]).Append(" ");
					sbuff.Append(fld_offset[i]).Append(" ");
					sbuff.Append(fld_order[i]).Append(" ");
					sbuff.Append("\n   ");
				}

				return sbuff.ToString();
			}
		}

		// private String clean(String name) {
		// return StringUtil2.remove(name.trim(), '.'); // just avoid the whole mess by removing "."
		// }

		/*
		 * private String readString(int len) throws IOException {
		 * byte[] b = new byte[len];
		 * raf.readFully(b);
		 * int count;
		 * for (count = 0; count < len; count++)
		 * if (b[count] == 0)
		 * break;
		 * return new String(b, 0, count, Encoding.UTF8);
		 * }
		 */

		private class MemTracker
		{
			private readonly H4header outerInstance;

			private List<Mem> memList = new List<Mem>();
			private StringBuilder sbuff = new StringBuilder();

			private long fileSize;

			internal MemTracker(H4header outerInstance, long fileSize)
			{
				this.outerInstance = outerInstance;
				this.fileSize = fileSize;
			}

			internal virtual void add(string name, long start, long end)
			{
				memList.Add(new Mem(name, start, end));
			}

			internal virtual void addByLen(string name, long start, long size)
			{
				memList.Add(new Mem(name, start, start + size));
			}

			internal virtual void report()
			{
				outerInstance.debugOut.println("======================================");
				outerInstance.debugOut.println("Memory used file size= " + fileSize);
				outerInstance.debugOut.println("  start    end   size   name");
				memList.Sort();
				Mem prev = null;
				foreach (Mem m in memList)
				{
					if ((prev != null) && (m.start > prev.end))
					{
						doOne('+', prev.end, m.start, m.start - prev.end, "*hole*");
					}
					char c = ((prev != null) && (prev.end != m.start)) ? '*' : ' ';
					doOne(c, m.start, m.end, m.end - m.start, m.name);
					prev = m;
				}
				outerInstance.debugOut.println();
			}

			internal virtual void doOne(char c, long start, long end, long size, string name)
			{
				sbuff.Length = 0;
				sbuff.Append(c);
				sbuff.Append(Format.l(start, 6));
				sbuff.Append(" ");
				sbuff.Append(Format.l(end, 6));
				sbuff.Append(" ");
				sbuff.Append(Format.l(size, 6));
				sbuff.Append(" ");
				sbuff.Append(name);
				outerInstance.debugOut.println(sbuff);
			}

			internal class Mem : IComparable<Mem>
			{
				internal string name;
				internal long start, end;

				internal Mem(string name, long start, long end)
				{
					this.name = name;
					this.start = start;
					this.end = end;
				}

				public virtual int CompareTo(Mem m)
				{
					return start.CompareTo(m.start);
				}

			}
		}

		public virtual IList<Tag> getTags()
		{
			return alltags;
		}

	}

}
