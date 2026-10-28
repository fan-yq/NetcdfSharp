// Single-file "collection" support for the GRIB IOSP.
//
// The full netcdf-java feature-collection layer (ncx/gbx9 indexes, partitions, inventories) is out
// of scope. This implementation scans a single GRIB1/GRIB2 data file, builds the coordinates and
// variables with the existing rectilyser in Grib1CollectionBuilder / Grib2CollectionBuilder, and
// keeps the record positions in memory. As a result the standard entry point
// NetcdfFiles.open("file.grb2") works without any index files.

using System;
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using CollectionUpdateType = NetcdfSharp.thredds.inventory.CollectionUpdateType;
	using MCollection = NetcdfSharp.thredds.inventory.MCollection;
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using MFiles = NetcdfSharp.thredds.inventory.MFiles;
	using PartitionManager = NetcdfSharp.thredds.inventory.partition.PartitionManager;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using NetcdfSharp.ucar.nc2.grib.coord;
	using CoordinateRuntime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime;
	using Coordinate = NetcdfSharp.ucar.nc2.grib.coord.Coordinate;
	using CoordinateTime2D = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime2D;
	using CoordinateTimeAbstract = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeAbstract;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib1RecordScanner = NetcdfSharp.ucar.nc2.grib.grib1.Grib1RecordScanner;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2RecordScanner = NetcdfSharp.ucar.nc2.grib.grib2.Grib2RecordScanner;
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;

	public class GribCdmIndex
	{
		public const string NCX_SUFFIX = ".ncx";

		public enum GribCollectionType
		{
			GRIB1, GRIB2, Partition1, Partition2, NotGrib
		}

		internal GribCdmIndex(NetcdfSharp.org.slf4j.Logger logger)
		{
		}

		public static GribCollectionType getType(RandomAccessFile raf)
		{
			// Java GribCdmIndex.getType() recognizes the magic headers of ncx index files. Index
			// files are not written or read here (the collection is built in memory), so this
			// deliberately reports the edition of a raw GRIB data file instead. Unknown editions
			// return NotGrib so that the IOSP validators fall back to Grib1/2RecordScanner.
			try
			{
				long pos = raf.FilePointer;
				raf.seek(0);
				byte[] b = new byte[8];
				raf.readFully(b);
				raf.seek(pos);
				if (b[0] == (byte)'G' && b[1] == (byte)'R' && b[2] == (byte)'I' && b[3] == (byte)'B')
				{
					if (b[7] == 1)
					{
						return GribCollectionType.GRIB1;
					}
					if (b[7] == 2)
					{
						return GribCollectionType.GRIB2;
					}
				}
			}
			catch (Exception)
			{
				// not a GRIB file (or too short)
			}
			return GribCollectionType.NotGrib;
		}

		/// <summary>
		/// Open (or build in memory) a collection for a single GRIB data file.
		/// </summary>
		public static GribCollectionImmutable openGribCollectionFromDataFile(bool isGrib1, MFile wantFile, CollectionUpdateType updateType, FeatureCollectionConfig gribConfig, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger)
		{
			if (wantFile == null || !wantFile.exists())
			{
				return null;
			}

			FeatureCollectionConfig config = gribConfig ?? new FeatureCollectionConfig();
			Formatter log = errlog ?? new Formatter();
			string name = wantFile.Name;

			MCollection dcm = new SingleFileCollection(wantFile, config);
			IList<MFile> allFiles = new List<MFile>();
			IList<GribCollectionBuilder.Group> groups;

			if (isGrib1)
			{
				Grib1CollectionBuilder builder = new Grib1CollectionBuilder(name, dcm, logger);
				groups = builder.makeGroups(allFiles, false, log);
				if (groups.Count == 0)
				{
					return null;
				}
				return buildGrib1Collection(wantFile, config, groups);
			}
			else
			{
				Grib2CollectionBuilder builder = new Grib2CollectionBuilder(name, dcm, logger);
				groups = builder.makeGroups(allFiles, false, log);
				if (groups.Count == 0)
				{
					return null;
				}
				return buildGrib2Collection(wantFile, config, groups);
			}
		}

		/// <summary>
		/// Open a GRIB collection directly from an already-open data file.
		/// </summary>
		public static GribCollectionImmutable openGribCollectionFromRaf(RandomAccessFile raf, FeatureCollectionConfig gribConfig, CollectionUpdateType updateType, NetcdfSharp.org.slf4j.Logger logger)
		{
			if (raf == null)
			{
				return null;
			}

			// Match Java GribCdmIndex.openGribCollectionFromRaf(): use the strict data-file
			// validators (not the ncx header sniffing in getType()), then close the caller's raf
			// because the collection owns its own data-file handle.
			bool isGrib2 = Grib2RecordScanner.isValidFile(raf);
			bool isGrib1 = !isGrib2 && Grib1RecordScanner.isValidFile(raf);
			if (!isGrib1 && !isGrib2)
			{
				// ncx/partition index files are not supported
				return null;
			}

			MFile mfile = MFiles.create(raf.Location);
			try
			{
				return openGribCollectionFromDataFile(isGrib1, mfile, updateType, gribConfig, null, logger);
			}
			finally
			{
				raf.close();
			}
		}

		public static MFile makeIndexFile(string name, MFile directory)
		{
			return directory.getChild(name + NCX_SUFFIX);
		}

		public static bool updateGribCollectionFromPCollection(bool isGrib1, PartitionManager part, CollectionUpdateType updateType, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger)
		{
			return true;
		}

		internal virtual void readMFiles(string indexFile, System.Collections.Generic.IList<MFile> oldFiles)
		{
		}

		////////////////////////////////////////////////////////////////////////////////////////////////

		private static GribCollectionImmutable buildGrib2Collection(MFile file, FeatureCollectionConfig config, IList<GribCollectionBuilder.Group> groups)
		{
			Grib2CollectionWriter.Group firstGroup = (Grib2CollectionWriter.Group) groups[0];
			Grib2Record firstRecord = firstGroup.records[0];

			GribCollectionMutable gc = newCollection(file, config, false, groups);
			gc.center = firstRecord.Id.Center_id;
			gc.subcenter = firstRecord.Id.Subcenter_id;
			gc.master = firstRecord.Id.Master_table_version;
			gc.local = firstRecord.Id.Local_table_version;
			gc.genProcessType = firstRecord.PDS.GenProcessType;
			gc.genProcessId = firstRecord.PDS.GenProcessId;
			gc.backProcessId = firstRecord.PDS.BackProcessId;

			Grib2Tables tables = Grib2Tables.factory(firstRecord);
			gc.cust = tables;

			GribCollectionMutable.Dataset ds = gc.makeDataset(collectionType(groups));
			foreach (GribCollectionBuilder.Group group in groups)
			{
				Grib2CollectionWriter.Group g = (Grib2CollectionWriter.Group) group;
				Grib2Record record = g.records[0];
				GribCollectionMutable.GroupGC groupGC = new GribCollectionMutable.GroupGC(gc);
				NetcdfSharp.ucar.nc2.grib.GdsHorizCoordSys hcs = record.GDS.makeHorizCoordSys();
				groupGC.horizCoordSys = new GribHorizCoordSystem(hcs, record.GDSsection.RawBytes, record.GDS, hcs.Name, hcs.Name, -1);
				groupGC.coords = g.Coordinates;
				groupGC.filenose = new int[] { 0 };

				foreach (Grib2CollectionBuilder.VariableBag vb in g.gribVars)
				{
					SparseArray<Grib2Record> sa = vb.coordND.SparseArray;
					GribCollectionMutable.VariableIndex vi = gc.makeVariableIndex(groupGC, tables, vb.first.Discipline, vb.first.Id.Center_id, vb.first.Id.Subcenter_id, vb.first.PDSsection.RawBytes, vb.coordIndex, 0, 0);
					vi.ndups = sa.getNdups();
					vi.nrecords = sa.countNotMissing();
					vi.nmissing = sa.countMissing();
					vi.inMemoryRecords = makeInMemoryRecords(sa, false);
					groupGC.addVariable(vi);
				}

				ds.groups.Add(groupGC);
			}

			return new Grib2Collection(gc);
		}

		private static GribCollectionImmutable buildGrib1Collection(MFile file, FeatureCollectionConfig config, IList<GribCollectionBuilder.Group> groups)
		{
			Grib1CollectionWriter.Group firstGroup = (Grib1CollectionWriter.Group) groups[0];
			Grib1Record firstRecord = firstGroup.records[0];

			GribCollectionMutable gc = newCollection(file, config, true, groups);
			gc.center = firstRecord.PDSsection.Center;
			gc.subcenter = firstRecord.PDSsection.SubCenter;
			gc.master = firstRecord.PDSsection.TableVersion;
			gc.local = 0;
			gc.genProcessType = firstRecord.PDSsection.GenProcess;
			gc.genProcessId = 0;
			gc.backProcessId = 0;

			Grib1Customizer tables = Grib1Customizer.factory(firstRecord, null);
			tables.TimeUnitConverter = config.gribConfig.TimeUnitConverter;
			gc.cust = tables;

			GribCollectionMutable.Dataset ds = gc.makeDataset(collectionType(groups));
			foreach (GribCollectionBuilder.Group group in groups)
			{
				Grib1CollectionWriter.Group g = (Grib1CollectionWriter.Group) group;
				Grib1Record record = g.records[0];
				GribCollectionMutable.GroupGC groupGC = new GribCollectionMutable.GroupGC(gc);
				NetcdfSharp.ucar.nc2.grib.GdsHorizCoordSys hcs = record.GDS.makeHorizCoordSys();
				groupGC.horizCoordSys = new GribHorizCoordSystem(hcs, record.GDSsection.RawBytes, record.GDS, hcs.Name, hcs.Name, -1);
				groupGC.coords = g.Coordinates;
				groupGC.filenose = new int[] { 0 };

				foreach (Grib1CollectionBuilder.VariableBag vb in g.gribVars)
				{
					SparseArray<Grib1Record> sa = vb.coordND.SparseArray;
					GribCollectionMutable.VariableIndex vi = gc.makeVariableIndex(groupGC, tables, 0, vb.first.PDSsection.Center, vb.first.PDSsection.SubCenter, vb.first.PDSsection.RawBytes, vb.coordIndex, 0, 0);
					vi.ndups = sa.getNdups();
					vi.nrecords = sa.countNotMissing();
					vi.nmissing = sa.countMissing();
					vi.inMemoryRecords = makeInMemoryRecords(sa, true);
					groupGC.addVariable(vi);
				}

				ds.groups.Add(groupGC);
			}

			return new Grib1Collection(gc);
		}

		private static GribCollectionMutable newCollection(MFile file, FeatureCollectionConfig config, bool isGrib1, IList<GribCollectionBuilder.Group> groups)
		{
			string name = System.IO.Path.GetFileNameWithoutExtension(file.Path);
			GribCollectionMutable gc = new GribCollectionMutable(name, file.Directory, config, isGrib1);
			// The ncx header version (Grib1/2CollectionWriter.version) is 3; the protobuf
			// GribCollectionProto.version read into gc.indexVersion is GribCollectionWriter.currentVersion (1).
			gc.version = 3;
			gc.indexVersion = 1;
			gc.indexFilename = file.Path;
			gc.fileMap = new Dictionary<int, MFile>();
			gc.fileMap[0] = file;
			gc.datasets = new List<GribCollectionMutable.Dataset>();

			SortedSet<long> runtimes = new SortedSet<long>();
			CalendarDateRange calendarDateRange = null;
			foreach (GribCollectionBuilder.Group group in groups)
			{
				runtimes.UnionWith(group.CoordinateRuntimes);
				foreach (Coordinate coord in group.Coordinates)
				{
					if (coord is CoordinateTimeAbstract)
					{
						CalendarDateRange range = ((CoordinateTimeAbstract) coord).makeCalendarDateRange(null);
						if (range != null)
						{
							calendarDateRange = calendarDateRange == null ? range : calendarDateRange.extend(range);
						}
					}
				}
			}
			if (runtimes.Count > 0)
			{
				gc.masterRuntime = new CoordinateRuntime(new List<long>(runtimes), null);
			}
			if (calendarDateRange != null)
			{
				gc.setCalendarDateRange(calendarDateRange.Start.Millis, calendarDateRange.End.Millis);
			}
			else if (runtimes.Count > 0)
			{
				gc.setCalendarDateRange(runtimes.Min, runtimes.Max);
			}
			return gc;
		}

		private static GribCollectionImmutable.GribType collectionType(IList<GribCollectionBuilder.Group> groups)
		{
			SortedSet<long> runtimes = new SortedSet<long>();
			bool allTimesUnique = true;
			foreach (GribCollectionBuilder.Group group in groups)
			{
				runtimes.UnionWith(group.CoordinateRuntimes);
				foreach (Coordinate coord in group.Coordinates)
				{
					if (coord is CoordinateTime2D)
					{
						CoordinateTime2D c2 = (CoordinateTime2D) coord;
						if (!c2.hasUniqueTimes())
						{
							allTimesUnique = false;
						}
					}
				}
			}

			if (runtimes.Count <= 1)
			{
				return GribCollectionImmutable.GribType.SRC;
			}
			return allTimesUnique ? GribCollectionImmutable.GribType.MRUTC : GribCollectionImmutable.GribType.MRC;
		}

		private static SparseArray<GribCollectionImmutable.Record> makeInMemoryRecords<T>(SparseArray<T> sa, bool isGrib1)
		{
			IList<GribCollectionImmutable.Record> records = new List<GribCollectionImmutable.Record>(sa.Content.Count);
			foreach (T item in sa.Content)
			{
				if (isGrib1)
				{
					Grib1Record gr = (Grib1Record)(object) item;
					records.Add(new GribCollectionImmutable.Record(gr.File, gr.Is.StartPos, 0, 0));
				}
				else
				{
					Grib2Record gr = (Grib2Record)(object) item;
					// Java Grib2CollectionWriter.writeSparseArray() only stores a bms offset when the
					// bitmap was replaced. Otherwise Grib2Record.readData() reads the standard bitmap
					// located immediately after the DRS.
					int bmsOffset = gr.BmsReplaced ? (int)(gr.BitmapSection.StartingPosition - gr.Is.StartPos) : 0;
					int drsOffset = (int)(gr.DataRepresentationSection.StartingPosition - gr.Is.StartPos);
					records.Add(new GribCollectionImmutable.Record(gr.File, gr.Is.StartPos, bmsOffset, drsOffset));
				}
			}
			return new SparseArray<GribCollectionImmutable.Record>(sa.Shape, (int[]) sa.Track.Clone(), records, sa.getNdups());
		}

		/// <summary>Minimal single-file MCollection used to drive the existing collection builders.</summary>
		private sealed class SingleFileCollection : MCollection
		{
			private readonly MFile file;
			private readonly FeatureCollectionConfig config;

			internal SingleFileCollection(MFile file, FeatureCollectionConfig config)
			{
				this.file = file;
				this.config = config;
			}

			public string Root
			{
				get
				{
					MFile dir = file.Directory;
					return dir == null ? System.IO.Path.GetDirectoryName(file.Path) : dir.Path;
				}
			}

			public CloseableIterator<MFile> FileIterator
			{
				get
				{
					return new SingleFileIterator(file);
				}
			}

			public object getAuxInfo(string key)
			{
				return key == FeatureCollectionConfig.AUX_CONFIG ? config : null;
			}

			public string getIndexFilename(string suffix)
			{
				return System.IO.Path.Combine(Root, file.Name + suffix);
			}
		}

		private sealed class SingleFileIterator : CloseableIterator<MFile>
		{
			private readonly MFile file;
			private int state = -1;

			internal SingleFileIterator(MFile file)
			{
				this.file = file;
			}

			public MFile Current
			{
				get
				{
					return state == 0 ? file : null;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public bool MoveNext()
			{
				if (state < 0)
				{
					state = 0;
					return true;
				}
				state = 1;
				return false;
			}

			public void Reset()
			{
				state = -1;
			}

			public void Dispose()
			{
			}
		}
	}
}
