// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using NetcdfSharp.ucar.nc2.grib.grib2;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateFormatter = NetcdfSharp.ucar.nc2.time.CalendarDateFormatter;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CalendarTimeZone = NetcdfSharp.ucar.nc2.time.CalendarTimeZone;
	using Coordinate = NetcdfSharp.ucar.nc2.grib.coord.Coordinate;
	using CoordinateRuntime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime;
	using CoordinateTime2D = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime2D;
	using CoordinateTimeAbstract = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeAbstract;
	using CoordinateTimeIntv = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeIntv;
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib1Gds = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Gds;
	using Grib1ParamTime = NetcdfSharp.ucar.nc2.grib.grib1.Grib1ParamTime;
	using Grib1SectionProductDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionProductDefinition;
	using Grib1Variable = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Variable;
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using GribIndexCache = NetcdfSharp.ucar.nc2.grib.GribIndexCache;
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// A mutable class for writing indices or building GribCollectionImmutable.
	/// Better to use a Builder?
	/// 
	/// @author John
	/// @since 12/1/13
	/// </summary>
	public class GribCollectionMutable : IDisposable
	{
		private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(GribCollectionMutable));
		internal const long MISSING_RECORD = -1;

		//////////////////////////////////////////////////////////

		internal static MFile makeIndexMFile(string collectionName, MFile directory)
		{
			MFile nameNoBlanks = GribCdmIndex.makeIndexFile(collectionName, directory);
			return new GcMFile(directory, nameNoBlanks.Name, -1, -1, -1); // LOOK dont know lastMod, size.
																		  // can it be added later?
		}

		private static readonly CalendarDateFormatter cf = new CalendarDateFormatter("yyyyMMdd-HHmmss", new CalendarTimeZone("UTC"));

		internal static string makeName(string collectionName, CalendarDate runtime)
		{
			string nameNoBlanks = StringUtil2.replace(collectionName, ' ', "_");
			return nameNoBlanks + "-" + cf.toString(runtime);
		}

		////////////////////////////////////////////////////////////////
		protected internal readonly string name; // collection name; index filename must be directory/name.ncx2
		protected internal readonly FeatureCollectionConfig config;
		protected internal readonly bool isGrib1;
		protected internal MFile directory;
		protected internal string orgDirectory;

		// set by the builder
		public int version; // the ncx version
		public int center, subcenter, master, local; // GRIB 1 uses "local" for table version
		public int genProcessType, genProcessId, backProcessId;
		public IList<Parameter> @params; // not used
		protected internal IDictionary<int, MFile> fileMap; // all the files used in the GC; key is the index in original collection, GC
															// has subset of them
		protected internal IList<Dataset> datasets;
		protected internal CoordinateRuntime masterRuntime;
		protected internal GribTables cust;
		protected internal int indexVersion;

		internal virtual void setCalendarDateRange(long startMsecs, long endMsecs)
		{
			this.dateRange = CalendarDateRange.of(CalendarDate.of(startMsecs), CalendarDate.of(endMsecs));
		}

		protected internal CalendarDateRange dateRange;

		// not stored in index
		protected internal RandomAccessFile indexRaf; // this is the raf of the index (ncx) file
		protected internal string indexFilename;
		protected internal long lastModified;
		protected internal long fileSize;

		private static int countGC;

		protected internal GribCollectionMutable(string name, MFile directory, FeatureCollectionConfig config, bool isGrib1)
		{
			countGC++;
			this.name = name;
			this.directory = directory;
			this.config = config;
			this.isGrib1 = isGrib1;
			if (config == null)
			{
				logger.error("GribCollection {} has empty config%n", name);
			}
			if (string.ReferenceEquals(name, null))
			{
				logger.error("GribCollection has null name dir={}%n", directory);
			}
		}

		// for making partition collection
		internal virtual void copyInfo(GribCollectionMutable from)
		{
			this.center = from.center;
			this.subcenter = from.subcenter;
			this.master = from.master;
			this.local = from.local;
			this.genProcessType = from.genProcessType;
			this.genProcessId = from.genProcessId;
			this.backProcessId = from.backProcessId;
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

		public virtual string Location
		{
			get
			{
				if (indexRaf != null)
				{
					return indexRaf.Location;
				}
				return IndexFilepathInCache;
			}
		}

		public virtual ICollection<MFile> Files
		{
			get
			{
				return fileMap.Values;
			}
		}

		public virtual FeatureCollectionConfig Config
		{
			get
			{
				return config;
			}
		}

		/// <summary>
		/// The files that comprise the collection.
		/// Actual paths, including the grib cache if used.
		/// </summary>
		/// <returns> list of filename. </returns>
		public virtual IList<string> Filenames
		{
			get
			{
				IList<string> result = new List<string>();
				foreach (MFile file in fileMap.Values)
				{
					result.Add(file.Path);
				}
				result.Sort();
				return result;
			}
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable MFile getIndexParentFile()
		internal virtual MFile IndexParentFile
		{
			get
			{
				if (indexRaf == null)
				{
					return null;
				}
				MFile index = MFiles.create(indexRaf.Location);
				try
				{
					return index.Parent;
				}
				catch (IOException)
				{
					return null;
				}
			}
		}

		public virtual string getFilename(int fileno)
		{
			return fileMap[fileno].Path;
		}

		public virtual IList<Dataset> Datasets
		{
			get
			{
				return datasets;
			}
		}

		internal virtual Dataset makeDataset(GribCollectionImmutable.GribType type)
		{
			Dataset result = new Dataset(this, type);
			datasets.Add(result);
			return result;
		}

		internal virtual GribCollectionMutable.Dataset DatasetCanonical
		{
			get
			{
				foreach (GribCollectionMutable.Dataset ds in datasets)
				{
					if (ds.gctype != GribCollectionImmutable.GribType.Best)
					{
						return ds;
					}
				}
				throw new System.InvalidOperationException("GC.getDatasetCanonical failed on=" + name);
			}
		}

		public virtual IDictionary<int, MFile> FileMap
		{
			set
			{
				this.fileMap = value;
			}
		}

		/// <summary>
		/// public by accident, do not use
		/// </summary>
		/// <param name="indexRaf"> the open raf of the index file </param>
		internal virtual RandomAccessFile IndexRaf
		{
			set
			{
				this.indexRaf = value;
				if (value != null)
				{
					this.indexFilename = value.Location;
				}
			}
		}

		/// <summary>
		/// get index filename
		/// </summary>
		/// <returns> index filename; may not exist; may be in disk cache </returns>
		private string IndexFilepathInCache
		{
			get
			{
				MFile indexFile = GribCdmIndex.makeIndexFile(name, directory);
				return GribIndexCache.getFileOrCache(indexFile.Path).Path;
			}
		}

		// set from GribCollectionBuilderFromIndex.readFromIndex()
		internal virtual MFile setOrgDirectory(string orgDirectory)
		{
			this.orgDirectory = orgDirectory;
			directory = MFiles.create(orgDirectory);
			if (!directory.exists())
			{
				MFile indexFile = MFiles.create(indexFilename);
				try
				{
					MFile parent = indexFile.Parent;
					if (parent != null && parent.exists())
					{
						directory = parent;
					}
				}
				catch (IOException)
				{
					// ignore
				}
			}
			return directory;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////
		// stuff for FileCacheable

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public void close() throws NetcdfSharp.java.io.IOException
		public virtual void Dispose()
		{

			if (indexRaf != null)
			{
				indexRaf.close();
				indexRaf = null;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////

		public class Dataset
		{
			private readonly GribCollectionMutable outerInstance;

			public GribCollectionImmutable.GribType gctype;
			internal IList<GroupGC> groups; // must be kept in order, because PartitionForVariable2D has index into it

			public Dataset(GribCollectionMutable outerInstance, GribCollectionImmutable.GribType type)
			{
				this.outerInstance = outerInstance;
				this.gctype = type;
				groups = new List<GroupGC>();
			}

			internal Dataset(GribCollectionMutable outerInstance, Dataset from)
			{
				this.outerInstance = outerInstance;
				this.gctype = from.gctype;
				groups = new List<GroupGC>(from.groups.Count);
			}

			internal virtual GroupGC addGroupCopy(GroupGC from)
			{
				GroupGC g = new GroupGC(outerInstance, from);
				groups.Add(g);
				return g;
			}

			public virtual IList<GroupGC> Groups
			{
				get
				{
					return groups;
				}
			}
		}

		public class GroupGC : IComparable<GroupGC>
		{
			private readonly GribCollectionMutable outerInstance;

			internal GribHorizCoordSystem horizCoordSys;
			internal readonly IList<VariableIndex> variList;
			internal IList<Coordinate> coords; // shared coordinates
			internal int[] filenose; // key for GC.fileMap
			internal Dictionary<GribCollectionMutable.VariableIndex, GribCollectionMutable.VariableIndex> varMap;
			internal bool isTwoD = true; // true except for Best (?)

			internal GroupGC(GribCollectionMutable outerInstance)
			{
				this.outerInstance = outerInstance;
				this.variList = new List<VariableIndex>();
				this.coords = new List<Coordinate>();
			}

			// copy constructor for PartitionBuilder
			internal GroupGC(GribCollectionMutable outerInstance, GroupGC from)
			{
				this.outerInstance = outerInstance;
				this.horizCoordSys = from.horizCoordSys; // reference
				this.variList = new List<VariableIndex>(from.variList.Count); // empty list
				this.coords = new List<Coordinate>(from.coords.Count); // empty list
				this.isTwoD = from.isTwoD;
			}

			public virtual VariableIndex addVariable(VariableIndex vi)
			{
				variList.Add(vi);
				return vi;
			}

			public virtual GribCollectionMutable GribCollection
			{
				get
				{
					return outerInstance;
				}
			}

			public virtual IEnumerable<VariableIndex> Variables
			{
				get
				{
					return variList;
				}
			}

			public virtual IEnumerable<Coordinate> Coordinates
			{
				get
				{
					return coords;
				}
			}

			// unique name for Group
			public virtual string Id
			{
				get
				{
					return horizCoordSys.Id;
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

			public virtual byte[] GdsBytes
			{
				get
				{
					return horizCoordSys.RawGds;
				}
			}

			public virtual object GdsHash
			{
				get
				{
					return horizCoordSys.GdsHash;
				}
			}

			// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
			// ORIGINAL LINE: @Override public int compareTo(@Nonnull GroupGC o)
			public virtual int CompareTo(GroupGC o)
			{
				return string.CompareOrdinal(Description, o.Description);
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

			public virtual IList<string> Filenames
			{
				get
				{
					IList<string> result = new List<string>();
					if (filenose == null)
					{
						return result;
					}
					foreach (int fileno in filenose)
					{
						result.Add(outerInstance.fileMap[fileno].Path);
					}
					result.Sort();
					return result;
				}
			}

			// get the variable in this group that has same object equality as want
			public virtual GribCollectionMutable.VariableIndex findVariableByHash(GribCollectionMutable.VariableIndex want)
			{
				if (varMap == null)
				{
					varMap = new Dictionary<GribCollectionMutable.VariableIndex, GribCollectionMutable.VariableIndex>(variList.Count * 2);
					foreach (VariableIndex vi in variList)
					{
						GribCollectionMutable.VariableIndex old = null;
						varMap.TryGetValue(vi, out old);
						varMap[vi] = vi;
						if (old != null)
						{
							logger.error("GribCollectionMutable has duplicate variable hash {} == {}", vi, old);
						}
					}
				}
				return varMap[want];
			}

			private CalendarDateRange dateRange;

			public virtual CalendarDateRange CalendarDateRange
			{
				get
				{
					if (dateRange == null)
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
						dateRange = result;
					}
					return dateRange;
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

			public virtual void show(Formatter f)
			{
				f.format("Group %s (%d) isTwoD=%s%n", horizCoordSys.Id, horizCoordSys.GdsHash.GetHashCode(), isTwoD);
				f.format(" nfiles %d%n", filenose == null ? 0 : filenose.Length);
				f.format(" hcs = %s%n", horizCoordSys.Hcs);
			}

			public override string ToString()
			{
				return MoreObjects.toStringHelper(this).add("horizCoordSys", horizCoordSys).add("variList", variList).add("coords", coords).add("filenose", filenose).add("varMap", varMap).add("isTwoD", isTwoD).add("dateRange", dateRange).ToString();
			}
		}

		internal virtual GribCollectionMutable.VariableIndex makeVariableIndex(GroupGC g, GribTables customizer, int discipline, int center, int subcenter, byte[] rawPds, IList<int> index, long recordsPos, int recordsLen)
		{
			return new VariableIndex(this, g, customizer, discipline, center, subcenter, rawPds, index, recordsPos, recordsLen);
		}

		internal virtual VariableIndex makeVariableIndex(GroupGC group, GribCollectionMutable.VariableIndex from)
		{
			VariableIndex vip = new VariableIndex(this, group, from);
			group.addVariable(vip);
			return vip;
		}

		public class VariableIndex : IComparable<VariableIndex>
		{
			private readonly GribCollectionMutable outerInstance;

			public GroupGC group; // belongs to this group
			public int tableVersion; // grib1 only : can vary by variable
			public int discipline, center, subcenter; // grib2 only
			public byte[] rawPds; // grib1 or grib2
			public long recordsPos; // where the records array is stored in the index. 0 means no records
			public int recordsLen;
			public object gribVariable; // use this to test for object equality

			// Non-index (single data file) collections keep the record positions in memory instead of a .gbx9 file.
			internal NetcdfSharp.ucar.nc2.grib.coord.SparseArray<GribCollectionImmutable.Record> inMemoryRecords;

			internal IList<int> coordIndex; // indexes into group.coords

			// derived from pds
			public int category, parameter, levelType, intvType, ensDerivedType, probType, percentile;
			private string intvName; // eg "mixed intervals, 3 Hour, etc"
			public string probabilityName;
			public bool isLayer, isEnsemble;
			public int genProcessType;
			public int spatialStatType;

			// stats
			public int ndups, nrecords, nmissing;

			// temporary storage while building - do not use
			internal IList<Coordinate> coords;

			internal VariableIndex(GribCollectionMutable outerInstance, GroupGC g, GribTables customizer, int discipline, int center, int subcenter, byte[] rawPds, IList<int> index, long recordsPos, int recordsLen)
			{
				this.outerInstance = outerInstance;
				this.group = g;
				this.discipline = discipline;
				this.rawPds = rawPds;
				this.center = center;
				this.subcenter = subcenter;
				this.coordIndex = index;
				this.recordsPos = recordsPos;
				this.recordsLen = recordsLen;

				if (outerInstance.isGrib1)
				{
					Grib1Customizer cust = (Grib1Customizer)customizer;
					Grib1SectionProductDefinition pds = new Grib1SectionProductDefinition(rawPds);

					// quantities that are stored in the pds
					this.category = 0;
					this.tableVersion = pds.TableVersion;
					this.parameter = pds.ParameterNumber;
					this.levelType = pds.LevelType;
					Grib1ParamTime ptime = cust.getParamTime(pds);
					if (ptime.IsInterval())
					{
						this.intvType = pds.TimeRangeIndicator;
					}
					else
					{
						this.intvType = -1;
					}
					this.isLayer = cust.isLayer(pds.LevelType);

					this.ensDerivedType = -1;
					this.probType = -1;
					this.probabilityName = null;
					this.percentile = -1;

					this.genProcessType = pds.GenProcess; // LOOK process vs process type ??
					this.isEnsemble = pds.Ensemble;
					this.spatialStatType = -1;

					// LOOK config vs serialized config
					gribVariable = new Grib1Variable(cust, pds, (Grib1Gds)g.GdsHash, outerInstance.config.gribConfig.useTableVersion, outerInstance.config.gribConfig.intvMerge, outerInstance.config.gribConfig.useCenter);

				}
				else
				{
					Grib2Tables cust2 = (Grib2Tables)customizer;

					Grib2SectionProductDefinition pdss = new Grib2SectionProductDefinition(rawPds);
					Grib2Pds pds = pdss.PDS;
					Debug.Assert(pds != null);
					this.tableVersion = -1;

					// quantities that are stored in the pds
					this.category = pds.ParameterCategory;
					this.parameter = pds.ParameterNumber;
					this.levelType = pds.LevelType1;
					this.intvType = pds.StatisticalProcessType;
					this.isLayer = Grib2Utils.isLayer(pds);

					if (pds.EnsembleDerived)
					{
						Grib2Pds.PdsEnsembleDerived pdsDerived = (Grib2Pds.PdsEnsembleDerived)pds;
						ensDerivedType = pdsDerived.DerivedForecastType; // derived type (table 4.7)
					}
					else
					{
						this.ensDerivedType = -1;
					}

					if (pds.Probability)
					{
						Grib2Pds.PdsProbability pdsProb = (Grib2Pds.PdsProbability)pds;
						probabilityName = pdsProb.ProbabilityName;
						probType = pdsProb.ProbabilityType;
					}
					else
					{
						this.probType = -1;
						this.probabilityName = null;
					}

					if (pds.Percentile)
					{
						Grib2Pds.PdsPercentile pdsPctl = (Grib2Pds.PdsPercentile)pds;
						this.percentile = pdsPctl.PercentileValue;
					}
					else
					{
						this.percentile = -1;
					}

					this.genProcessType = pds.GenProcessType;
					this.isEnsemble = pds.Ensemble;

					if (pds.SpatialInterval)
					{
						Grib2Pds.PdsSpatialInterval pdsSpatial = (Grib2Pds.PdsSpatialInterval)pds;
						this.spatialStatType = pdsSpatial.SpatialStatisticalProcessType;
					}
					else
					{
						this.spatialStatType = -1;
					}

					// LOOK config vs serialized config
					gribVariable = new Grib2Variable(cust2, discipline, center, subcenter, (Grib2Gds)g.GdsHash, pds, outerInstance.config.gribConfig.intvMerge, outerInstance.config.gribConfig.useGenType);
				}
			}

			internal VariableIndex(GribCollectionMutable outerInstance, GroupGC g, VariableIndex other)
			{
				this.outerInstance = outerInstance;
				this.group = g;
				this.tableVersion = other.tableVersion;
				this.discipline = other.discipline;
				this.center = other.center;
				this.subcenter = other.subcenter;
				this.rawPds = other.rawPds;
				this.gribVariable = other.gribVariable;
				this.coordIndex = new List<int>(other.coordIndex);
				this.recordsPos = 0;
				this.recordsLen = 0;

				this.category = other.category;
				this.parameter = other.parameter;
				this.levelType = other.levelType;
				this.intvType = other.intvType;
				this.isLayer = other.isLayer;
				this.ensDerivedType = other.ensDerivedType;
				this.probabilityName = other.probabilityName;
				this.probType = other.probType;
				this.genProcessType = other.genProcessType;
				this.spatialStatType = other.spatialStatType;
				this.isEnsemble = other.isEnsemble;
				this.percentile = other.percentile;
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

			public virtual int getCoordinateIdx(Coordinate.Type want)
			{
				foreach (int idx in coordIndex)
				{
					if (group.coords[idx].getType() == want)
					{
						return idx;
					}
				}
				return -1;
			}

			public virtual string getTimeIntvName()
			{
				if (!string.ReferenceEquals(intvName, null))
				{
					return intvName;
				}
				CoordinateTimeIntv timeiCoord = (CoordinateTimeIntv)getCoordinate(Coordinate.Type.timeIntv);
				if (timeiCoord != null)
				{
					intvName = timeiCoord.TimeIntervalName;
					return intvName;
				}

				CoordinateTime2D time2DCoord = (CoordinateTime2D)getCoordinate(Coordinate.Type.time2D);
				if (time2DCoord == null || !time2DCoord.TimeInterval)
				{
					return null;
				}
				intvName = time2DCoord.TimeIntervalName;
				return intvName;
			}

			/////////////////////////////
			public virtual string id()
			{
				return discipline + "-" + category + "-" + parameter;
			}

			public virtual int getVarid()
			{
				return (discipline << 16) + (category << 8) + parameter;
			}

			public override string ToString()
			{
				return MoreObjects.toStringHelper(this).add("tableVersion", tableVersion).add("discipline", discipline).add("category", category).add("parameter", parameter).add("levelType", levelType).add("intvType", intvType).add("ensDerivedType", ensDerivedType).add("probType", probType).add("intvName", intvName).add("probabilityName", probabilityName).add("isLayer", isLayer).add("genProcessType", genProcessType).add("cdmHash", gribVariable.GetHashCode()).ToString();
			}

			public virtual string toStringComplete()
			{
				return MoreObjects.toStringHelper(this).add("group", group).add("tableVersion", tableVersion).add("discipline", discipline).add("center", center).add("subcenter", subcenter).add("recordsPos", recordsPos).add("recordsLen", recordsLen).add("gribVariable", gribVariable).add("coordIndex", coordIndex).add("category", category).add("parameter", parameter).add("levelType", levelType).add("intvType", intvType).add("ensDerivedType", ensDerivedType).add("probType", probType).add("intvName", intvName).add("probabilityName", probabilityName).add("isLayer", isLayer).add("isEnsemble", isEnsemble).add("genProcessType", genProcessType).add("spatialStatType", spatialStatType).ToString();
			}

			public virtual string toStringShort()
			{
				using (Formatter sb = new Formatter())
				{
					sb.format("Variable {%d-%d-%d", discipline, category, parameter);
					sb.format(", levelType=%d", levelType);
					sb.format(", intvType=%d", intvType);
					if (!string.ReferenceEquals(intvName, null) && intvName.Length > 0)
					{
						sb.format(" intv=%s", intvName);
					}
					if (!string.ReferenceEquals(probabilityName, null) && probabilityName.Length > 0)
					{
						sb.format(" prob=%s", probabilityName);
					}
					sb.format(" cdmHash=%d}", gribVariable.GetHashCode());
					return sb.ToString();
				}
			}

			// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
			// ORIGINAL LINE: @Override public int compareTo(@Nonnull VariableIndex o)
			public virtual int CompareTo(VariableIndex o)
			{
				int r = discipline - o.discipline; // LOOK add center, subcenter, version?
				if (r != 0)
				{
					return r;
				}
				r = category - o.category;
				if (r != 0)
				{
					return r;
				}
				r = parameter - o.parameter;
				if (r != 0)
				{
					return r;
				}
				r = levelType - o.levelType;
				if (r != 0)
				{
					return r;
				}
				r = intvType - o.intvType;
				return r;
			}

			public override bool Equals(object o)
			{
				if (this == o)
				{
					return true;
				}
				if (!(o is VariableIndex))
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

		} // VariableIndex

		public class Record
		{
			public readonly int fileno; // which file
			public readonly long pos; // offset on file where data starts
			public readonly long bmsPos; // if non-zero, offset where bms starts
			public readonly int scanMode; // from gds

			public Record(int fileno, long pos, long bmsPos, int scanMode)
			{
				this.fileno = fileno;
				this.pos = pos;
				this.bmsPos = bmsPos;
				this.scanMode = scanMode;
			}

			public override string ToString()
			{
				return MoreObjects.toStringHelper(this).add("fileno", fileno).add("pos", pos).add("bmsPos", bmsPos).add("scanMode", scanMode).ToString();
			}
		}

		public virtual void showIndex(Formatter f)
		{
			f.format("Class (%s)%n", GetType().Name);
			f.format("%s%n%n", ToString());

			foreach (Dataset ds in datasets)
			{
				f.format("Dataset %s%n", ds.gctype);
				foreach (GroupGC g in ds.groups)
				{
					f.format(" Group %s%n", g.horizCoordSys.Id);
					foreach (VariableIndex v in g.variList)
					{
						f.format("  %s%n", v.toStringShort());
					}
				}
			}
			if (fileMap == null)
			{
				f.format("Files empty%n");
			}
			else
			{
				f.format("Files (%d)%n", fileMap.Count);
				foreach (int index in fileMap.Keys)
				{
					f.format("  %d: %s%n", index, fileMap[index]);
				}
				f.format("%n");
			}
		}

		public override string ToString()
		{
			return MoreObjects.toStringHelper(this).add("name", name).add("config", config).add("isGrib1", isGrib1).add("directory", directory).add("orgDirectory", orgDirectory).add("version", version).add("center", center).add("subcenter", subcenter).add("master", master).add("local", local).add("genProcessType", genProcessType).add("genProcessId", genProcessId).add("backProcessId", backProcessId).add("params", @params).add("fileMap", fileMap).add("datasets", datasets).add("masterRuntime", masterRuntime).add("cust", cust).add("indexVersion", indexVersion).add("dateRange", dateRange).add("indexRaf", indexRaf).add("indexFilename", indexFilename).add("lastModified", lastModified).add("fileSize", fileSize).ToString();
		}

		public virtual string showLocation()
		{
			return "name=" + name + " directory=" + directory;
		}

		public virtual GroupGC makeGroup()
		{
			return new GroupGC(this);
		}

	}

}
