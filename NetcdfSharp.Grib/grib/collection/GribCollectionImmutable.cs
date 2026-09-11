// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using NetcdfSharp.ucar.nc2.grib;
	using NetcdfSharp.ucar.nc2.grib.coord;
	using AbstractIOServiceProvider = NetcdfSharp.ucar.nc2.iosp.AbstractIOServiceProvider;
	using AttributeContainer = NetcdfSharp.ucar.nc2.AttributeContainer;
	using AttributeContainerMutable = NetcdfSharp.ucar.nc2.AttributeContainerMutable;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using CommonCodeTable = NetcdfSharp.ucar.nc2.wmo.CommonCodeTable;
	using Coordinate = NetcdfSharp.ucar.nc2.grib.coord.Coordinate;
	using CoordinateEns = NetcdfSharp.ucar.nc2.grib.coord.CoordinateEns;
	using CoordinateRuntime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime;
	using CoordinateTime2D = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime2D;
	using CoordinateTimeAbstract = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeAbstract;
	using CoverageCollection = NetcdfSharp.ucar.nc2.ft2.coverage.CoverageCollection;
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using FeatureType = NetcdfSharp.ucar.nc2.constants.FeatureType;
	using FileCacheable = NetcdfSharp.ucar.nc2.util.cache.FileCacheable;
	using FileCacheIF = NetcdfSharp.ucar.nc2.util.cache.FileCacheIF;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib1Variable = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Variable;
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using NetcdfDataset = NetcdfSharp.ucar.nc2.dataset.NetcdfDataset;
	using NetcdfDatasets = NetcdfSharp.ucar.nc2.dataset.NetcdfDatasets;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using NetcdfFiles = NetcdfSharp.ucar.nc2.NetcdfFiles;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using SubsetParams = NetcdfSharp.ucar.nc2.ft2.coverage.SubsetParams;
	using TimeCoordIntvValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvValue;
	using VertCoordValue = NetcdfSharp.ucar.nc2.grib.coord.VertCoordValue;

	/// <summary>
	/// An Immutable GribCollection, corresponds to one index (ncx) file.
	/// The index file has already been read; it is opened and the closed when a variable is first accessed to read in the
	/// record array (sa).
	/// <para>
	/// Could use the Proto equivalents, and eliminate GribCollectionMutable ?
	/// 
	/// @author caron
	/// @since 11/10/2014
	/// </para>
	/// </summary>
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @Immutable public abstract class GribCollectionImmutable implements NetcdfSharp.java.io.Closeable, NetcdfSharp.ucar.nc2.util.cache.FileCacheable
	public abstract class GribCollectionImmutable : Closeable, FileCacheable
	{
		private static readonly Logger logger = LoggerFactory.getLogger(typeof(GribCollectionImmutable));
		public static int countGC; // debug

		public sealed class GribType
		{ // must match with GribCollectionProto.Dataset.Type
			public static readonly GribType SRC = new GribType("SRC", InnerEnum.SRC); // GC: Single Runtime Collection [ntimes]
			public static readonly GribType MRC = new GribType("MRC", InnerEnum.MRC); // GC: Multiple Runtime Collection [nruns, ntimes]
																					  // MRSTC, // GC: Multiple Runtime Single Time Collection [nruns, 1]
			public static readonly GribType MRUTC = new GribType("MRUTC", InnerEnum.MRUTC); // GC: Multiple Runtime Unique Time Collection [ntimes]

			// MRSTP, // PC: Multiple Runtime Single Time Partition [nruns, 1]
			public static readonly GribType TwoD = new GribType("TwoD", InnerEnum.TwoD); // PC: TwoD time partition [nruns, ntimes]
			public static readonly GribType Best = new GribType("Best", InnerEnum.Best); // PC: Best time partition [ntimes]
			public static readonly GribType BestComplete = new GribType("BestComplete", InnerEnum.BestComplete); // PC: Best complete time partition (not done) [ntimes]
			public static readonly GribType MRUTP = new GribType("MRUTP", InnerEnum.MRUTP); // PC: Multiple Runtime Unique Time Partition [ntimes]

			private static readonly List<GribType> valueList = new List<GribType>();

			static GribType()
			{
				valueList.Add(SRC);
				valueList.Add(MRC);
				valueList.Add(MRUTC);
				valueList.Add(TwoD);
				valueList.Add(Best);
				valueList.Add(BestComplete);
				valueList.Add(MRUTP);
			}

			public enum InnerEnum
			{
				SRC,
				MRC,
				MRUTC,
				TwoD,
				Best,
				BestComplete,
				MRUTP
			}

			public readonly InnerEnum innerEnumValue;
			private readonly string nameValue;
			private readonly int ordinalValue;
			private static int nextOrdinal = 0;

			private GribType(string name, InnerEnum innerEnum)
			{
				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			public bool SingleRuntime
			{
				get
				{
					return this == SRC;
				}
			}

			public bool UniqueTime
			{
				get
				{
					return this == MRUTC || this == MRUTP || this == SRC;
				}
			}

			public bool isTwoD()
			{
				return this == MRC || this == TwoD;
			}

			public static GribType[] values()
			{
				return valueList.ToArray();
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static GribType valueOf(string name)
			{
				foreach (GribType enumInstance in GribType.valueList)
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new ArgumentException(name);
			}
		}

		////////////////////////////////////////////////////////////////
		protected internal readonly string name; // collection name; index filename must be directory/name.ncx2
		protected internal readonly MFile directory;
		protected internal readonly FeatureCollectionConfig config;
		public readonly bool isGrib1;
		protected internal readonly Info info;

		protected internal readonly IList<Dataset> datasets;
		protected internal readonly CoordinateRuntime masterRuntime;
		protected internal readonly CalendarDateRange dateRange;

		protected internal readonly IDictionary<int, MFile> fileMap; // all the files used in the GC; key is the index in original collection,
																	 // GC has subset of them
		protected internal readonly GribTables cust;
		protected internal readonly string indexFilename; // full path of index Filename

		private FileCacheIF objCache; // optional object cache - used in the TDS

		internal GribCollectionImmutable(GribCollectionMutable gc)
		{
			countGC++;

			this.config = gc.config;
			this.name = gc.name;
			this.directory = gc.directory;
			this.isGrib1 = gc.isGrib1;
			this.info = new Info(gc);

			IList<Dataset> work = new List<Dataset>(gc.datasets.Count);
			foreach (GribCollectionMutable.Dataset gcDataset in gc.datasets)
			{
				work.Add(new Dataset(this, gcDataset.gctype, gcDataset.groups));
			}
			this.datasets = work.AsReadOnly();

			// this.horizCS = Collections.unmodifiableList(gc.horizCS);
			this.masterRuntime = gc.masterRuntime;
			this.fileMap = gc.fileMap;
			this.cust = gc.cust;
			this.dateRange = gc.dateRange;

			if (!string.ReferenceEquals(gc.indexFilename, null))
			{
				indexFilename = gc.indexFilename;

			}
			else
			{
				MFile indexFile = GribCdmIndex.makeIndexFile(name, directory);
				MFile indexFileInCache = GribIndexCache.getExistingFileOrCache(indexFile.Path);
				if (indexFileInCache == null)
				{
					throw new System.InvalidOperationException(indexFile.Path + " does not exist, nor in cache");
				}
				indexFilename = indexFileInCache.Path;
			}
		}

		// overridden in PartitionCollection
		protected internal virtual VariableIndex makeVariableIndex(GroupGC group, GribCollectionMutable.VariableIndex mutableVar)
		{
			return new VariableIndex(this, group, mutableVar);
		}

		public virtual IList<Dataset> Datasets
		{
			get
			{
				return datasets;
			}
		}

		public virtual Dataset getDataset(int idx)
		{
			return datasets[idx];
		}

		public virtual Dataset DatasetCanonical
		{
			get
			{
				foreach (Dataset ds in datasets)
				{
					if (ds.gctype != GribCollectionImmutable.GribType.Best)
					{
						return ds;
					}
				}
				throw new System.InvalidOperationException("GC.getDatasetCanonical failed on=" + name);
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual MFile Directory
		{
			get
			{
				return directory;
			}
		}

		public virtual CoordinateRuntime MasterRuntime
		{
			get
			{
				return masterRuntime;
			}
		}

		public virtual CalendarDate MasterFirstDate
		{
			get
			{
				return masterRuntime.FirstDate;
			}
		}

		public virtual int Version
		{
			get
			{
				return info.version;
			}
		}

		public virtual int Center
		{
			get
			{
				return info.center;
			}
		}

		public virtual int Subcenter
		{
			get
			{
				return info.subcenter;
			}
		}

		public virtual int Master
		{
			get
			{
				return info.master;
			}
		}

		public virtual int Local
		{
			get
			{
				return info.local;
			}
		}

		public virtual int GenProcessType
		{
			get
			{
				return info.genProcessType;
			}
		}

		public virtual int GenProcessId
		{
			get
			{
				return info.genProcessId;
			}
		}

		public virtual int BackProcessId
		{
			get
			{
				return info.backProcessId;
			}
		}

		/*
		 * String val = CommonCodeTable.getCenterName(gribCollection.getCenter(), 2);
		 * ncfile.addAttribute(null, new Attribute(GribUtils.CENTER, val == null ?
		 * JavaLangCompat.IntToString(gribCollection.getCenter()) : val));
		 * val = gribTable.getSubCenterName(gribCollection.getCenter(), gribCollection.getSubcenter());
		 * ncfile.addAttribute(null, new Attribute(GribUtils.SUBCENTER, val == null ?
		 * JavaLangCompat.IntToString(gribCollection.getSubcenter()) : val));
		 * ncfile.addAttribute(null, new Attribute(GribUtils.TABLE_VERSION, gribCollection.getMaster() + "," +
		 * gribCollection.getLocal())); // LOOK
		 * 
		 * addGlobalAttributes(ncfile);
		 * 
		 * ncfile.addAttribute(null, new Attribute(CDM.CONVENTIONS, "CF-1.6"));
		 * ncfile.addAttribute(null, new Attribute(CDM.HISTORY, "Read using CDM IOSP GribCollection v3"));
		 * ncfile.addAttribute(null, new Attribute(CF.FEATURE_TYPE, FeatureType.GRID.name()));
		 * ncfile.addAttribute(null, new Attribute(CDM.FILE_FORMAT, getFileTypeId()));
		 */

		public virtual AttributeContainer GlobalAttributes
		{
			get
			{
				AttributeContainerMutable result = new AttributeContainerMutable(name);
				string val = CommonCodeTable.getCenterName(Center, 2);
				result.addAttribute(new NcAttribute(GribUtils.CENTER, string.ReferenceEquals(val, null) ? Convert.ToString(Center) : val));
				val = cust.getSubCenterName(Center, Subcenter);
				result.addAttribute(new NcAttribute(GribUtils.SUBCENTER, string.ReferenceEquals(val, null) ? Convert.ToString(Subcenter) : val));
				result.addAttribute(new NcAttribute(GribUtils.TABLE_VERSION, Master + "," + Local)); // LOOK

				addGlobalAttributes(result); // add subclass atts

				result.addAttribute(new NcAttribute(CDM.CONVENTIONS, "CF-1.6"));
				result.addAttribute(new NcAttribute(CDM.HISTORY, "Read using CDM IOSP GribCollection v3"));
				result.addAttribute(new NcAttribute(CF.FEATURE_TYPE, FeatureType.GRID.ToString()));

				return result;
			}
		}

		public abstract void addGlobalAttributes(AttributeContainer result);

		public abstract void addVariableAttributes(AttributeContainer v, GribCollectionImmutable.VariableIndex vindex);

		public abstract string makeVariableId(VariableIndex v);

		public class Info
		{
			internal readonly int version; // the ncx version
			internal readonly int center, subcenter, master, local; // GRIB 1 uses "local" for table version
			internal readonly int genProcessType, genProcessId, backProcessId;

			internal Info(int version, int center, int subcenter, int master, int local, int genProcessType, int genProcessId, int backProcessId)
			{
				this.version = version;
				this.center = center;
				this.subcenter = subcenter;
				this.master = master;
				this.local = local;
				this.genProcessType = genProcessType;
				this.genProcessId = genProcessId;
				this.backProcessId = backProcessId;
			}

			internal Info(GribCollectionMutable gc)
			{
				this.version = gc.version;
				this.center = gc.center;
				this.subcenter = gc.subcenter;
				this.master = gc.master;
				this.local = gc.local;
				this.genProcessType = gc.genProcessType;
				this.genProcessId = gc.genProcessId;
				this.backProcessId = gc.backProcessId;
			}

			public override string ToString()
			{
				return "Info{" + "version=" + version + ", center=" + center + ", subcenter=" + subcenter + ", master=" + master + ", local=" + local + ", genProcessType=" + genProcessType + ", genProcessId=" + genProcessId + ", backProcessId=" + backProcessId + '}';
			}
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Immutable public class Dataset
		public class Dataset
		{
			private readonly GribCollectionImmutable outerInstance;

			internal readonly GribType gctype;
			internal readonly IList<GroupGC> groups; // must be kept in order, because PartitionForVariable2D has index into it

			public Dataset(GribCollectionImmutable outerInstance, GribType gctype, IList<GribCollectionMutable.GroupGC> groups)
			{
				this.outerInstance = outerInstance;
				this.gctype = gctype;
				IList<GroupGC> work = new List<GroupGC>(groups.Count);
				foreach (GribCollectionMutable.GroupGC gcGroup in groups)
				{
					work.Add(new GroupGC(outerInstance, this, gcGroup));
				}
				this.groups = work.AsReadOnly();
			}

			public virtual IEnumerable<GroupGC> Groups
			{
				get
				{
					return groups;
				}
			}

			public virtual int GroupsSize
			{
				get
				{
					return groups.Count;
				}
			}

			public virtual GribCollectionImmutable.GribType Type
			{
				get
				{
					return gctype;
				}
			}

			public virtual GroupGC getGroup(int index)
			{
				return groups[index];
			}

			public virtual GroupGC findGroupById(string id)
			{
				foreach (GroupGC g in Groups)
				{
					if (g.Id.Equals(id))
					{
						return g;
					}
				}
				return null;
			}
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Immutable public class GroupGC
		public class GroupGC
		{
			private readonly GribCollectionImmutable outerInstance;

			internal readonly Dataset ds;
			public readonly GribHorizCoordSystem horizCoordSys;
			internal readonly IList<VariableIndex> variList;
			internal readonly IList<Coordinate> coords; // shared coordinates
			internal readonly int[] filenose; // key for GC.fileMap
			private readonly IDictionary<VariableIndex, VariableIndex> varMap;

			public GroupGC(GribCollectionImmutable outerInstance, Dataset ds, GribCollectionMutable.GroupGC gc)
			{
				this.outerInstance = outerInstance;
				this.ds = ds;
				this.horizCoordSys = gc.horizCoordSys;
				this.coords = gc.coords;
				this.filenose = gc.filenose;
				this.varMap = new Dictionary<VariableIndex, VariableIndex>(gc.variList.Count * 2);

				IList<GribCollectionMutable.VariableIndex> gcVars = gc.variList;
				IList<VariableIndex> work = new List<VariableIndex>(gcVars.Count);
				foreach (GribCollectionMutable.VariableIndex gcVar in gcVars)
				{
					VariableIndex vi = outerInstance.makeVariableIndex(this, gcVar);
					work.Add(vi);
					varMap[vi] = vi;
				}
				this.variList = work.AsReadOnly();
			}

			public virtual GribCollectionImmutable.GribType Type
			{
				get
				{
					return ds.gctype;
				}
			}

			public virtual string Id
			{
				get
				{
					return horizCoordSys.Id;
				}
			}

			public virtual object GdsHash
			{
				get
				{
					return horizCoordSys.GdsHash;
				}
			}

			public virtual GribCollectionImmutable GribCollection
			{
				get
				{
					return outerInstance;
				}
			}

			// human readable
			public virtual string Description
			{
				get
				{
					return horizCoordSys.Description;
				}
			}

			public virtual GribHorizCoordSystem GribHorizCoordSys
			{
				get
				{
					return horizCoordSys;
				}
			}

			public virtual GdsHorizCoordSys GdsHorizCoordSys
			{
				get
				{
					return horizCoordSys.Hcs;
				}
			}

			public virtual GdsHorizCoordSys getGdsHorizCoordSys()
			{
				return horizCoordSys.Hcs;
			}

			internal virtual VariableIndex findVariableByHash(VariableIndex vi)
			{
				return varMap[vi];
			}

			public virtual Optional<Coordinate> findCoordinate(string name)
			{
				return Optional<Coordinate>.ofNullable(coords.Where(x => x.Name.Equals(name)).FirstOrDefault());
			}

			public virtual IList<VariableIndex> Variables
			{
				get
				{
					return variList;
				}
			}

			public virtual IList<Coordinate> Coordinates
			{
				get
				{
					return coords;
				}
			}

			public virtual int Nruntimes
			{
				get
				{
					return outerInstance.masterRuntime.Size;
				}
			}

			public virtual int NFiles
			{
				get
				{
					if (filenose == null)
					{
						return 0;
					}
					return filenose.Length;
				}
			}

			public virtual IList<MFile> Files
			{
				get
				{
					IList<MFile> result = new List<MFile>();
					if (filenose == null)
					{
						return result;
					}
					foreach (int fileno in filenose)
					{
						result.Add(outerInstance.fileMap[fileno]);
					}
					var sortedResult = result.OrderBy(m => m.getName(), StringComparer.Ordinal).ToList();
					result = sortedResult;
					return result;
				}
			}

			public virtual CalendarDateRange makeCalendarDateRange()
			{
				CalendarDateRange result = null;
				foreach (Coordinate coord in coords)
				{
					switch (coord.getType().innerEnumValue)
					{
						case Coordinate.Type.InnerEnum.time:
						case Coordinate.Type.InnerEnum.timeIntv:
						case Coordinate.Type.InnerEnum.time2D:
							CoordinateTimeAbstract time = (CoordinateTimeAbstract)coord;
							CalendarDateRange range = time.makeCalendarDateRange(null);
							if (result == null)
							{
								result = range;
							}
							else
							{
								result = result.extend(range);
							}
							break;
					}
				}

				return result;
			}

			public override string ToString()
			{
				return MoreObjects.toStringHelper(this).add("horizCoordSys", horizCoordSys.Description).ToString();
			}

			public virtual void show(Formatter f)
			{
				f.format("Group %s (%d) type=%s%n", horizCoordSys.Id, horizCoordSys.GdsHash.GetHashCode(), ds.gctype);
				f.format(" nfiles %d%n", filenose == null ? 0 : filenose.Length);
				f.format(" hcs = %s%n", horizCoordSys.Hcs);
			}
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Immutable // except for sa
		// ORIGINAL LINE: public class VariableIndex
		public class VariableIndex
		{
			private readonly GribCollectionImmutable outerInstance;

			internal readonly GroupGC group; // belongs to this group
			internal readonly VariableIndex.Info info;
			internal readonly object gribVariable;

			internal readonly IList<int> coordIndex; // indexes into group.coords
			internal readonly long recordsPos; // where the records array is stored in the index. 0 means no records
			internal readonly int recordsLen;

			// stats
			internal readonly int ndups, nrecords, nmissing;

			// read in on demand
			private SparseArray<Record> sa; // for GC only; lazily read; same array shape as variable, minus x and y

			protected internal VariableIndex(GribCollectionImmutable outerInstance, GroupGC g, GribCollectionMutable.VariableIndex gcVar)
			{
				this.outerInstance = outerInstance;
				this.group = g;
				this.info = new Info(this, gcVar);
				this.gribVariable = gcVar.gribVariable;

				this.coordIndex = gcVar.coordIndex;
				this.recordsPos = gcVar.recordsPos;
				this.recordsLen = gcVar.recordsLen;

				this.ndups = gcVar.ndups;
				this.nrecords = gcVar.nrecords;
				this.nmissing = gcVar.nmissing;
			}

			public virtual void readRecords()
			{
				lock (this)
				{
					if (this.sa != null)
					{
						return;
					}

					if (recordsLen == 0)
					{
						return;
					}
					byte[] b = new byte[recordsLen];

					try
					{
						using (RandomAccessFile indexRaf = RandomAccessFile.acquire(outerInstance.indexFilename))
						{
							indexRaf.seek(recordsPos);
							indexRaf.readFully(b);

							/*
							 * message SparseArray {
							 * repeated uint32 size = 2 [packed=true]; // multidim sizes = shape[]
							 * repeated uint32 track = 3 [packed=true]; // 1-based index into record list, 0 == missing
							 * repeated Record records = 4; // List<Record>
							 * uint32 ndups = 5; // duplicates found when creating
							 * }
							 */
							GribCollectionProto.SparseArray proto = GribCollectionProto.SparseArray.parseFrom(b);

							int nsizes = proto.getSizeCount();
							int[] size = new int[nsizes];
							for (int i = 0; i < nsizes; i++)
							{
								size[i] = proto.getSize(i);
							}

							int ntrack = proto.getTrackCount();
							int[] track = new int[ntrack];
							for (int i = 0; i < ntrack; i++)
							{
								track[i] = proto.getTrack(i);
							}

							int n = proto.getRecordsCount();
							IList<Record> records = new List<Record>(n);
							for (int i = 0; i < n; i++)
							{
								GribCollectionProto.Record pr = proto.getRecords(i);
								records.Add(new Record(pr.getFileno(), pr.getStartPos(), pr.getBmsOffset(), pr.getDrsOffset()));
							}
							int ndups = proto.getNdups();
							this.sa = new SparseArray<Record>(size, track, records, ndups);
						}

					}
					catch (NetcdfSharp.com.google.protobuf.InvalidProtocolBufferException)
					{
						logger.error(" file={} recordsLen={} recordPos={}", outerInstance.indexFilename, recordsLen, recordsPos);
						throw;
					}
				}
			}

			internal virtual Record getRecordAt(int sourceIndex)
			{
				lock (this)
				{
					return sa.getContent(sourceIndex);
				}
			}

			internal virtual Record getRecordAt(int[] sourceIndex)
			{
				lock (this)
				{
					return sa.getContent(sourceIndex);
				}
			}

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// coord based record finding. note only one record at a time
			internal virtual Record getRecordAt(SubsetParams coords)
			{
				lock (this)
				{
					int[] want = new int[Rank];
					int count = 0;
					int runIdx = -1;
					foreach (Coordinate coord in Coordinates)
					{
						int idx = -1;
						switch (coord.getType().innerEnumValue)
						{
							case Coordinate.Type.InnerEnum.runtime:
								CalendarDate runtimeCooord = coords.getRunTime();
								idx = coord.getIndex(runtimeCooord);
								runIdx = idx;
								break;

							case Coordinate.Type.InnerEnum.timeIntv:
								double[] timeIntv = coords.getTimeOffsetIntv();
								idx = coord.getIndex(new TimeCoordIntvValue((int)timeIntv[0], (int)timeIntv[1]));
								break;

							case Coordinate.Type.InnerEnum.time:
								double timeOffset = coords.getTimeOffset(); // Double
								int coordInt = (int)timeOffset;
								idx = coord.getIndex(coordInt);
								break;

							case Coordinate.Type.InnerEnum.time2D:
								timeIntv = coords.getTimeOffsetIntv();
								if (timeIntv != null)
								{
									TimeCoordIntvValue coordTinv = new TimeCoordIntvValue((int)timeIntv[0], (int)timeIntv[1]);
									idx = ((CoordinateTime2D)coord).findTimeIndexFromVal(runIdx, coordTinv); // LOOK can only use if
																											 // orthogonal
									break;
								}
								double timeCoord = coords.getTimeOffset();
								if (timeCoord != 0.0)
								{
									coordInt = (int)timeCoord;
									idx = ((CoordinateTime2D)coord).findTimeIndexFromVal(runIdx, coordInt);
									break;
								}

								// the OneTime case
								CoordinateTime2D coord2D = (CoordinateTime2D)coord;
								if (coord2D.Ntimes == 1)
								{
									idx = 0;
									break;
								}
								throw new System.InvalidOperationException("time2D must have timeOffset ot timeOffsetIntv coordinare");

							case Coordinate.Type.InnerEnum.vert:
								double[] vertIntv = coords.getVertCoordIntv();
								if (vertIntv != null)
								{
									VertCoordValue coordVert = new VertCoordValue(vertIntv[0], vertIntv[1]);
									idx = coord.getIndex(coordVert);
									break;
								}
								double vertCoord = coords.getVertCoord();
								if (vertCoord != 0.0)
								{
									VertCoordValue coordVert = new VertCoordValue(vertCoord);
									idx = coord.getIndex(coordVert);
								}
								break;

							case Coordinate.Type.InnerEnum.ens:
								double ensVal = coords.getEnsCoord();
								idx = ((CoordinateEns)coord).getIndexByMember(ensVal);
								break;

							default:
								logger.warn("GribCollectionImmutable: missing CoordVal for {}%n", coord.Name);
								break;
						}

						if (idx < 0)
						{
							logger.debug("Cant find index for value in axis {} in variable {}", coord.Name, outerInstance.name);
							return null;
						}

						want[count++] = idx;
					}
					return sa.getContent(want);
				}
			}

			public virtual IList<Coordinate> Coordinates
			{
				get
				{
					IList<Coordinate> result = new List<Coordinate>(coordIndex.Count);
					foreach (int idx in coordIndex)
					{
						result.Add(group.coords[idx]);
					}
					return result;
				}
			}

			public virtual Coordinate getCoordinate(Coordinate.Type want)
			{
				foreach (int idx in coordIndex)
				{
					if (group.coords[idx].getType() == want)
					{
						return group.coords[idx];
					}
				}
				return null;
			}

			internal virtual CoordinateTimeAbstract getCoordinateTime()
			{
				foreach (int idx in coordIndex)
				{
					if (group.coords[idx] is CoordinateTimeAbstract)
					{
						return (CoordinateTimeAbstract)group.coords[idx];
					}
				}
				return null;
			}

			// get the ith coordinate
			public virtual Coordinate getCoordinate(int index)
			{
				int grpIndex = coordIndex[index];
				return group.coords[grpIndex];
			}

			public virtual IEnumerable<int> CoordinateIndex
			{
				get
				{
					return coordIndex;
				}
			}

			public virtual SparseArray<Record> SparseArray
			{
				get
				{
					return sa;
				}
			}

			public virtual int countNRecords()
			{
				return sa == null ? -1 : sa.countNotMissing();
			}

			public virtual int TableVersion
			{
				get
				{
					return info.tableVersion;
				}
			}

			public virtual int Discipline
			{
				get
				{
					return info.discipline;
				}
			}

			public virtual int Category
			{
				get
				{
					return info.category;
				}
			}

			public virtual int Parameter
			{
				get
				{
					return info.parameter;
				}
			}

			public virtual int LevelType
			{
				get
				{
					return info.levelType;
				}
			}

			public virtual int IntvType
			{
				get
				{
					return info.intvType;
				}
			}

			public virtual int SpatialStatisticalProcessType
			{
				get
				{
					return info.spatialStatType;
				}
			}

			public virtual int EnsDerivedType
			{
				get
				{
					return info.ensDerivedType;
				}
			}

			public virtual int ProbType
			{
				get
				{
					return info.probType;
				}
			}

			public virtual int PercentileValue
			{
				get
				{
					return info.percentile;
				}
			}

			public virtual string IntvName
			{
				get
				{
					return info.intvName;
				}
			}

			public virtual string ProbabilityName
			{
				get
				{
					return info.probabilityName;
				}
			}

			public virtual bool Layer
			{
				get
				{
					return info.isLayer;
				}
			}

			public virtual bool Ensemble
			{
				get
				{
					return info.isEnsemble;
				}
			}

			public virtual int GenProcessType
			{
				get
				{
					return info.genProcessType;
				}
			}

			public virtual int Ndups
			{
				get
				{
					return ndups;
				}
			}

			public virtual GroupGC Group
			{
				get
				{
					return group;
				}
			}

			public virtual int Nmissing
			{
				get
				{
					return nmissing;
				}
			}

			public virtual int Nrecords
			{
				get
				{
					return nrecords;
				}
			}

			public virtual int Size
			{
				get
				{
					int size = 1;
					foreach (int idx in coordIndex)
					{
						Coordinate c = group.coords[idx];
						int csize = (c is CoordinateTime2D) ? ((CoordinateTime2D)c).Ntimes : c.Size;
						size *= csize;
					}
					return size;
				}
			}

			public override string ToString()
			{
				return MoreObjects.toStringHelper(this).add("info", info).ToString();
			}

			public virtual int Rank
			{
				get
				{
					return coordIndex.Count;
				}
			}

			public virtual string toStringFrom()
			{
				using (Formatter sb = new Formatter())
				{
					sb.format("Variable {%d-%d-%d", info.discipline, info.category, info.parameter);
					sb.format(", levelType=%d", info.levelType);
					sb.format(", intvType=%d", info.intvType);
					sb.format(", nrecords=%d", nrecords);
					sb.format(", ndups=%d", ndups);
					sb.format(", nmiss=%d}", nmissing);
					return sb.ToString();
				}
			}

			public override bool Equals(object o)
			{
				if (this == o)
				{
					return true;
				}
				if (o == null || this.GetType() != o.GetType())
				{
					return false;
				}

				VariableIndex that = (VariableIndex)o;
				return gribVariable.Equals(that.gribVariable);
			}

			public override int GetHashCode()
			{
				return gribVariable.GetHashCode();
			}

			public virtual string makeVariableName()
			{
				if (outerInstance.isGrib1)
				{
					return ((Grib1Variable)gribVariable).makeVariableName(outerInstance.config.gribConfig);
				}
				else
				{
					return Grib2Iosp.makeVariableNameFromTable((Grib2Tables)outerInstance.cust, outerInstance, this, outerInstance.config.gribConfig.useGenType);
				}
			}

			public virtual string makeVariableUnits()
			{
				if (outerInstance.isGrib1)
				{
					return Grib1Iosp.makeVariableUnits((Grib1Customizer)outerInstance.cust, outerInstance, this);
				}
				else
				{
					return Grib2Iosp.makeVariableUnits((Grib2Tables)outerInstance.cust, this);
				}
			}

			public virtual string makeVariableDescription()
			{
				if (outerInstance.isGrib1)
				{
					return Grib1Iosp.makeVariableLongName((Grib1Customizer)outerInstance.cust, outerInstance.Center, outerInstance.Subcenter, TableVersion, Parameter, LevelType, Layer, IntvType, IntvName, ProbabilityName);
				}
				else
				{
					return Grib2Iosp.makeVariableLongName((Grib2Tables)outerInstance.cust, this, outerInstance.config.gribConfig.useGenType);
				}
			}

			public virtual GribTables.Parameter GribParameter
			{
				get
				{
					if (outerInstance.isGrib1)
					{
						return ((Grib1Customizer)outerInstance.cust).getParameter(outerInstance.Center, outerInstance.Subcenter, outerInstance.Version, Parameter);
					}
					else
					{
						return ((Grib2Tables)outerInstance.cust).getParameter(this);
					}
				}
			}

			public virtual GribStatType getStatType()
			{
				return outerInstance.cust.getStatType(IntvType);
			}

			// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
			// ORIGINAL LINE: @Immutable public final class Info
			public class Info
			{
				private readonly VariableIndex outerInstance;

				internal readonly int tableVersion; // grib1 only : can vary by variable
				internal readonly int discipline; // grib2 only

				// derived from pds
				internal readonly int category, parameter, levelType, intvType, ensDerivedType, probType, percentile;
				internal readonly string intvName; // eg "mixed intervals, 3 Hour, etc"
				internal readonly string probabilityName;
				internal readonly bool isLayer, isEnsemble;
				internal readonly int genProcessType;
				internal readonly int spatialStatType;

				public Info(VariableIndex outerInstance, GribCollectionMutable.VariableIndex gcVar)
				{
					this.outerInstance = outerInstance;
					this.tableVersion = gcVar.tableVersion;
					this.discipline = gcVar.discipline;
					this.category = gcVar.category;
					this.parameter = gcVar.parameter;
					this.levelType = gcVar.levelType;
					this.intvType = gcVar.intvType;
					this.ensDerivedType = gcVar.ensDerivedType;
					this.probType = gcVar.probType;
					this.intvName = gcVar.getTimeIntvName();
					this.probabilityName = gcVar.probabilityName;
					this.isLayer = gcVar.isLayer;
					this.isEnsemble = gcVar.isEnsemble;
					this.genProcessType = gcVar.genProcessType;
					this.spatialStatType = gcVar.spatialStatType;
					this.percentile = gcVar.percentile;
				}

				public override string ToString()
				{
					return MoreObjects.toStringHelper(this).add("discipline", discipline).add("category", category).add("parameter", parameter).ToString();
				}
			}

		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Immutable public static class Record
		public class Record
		{
			public readonly int fileno; // which file
			public readonly long pos; // offset on file where message starts
			public readonly int bmsOffset; // if non-zero, offset where bms starts (grib2)
			public readonly int drsOffset; // if non-zero, offset where drs starts (grib2)

			public Record(int fileno, long pos, int bmsOffset, int drsOffset)
			{ // }, int scanMode) {
				this.fileno = fileno;
				this.pos = pos;
				this.bmsOffset = bmsOffset;
				this.drsOffset = drsOffset;
			}

			public override string ToString()
			{
				return "Record{" + "fileno=" + fileno + ", pos=" + pos + ", bmsOffset=" + bmsOffset + ", drsOffset=" + drsOffset + '}';
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////
		// stuff for FileCacheable

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public synchronized void close() throws NetcdfSharp.java.io.IOException
		public virtual void close()
		{
			lock (this)
			{
				if (objCache != null)
				{
					objCache.release(this);
				}
			}
		}

		public virtual void Dispose()
		{
			close();
		}

		/// <summary>
		/// release any resources like file handles.
		/// </summary>
		/// <deprecated> do not use. </deprecated>
		[Obsolete("do not use.")]
		public virtual void release()
		{
		}

		/// <summary>
		/// reacquire any resources like file handles.
		/// </summary>
		/// <deprecated> do not use. </deprecated>
		[Obsolete("do not use.")]
		public virtual void reacquire()
		{
		}

		public virtual string Location
		{
			get
			{
				return indexFilename;
			}
		}

		public virtual long LastModified
		{
			get
			{
				MFile indexFile = MFiles.create(indexFilename);
				return indexFile.LastModified;
			}
		}

		///////////////

		public virtual void showStatus(Formatter f)
		{
			showIndexFile(f);
			f.format("Class (%s)%n", GetType().Name);
			f.format("%s%n%n", info.ToString());

			f.format("masterRuntime: size=%d%n", masterRuntime.Size);
			if (masterRuntime.Size < 20)
			{
				masterRuntime.showCoords(f);
			}

			foreach (Dataset ds in datasets)
			{
				f.format("%nDataset %s%n", ds.Type);
				foreach (GroupGC g in ds.groups)
				{
					int nrecords = 0, ndups = 0, nmissing = 0;
					f.format(" Group %s%n", g.horizCoordSys.Id);
					foreach (VariableIndex v in g.variList)
					{
						f.format("  %s%n", v.toStringFrom());
						nrecords += v.nrecords;
						ndups += v.ndups;
						nmissing += v.nmissing;
					}
					f.format(" Group total nrecords=%d", nrecords);
					f.format(", ndups=%d", ndups);
					f.format(", nmiss=%d%n", nmissing);

					int nruntimes = 0, ntimes = 0, ntimes2D = 0, ntimeIntvs = 0;
					foreach (Coordinate coord in g.Coordinates)
					{
						if (coord.getType() == Coordinate.Type.runtime)
						{
							nruntimes++;
						}
						if (coord.getType() == Coordinate.Type.time)
						{
							ntimes++;
						}
						if (coord.getType() == Coordinate.Type.timeIntv)
						{
							ntimeIntvs++;
						}
						if (coord.getType() == Coordinate.Type.time2D)
						{
							ntimes2D++;
						}
					}
					f.format(" Group nruntimes=%d ntimes=%d ntimeIntvs=%d ntimes2D=%d%n", nruntimes, ntimes, ntimeIntvs, ntimes2D);
				}
			}
			if (fileMap == null)
			{
				f.format("%nFiles empty%n");
			}
			else
			{
				f.format("%nFiles count = %d%n", fileMap.Count);
				foreach (int index in fileMap.Keys)
				{
					f.format("  %d: %s%n", index, fileMap[index]);
				}
				f.format("%n");
			}
		}

		public virtual void showStatusSummary(Formatter f, string type)
		{
			Dataset ds = DatasetCanonical;
			if (ds == null)
			{
				return;
			}

			if (type.Equals("csv", StringComparison.OrdinalIgnoreCase))
			{
				foreach (GroupGC g in ds.groups)
				{
					int nrecords = 0, ndups = 0, nmissing = 0;
					foreach (VariableIndex v in g.variList)
					{
						nrecords += v.nrecords;
						ndups += v.ndups;
						nmissing += v.nmissing;
					}
					if (nrecords == 0)
					{
						nrecords = 1;
					}
					f.format("%s, %s, %s, %s, %d, %d, %f, %d, %f%n", name, config.type, ds.Type, g.Description, nrecords, ndups, ((float)ndups / nrecords), nmissing, ((float)nmissing / nrecords));
				}
			}
			else
			{
				foreach (GroupGC g in ds.groups)
				{
					int nrecords = 0, ndups = 0, nmissing = 0;
					foreach (VariableIndex v in g.variList)
					{
						nrecords += v.nrecords;
						ndups += v.ndups;
						nmissing += v.nmissing;
					}
					f.format(" Group %s (%s) total nrecords=%d", g.Description, ds.Type, nrecords);
					if (nrecords == 0)
					{
						nrecords = 1;
					}
					f.format(", ndups=%d (%f)", ndups, ((float)ndups / nrecords));
					f.format(", nmiss=%d (%f)%n", nmissing, ((float)nmissing / nrecords));
				}
			}

		}

		private void showIndexFile(Formatter f)
		{
			if (string.ReferenceEquals(indexFilename, null))
			{
				return;
			}
			f.format("indexFile=%s%n", indexFilename);
			MFile mfile = MFiles.create(indexFilename);
			if (mfile.exists())
			{
				f.format("  size=%d lastModifiedTime=%s%n", mfile.getLength(), CalendarDate.of(mfile.getLastModified()));
			}
			f.format("%n");
		}

		public virtual void showIndex(Formatter f)
		{
			showIndexFile(f);
			f.format("Class (%s)%n", GetType().Name);
			f.format(" version %d%n", info.version);
			f.format("%s%n%n", ToString());
			f.format("%s%n%n", info.ToString());

			f.format("masterRuntime: size=%d%n", masterRuntime.Size);
			if (masterRuntime.Size < 200)
			{
				masterRuntime.showCoords(f);
			}

			foreach (Dataset ds in datasets)
			{
				f.format("%nDataset %s%n", ds.Type);
				foreach (GroupGC g in ds.groups)
				{
					f.format(" Group %s%n", g.horizCoordSys.Id);
					foreach (VariableIndex v in g.variList)
					{
						f.format("  %s%n", v.toStringFrom());
					}
				}
			}

			f.format("%n");
			if (fileMap == null)
			{
				f.format("Files empty%n");
			}
			else
			{
				f.format("Files count = %d%n", fileMap.Count);
			}
		}

		public override string ToString()
		{
			return "GribCollectionImmutable{" + "name='" + name + '\'' + ", directory=" + directory + ", config=" + config + ", isGrib1=" + isGrib1 + ", info=" + info + ", masterRuntime=" + masterRuntime.Name + ", dateRange=" + dateRange + ", indexFilename='" + indexFilename + '\'' + '}';
		}

		////////////////////////////////////////

		public virtual long IndexFileSize
		{
			get
			{
				MFile indexFile = MFiles.create(indexFilename);
				return indexFile.getLength();
			}
		}

		public virtual MFile getFile(int fileno)
		{
			return fileMap[fileno];
		}

		public virtual string getFilename(int fileno)
		{
			return fileMap[fileno].Path;
		}

		public virtual ICollection<MFile> getFiles()
		{
			return fileMap.Values;
		}

		public virtual MFile findMFileByName(string filename)
		{
			foreach (MFile file in fileMap.Values)
			{
				if (file.Name.Equals(filename))
				{
					return file;
				}
			}
			return null;
		}

		internal virtual RandomAccessFile getDataRaf(int fileno)
		{
			// absolute location
			MFile dataFile = fileMap[fileno];

			// if data file does not exist, check relative location - eg may be /upc/share instead of Q:
			if (!dataFile.exists())
			{
				MFile relativeFile;
				if (fileMap.Count == 1)
				{
					relativeFile = directory.getChild(name); // single file case
				}
				else
				{
					relativeFile = directory.getChild(dataFile.Name); // must be in same directory as the ncx file
				}
				if (relativeFile != null)
				{
					dataFile = relativeFile;
				}
			}

			// data file not here
			if (!dataFile.exists())
			{
				throw new System.IO.FileNotFoundException("data file not found = " + dataFile.Path);
			}

			RandomAccessFile want = NetcdfFiles.getRaf(dataFile.Path, -1);
			want.order(RandomAccessFile.BIG_ENDIAN);
			return want;
		}

		internal virtual string getDataRafFilename(int fileno)
		{
			MFile mfile = fileMap[fileno];
			return mfile.Path;
		}

		protected internal static NetcdfDataset buildNetcdfDataset(AbstractIOServiceProvider iosp, string location)
		{
			RandomAccessFile raf = (RandomAccessFile)iosp.sendIospMessage(NetcdfFile.IOSP_MESSAGE_RANDOM_ACCESS_FILE);
			NetcdfFile ncfile = NetcdfFiles.build(iosp, raf, location, null);
			return (NetcdfDataset)NetcdfDatasets.enhance(ncfile, NetcdfDataset.getDefaultEnhanceMode(), null);
		}

		///////////////////////

		// stuff needed by InvDatasetFcGrib
		public abstract NetcdfSharp.ucar.nc2.dataset.NetcdfDataset getNetcdfDataset(Dataset ds, GroupGC group, string filename, FeatureCollectionConfig gribConfig, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger);

		public abstract NetcdfSharp.ucar.nc2.dt.grid.GridDataset getGridDataset(Dataset ds, GroupGC group, string filename, FeatureCollectionConfig gribConfig, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger);

		public abstract CoverageCollection getGridCoverage(Dataset ds, GroupGC group, string filename, FeatureCollectionConfig gribConfig, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger);


	}

}
